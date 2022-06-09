using UnityEngine;

public class ActivatePurchasableItem : MonoBehaviour
{
    [SerializeField] private string enhancementName = "";

    /// <summary>
    /// Активирует объекты в зависимости от того, куплены ли улучшения для них.
    /// </summary>
    private void OnEnable()
    {
        var reader = QSReader.Create("PurchasedEnhancements");
        gameObject.SetActive(reader.Exists(enhancementName));
    }
}
