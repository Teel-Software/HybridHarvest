using UnityEngine;
using Random = System.Random;

public class StatPanelTest : MonoBehaviour
{
    [SerializeField] public GameObject StatPanel;
    
    public void LetsGo()
    {
        var inv = GameObject.Find("DataKeeper").GetComponent<Inventory>();
        var rand = new Random();
        var num = rand.Next(0, inv.Elements.Count);
        
        var stat = Instantiate(StatPanel, GameObject.Find("Canvas").transform);
        var statDrawer = stat.GetComponentInChildren<StatPanelDrawer>();
        
        var seedItem = inv.Elements[num];
        statDrawer.DisplayStats(seedItem);
    }
}
