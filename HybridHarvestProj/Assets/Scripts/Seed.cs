using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "seeds", menuName = "Seed")]
[System.Serializable]
public class Seed : ScriptableObject
{
    public string Name;
    public int Price;
    public Sprite MyImage;
    [SerializeField] public int GrowTime;

    public Seed(string name, int price, string image)
    {
        Name = name;
        Price = price;
        MyImage =  Resources.Load<Sprite>("SeedsIcons\\"+image);
        if (MyImage == null)
            Debug.Log("not ok 1");
        if (Resources.Load("SeedsIcons\\appl") == null)
            Debug.Log("not ok 2");
    }
    public Seed(string data)
    {
        var parameters = data.Split('|');
        Name = parameters[0];
            Price = int.Parse(parameters[1]);
        MyImage = Resources.Load<Sprite>("SeedsIcons\\" + parameters[2]);
        GrowTime = int.Parse(parameters[3]);
    }

    public override string ToString()
    {
        return Name+"|"+Price+"|"+MyImage.name+"|"+GrowTime;
    }
}
