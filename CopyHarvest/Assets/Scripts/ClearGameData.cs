using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearGameData : MonoBehaviour
{
    public void ClearInventory()
    {
        PlayerPrefs.SetInt("mony", 0);
        PlayerPrefs.SetInt("repa", 0);
        PlayerPrefs.SetInt("amo", 0);
    }

    public void UndoGameInitialization()
    {
        PlayerPrefs.DeleteKey("GameInitialised");
    }
}
