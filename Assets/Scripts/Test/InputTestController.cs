using UnityEngine;
using UnityEngine.InputSystem;
public class InputTestController : MonoBehaviour
{
    private PlayerInputActions inputActions;
    [SerializeField] private float moveSpeed = 5f;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
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
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 通常攻撃
        if (inputActions.GamePlay.Punch.WasPressedThisFrame())
        {
            Debug.Log("Attack");
        }

        // ダッシュ
        if (inputActions.GamePlay.Dash.WasPressedThisFrame())
        {
            Debug.Log("Dash");
        }

        // ジャンプ
        if (inputActions.GamePlay.Jump.WasPressedThisFrame())
        {
            Debug.Log("Jump");
        }

        // アイテム使用
        if (inputActions.GamePlay.UseItem.WasPressedThisFrame())
        {
            Debug.Log("UseItem");
        }

        // ポーズ
        if (inputActions.GamePlay.Pause.WasPressedThisFrame())
        {
            Debug.Log("Pause");
        }
    }

}
