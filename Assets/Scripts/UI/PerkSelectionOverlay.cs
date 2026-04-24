using System.Collections.Generic;
using UnityEngine;

public class PerkSelectionOverlay : MonoBehaviour
{
    [Header("Perk Pool")]
    [SerializeField] private PerkData[] perkPool;

    [Header("UI")]
    [SerializeField] private PerkCard cardPrefab;
    [SerializeField] private Transform cardsContainer;

    [Header("Player References")]
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] private Spaceship spaceship;

    public bool IsDone { get; private set; } = false;

    private const int CardsToShow = 3;

    void OnEnable()
    {
        IsDone = false;

        if (playerShooting != null) playerShooting.enabled = false;
        if (spaceship != null) spaceship.enabled = false;

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetMenuCursor();
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        BuildCards();
    }

    void OnDisable()
    {
        if (playerShooting != null) playerShooting.enabled = true;
        if (spaceship != null) spaceship.enabled = true;

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetGameplayCursor();
    }

    private void BuildCards()
    {
        for (int i = cardsContainer.childCount - 1; i >= 0; i--)
            Destroy(cardsContainer.GetChild(i).gameObject);

        var pool = new List<PerkData>(perkPool);
        int count = Mathf.Min(CardsToShow, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, pool.Count);
            PerkCard card = Instantiate(cardPrefab, cardsContainer);
            card.Bind(pool[idx], OnPerkChosen);
            pool.RemoveAt(idx);
        }
    }

    private void OnPerkChosen(PerkData perk)
    {
        if (PerkManager.Instance != null)
            PerkManager.Instance.ApplyPerk(perk);

        IsDone = true;
        gameObject.SetActive(false);
    }
}
