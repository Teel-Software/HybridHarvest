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
        panel.description.text = $"Вы достигли уровня {level}!\n";

        var dialogPanel = GameObject.FindGameObjectWithTag("DialogPanel");
        if (dialogPanel != null)
            panel.transform.SetSiblingIndex(dialogPanel.transform.GetSiblingIndex());

        var addition = level switch
        {
            2 => "Вам открыты:" +
                 "\n- Семена помидоров\n- Дополнительное место в инвентаре\n- Доступ к заданиям\n- Доступ к выставке",
            3 => "Вам открыты:\n- Доступ к лаборатории\n- Дополнительная энергия",
            4 => "Вам открыты:\n- Семена гороха\n- Дополнительное место в инвентаре",
            5 => "Вам открыты:\n- Доступ к КВАНТу\n- Дополнительная энергия\n- Дополнительная грядка на поле",
            6 => "Вам открыты:\n- Семена картофеля\n- Дополнительное место в инвентаре",
            7 => "Вам открыты:\n- Дополнительный горшок в лаборатории\n- Дополнительная энергия",
            8 => "Вам открыты:\n- Семена моркови\n- Дополнительное место в инвентаре",
            9 => "Вам открыты:\n- Возможность выкапывать семена на грядке\n- Дополнительная энергия",
            10 => "Вам открыты:\n- Доступ к битве с боссом на выставке\n- Дополнительная энергия",
            // 11 => "Вам открыты:\n- Книга рецептов\n- Возможность изменения названия культуры",
            12 => "Вам открыты:\n- Улучшение семян в магазине\n- Дополнительная грядка на поле",
            13 => "Вам открыты:" +
                  // "\n- Дополнительный горшок в лаборатории" +
                  "\n- Дополнительная энергия",
            14 => "Вам открыты:" +
                  // "\n- Семена культуры" +
                  "\n- Дополнительное место в инвентаре",
            15 => "Вам открыты:" +
                  // "\n- Второе скрещивание КВАНТа" +
                  "\n- Дополнительная энергия",
            _ => ""
        };
        panel.description.text += addition;

        if (addition == "")
            panel.description.alignment = TextAnchor.UpperCenter;

        panel.LastAction = LastAction;
        LastAction = null;

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
