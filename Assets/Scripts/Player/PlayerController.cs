using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Collections.Generic;
public class PlayerController : MonoBehaviour
{
    //プレイヤーの状態
    public enum PlayerState
    {
        Normal,
        Attacking,
        Knockback,
        Respawning
    }
    [Header("アクション関連")]
    [SerializeField] private bool isDashing; //ダッシュしているか
    [SerializeField] private bool isDashPressed; //ダッシュボタンを押しているかどうか
    [SerializeField] private bool canDash; //ダッシュできるか
    [SerializeField] private float gravity; //重力加速度
    [SerializeField] private float jumpPower; //ジャンプ力
    [SerializeField] private float rotationSpeed; //視点回転速度
    [SerializeField] private Transform attackPoint; //攻撃発生ポイント
    [SerializeField] private Vector3 attackRange; //攻撃範囲
    [SerializeField] private float attackDuration; //攻撃持続時間
    [SerializeField] private float attackRecoveryTime; //攻撃の後隙
    [SerializeField] private GameObject energyBulletPrefab; //射撃攻撃プレハブ
    [SerializeField] private Transform shotPoint; //発射位置
    [SerializeField] private float maxChargeTime; //最大チャージ時間
    [SerializeField] private float chargeTime; //チャージ時間
    [SerializeField] private bool isCharging; //チャージしているかどうか
    [SerializeField] private float shotRecoveryTime; //チャージショットの最大後隙
    [SerializeField] private GameObject minePrefab; //地雷プレハブ
    [SerializeField] private Transform minePoint; //地雷設置位置
    [SerializeField] private GameObject shockwaveItem; //衝撃波アイテム

    [Header("内部情報")]
    private PlayerState currentState; //プレイヤーの現在の状態
    private float moveSpeed;　//移動速度
    private Vector3 moveDirection;　//移動方向
    private float verticalVelocity; //上下速度の速度
    private Vector3 knockbackVelocity; //ノックバック時の速度

    [Header("他クラス参照")]
    private PlayerAnimationController animationController; //アニメーションコントローラ
    private CharacterController characterController; //キャラクターコントローラ
    private PlayerStatus playerStatus; //プレイヤーステータス
    private PlayerMineController mineController; //地雷の設置状況
    private PlayerInventory playerInventory;
    private HashSet<PlayerController> hitPlayersController = new HashSet<PlayerController>();
    private HashSet<PlayerStatus> hitPlayersStatus = new HashSet<PlayerStatus>();
    private CancellationTokenSource attackCancellation;

    [Header("外部から参照するもの")]
    public PlayerState CurrentState => currentState;
    public bool IsDashing => isDashing;
    void Awake()
    {
        animationController = GetComponent<PlayerAnimationController>();
        characterController = GetComponent<CharacterController>();
        playerStatus = GetComponent<PlayerStatus>();
        mineController = GetComponent<PlayerMineController>();
        playerInventory = GetComponent<PlayerInventory>();
    }
    void Update()
    {
        if(currentState != PlayerState.Respawning)
        {
            // スタミナ管理
            if (isDashPressed)
            {
                if (!playerStatus.IsDashUnlimited)
                {
                    playerStatus.ConsumeStamina(Time.deltaTime);
                }
            }
            else
            {
                playerStatus.RecoverStamina(Time.deltaTime);
            }

            if (playerStatus.Stamina > 0f)
            {
                canDash = true;
            }
            else
            {
                canDash = false;
            }

            //走れってるか判定
            isDashing = playerStatus.IsDashUnlimited || (canDash && isDashPressed);

            Vector3 velocity = Vector3.zero;

            // 重力処理
            if (currentState == PlayerState.Knockback)
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
            else if (characterController.isGrounded)
            {
                if (verticalVelocity < 0)
                {
                    verticalVelocity = -2f;
                }
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            moveSpeed = isDashing ? playerStatus.DashSpeed : playerStatus.WalkSpeed;

            // 状態に応じた水平方向の移動
            if (currentState == PlayerState.Normal)
            {
                velocity = moveDirection * moveSpeed;

                float currentSpeed = moveDirection.magnitude * moveSpeed;
                animationController.UpdateMoveSpeed(currentSpeed);
                animationController.UpdateDash(isDashing);

                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }
            }
            else if (currentState == PlayerState.Knockback)
            {
                velocity = knockbackVelocity;
            }

            // 上下方向の速度を加える
            velocity.y = verticalVelocity;

            // 移動
            characterController.Move(velocity * Time.deltaTime);

            // ノックバック終了判定
            if (currentState == PlayerState.Knockback &&
                characterController.isGrounded &&
                verticalVelocity <= 0)
            {
                currentState = PlayerState.Normal;
                knockbackVelocity = Vector3.zero;
                verticalVelocity = -2f;
            }

            //チャージ処理
            if (isCharging)
            {
                chargeTime = Mathf.Min(chargeTime + Time.deltaTime, maxChargeTime);
            }
        }
    }

