using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private PlayerController controller;
    private bool isDash;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        controller = GetComponent<PlayerController>();
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
    void Update()
    {
        Vector2 input = inputActions.GamePlay.Move.ReadValue<Vector2>();
        Vector3 direction = new Vector3(input.x, 0, input.y);

        //ベクトルの正規化
        direction = Vector3.ClampMagnitude(direction, 1f);

        // ダッシュ
        isDash = inputActions.GamePlay.Dash.IsPressed();
        controller.SetMoveInput(direction, isDash);

        // 近接攻撃
        if (inputActions.GamePlay.Punch.WasPressedThisFrame())
        {
            controller.Attack();
        }

        //射撃攻撃
        if (inputActions.GamePlay.Shot.WasPressedThisFrame())
        {
            controller.StartCharging();
        }
        if (inputActions.GamePlay.Shot.WasReleasedThisFrame())
        {
            controller.ReleaseShot();
        }

        //地雷設置
        if (inputActions.GamePlay.Mine.WasPressedThisFrame())
        {
            controller.SetMine();
        }

        // ジャンプ
        if (inputActions.GamePlay.Jump.WasPressedThisFrame())
        {
            controller.Jump();
        }

        // アイテム使用
        if (inputActions.GamePlay.UseItem.WasPressedThisFrame())
        {
            controller.UseItem();
        }

        // ポーズ
        if (inputActions.GamePlay.Pause.WasPressedThisFrame())
        {
            Debug.Log("Pause");
        }
    }
}
