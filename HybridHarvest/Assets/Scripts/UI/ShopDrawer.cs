using System;
using UnityEngine;

public class ShopDrawer : MonoBehaviour
{
    [SerializeField] private RectTransform shoppingPlace;
    [SerializeField] private GameObject ItemIcon;
    private void OnEnable()
    {
        var shopLogic = GetComponent<ShopLogic>();
        shopLogic.Awake();
        foreach (var seedName in shopLogic.unlockedSeeds)
        {
            var itemIcon = Instantiate(ItemIcon, shoppingPlace.transform);
            itemIcon.name = $"Buy{seedName}";
            itemIcon.transform.localScale = new Vector3(0.9f, 0.9f);
            var itemIconDrawer = itemIcon.GetComponent<ItemIconDrawer>();
            // покупать можно нормально заданные семена
            // хотя через ресурсы тоже вроде норм
            var seed = (Seed)Resources.Load("Seeds\\" + seedName);
            itemIconDrawer.SetSeed(seed);
            itemIconDrawer.Button.onClick.AddListener(() => shopLogic.PrepareConfirmation(seed));
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
