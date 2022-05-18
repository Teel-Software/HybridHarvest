using UnityEngine;
using UnityEngine.UI;

public class OpenOnLevel : MonoBehaviour
{
    [SerializeField] private int level;
    private Inventory inventory;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }

    /// <summary>
    /// Делает кнопку активной при достижении определённого уровня.
    /// </summary>
    private void Update()
    {
        var btnComp = GetComponent<Button>();
        if (btnComp == null) return;
        
        btnComp.interactable = inventory.Level >= level;
    }
}