    //移動入力の受け取り
    public void SetMoveInput(Vector3 direction, bool dash)
    {
        if (currentState == PlayerState.Respawning)
        {
            moveDirection = Vector3.zero;
            isDashPressed = false;
            return;
        }
        moveDirection = direction;
        isDashPressed = dash;
    }

    //近接攻撃
    public void Attack()
    {
        if(currentState != PlayerState.Normal)
        {
            return;
        }
        currentState = PlayerState.Attacking;
        animationController.PlayAttack();
    }
    public void StartAttackHitbox()
    {
        AttackHitboxAsync().Forget();
    }
    private async UniTask AttackHitboxAsync()
    {
        hitPlayersController.Clear();
        hitPlayersStatus.Clear();
        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            CheckAttackHit();

            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }
    }
    private void CheckAttackHit()
    {
        float multiplier = playerStatus.KnockbackMultiplier;

        Collider[] hits = Physics.OverlapBox(
            attackPoint.position,
            attackRange / 2f,
            attackPoint.rotation
        );
        foreach (Collider hit in hits)
        {
            PlayerController targetController = hit.GetComponentInParent<PlayerController>();
            PlayerStatus targetStatus = hit.GetComponentInParent<PlayerStatus>();

            if (targetController == null || targetController == this || targetStatus == null)
            {
                continue;
            }

            if(targetController.CurrentState != PlayerState.Respawning)
            {
                if (hitPlayersController.Add(targetController))
                {
                    targetController.StartKnockback(transform.position, playerStatus.KnockbackPower * multiplier, playerStatus.KnockbackUpPower * multiplier);
                }
                if (hitPlayersStatus.Add(targetStatus))
                {
                    targetStatus.Damage(playerStatus.AttackPower);
                }
            }
        }
    }
    public void OnAttackAnimationEnd()
    {
        AttackRecoveryAsync(attackRecoveryTime).Forget();
    }

    //攻撃後の後隙
    private async UniTask AttackRecoveryAsync(float time)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        if (currentState == PlayerState.Attacking)
        {
            currentState = PlayerState.Normal;
        }
    }
    public void Jump()
    {
        if (currentState != PlayerState.Normal || !characterController.isGrounded)
        {
            return;
        }
        animationController.PlayJump();
        verticalVelocity = jumpPower;
    }

    //射撃処理
    public void StartCharging()
    {
        if(currentState == PlayerState.Normal)
        {
            chargeTime = 0f;
            isCharging = true;
        }
    }
    public void ReleaseShot()
    {
        if (!isCharging || currentState == PlayerState.Respawning) return;

        currentState = PlayerState.Attacking;

        isCharging = false;

        float chargeRate = chargeTime / maxChargeTime;
        EnergyBullet energyBullet = Instantiate(energyBulletPrefab, shotPoint.position, Quaternion.LookRotation(transform.forward)).GetComponent<EnergyBullet>();
        energyBullet.Initialize(chargeRate, playerStatus.PlayerId, transform.forward);

        chargeTime = 0f;
        float time = Mathf.Lerp(0, shotRecoveryTime, chargeRate);
        AttackRecoveryAsync(time).Forget();
    }

    //地雷設置処理
    public void SetMine()
    {
        if (currentState != PlayerState.Normal)
        {
            return;
        }

        PaintMine mine = Instantiate(minePrefab, minePoint.position, minePrefab.transform.rotation).GetComponent<PaintMine>();
        mine.Initialize(playerStatus.PlayerId, playerStatus.PlayerColor, mineController);

        mineController.AddMine(mine);
    }
    public void UseItem()
    {
        if (currentState != PlayerState.Normal)
        {
            return;
        }
        if (!playerInventory.UseShockwaveItem())
        {
            return;
        }

        ShockwaveItem item = Instantiate(shockwaveItem, transform.position, shockwaveItem.transform.rotation).GetComponent<ShockwaveItem>();
        item.Initialize(playerStatus.PlayerId, playerStatus.PlayerColor);

        Debug.Log("UseItem");
    }
    public void StartKnockback(Vector3 attackerPosition, float power, float upPower)
    {
        if (currentState == PlayerState.Knockback)
        {
            return;
        }
        if (isCharging)
        {
            isCharging = false;
            chargeTime = 0f;
        }
        currentState = PlayerState.Knockback;

        Vector3 direction = transform.position - attackerPosition;
        direction.y = 0;
        direction.Normalize();

        knockbackVelocity = direction * power;
        verticalVelocity = upPower;
    }
    public void StartRespawn()
    {

    }
}
