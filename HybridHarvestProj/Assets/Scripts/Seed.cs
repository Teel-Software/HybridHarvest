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

    public Seed(string name, int price, Sprite image)
    {
        Name = name;
        Price = price;
        MyImage = image;
    }
}
