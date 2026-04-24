using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponSelectionMenu : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private WeaponData[] availableWeapons;

    [Header("UI")]
    [SerializeField] private WeaponCard cardPrefab;
    [SerializeField] private Transform cardsContainer;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    void OnEnable()
    {
        BuildCards();
    }

    private void BuildCards()
    {
        if (cardPrefab == null || cardsContainer == null) return;

        for (int i = cardsContainer.childCount - 1; i >= 0; i--)
            Destroy(cardsContainer.GetChild(i).gameObject);

        foreach (var weapon in availableWeapons)
        {
            if (weapon == null) continue;
            if (!UnlockManager.IsWeaponUnlocked(weapon)) continue;
            WeaponCard card = Instantiate(cardPrefab, cardsContainer);
            card.Bind(weapon, HandleWeaponSelected);
        }
    }

    private void HandleWeaponSelected(WeaponData weapon)
    {
        WeaponSelection.SetWeapon(weapon);

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetGameplayCursor();

        SceneManager.LoadScene(gameSceneName);
    }
}
