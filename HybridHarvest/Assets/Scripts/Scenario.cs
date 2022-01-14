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

    private GameObject lastButton;

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
                ExecuteTutorialPart("BeginningChoice", activeButtonName: "SceneButtonField",
                    firstCharacterPhrases: new string[] { "Хороший денёк, однако выдался! Помнится, вчера я хотел посадить семена, да вот забыл... Ну ничего, сделаю это сейчас!" },
                   narratorPhrases: new string[] { "Нажмите на кнопку \"Грядка\"." },
                   award: new Award(AwardType.Seed, seedName: "Potato"));
                break;
            case 2:
                ExecuteTutorialPart("BeginningField", activeButtonName: "FarmSpot",
                    narratorPhrases: new string[] { "Это поле. Здесь можно посадить семена, которые есть на складе. Нажмите на грядку." });
                break;
            case 3:
                ExecuteTutorialPart("BeginningLab", firstCharacterPhrases: new string[] {
                   "Это лаборатория. Здесь можно скрестить семена, которые есть у тебя на складе. Скрещивать можно только семена одного вида!" });
                break;
                //case 4:
                //    ExecuteTutorialPart("BeginningQuantum", "ExitScene",
                //       "Это К.В.А.Н.Т. Здесь можно скрестить семена, которые есть у тебя на складе. Скрещивать можно что угодно, но один раз в день!");
                //    break;
        }
    }

    public void Tutorial_Inventory()
    {
        if (!QSReader.Create("TutorialState").Exists("Tutorial_Inventory_Played", "TutorialSkipped"))
        {
            // деактивирует кнопку выхода из инвентаря
            GameObject.Find("ExitInventory")?.SetActive(false);
            // деактивирует кнопку выхода со сцены
            GameObject.Find("ExitScene")?.SetActive(false);
        }

        ExecuteTutorialPart("Inventory", narratorPhrases: new string[] {
        "Это склад. Здесь хранятся пакеты семян. Следите за заполнением места, ведь склад не бесконечен! Посмотреть, на сколько склад заполнен можно в правом нижнем углу.",
        "Нажмите на пакет с семенами картофеля."});
    }

    public void Tutorial_StatPanel()
    {
        ExecuteTutorialPart("StatPanel", narratorPhrases: new string[] { "На этой панели написана вся нужная информация о семечке. Нажмите на кнопку \"Посадить\"." });
    }

    public void Tutorial_FarmSpot()
    {
        ExecuteTutorialPart("FarmSpot", activeButtonName: "FarmSpot",
            firstCharacterPhrases: new string[] { "Ого, картошка нынче быстро растёт! Видимо повлияли хорошие погодные условия." },
            narratorPhrases: new string[] { "Первый раз мы ускорили время роста картошки, чтобы вы долго не ждали. " +
            "Обычные семена прорастают намного медленнее. Каждый раз при посадке семечка тратится 1 единица энергии.",
            "Нажмите на картофель, как только он вырастет."});
    }

    public void Tutorial_HarvestPlace()
    {
        ExecuteTutorialPart("HarvestPlace", activeButtonName: "AddToInventory",
            firstCharacterPhrases: new string[] { "Ух ты, сколько картошки выросло! Даже и не знаю, какую выбрать!" },
            narratorPhrases: new string[] { "На данной панели отображается всё, что выросло из посаженного семечка. Чтобы добавить понравившиеся семена на склад - нажмите на кнопку \"+\"." });
    }

    public void Tutorial_ReplaceItem()
    {
        if (!QSReader.Create("TutorialState").Exists("Tutorial_ReplaceItem_Played", "TutorialSkipped"))
        {
            // деактивирует кнопку добавления элемента в инвентаре
            GameObject.FindGameObjectWithTag("InventoryPlusBtn")?.SetActive(false);
        }

        ExecuteTutorialPart("ReplaceItem", narratorPhrases: new string[] { "Нажмите на пакет семян, после чего нажмите на кнопку \"Заменить\"." });
    }

    public void Tutorial_HarvestPlaceSellAll()
    {
        ExecuteTutorialPart("HarvestPlaceSellAll", activeButtonName: "SellAll",
            firstCharacterPhrases: new string[] { "Думаю, что остальную картошку можно продать, сейчас она всё равно мне не понадобится." },
            narratorPhrases: new string[] { "Нажмите на кнопку \"Продать всё\"." });
    }

    public void Tutorial_FieldEnding()
    {
        if (!QSReader.Create("TutorialState").Exists("Tutorial_FieldEnding_Played", "TutorialSkipped"))
        {
            // активирует кнопку выхода со сцены
            var btn = GameObject.FindGameObjectWithTag("Canvas")
                 ?.transform
                 ?.Find("ExitScene")
                 ?.gameObject;
            btn?.SetActive(true);
        }

        ExecuteTutorialPart("FieldEnding", activeButtonName: "ExitScene",
            firstCharacterPhrases: new string[] { "Отлично, с урожаем я разобрался, теперь самое время заглянуть к торговцу!" });
    }

    public void Tutorial_Market()
    {
        ExecuteTutorialPart("Market",
            firstCharacterPhrases: new string[] { "Ого! С ценой всё в порядке... У меня первый раз такое!" },
            narratorPhrases: new string[] { "Это биржа. Здесь отображается текущее положение цен на рынке. Цифра справа от семечка означает, насколько изменилась его цена по сравнению с начальной.",
            "Если цифра меньше единицы - цена уменьшилась, если равна единице - цена не изменилась, если больше единицы - цена увеличилась. " +
            "Цифра будет меняться по ходу игры, следите за ней и продавайте семена вовремя."});
    }

    public void Tutorial_Energy()
    {
        ExecuteTutorialPart("Energy", narratorPhrases: new string[] {
            "Энергия тратится, когда вы садите растения. Она сама восстанавливается со временем, но если вы хотите - можно смотреть рекламу и получать энергию БЕСПЛАТНО!" });
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


    //public void Tutorial_Exhibition()
    //{
    //    ExecuteTutorialPart("Exhibition",
    //       "", "Это выставка. Здесь можно выставить на всеобщее обозрение свои лучшие продукты. Пусть все знают, кто тут настоящий садовод!");
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
    /// <param name="activeButtonTag">Тег кнопки, которую следует сделать активной после окончания части вступления</param>
    /// <param name="firstCharacterPhrases">Фразы, которые говорит первый персонаж</param>
    /// <param name="narratorPhrases">Фразы, которые говорит рассказчик</param>
    /// <param name="award">Награда после слов первого персонажа</param>
    private void ExecuteTutorialPart(string keyPart, string activeButtonName = null, string activeButtonTag = null,
        string[] firstCharacterPhrases = null, string[] narratorPhrases = null, Award award = null)
    {
        var key = $"Tutorial_{keyPart}_Played";
        if (QSReader.Create("TutorialState").Exists(key, "TutorialSkipped")) return;
        SaveTutorialData(key);

        DialogPanel.CreateDialogPanel(FirstCharacterSprite, SecondCharacterSprite, NarratorSprite);

        if (firstCharacterPhrases != null)
            foreach (var ph in firstCharacterPhrases)
                DialogPanel.AddPhrase(NowTalking.First, ph);
        if (narratorPhrases != null)
            foreach (var ph in narratorPhrases)
                DialogPanel.AddPhrase(NowTalking.Narrator, ph);

        // добавляет награду после слов первого персонажа
        if (award != null)
            DialogPanel.AddAward(firstCharacterPhrases == null ? 0 : firstCharacterPhrases.Length, award);

        DialogPanel.SkipTutorialBtnActive = true;
        DialogPanel.StartDialog();

        if (activeButtonName != null)
            HighlightNextButton(GameObject.Find(activeButtonName));
        else if (activeButtonTag != null)
            HighlightNextButton(GameObject.FindGameObjectWithTag(activeButtonTag));
    }

    /// <summary>
    /// Подсвечивает нужную кнопку
    /// </summary>
    /// <param name="activeButton">Кнопка, которую следует выделить</param>
    private void HighlightNextButton(GameObject activeButton)
    {
        if (activeButton == null) return;
        var canvas = GameObject.FindGameObjectWithTag("Canvas");

        // создаёт блокер и дублирует нужную кнопку
        if (BlockerPrefab != null)
        {
            DialogPanel.LastAction = () =>
            {
                Instantiate(BlockerPrefab, canvas.transform, false);
                var newBtn = Instantiate(activeButton, activeButton.transform.parent);
                newBtn.transform.SetSiblingIndex(activeButton.transform.GetSiblingIndex());
                newBtn.name = "SuperFakeButton271386";

                lastButton = newBtn;

                if (activeButton.name == "FarmSpot")
                {
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

                activeButton.transform.SetParent(canvas.transform, true);
                activeButton.transform.SetAsLastSibling();
                activeButton.GetComponent<Button>()
                    .onClick.AddListener(() =>
                    {
                        // удаляет блокер и фейковую кнопку
                        var placeholder = lastButton;
                        var blocker = GameObject.FindGameObjectWithTag("Blocker");

                        if (placeholder != null)
                        {
                            activeButton.transform.SetParent(placeholder.transform.parent, true);
                            activeButton.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
                        }
                        Destroy(placeholder);
                        Destroy(blocker);
                    });
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
