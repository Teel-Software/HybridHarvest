using CI.QuickSave;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearGameData : MonoBehaviour
{
    [SerializeField] Inventory Inventory;
    [SerializeField] InventoryDrawer InventoryFrame;
    [SerializeField] GameObject[] RewatchButtons; // кнопки просмотра начальных роликов

    public void ClearAll()
    {
        PlayerPrefs.DeleteAll();

        var quickSavePath = Path.Combine(QuickSaveGlobalSettings.StorageLocation, "QuickSave");
        if (Directory.Exists(quickSavePath))
            Directory.Delete(quickSavePath, true);

        ClearPlayerStats();

        PlayerPrefs.Save();

        if (Inventory != null)
            Inventory.Awake();
        if (InventoryFrame != null)
            InventoryFrame.Redraw();
    }

    /// <summary>
    /// Выключает кнопки просмотра начальных роликов
    /// </summary>
    public void DisableRewatchButtons()
    {
        if (!QSReader.Create("GameState").Exists("GameInitialised") && RewatchButtons != null)
            foreach (var btn in RewatchButtons)
                btn.SetActive(false);
    }

    /// <summary>
    /// Перезапускает туториал
    /// </summary>
    public void WatchTutorial()
    {
        var tutorialSavePath = Path.Combine(QuickSaveGlobalSettings.StorageLocation, "QuickSave\\TutorialState.json");
        if (File.Exists(tutorialSavePath))
            File.Delete(tutorialSavePath);
        else Debug.Log($@"Файл {tutorialSavePath} не найден.");

        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Выходит с начальной сцены, либо деактивирует текущий объект (использовать на последнем слайде)
    /// </summary>
    public void ChangeStartSceneOrDisable()
    {
        var key = "GameInitialised";

        if (!QSReader.Create("GameState").Exists(key))
        {
            var writer = QuickSaveWriter.Create("GameState");
            writer.Write(key, true);
            writer.Commit();

            SceneManager.LoadScene(1);
        }
        else gameObject.SetActive(false);
    }

    /// <summary>
    /// Удаляет все дочерние объекты
    /// </summary>
    public void ClearChildren(GameObject obj)
    {
        var allChildren = new HashSet<GameObject>();

        foreach (Transform child in obj.transform)
            allChildren.Add(child.gameObject);

        foreach (GameObject child in allChildren)
            Destroy(child);
    }

    /// <summary>
    /// Закрывает игру
    /// </summary>
    public void CloseGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Приводит статистику игрока к дефолтным значениям
    /// </summary>
    private void ClearPlayerStats()
    {
        PlayerPrefs.SetInt("money", 100);
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
