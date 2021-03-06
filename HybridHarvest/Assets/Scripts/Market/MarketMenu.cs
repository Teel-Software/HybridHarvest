using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketMenu : MonoBehaviour
{
    [SerializeField] public GridLayoutGroup ScrollList;
    [SerializeField] public GameObject Listing;

    public void CloseMenu()
    {
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        foreach (var seedName in Market.PriceMultipliers.Keys)
        {
            var seed = (Seed) Resources.Load("Seeds\\" + seedName);
            if (seed == null)
            {
                Debug.Log($"Биржа: в ресурсах не найдено семечко \"{seedName}\".");
                return;
            }

            var objName = $"{seedName}Multiplier";
            var listing = Instantiate(Listing, ScrollList.transform);
            listing.name = objName;
            // Plant sprite
            listing.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>($"SeedsIcons\\{seedName}");
            // Multiplier text
            var txtComponent = listing.GetComponentInChildren<Text>();
            var multiplier = Market.PriceMultipliers[seedName];
            txtComponent.text = $"x {multiplier}";
            txtComponent.color = multiplier == 1.0f
                ? new Color(0, 0, 0)
                : multiplier > 1.0f
                    ? new Color(0f, 0.76f, 0.02f)
                    : new Color(0.75f, 0.16f, 0.13f);
            // Plant name
            var plantName = listing.GetComponentInChildren<TextMeshProUGUI>();
            plantName.text = seed.NameInRussian;
        }
    }

    private void OnDisable()
    {
        ClearGameData.ClearChildren(ScrollList.gameObject);
    }
}
