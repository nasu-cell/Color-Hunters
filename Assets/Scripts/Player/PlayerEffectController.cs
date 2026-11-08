using UnityEngine;

public class PlayerEffectController : MonoBehaviour
{
    [SerializeField] ParticleSystem dashItemEffect;
    [SerializeField] ParticleSystem paintItemEffect;
    private PlayerStatus playerStatus;
    private PlayerController playerController;
    void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.IsDashing)
        {
            if (!dashItemEffect.isPlaying)
            {
                dashItemEffect.Play();
            }
        }
        else
        {
            if (dashItemEffect.isPlaying)
            {
                dashItemEffect.Stop();
            }
        }
    }
}
