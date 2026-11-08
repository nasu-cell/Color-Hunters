using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private float itemDuration;
    [SerializeField] private int maxShockwaveCount;

    private float dashItemTime;
    private float paintItemTime;
    private int shockwaveCount;

    public float DashItemTime => dashItemTime;
    public float PaintItemTime => paintItemTime;
    public int ShockwaveCount => shockwaveCount;
    void Update()
    {
        if (dashItemTime > 0)
        {
            dashItemTime -= Time.deltaTime;
            if (dashItemTime <= 0)
            {
                dashItemTime = 0;
                playerStatus.SetDashUnlimited(false);
            }
        }
        if (paintItemTime > 0)
        {
            paintItemTime -= Time.deltaTime;
            if (paintItemTime <= 0)
            {
                paintItemTime = 0;
                playerStatus.SetPaintRangeExpanded(false);
            }
        }

    }
    public void GetDashItem()
    {
        dashItemTime = itemDuration;
        playerStatus.SetDashUnlimited(true);
    }
    public void GetPaintItem()
    {
        paintItemTime = itemDuration;
        playerStatus.SetPaintRangeExpanded(true);
    }
    public void GetShockwaveItem()
    {
        shockwaveCount = Mathf.Min(shockwaveCount + 1, maxShockwaveCount);
        Debug.Log("shockwave count: "+ shockwaveCount);
    }

    public bool UseShockwaveItem()
    {
        if (shockwaveCount <= 0)
            return false;

        shockwaveCount--;
        return true;
    }
}
