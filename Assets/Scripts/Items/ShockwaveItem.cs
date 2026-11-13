using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class ShockwaveItem : MonoBehaviour
{ 
    [SerializeField] private Vector3 halfExtents;
    [SerializeField] private float damage;
    [SerializeField] private float knockbackPower;
    [SerializeField] private float knockbackUpPower;
    private int ownerId;
    private Color ownerColor;
    public void Initialize(int id, Color color)
    {
        ownerId = id;
        ownerColor = color;
        PaintTiles(transform.position, halfExtents);
        CheckAttackHit(transform.position, halfExtents);

        Destroy(gameObject);
    }
    public void PaintTiles(Vector3 center, Vector3 halfExtents)
    {
        Collider[] hits = Physics.OverlapBox(center, halfExtents);

        foreach (Collider hit in hits)
        {
            Tile tile = hit.GetComponent<Tile>();

            if (tile == null) continue;

            tile.Paint(ownerId, ownerColor);
        }
    }
    public void CheckAttackHit(Vector3 center, Vector3 halfExtents)
    {
        Collider[] hits = Physics.OverlapBox(center, halfExtents);

        foreach(Collider hit in hits)
        {
            PlayerController targetController = hit.GetComponent<PlayerController>();
            PlayerStatus targetStatus = hit.GetComponent<PlayerStatus>();

            if (targetController == null || targetStatus == null)
            {
                continue;
            }

            if(targetStatus.PlayerId == ownerId)
            {
                continue;
            }

            if(targetController.CurrentState != PlayerState.Respawning)
            {
                targetController.StartKnockback(transform.position, knockbackPower, knockbackUpPower);
                targetStatus.Damage(damage);
            }
        }
    }
}
