using System;
using UnityEngine;

public class ShopDrawer : MonoBehaviour
{
    [SerializeField] private RectTransform shoppingPlace;
    [SerializeField] private GameObject ItemIcon;
    private void OnEnable()
    {
        var shopLogic = GetComponent<ShopLogic>();
        foreach (var seedName in shopLogic.unlockedSeeds)
        {
            var itemIcon = Instantiate(ItemIcon, shoppingPlace.transform);
            itemIcon.name = $"Buy{seedName}";
            itemIcon.transform.localScale = new Vector3(0.9f, 0.9f);
            var itemIconDrawer = itemIcon.GetComponent<ItemIconDrawer>();
            // покупать можно нормально заданные семена
            itemIconDrawer.SetSeed((Seed)Resources.Load("Seeds\\" + seedName));
            itemIconDrawer.Button.onClick.AddListener(() => shopLogic.PrepareConfirmation(seedName));
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in shoppingPlace.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
