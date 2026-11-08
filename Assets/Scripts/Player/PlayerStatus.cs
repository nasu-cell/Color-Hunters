using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("基本情報")]
    [SerializeField] private int playerId;
    [SerializeField] private Color playerColor;

    [Header("能力値")]
    [SerializeField] private Vector3 paintRange;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float knockbackPower;
    [SerializeField] private float knockbackUpPower;
    [SerializeField] private float knockbackMultiplier;

    [Header("スタミナ")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaConsumption;
    [SerializeField] private float staminaRecovery;
    [SerializeField] private float stamina;

    [Header("アイテム能力向上")]
    [SerializeField] private bool isPaintRangeExpanded;
    [SerializeField] private bool isDashUnlimited;

    public int PlayerId => playerId;
    public Color PlayerColor => playerColor;
    public Vector3 PaintRange => paintRange;
    public float WalkSpeed => walkSpeed;
    public float DashSpeed => dashSpeed;
    public float KnockbackPower => knockbackPower;
    public float KnockbackUpPower => knockbackUpPower;
    public float KnockbackMultiplier => knockbackMultiplier;
    public float Stamina => stamina;
    public float MaxStamina => maxStamina;
    public bool IsPaintRangeExpanded => isPaintRangeExpanded;
    public bool IsDashUnlimited => isDashUnlimited;

    private void Awake()
    {
        stamina = maxStamina;
    }
    //スタミナ消費処理
    public void ConsumeStamina(float deltaTime)
    {
        stamina -= staminaConsumption * deltaTime;
        stamina = Mathf.Max(stamina, 0f);
    }

    public void RecoverStamina(float deltaTime)
    {
        stamina += staminaRecovery * deltaTime;
        stamina = Mathf.Min(stamina, maxStamina);
    }
    //アイテム取得時処理
    public void SetDashUnlimited(bool value)
    {
        isDashUnlimited = value;
    }

    public void SetPaintRangeExpanded(bool value)
    {
        isPaintRangeExpanded = value;
    }
}