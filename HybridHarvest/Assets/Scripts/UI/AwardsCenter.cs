using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class AwardsCenter : MonoBehaviour
{
    [SerializeField] GameObject awardsPanelPrefab;

    public GameObject awardPrefab;
    public IEnumerable<Award> currentAwards { get; private set; }
    private GameObject currentAwardsPanel;

    /// <summary>
    /// ������� �� ������ �������.
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
                    tmPro.text = $"<sprite=0> x {aw.Money}";
                    tmPro.spriteAsset = (TMP_SpriteAsset)Resources.Load($"TMP_Assets\\Money");
                    break;
                case AwardType.Seed:
                    tmPro.text = $"<sprite name=\"{aw.SeedName}\"> x 1";
                    tmPro.spriteAsset = (TMP_SpriteAsset)Resources.Load($"TMP_Assets\\Seeds");
                    break;
                case AwardType.Reputation:
                    tmPro.text = $"<sprite=0> x {aw.Reputation}";
                    tmPro.spriteAsset = (TMP_SpriteAsset)Resources.Load($"TMP_Assets\\Reputation");
                    break;
                case AwardType.Achievement:
                    tmPro.text = $"���������� x 1";
                    break;
            }
        }
    }

    /// <summary>
    /// ������������ ��������� ������. �������� ������ �� ������ �������.
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
                    targetInventory.ChangeMoney(aw.Money);
                    targetInventory.SaveAllData();
                    break;
                case AwardType.Seed:
                    var seed = (Seed)Resources.Load("Seeds\\" + aw.SeedName);
                    targetInventory.AddItem(seed, true);
                    targetInventory.SaveAllData();
                    break;
                case AwardType.Reputation:
                    targetInventory.ChangeReputation(aw.Reputation);
                    targetInventory.SaveAllData();
                    break;
                case AwardType.Achievement:
                    GetComponent<NotificationCenter>().Show(aw.Message);
                    break;
            }
        }

        // ������� ������� ������
        Destroy(gameObject);
    }
}
