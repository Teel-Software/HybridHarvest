using UnityEngine;
using UnityEngine.UI;

public class OpenOnLevel : MonoBehaviour
{
    [SerializeField] private int level;
    private Inventory inventory;
    private Button btnComp;
    private Button.ButtonClickedEvent originOnClick;
    private bool isButton => btnComp != null;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        btnComp = GetComponent<Button>();

        if (!isButton) return;

        var notif = GameObject.FindGameObjectWithTag("Inventory").GetComponent<NotificationCenter>();
        originOnClick = btnComp.onClick;
        btnComp.onClick = new Button.ButtonClickedEvent();
        
        btnComp.onClick.AddListener(() =>
        {
            if (inventory.Level >= level)
            {
                btnComp.onClick.RemoveAllListeners();
                btnComp.onClick = originOnClick;
                btnComp.onClick.Invoke();
            }
            else
                notif.Show($"Откроется на уровне {level}.");
        });
    }

    // /// <summary>
    // /// Делает кнопку активной при достижении определённого уровня.
    // /// </summary>
    // private void Update()
    // {
    //     if (!isButton) return;
    //     
    //     btnComp.interactable = inventory.Level >= level;
    // }
}
