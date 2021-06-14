using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearInventory : MonoBehaviour
{
    public void ClearIt()
    {
        PlayerPrefs.SetInt("mony", 0);
        PlayerPrefs.SetInt("repa", 0);
        PlayerPrefs.SetInt("amo", 0);
    }
}
