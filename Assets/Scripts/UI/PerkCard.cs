using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkCard : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private TMP_Text descLabel;
    [SerializeField] private Button selectButton;

    public void Bind(PerkData data, Action<PerkData> onSelected)
    {
        if (iconImage != null)
            iconImage.sprite = data.icon;
        if (nameLabel != null) nameLabel.text = data.perkName;
        if (descLabel != null) descLabel.text = data.description;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelected(data));
    }
}
