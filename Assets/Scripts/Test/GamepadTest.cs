using UnityEngine;
using UnityEngine.InputSystem;
public class GamepadTest : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    void Start()
    {
        
    }

    void Update()
    {
        if (Gamepad.current == null)
        {
            return;
        }
        Vector2 input = Gamepad.current.leftStick.ReadValue();
        Vector3 direction = new Vector3(input.x, 0, input.y);
        transform.position += direction * moveSpeed * Time.deltaTime;

    }
}
