using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Normal,
        Attacking,
        Knockback
    }
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private bool isDashing;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float attackRecoveryTime = 0.5f;
    private PlayerAnimationController animationController;
    private PlayerState currentState;
    private CancellationTokenSource attackCancellation;
    void Awake()
    {
        animationController = GetComponent<PlayerAnimationController>();
    }

    void Update()
    {
        moveSpeed = isDashing ? dashSpeed : walkSpeed;
        if(currentState == PlayerState.Normal)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            float currentSpeed = moveDirection.magnitude * moveSpeed;
            animationController.UpdateMoveSpeed(currentSpeed);
            animationController.UpdateDash(isDashing);
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
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
        if (currentState != PlayerState.Normal)
        {
            return;
        }
        animationController.PlayJump();
    }
    public void UseItem()
    {
        if (currentState != PlayerState.Normal)
        {
            return;
        }
        Debug.Log("UseItem");
    }

}
