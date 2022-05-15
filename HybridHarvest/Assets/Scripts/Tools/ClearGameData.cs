using CI.QuickSave;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearGameData : MonoBehaviour
{
    [SerializeField] private Inventory Inventory;
    [SerializeField] InventoryDrawer InventoryFrame;
    [SerializeField] GameObject[] RewatchButtons; // кнопки просмотра начальных роликов

    public void ClearAll()
    {
        var quickSavePath = Path.Combine(QuickSaveGlobalSettings.StorageLocation, "QuickSave");
        if (Directory.Exists(quickSavePath))
            Directory.Delete(quickSavePath, true);

        ResetPlayerPrefs();
        ClearExhibition();

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
        if (QSReader.Create("GameState").Exists("GameInitialised") || RewatchButtons == null) return;

        foreach (var btn in RewatchButtons)
            btn.SetActive(false);
    }

    /// <summary>
    /// Перезапускает туториал
    /// </summary>
    public void WatchTutorial()
    {
        ClearAll();
        ChangeStartSceneOrDisable();

        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Выходит с начальной сцены, либо деактивирует текущий объект (использовать на последнем слайде)
    /// </summary>
    public void ChangeStartSceneOrDisable()
    {
        const string key = "GameInitialised";

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
    /// Закрывает игру
    /// </summary>
    public void CloseGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Удаляет все дочерние объекты
    /// </summary>
    public static void ClearChildren(GameObject obj)
    {
        var allChildren = new HashSet<GameObject>();

        foreach (Transform child in obj.transform)
            allChildren.Add(child.gameObject);

        foreach (var child in allChildren)
            Destroy(child);
    }

    /// <summary>
    /// Очищает данные выставки
    /// </summary>
    private static void ClearExhibition()
    {
        GameObject.Find("Exhibition")?.SetActive(false);
        var writer = QuickSaveWriter.Create("ExhibitionData");
        writer.Write("ExhSeed", "no");
        writer.Commit();
    }

    /// <summary>
    /// Приводит статистику игрока к дефолтным значениям
    /// </summary>
    private static void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
