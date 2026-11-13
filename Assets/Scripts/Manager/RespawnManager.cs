using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] PlayerStatus playerStatus0;
    [SerializeField] PlayerStatus playerStatus1;
    [SerializeField] PlayerController playerController0;
    [SerializeField] PlayerController playerController1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerStatus0.CurrentHP <= 0 && playerController0.CurrentState != PlayerController.PlayerState.Respawning)
        {
            playerStatus0.ResetHP();
            playerController0.StartRespawn();
        }
        if (playerStatus1.CurrentHP <= 0 && playerController1.CurrentState != PlayerController.PlayerState.Respawning)
        {
            playerStatus1.ResetHP();
            playerController1.StartRespawn();
        }
    }
}
