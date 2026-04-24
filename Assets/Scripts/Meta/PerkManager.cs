using UnityEngine;

public class PerkManager : MonoBehaviour
{
    public static PerkManager Instance;

    public float FireRateMultiplier { get; private set; } = 1f;
    public int DamageBonus { get; private set; } = 0;
    public float SpeedMultiplier { get; private set; } = 1f;
    public bool ShieldActive { get; private set; } = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void ApplyPerk(PerkData perk)
    {
        if (perk == null) return;
        switch (perk.type)
        {
            case PerkType.FireRate:
                FireRateMultiplier += perk.value;
                break;
            case PerkType.MoveSpeed:
                SpeedMultiplier += perk.value;
                break;
            case PerkType.Damage:
                DamageBonus += Mathf.RoundToInt(perk.value);
                break;
            case PerkType.Shield:
                ShieldActive = true;
                break;
            case PerkType.ExtraLife:
                var health = FindFirstObjectByType<PlayerHealth>();
                if (health != null) health.AddLife();
                break;
        }
    }

    public void OnShieldHit()
    {
        ShieldActive = false;
    }
}
