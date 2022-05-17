using TMPro;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Определяет возможный приз
/// </summary>
public enum AwardType
{
    Money,
    Seed,
    Achievement,
    Reputation
}

/// <summary>
/// Содержит приз и его компоненты
/// </summary>
public class Award
{
    /// <summary>
    /// Содержит приз и его компоненты
    /// </summary>
    public Award(AwardType currentPrize, string message = "", int amount = 0, string seedName = "")
    {
        CurrentPrize = currentPrize;
        Message = message;
        Amount = amount;
        SeedName = seedName;
    }

    public AwardType CurrentPrize { get; }
    public string Message { get; }
    public int Amount { get; }
    public string SeedName { get; }
}

public class AwardsCenter : MonoBehaviour
{
    [SerializeField] private GameObject awardsPanelPrefab;

    public GameObject awardPrefab;
    public IEnumerable<Award> currentAwards { get; private set; }
    private GameObject currentAwardsPanel;

    /// <summary>
    /// Выводит на панель награды.
    /// </summary>
    public void Show(IEnumerable<Award> awards)
    {
        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        currentAwardsPanel = Instantiate(awardsPanelPrefab, canvas.transform, false);
        awardPrefab = currentAwardsPanel.GetComponent<AwardsCenter>().awardPrefab;
        var awPlace = GameObject.FindGameObjectWithTag("AwardsPlace");
        currentAwardsPanel.GetComponent<AwardsCenter>().currentAwards = awards;

        foreach (var aw in awards)
        {
            var newAw = Instantiate(awardPrefab, awPlace.transform, false);
            var tmPro = newAw.GetComponent<TextMeshProUGUI>();

            switch (aw.CurrentPrize)
            {
                case AwardType.Money:
                    tmPro.text = $"<sprite=0> x {aw.Amount}";
                    tmPro.spriteAsset = (TMP_SpriteAsset) Resources.Load($"TMP_Assets\\Money");
                    break;
                case AwardType.Reputation:
                    tmPro.text = $"<sprite=0> x {aw.Amount}";
                    tmPro.spriteAsset = (TMP_SpriteAsset) Resources.Load($"TMP_Assets\\Reputation");
                    break;
                case AwardType.Seed:
                    tmPro.text = $"<sprite=0> x 1";
                    tmPro.spriteAsset = (TMP_SpriteAsset) Resources.Load($"TMP_Assets\\{aw.SeedName}Packet");
                    break;
                case AwardType.Achievement:
                    tmPro.text = $"Достижение x 1";
                    break;
            }
        }
    }

    /// <summary>
    /// Осуществляет получение наград. Вызывать ТОЛЬКО из самого префаба.
    /// </summary>
    public void ApplyAwards()
    {
        var targetInventory = GameObject
            .FindGameObjectWithTag("Inventory")
            .GetComponent<Inventory>();

        foreach (var aw in currentAwards)
        {
            switch (aw.CurrentPrize)
            {
                case AwardType.Money:
                    targetInventory.AddMoney(aw.Amount);
                    targetInventory.Save();
                    break;
                case AwardType.Reputation:
                    targetInventory.ChangeReputation(aw.Amount);
                    targetInventory.Save();
                    break;
                case AwardType.Seed:
                    var seed = (Seed) Resources.Load("Seeds\\" + aw.SeedName);
                    targetInventory.AddItem(seed, true);
                    targetInventory.Save();
                    break;
                case AwardType.Achievement:
                    GetComponent<NotificationCenter>().Show(aw.Message);
                    break;
            }
        }

        // Продолжает диалог, если таковой имеется
        var dPanel = GameObject.FindGameObjectWithTag("DialogPanel");
        if (dPanel?.activeSelf == true)
            dPanel.GetComponent<DialogPanelLogic>().LoadNextPhrase();

        // удаляет текущую панель
        Destroy(gameObject);
    }
}
