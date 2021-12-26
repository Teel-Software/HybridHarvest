using CI.QuickSave;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketButton : MonoBehaviour
{
    public GameObject ConfirmationPanel;
    public Inventory targetInventory;

    /// <summary>
    /// Создаёт подтверждающую панель в магазине
    /// </summary>
    /// <param name="seedName">Имя семечка</param>
    public void PrepareConfirmation(string seedName)
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

        script.DefineItem(seedName);

        panelObj.SetActive(true);
    }
}
