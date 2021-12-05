using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketButton : MonoBehaviour
{
    [SerializeField] public GameObject ConfirmationPanel;
    [SerializeField] public Inventory targetInventory;

    /// <summary>
    /// ������ �������������� ������ � ��������
    /// </summary>
    /// <param name="name">��� �������</param>
    public void PrepareConfirmation(string name)
    {
        var panel = Instantiate(ConfirmationPanel, GameObject.Find("Shop").transform);
        var text = panel.transform.Find("QuestionText").GetComponent<Text>();
        var yes = panel.transform.Find("YesButton").GetComponent<Button>();
        var script = panel.transform.Find("YesButton").GetComponent<ConfirmationPanelLogic>();
        script.targetInventory = targetInventory;
        text.text = "������";
        yes.onClick.AddListener(script.AddOneMore);

        script.DefineItem(name);

        panel.SetActive(true);
    }
}
