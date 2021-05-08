using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="seeds", menuName ="Seed")]
public class Seed : ScriptableObject
{
    public string name;
    public int price;
    public Sprite image;
}
