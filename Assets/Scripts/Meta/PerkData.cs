using UnityEngine;

public enum PerkType { ExtraLife, FireRate, MoveSpeed, Damage, Shield }

[CreateAssetMenu(menuName = "Space Evader/Perk", fileName = "NewPerk")]
public class PerkData : ScriptableObject
{
    [Header("Identity")]
    public string perkName = "Perk";
    [TextArea(1, 3)] public string description = "";
    public Sprite icon;

    [Header("Effect")]
    public PerkType type;
    [Tooltip("FireRate: additive multiplier (0.15 = +15%). Damage: flat bonus (+1). Ignored for ExtraLife and Shield.")]
    public float value;
}
