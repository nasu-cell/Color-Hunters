using UnityEngine;
using static PlayerController;
public class PaintMine : MonoBehaviour
{
    [SerializeField] private Collider hitPoint;
    [SerializeField] private float damage;
    [SerializeField] private float knockbackPower;
    [SerializeField] private float knockbackUpPower;
    [SerializeField] private Vector3 center;
    [SerializeField] private Vector3 halfExtents;
    private int ownerId;
    private Color ownerColor;
    private PlayerMineController ownerMineController;
    public void Initialize(int id, Color color, PlayerMineController mineController)
    {
        ownerId = id;
        ownerColor = color;
        ownerMineController = mineController;
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerController targetController = other.GetComponentInParent<PlayerController>();
        PlayerStatus targetStatus = other.GetComponentInParent<PlayerStatus>();

        if (targetController == null || targetStatus == null)
        {
            return;
        }
        if (targetStatus.PlayerId == ownerId)
        {
            return;
        }
        targetController.StartKnockback(transform.position, knockbackPower, knockbackUpPower);
        targetStatus.Damage(damage);
        
        PaintTiles(center, halfExtents);

        ownerMineController.RemoveMine(this);
        Destroy(gameObject);
    }
    public void PaintTiles(Vector3 center, Vector3 halfExtents)
    {
        Collider[] hits = Physics.OverlapBox(transform.position + center, halfExtents);

        foreach (Collider hit in hits)
        {
            Tile tile = hit.GetComponent<Tile>();

            if (tile == null) continue;

            tile.Paint(ownerId, ownerColor);
        }
    }
}
