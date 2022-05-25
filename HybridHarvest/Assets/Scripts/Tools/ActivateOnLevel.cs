using UnityEngine;

/// <summary>
/// Делает объект кнопки активным при достижении определённого уровня.
/// </summary>
public class ActivateOnLevel : MonoBehaviour, IUpdateable
{
    [SerializeField] private int level;
    private Inventory inventory;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }

    public void Update()
    {
        gameObject.SetActive(inventory.Level >= level);
    }
}
