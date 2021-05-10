using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="seeds", menuName ="Seed")]
[System.Serializable]
public class Seed : ScriptableObject
{
    public string Name;
    public int Price;
    public Sprite MyImage;

    public Seed(string name, int price, string image)
    {
        Name = name;
        Price = price;
        MyImage =  Resources.Load<Sprite>("SeedsIcons\\"+image);
        //if (MyImage == null)
           // Debug.Log("not ok 1");
        //if (Resources.Load("SeedsIcons\\appl") == null)
        //    Debug.Log("not ok 2");
    }

    public override string ToString()
    {
        return Name +"|"+Price.ToString()+"|"+MyImage.name;
    }
}
