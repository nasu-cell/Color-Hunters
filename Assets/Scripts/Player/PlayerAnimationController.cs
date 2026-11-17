using UnityEngine;
using Fusion; //【追加】

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;
    private NetworkMecanimAnimator networkAnimator; //【追加】アニメーションを同期するFusionのコンポーネント

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        //【追加】オフライン用のキャラには付いていないので、その場合はnullになる
        networkAnimator = GetComponent<NetworkMecanimAnimator>();
    }

    // MoveSpeed と IsDashing は Animator の「値」なので、
    // NetworkMecanimAnimator が自動で相手に送ってくれる(変更なし)
    public void UpdateMoveSpeed(float speed)
    {
        animator.SetFloat("MoveSpeed", speed);
    }
    public void UpdateDash(bool isDashing)
    {
        animator.SetBool("IsDashing", isDashing);
    }

    // トリガーは NetworkMecanimAnimator 経由で設定しないと相手に届かない
    public void PlayJump()
    {
        SetTrigger("Jump"); //【変更】animator.SetTrigger → SetTrigger
    }
    public void PlayAttack()
    {
        SetTrigger("Attack"); //【変更】animator.SetTrigger → SetTrigger
    }

    // アニメーションイベントは「そのアニメーションが再生された全員の画面」で呼ばれる
    // 攻撃判定や後隙の処理は、自分のキャラのときだけ行う
    public void OnAttackHitStart()
    {
        if (!IsLocallyControlled()) return; //【追加】
        playerController.StartAttackHitbox();
    }
    public void OnAttackAnimationEnd()
    {
        Debug.Log($"[Attack] {name} 攻撃判定イベント 自分のキャラ={IsLocallyControlled()}"); // 追加
        if (!IsLocallyControlled()) return; //【追加】
        playerController.OnAttackAnimationEnd();
    }

    //【追加】オンラインなら NetworkMecanimAnimator、オフラインなら Animator でトリガーを設定する
    private void SetTrigger(string triggerName)
    {
        if (networkAnimator != null)
        {
            networkAnimator.SetTrigger(triggerName);
        }
        else
        {
            animator.SetTrigger(triggerName);
        }
    }

    //【追加】このキャラを自分が操作しているか
    private bool IsLocallyControlled()
    {
        // ネットワークに生成されていない(オフライン)なら、常に自分のキャラ
        if (playerController.Object == null) return true;
        return playerController.HasStateAuthority;
    }
}
