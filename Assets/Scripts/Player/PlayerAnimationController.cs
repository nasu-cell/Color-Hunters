using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    public void UpdateMoveSpeed(float speed)
    {
        animator.SetFloat("MoveSpeed", speed);
    }
    public void UpdateDash(bool isDashing)
    {
        animator.SetBool("IsDashing", isDashing);
    }
    public void PlayJump()
    {
        animator.SetTrigger("Jump");
    }
    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }
    public void OnAttackHitStart()
    {
        playerController.StartAttackHitbox();
    }
    public void OnAttackAnimationEnd()
    {
        playerController.OnAttackAnimationEnd();
    }
}
