using UnityEngine;

public class TilePaintSystem : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private PlayerController playerController;

    private void Awake()
    {
        playerStatus = GetComponent<PlayerStatus>();
        playerController = GetComponent<PlayerController>();
    }
    private void Update()
    {
        Vector3 center = transform.position;
        Vector3 halfExtents = playerStatus.PaintRange / 2f;

        if (playerStatus.IsPaintRangeExpanded)
        {
            halfExtents *= 3f;
        }
        if (playerController.CurrentState != PlayerController.PlayerState.Normal)
        {
            return;
        }
        PaintTiles(center, halfExtents);
    }
    public void PaintTiles(Vector3 center, Vector3 halfExtents)
    {
        Collider[] hits = Physics.OverlapBox(center, halfExtents);

        foreach (Collider hit in hits)
        {
            Tile tile = hit.GetComponent<Tile>();

            if (tile == null) continue;

            tile.Paint(playerStatus.PlayerId, playerStatus.PlayerColor);
        }
    }
}