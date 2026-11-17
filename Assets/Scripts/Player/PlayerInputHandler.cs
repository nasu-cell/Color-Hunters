using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
#if UNITY_EDITOR
using Unity.Multiplayer.PlayMode;
#endif

public class PlayerInputHandler : MonoBehaviour
{
    public enum DeviceMode
    {
        All,               // 全デバイス(本番・2台のPCで遊ぶとき)
        KeyboardMouseOnly, // キーボード&マウスのみ
        GamepadOnly,       // ゲームパッドのみ
    }
    [SerializeField] private DeviceMode deviceMode = DeviceMode.All;
    private PlayerInputActions inputActions;
    private PlayerController controller;
    private bool isDash;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        controller = GetComponent<PlayerController>();
        ResolveDeviceMode();
    }
    private void OnEnable()
    {
        inputActions.Enable();
        ApplyDeviceMode();
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

    // どのデバイスモードを使うか決める
    // 優先順位: MPPMのタグ(エディタのみ) > 起動引数(ビルド) > Inspectorの設定
    private void ResolveDeviceMode()
    {
        // ビルド版: 起動引数 -inputDevice keyboard / gamepad / all
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] != "-inputDevice") continue;
            switch (args[i + 1].ToLower())
            {
                case "keyboard": deviceMode = DeviceMode.KeyboardMouseOnly; break;
                case "gamepad": deviceMode = DeviceMode.GamepadOnly; break;
                case "all": deviceMode = DeviceMode.All; break;
            }
        }

#if UNITY_EDITOR
        // エディタ: Multiplayer Play Mode のPlayer Tagで切り替える
        var tags = CurrentPlayer.Tags;
        if (tags.Contains("Keyboard")) deviceMode = DeviceMode.KeyboardMouseOnly;
        if (tags.Contains("Gamepad")) deviceMode = DeviceMode.GamepadOnly;
#endif
    }

    // InputActionsが反応するデバイスを制限する
    private void ApplyDeviceMode()
    {
        if (deviceMode == DeviceMode.All)
        {
            inputActions.devices = null; // 制限なし
            return;
        }

        var list = new List<InputDevice>();
        if (deviceMode == DeviceMode.KeyboardMouseOnly)
        {
            if (Keyboard.current != null) list.Add(Keyboard.current);
            if (Mouse.current != null) list.Add(Mouse.current);
        }
        else
        {
            foreach (var pad in Gamepad.all) list.Add(pad);
        }
        inputActions.devices = new ReadOnlyArray<InputDevice>(list.ToArray());
        Debug.Log($"[Input] DeviceMode = {deviceMode}");
    }
}
