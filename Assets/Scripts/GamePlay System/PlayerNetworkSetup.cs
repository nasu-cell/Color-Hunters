using Fusion;
using UnityEngine;

// プレイヤーPrefabに追加するスクリプト
// 「このプレイヤーは自分のものか、他人のものか」で既存コンポーネントのON/OFFを切り替える
public class PlayerNetworkSetup : NetworkBehaviour
{
    private PlayerInputHandler inputHandler;
    private PlayerController playerController;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        playerController = GetComponent<PlayerController>();
    }

    // ネットワーク上に生成され、同期の準備が整ったときに全員のPCで呼ばれる
    public override void Spawned()
    {
        // HasStateAuthority = 「このオブジェクトの状態を書き込む権限を自分が持っているか」
        bool isMine = HasStateAuthority;

        name = $"Player_{Object.StateAuthority}_{(isMine ? "Mine" : "Remote")}";

        if (isMine)
        {
            // 自分のプレイヤー: 入力を受け付け、カメラを追従させる
            inputHandler.enabled = true;

            CameraController cam = FindFirstObjectByType<CameraController>();
            if (cam != null)
            {
                cam.SetTarget(transform);
            }
            else
            {
                Debug.LogWarning("CameraController がシーンに見つかりません");
            }
        }
        else
        {
            // 他人のプレイヤー(プロキシ): 自分では動かさない
            // 位置は NetworkTransform が相手のPCから受け取った値で更新する
            inputHandler.enabled = false;
            playerController.enabled = false;
        }
    }
}
