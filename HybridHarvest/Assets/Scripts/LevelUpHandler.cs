using System;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpHandler : MonoBehaviour
{
    [SerializeField] private LevelUpHandler LevelUpBannerPrefab;
    [SerializeField] private Text description;

    public Action LastAction { get; set; }
    private LevelUpHandler panel;

    public void SpawnLevelUpBanner(int level)
    {
        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        panel = Instantiate(LevelUpBannerPrefab, canvas.transform, false);
        panel.description.text = level switch
        {
            // 2 => "Поздравляем! Вы достигли второго уровня! В награду за повышение уровня открываются новые предметы в магазине, а также улучшается различные характеристики.",
            _ => $"Вы достигли уровня {level}!"
        };
        panel.LastAction = LastAction;
        
        var writer = QuickSaveWriter.Create("LevelState");
        writer.Write($"LevelUp{level}", true);
        writer.Commit();
    }

    public void Close()
    {
        LastAction?.Invoke();
        Destroy(gameObject);
    }
}
