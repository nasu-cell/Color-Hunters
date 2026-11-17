using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

// オンライン接続の窓口となるManager
// ・NetworkRunner と「同じGameObject」に付けること
//   (Runnerは同じGameObjectにある SimulationBehaviour を自動で登録し、
//    IPlayerJoined などのコールバックを呼んでくれる)
public class NetworkManager : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkRunner runner;

    //【変更】プレイヤーPrefabを配列にする(0番目=赤, 1番目=青)
    // 要素の番号がそのまま「何番目に参加したプレイヤーが使うか」になる
    [SerializeField] private NetworkPrefabRef[] playerPrefabs;

    [SerializeField] private string sessionName = "TestRoom";
    [SerializeField] private Vector3[] spawnPoints = { new Vector3(0, 1, 0), new Vector3(3, 1, 0) };
    [SerializeField] private bool autoStart = true; // 再生したら自動で接続する

    private bool isStarting;

    private void Start()
    {
        if (autoStart)
        {
            StartShared();
        }
    }

    // Sharedモードでセッションに参加する(同じsessionNameの人と同じ部屋に入る)
    public async void StartShared()
    {
        if (isStarting) return;
        isStarting = true;

        // 今開いているシーンをネットワークシーンとして扱う
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            Scene = sceneInfo,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (result.Ok)
        {
            Debug.Log($"[Network] 接続成功 LocalPlayer={runner.LocalPlayer}");
        }
        else
        {
            Debug.LogError($"[Network] 接続失敗: {result.ShutdownReason}");
            isStarting = false;
        }
    }

    // 誰かがセッションに参加したとき「全員のPCで」呼ばれる
    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"[Network] PlayerJoined: {player} (自分={player == Runner.LocalPlayer})");

        // Sharedモードでは「自分のプレイヤーは自分でSpawnする」
        // Spawnした人がそのオブジェクトの StateAuthority(書き込み権限)を持つ
        if (player != Runner.LocalPlayer) return;

        //【追加】参加した順番から「何番目のプレイヤーか」を決める
        // AsIndex は 1人目=0, 2人目=1, ... になる
        int slot = player.AsIndex % playerPrefabs.Length;

        Vector3 pos = spawnPoints.Length > 0
            ? spawnPoints[slot % spawnPoints.Length]
            : Vector3.up;

        //【変更】slot番目のPrefab(0=赤, 1=青)を生成する
        Runner.Spawn(playerPrefabs[slot], pos, Quaternion.identity);
        Debug.Log($"[Network] {player} を slot {slot} として生成");
    }

    public void PlayerLeft(PlayerRef player)
    {
        Debug.Log($"[Network] PlayerLeft: {player}");
    }

    // テスト用の接続ボタン(autoStartがオフのときに使う。後でUIManagerのボタンに置き換える予定)
    private void OnGUI()
    {
        if (isStarting) return;
        if (GUI.Button(new Rect(10, 10, 200, 40), "Start (Shared)"))
        {
            StartShared();
        }
    }
}
