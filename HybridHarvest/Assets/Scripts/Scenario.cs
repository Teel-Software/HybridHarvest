using CI.QuickSave;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scenario : MonoBehaviour
{
    [SerializeField] DialogPanelLogic DialogPanel;
    [SerializeField] Sprite FirstCharacterSprite;
    [SerializeField] Sprite SecondCharacterSprite;
    [SerializeField] Sprite NarratorSprite;
    [SerializeField] GameObject BlockerPrefab;

    /// <summary>
    /// В подобных методах нужно создавать диалоги
    /// </summary>
    public void MakeTestDialog()
    {
        // !!! ID НАЧИНАЕТСЯ С ЦИФРЫ 1 !!!
        // ID присваивается фразе автоматически при вызове метода AddPhrase

        DialogPanel.CreateDialogPanel(FirstCharacterSprite, SecondCharacterSprite, NarratorSprite);

        DialogPanel.AddPhrase(NowTalking.Second, "Приветствую путник, вижу ты первый раз здесь."); // ID = 1
        DialogPanel.AddPhrase(NowTalking.First, "Эээээ, ну здрасьте!");


        DialogPanel.AddPhrase(NowTalking.Second, "Я - Великий Стонкс и у меня есть всевозможные товары, начиная с огурца и заканчивая морковью."); // ID = 3
        DialogPanel.AddPhrase(NowTalking.First, "1. А как же сосисочки?", 3);  // ID = 4
        DialogPanel.AddAward(4, new Award(AwardType.Achievement, message: "Достижение разблокировано: \"Вернуться в 2007\""));
        DialogPanel.AddPhrase(NowTalking.Second, "Да, и хороший борщик... Не продаём такое, к сожалению.", 4);

        DialogPanel.AddPhrase(NowTalking.First, "2. Вы любите розы?", 3); // ID = 6
        DialogPanel.AddAward(6, new Award(AwardType.Money, money: 500));
        DialogPanel.AddPhrase(NowTalking.Second, "А я на них... не вижу никакого подтекста.", 6);

        DialogPanel.AddPhrase(NowTalking.First, "3. Получить огурчик!", 3); // ID = 8
        DialogPanel.AddAward(8, new Award(AwardType.Seed, seedName: "Cucumber"));


        DialogPanel.AddPhrase(NowTalking.First, "Хочется взять что-то необычное... Я думаю, огурец выглядит неплохо, поэтому возьму Иерусалим.");
        DialogPanel.AddPhrase(NowTalking.Second, "???????????????\n[Великий Стонкс помер от такого]");
        DialogPanel.AddPhrase(NowTalking.Narrator, "Со смертью этого персонажа нить повествования обрывается.");

        DialogPanel.AddPhrase(NowTalking.Narrator, "Хочешь бесплатную репутацию?"); // ID = 12
        DialogPanel.AddPhrase(NowTalking.First, "1. Да!", 12); // ID = 13
        DialogPanel.AddAward(13, new Award(AwardType.Reputation, reputation: 500));
        DialogPanel.AddPhrase(NowTalking.First, "2. Нет!", 12);
        DialogPanel.AddPhrase(NowTalking.First, "3. Четыре!", 12);
        DialogPanel.AddPhrase(NowTalking.First, "4. Мне чизбургер без сыра, пожалуйста.", 12);

        DialogPanel.StartDialog();
    }

    public void Tutorial_Beginning()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                ExecuteTutorialPart("BeginningChoice", "SceneButtonField", FirstCharacterPhrases: new string[] {
                   "Хороший денёк, однако выдался! Помнится, вчера я хотел посадить семена, да вот забыл... Ну ничего, сделаю это сейчас!"},
                   NarratorPhrases: new string[] { "Нажмите на кнопку \"Грядка\"" },
                   award: new Award(AwardType.Seed, seedName: "Potato"));
                break;
            case 2:
                ExecuteTutorialPart("BeginningField", "FarmSpot", FirstCharacterPhrases: new string[] {
                   "Это поле. Здесь можно посадить семена, которые есть у тебя на складе. Нажми на грядку, посмотри, что будет." });
                break;
            case 3:
                ExecuteTutorialPart("BeginningLab", "ExitScene", FirstCharacterPhrases: new string[] {
                   "Это лаборатория. Здесь можно скрестить семена, которые есть у тебя на складе. Скрещивать можно только семена одного вида!" });
                break;
                //case 4:
                //    ExecuteTutorialPart("BeginningQuantum", "ExitScene",
                //       "Это К.В.А.Н.Т. Здесь можно скрестить семена, которые есть у тебя на складе. Скрещивать можно что угодно, но один раз в день!");
                //    break;
        }
    }

    //public void Tutorial_SideMenu()
    //{
    //    ExecuteTutorialPart("SideMenu",
    //        "", "Это боковое меню. Одна из самых важных частей игры. Отсюда ты можешь попасть в четыре места: магазин, задания, склад и выставку.",
    //        "Посмотри, что в каждом из этих мест находится. Помни, про кнопку выхода в левом верхнем углу. Если не видишь стрелочку - ищи крестик. Всё просто :)");
    //}

    //public void Tutorial_Shop()
    //{
    //    ExecuteTutorialPart("Shop",
    //        "", "Это магазин. Здесь можно покупать семена разных культур. Купленные пакеты семян можно найти на складе.");
    //}

    //public void Tutorial_Quests()
    //{
    //    ExecuteTutorialPart("Quests",
    //        "", "Это доска объявлений. Здесь появляеются задания от жителей, которым нужна помощь. За выполнение заданий ты получишь от них награду.");
    //}

    //public void Tutorial_Inventory()
    //{
    //    ExecuteTutorialPart("Inventory",
    //        "", "Это склад. Здесь хранятся все пакеты семян, которые ты получил. Следи за заполнением места, ведь склад не бесконечен! Посмотреть, на сколько склад заполнен можно в правом нижнем углу.");
    //}

    //public void Tutorial_Exhibition()
    //{
    //    ExecuteTutorialPart("Exhibition",
    //       "", "Это выставка. Здесь можно выставить на всеобщее обозрение свои лучшие продукты. Пусть все знают, кто тут настоящий садовод!");
    //}

    //public void Tutorial_Market()
    //{
    //    ExecuteTutorialPart("Market",
    //       "", "Это биржа. Здесь отображается текущее положение цен на рынке. Цифра справа от семечка означает, насколько изменилась его цена по сравнению с начальной.");
    //}

    //public void Tutorial_HybridPanel()
    //{
    //    ExecuteTutorialPart("HybridPanel",
    //       "", "Это панель для скрещивания. Выбери семечко слева, затем семечко справа. После этого нажми \"Скрестить\" и наслаждайся результатом.");
    //}

    /// <summary>
    /// Проигрывает часть туториала
    /// </summary>
    /// <param name="keyPart">Название, по которому идёт сохранение</param>
    /// <param name="activeButtonName">Название кнопки, которую следует сделать активной после окончания части вступления</param>
    /// <param name="NarratorPhrases">Фразы, которые говорит рассказчик</param>
    private void ExecuteTutorialPart(string keyPart, string activeButtonName, string[] FirstCharacterPhrases = null, string[] NarratorPhrases = null, Award award = null)
    {
        var key = $"Tutorial_{keyPart}_Played";
        if (QSReader.Create("TutorialState").Exists(key, "TutorialSkipped")) return;
        SaveTutorialData(key);

        DialogPanel.CreateDialogPanel(FirstCharacterSprite, SecondCharacterSprite, NarratorSprite);

        if (FirstCharacterPhrases != null)
            foreach (var ph in FirstCharacterPhrases)
                DialogPanel.AddPhrase(NowTalking.First, ph);
        if (NarratorPhrases != null)
            foreach (var ph in NarratorPhrases)
                DialogPanel.AddPhrase(NowTalking.Narrator, ph);

        // добавляет награду после слов первого персонажа
        if (award != null)
            DialogPanel.AddAward(FirstCharacterPhrases == null ? 0 : FirstCharacterPhrases.Length, award);

        DialogPanel.SkipTutorialBtnActive = true;
        DialogPanel.StartDialog();

        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        var btn = GameObject.Find(activeButtonName);

        // создаёт блокер и дублирует нужную кнопку
        if (BlockerPrefab != null)
        {
            DialogPanel.LastAction = () =>
            {
                Instantiate(BlockerPrefab, canvas.transform, false);
                var newBtn = Instantiate(btn, canvas.transform, true);
                newBtn.GetComponent<Button>()
                    .onClick.AddListener(() =>
                    {
                        // удаляет блокер и кнопку после клика на кнопку
                        var currentBtn = EventSystem.current.currentSelectedGameObject;
                        var blocker = GameObject.FindGameObjectWithTag("Blocker");
                        if (currentBtn == null) return;

                        if (blocker != null)
                            Destroy(blocker);
                        Destroy(currentBtn);
                    });

                if (btn.name == "FarmSpot")
                {
                    newBtn.GetComponent<PatchGrowth>().Patch = btn.GetComponent<PatchGrowth>().Patch;
                    Seed tutorSeed = null;

                    try
                    {
                        tutorSeed = GameObject.FindGameObjectWithTag("Inventory")
                            .GetComponent<Inventory>()
                            .Elements
                            .Where(s => s.Name == "Potato")
                            .Last();
                    }
                    catch
                    {
                        Debug.Log("Э, куда картошку обучающую дел??? Перезапускай тутор.");
                    }

                    if (tutorSeed != null)
                    {
                        tutorSeed.GrowTime = 10;
                        tutorSeed.UpdateRating();
                    }
                }
            };
        }
        else Debug.Log("Префаб блокера для туториала не указан!");
    }

    /// <summary>
    /// Сохраняет данные о том, что определённая часть туториала уже проигрывалась
    /// </summary>
    private static void SaveTutorialData(string key)
    {
        var writer = QuickSaveWriter.Create("TutorialState");
        writer.Write(key, true);
        writer.Commit();
    }
}
