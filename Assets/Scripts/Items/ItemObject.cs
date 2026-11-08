using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemType itemType;   
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.gameObject.GetComponent<PlayerInventory>();

            if (playerInventory == null) return;

            if (itemType == ItemType.Dash)
            {
                playerInventory.GetDashItem();
                Destroy(gameObject);
            }
            else if (itemType == ItemType.PaintRange)
            {
                playerInventory.GetPaintItem();
                Destroy(gameObject);
            }
            else
            {
                playerInventory.GetShockwaveItem();
                Destroy(gameObject);
            }
        }
    }
}
