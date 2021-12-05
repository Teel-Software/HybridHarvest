using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearGameData : MonoBehaviour
{
    [SerializeField] Inventory Inventory;
    [SerializeField] Drawinventory InventoryFrame;
    [SerializeField] GameObject RewatchButton; // кнопка просмотра вступления

    public void ClearAll()
    {
        PlayerPrefs.DeleteAll();

        ClearPlayerStats();

        PlayerPrefs.Save();

        Inventory.Awake();
        InventoryFrame.Redraw();
    }

    /// <summary>
    /// Выключает кнопку просмотра вступления
    /// </summary>
    public void DisableRewatchButton()
    {
        if (!PlayerPrefs.HasKey("GameInitialised") && RewatchButton != null)
            RewatchButton.SetActive(false);
    }

    /// <summary>
    /// Выходит с начальной сцены, либо деактивирует текущий объект (использовать на последнем слайде)
    /// </summary>
    public void ChangeStartSceneOrDisable()
    {
        if (!PlayerPrefs.HasKey("GameInitialised"))
        {
            PlayerPrefs.SetInt("GameInitialised", 1);
            SceneManager.LoadScene(1);
        }
        else gameObject.SetActive(false);
    }

    /// <summary>
    /// Удаляет все дочерние объекты
    /// </summary>
    public void DeleteChildren(GameObject obj)
    {
        var allChildren = new HashSet<GameObject>();

        foreach (Transform child in obj.transform)
            allChildren.Add(child.gameObject);

        foreach (GameObject child in allChildren)
            Destroy(child);
    }

    private void ClearPlayerStats()
    {
        PlayerPrefs.SetInt("money", 0);
        PlayerPrefs.SetInt("reputation", 0);
        PlayerPrefs.SetInt("reputationLimit", 500);
        PlayerPrefs.SetInt("reputationLevel", 1);
        PlayerPrefs.SetInt("amount", 0);
        PlayerPrefs.SetInt("energyMax", 10);
        PlayerPrefs.SetInt("energy", 0);
        PlayerPrefs.SetFloat("energytimebuffer", 0);
        PlayerPrefs.SetString("energytime", DateTime.Now.ToString());
    }
}
