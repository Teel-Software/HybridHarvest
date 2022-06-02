using UnityEngine;

/// <summary>
/// Делает объект кнопки активным при достижении определённого уровня.
/// </summary>
public class ActivateOnLevel : MonoBehaviour, IUpdateable
{
    // [SerializeField] private int level;
    [SerializeField] private string enhancementName = "";

    // private Inventory inventory;

    private void Start()
    {
        // inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

        var reader = QSReader.Create("PurchasedEnhancements");
        gameObject.SetActive(reader.Exists(enhancementName));
    }

    public void Update()
    {
        // gameObject.SetActive(inventory.Level >= level);
    }
}
