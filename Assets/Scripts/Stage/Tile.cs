using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private ParticleSystem tilePaintEffect;
    private int ownerId = -1;
    private Renderer tileRenderer;
    public int OwnerId => ownerId;
    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
    }
    public void Paint(int playerId, Color color)
    {
        if (ownerId == playerId) return;
        ownerId = playerId;
        tileRenderer.material.color = color;

        var main = tilePaintEffect.main;
        main.startColor = color;
        
        tilePaintEffect.Play();
    }
}
