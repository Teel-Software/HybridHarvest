using UnityEngine;

/// <summary>
/// Делает объект кнопки активным при достижении определённого уровня.
/// </summary>
public class ActivateOnLevel : MonoBehaviour
{
    [SerializeField] private string enhancementName = "";

    /// <summary>
    /// Активирует объекты в зависимости от того, куплены ли улучшения для них.
    /// </summary>
    private void Start()
    {
        var reader = QSReader.Create("PurchasedEnhancements");
        gameObject.SetActive(reader.Exists(enhancementName));
    }
}
