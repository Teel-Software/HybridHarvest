using UnityEngine;
using UnityEngine.UI;

public class MarketButton : MonoBehaviour
{
    [SerializeField] public GameObject ConfirmationPanel;
    [SerializeField] public Inventory targetInventory;

    /// <summary>
    /// создаёт подтверждающую панель в магазине
    /// </summary>
    /// <param name="name">Имя семечка</param>
    public void PrepareConfirmation(string name)
    {
        var panelObj = Instantiate(ConfirmationPanel, GameObject.Find("Shop").transform);
        var panel = panelObj.transform.Find("Panel");
        var text = panel.transform.Find("QuestionText").GetComponent<Text>();
        var yes = panel.transform.Find("YesButton").GetComponent<Button>();
        var script = panel.transform.Find("YesButton").GetComponent<ConfirmationPanelLogic>();

        script.targetInventory = targetInventory;
        text.text = "Купить";
        script.HasPrice = true;
        yes.onClick.AddListener(script.AddOneMore);

        script.DefineItem(name);

        panelObj.SetActive(true);
    }
}
