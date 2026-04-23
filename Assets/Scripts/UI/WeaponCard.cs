using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private TMP_Text descriptionLabel;
    [SerializeField] private TMP_Text statsLabel;
    [SerializeField] private Button selectButton;

    private WeaponData weapon;
    private Action<WeaponData> onSelected;

    public void Bind(WeaponData data, Action<WeaponData> onSelectedCallback)
    {
        weapon = data;
        onSelected = onSelectedCallback;

        if (nameLabel != null) nameLabel.text = data.weaponName;
        if (descriptionLabel != null) descriptionLabel.text = data.description;
        if (statsLabel != null) statsLabel.text = BuildStats(data);

        if (iconImage != null)
        {
            iconImage.sprite = data.icon;
            iconImage.enabled = data.icon != null;
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        onSelected?.Invoke(weapon);
    }

    private static string BuildStats(WeaponData d)
    {
        string fire = d.autoFire ? "Auto" : "Manual";
        string charge = d.requiresCharging ? $" + Charge {d.chargeTime:0.#}s" : "";
        return $"DMG {d.damage}   CD {d.shootCooldown:0.##}s\n{fire}{charge}";
    }
}
