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
        Knockback
    }
    [SerializeField] private bool isDashing; //ダッシュしているか
    [SerializeField] private float walkSpeed; //歩行速度
    [SerializeField] private float dashSpeed; //走行速度
    [SerializeField] private float gravity; //重力加速度
    [SerializeField] private float jumpPower; //ジャンプ力
    [SerializeField] private float knockbackPower; //後ろに吹っ飛ぶ速度
    [SerializeField] private float knockbackUpPower; //上に吹っ飛ぶ速度
    [SerializeField] private float rotationSpeed; //視点回転速度
    [SerializeField] private Transform attackPoint; //攻撃発生ポイント
    [SerializeField] private Vector3 attackRange; //攻撃範囲
    [SerializeField] private float attackDuration; //攻撃持続時間
    [SerializeField] private float attackRecoveryTime; //攻撃の後隙
    private float moveSpeed;　//移動速度
    private Vector3 moveDirection;　//移動方向
    private float verticalVelocity; //上下速度の速度
    private Vector3 knockbackVelocity; //ノックバック時の速度
    private PlayerAnimationController animationController; //アニメーションコントローラ
    private CharacterController characterController; //キャラクターコントローラ
    private PlayerState currentState; //プレイヤーの現在の状態
    private HashSet<PlayerController> hitPlayers = new HashSet<PlayerController>();
    private CancellationTokenSource attackCancellation;
    void Awake()
    {
        animationController = GetComponent<PlayerAnimationController>();
        characterController = GetComponent<CharacterController>();
    }
    void Update()
    {
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

        moveSpeed = isDashing ? dashSpeed : walkSpeed;

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
    }
    public void SetMoveInput(Vector3 direction, bool dash)
    {
        moveDirection = direction;
        isDashing = dash;
    }
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
        hitPlayers.Clear();
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
        Debug.Log("攻撃判定中");
        Collider[] hits = Physics.OverlapBox(
            attackPoint.position,
            attackRange / 2f,
            attackPoint.rotation
        );
        foreach (Collider hit in hits)
        {
            PlayerController target = hit.GetComponentInParent<PlayerController>();

            if (target == null || target == this)
            {
                continue;
            }

            if (hitPlayers.Add(target))
            {
                target.StartKnockback(transform.position);
            }
        }
    }
    public void OnAttackAnimationEnd()
    {
        AttackRecoveryAsync().Forget();
    }
    private async UniTask AttackRecoveryAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(attackRecoveryTime));
        currentState = PlayerState.Normal;
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
    public void UseItem()
    {
        if (currentState != PlayerState.Normal)
        {
            return;
        }
        Debug.Log("UseItem");
    }
    public void StartKnockback(Vector3 attackerPosition)
    {
        if (currentState == PlayerState.Knockback)
        {
            return;
        }

        currentState = PlayerState.Knockback;

        Vector3 direction = transform.position - attackerPosition;
        direction.y = 0;
        direction.Normalize();

        knockbackVelocity = direction * knockbackPower;
        verticalVelocity = knockbackUpPower;
    }

}
