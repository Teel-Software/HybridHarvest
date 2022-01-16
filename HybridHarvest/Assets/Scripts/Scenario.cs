using CI.QuickSave;
using System.Collections.Generic;
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

    private readonly Dictionary<GameObject, GameObject> fakeButtons = new Dictionary<GameObject, GameObject>();

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
                // тутор для третьего захода в меню выбора
                if (QSReader.Create("TutorialState").Exists("Tutorial_LabEnding_Played"))
                    ExecuteTutorialPart("ChoiceThird", lastPart: true, narratorPhrases: new string[] {
                        "С этого момента вы можете исследовать всё сами! Приятной игры! P. S. Обязательно загляните в К.В.А.Н.Т. При скрещивании разных растений получаются очень смешные названия :)" });

                // тутор для повторного захода в меню выбора
                if (QSReader.Create("TutorialState").Exists("Tutorial_FieldEnding_Played"))
                    ExecuteTutorialPart("ChoiceSecond", activeButtonName: "SideMenuButton",
                        firstCharacterPhrases: new string[] { "Почти пришли! Отсюда до магазина совсем не далеко!" });

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
                ExecuteTutorialPart("BeginningLab", activeButtonName: "Pot",
                    narratorPhrases: new string[] { "Это лаборатория. Здесь можно скрестить семена, которые есть на складе. Скрещивать можно только семена одного вида!" });
                break;
            case 4:
                ExecuteTutorialPart("BeginningQuantum",
                   narratorPhrases: new string[] { "Это К.В.А.Н.Т. Здесь можно скрестить семена, которые есть на складе. Скрещивать можно что угодно, но один раз в день!" });
                break;
        }
    }

    public void Tutorial_Inventory()
    {
        ExecuteTutorialPart("Inventory", activeButtonName: "EnergyPanel", narratorPhrases: new string[] {
        "Это склад. Здесь хранятся пакеты семян. Следите за заполнением места, ведь склад не бесконечен! Посмотреть, на сколько склад заполнен можно в правом нижнем углу.",
        "Для начала познакамимся с энергией."});
    }

    public void Tutorial_Energy()
    {
        ExecuteTutorialPart("Energy", activeButtonName: "EnergyExit", narratorPhrases: new string[] {
            "Энергия тратится, когда вы садите растения. Она сама восстанавливается со временем, но если вы хотите - можно смотреть рекламу и получать энергию БЕСПЛАТНО!" });
    }

    public void Tutorial_ChooseItemToPlant()
    {
        ExecuteTutorialPart("ChooseItemToPlant", activeButtonTag: "TutorialPotato",
            firstCharacterPhrases: new string[] { "А теперь самое время посадить картошку!" });
    }

    public void Tutorial_StatPanel()
    {
        ExecuteTutorialPart("StatPanel", activeButtonName: "ProceedButton",
            narratorPhrases: new string[] { "На этой панели написана вся нужная информация о семечке. Нажмите на кнопку \"Посадить\"." });
    }

    public void Tutorial_WaitForGrowing()
    {
        ExecuteTutorialPart("WaitForGrowing", activeButtonName: "FarmSpot",
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

    public void Tutorial_ChooseItemToReplace()
    {
        ExecuteTutorialPart("ChooseItemToReplace", activeButtonTag: "TutorialPotato",
            narratorPhrases: new string[] { "Нажмите на пакет семян, а затем на кнопку \"Заменить\"." });
    }

    public void Tutorial_ReplaceItem()
    {
        ExecuteTutorialPart("ReplaceItem", activeButtonName: "ProceedButton");
    }

    public void Tutorial_HarvestPlaceSellAll()
    {
        ExecuteTutorialPart("HarvestPlaceSellAll", activeButtonName: "SellAll",
            firstCharacterPhrases: new string[] { "Думаю, что остальную картошку можно продать, сейчас она всё равно мне не понадобится." },
            narratorPhrases: new string[] { "Нажмите на кнопку \"Продать всё\"." });
    }

    public void Tutorial_FieldEnding()
    {
        ExecuteTutorialPart("FieldEnding", activeButtonName: "ExitScene",
            firstCharacterPhrases: new string[] { "Отлично, с урожаем я разобрался, теперь самое время заглянуть к торговцу!" });
    }

    public void Tutorial_SideMenu()
    {
        ExecuteTutorialPart("SideMenu", activeButtonName: "ShopLabel", narratorPhrases: new string[] {
            "Это боковое меню. Одна из самых важных частей игры. Отсюда ты можешь попасть в четыре места: магазин, задания, склад и выставку." });
    }

    public void Tutorial_Shop()
    {
        ExecuteTutorialPart("Shop", activeButtonName: "BuyTomato",
            firstCharacterPhrases: new string[] { "Приветствую, Порфирий! Сколько лет, сколько зим!", "Да ладно тебе, я тут по делу. Мне бы семян томата прикупить." },
            secondCharacterPhrases: new string[] { "Если быть точным, то 0 лет и 0 зим, мы же только вчера виделись.", "Пожалуйста, выбирай. Всё, что есть - на прилавке." },
            narratorPhrases: new string[] { "Купленные пакеты семян можно найти на складе." });
    }

    public void Tutorial_BuyItem()
    {
        ExecuteTutorialPart("BuyItem", activeButtonName: "ProceedButton");
    }

    public void Tutorial_AddItem()
    {
        // может быть фейл, если в инвентаре уже заполнено всё место
        ExecuteTutorialPart("AddItem", activeButtonTag: "InventoryPlusBtn",
            narratorPhrases: new string[] { "Новый пакет можно не заменять, а просто добавить на склад, если в нём достаточно места." });
    }

    public void Tutorial_ShopExit()
    {
        ExecuteTutorialPart("ShopExit", activeButtonName: "SideMenuButton");
    }

    public void Tutorial_SideMenuInventory()
    {
        ExecuteTutorialPart("SideMenuInventory", activeButtonName: "InventoryLabel");
    }

    public void Tutorial_ChooseItemToSell()
    {
        ExecuteTutorialPart("ChooseItemToSell", activeButtonTag: "TutorialPotato",
            firstCharacterPhrases: new string[] { "Самое время продать выращенную картошку!" });
    }

    public void Tutorial_SellItem()
    {
        ExecuteTutorialPart("SellItem", activeButtonName: "ProceedButton",
            narratorPhrases: new string[] { "После продажи семян вы получаете опыт, благодаря которому прокачивается ваш уровень." });
    }

    public void Tutorial_GoToMarket()
    {
        ExecuteTutorialPart("GoToMarket", activeButtonName: "MarketPanel",
            narratorPhrases: new string[] { "Теперь пройдёмте на биржу!" });
    }

    public void Tutorial_Market()
    {
        ExecuteTutorialPart("Market", activeButtonName: "MarketExit",
            firstCharacterPhrases: new string[] { "Ого! С ценой всё в порядке... У меня первый раз такое!" }, narratorPhrases: new string[] {
                "Это биржа. Здесь отображается текущее положение цен на рынке. Цифра справа от семечка означает, насколько изменилась его цена по сравнению с начальной.",
                "Если цифра меньше единицы - цена уменьшилась, если равна единице - цена не изменилась, если больше единицы - цена увеличилась. " +
                "Цифра будет меняться по ходу игры, следите за ней и продавайте семена вовремя."});
    }

    public void Tutorial_InventoryExit()
    {
        ExecuteTutorialPart("InventoryExit", activeButtonTag: "ExitInventory",
            firstCharacterPhrases: new string[] { "Теперь можно проведать, как там дела в лаборатории!" });
    }

    public void Tutorial_GoToLab()
    {
        ExecuteTutorialPart("GoToLab", activeButtonName: "SceneButtonLab");
    }

    public void Tutorial_HybridPanel()
    {
        ExecuteTutorialPart("HybridPanel", activeButtonName: "FirstButton",
            narratorPhrases: new string[] { "Это панель для скрещивания. Выберите семечко слева, затем семечко справа. После этого нажмите \"Скрестить\" и наслаждайтесь результатом." });
    }

    public void Tutorial_ChooseItemToCrossFirst()
    {
        ExecuteTutorialPart("ChooseItemToCrossFirst", activeButtonTag: "TutorialTomato",
            firstCharacterPhrases: new string[] { "Ага! Не зря покупал семена помидора, сейчас их и испробую в скрещивании!" });
    }

    public void Tutorial_ApplyItemToCrossFirst()
    {
        ExecuteTutorialPart("ApplyItemToCrossFirst", activeButtonName: "ProceedButton");
    }

    public void Tutorial_HybridPanelSecond()
    {
        ExecuteTutorialPart("HybridPanelSecond", activeButtonName: "SecondButton",
            firstCharacterPhrases: new string[] { "Теперь нужно выбрать второе семечко." });
    }

    public void Tutorial_ChooseItemToCrossSecond()
    {
        ExecuteTutorialPart("ChooseItemToCrossSecond", activeButtonTag: "TutorialTomato",
            firstCharacterPhrases: new string[] { "Выбор невелик, поэтому придётся взять семечко из того же пакета." });
    }

    public void Tutorial_ApplyItemToCrossSecond()
    {
        ExecuteTutorialPart("ApplyItemToCrossSecond", activeButtonName: "ProceedButton");
    }

    public void Tutorial_ApplyCrossing()
    {
        ExecuteTutorialPart("ApplyCrossing", activeButtonName: "StartHybrid",
            firstCharacterPhrases: new string[] { "Ну, сейчас посмотрим, что получится!" });
    }

    public void Tutorial_WaitForCrossing()
    {
        ExecuteTutorialPart("WaitForCrossing", activeButtonName: "Pot",
            narratorPhrases: new string[] { "Время ожидания значительно ускорено. Обычно оно складывается из времени роста двух выбранных семян." });
    }

    public void Tutorial_MiniGame()
    {
        ExecuteTutorialPart("MiniGame",
            firstCharacterPhrases: new string[] { "Что ж, надеюсь, в этот раз повезёт, ведь вчера мне выпала довольно редкая характеристика. Будет отлично, если это повторится!" },
            narratorPhrases: new string[] { "На всех карточках содержатся характеристики выросшего семечка. Шанс выпадения характеристик совпадает с тем, что вы видели на панели скрещивания." });
    }

    public void Tutorial_ReplaceOrAddItem()
    {
        // деактивирует кнопку выхода из инвентаря
        if (!QSReader.Create("TutorialState").Exists("Tutorial_ReplaceOrAddItem_Played", "TutorialSkipped"))
            GameObject.FindGameObjectWithTag("ExitInventory")?.SetActive(false);

        ExecuteTutorialPart("ReplaceOrAddItem",
            narratorPhrases: new string[] { "Теперь сами выберите, что сделать с пакетом семян: заменить существующий или добавить новый." });
    }

    public void Tutorial_LabEnding()
    {
        ExecuteTutorialPart("LabEnding", activeButtonName: "ExitScene",
            firstCharacterPhrases: new string[] { "Что ж, в лаборатории я сделал всё, что хотел." });
    }

    public void Tutorial_Quests()
    {
        ExecuteTutorialPart("Quests",
            narratorPhrases: new string[] { "Это доска объявлений. Здесь появляются задания от жителей, которым нужна помощь. За выполнение заданий вы получите от них награду." });
    }

    public void Tutorial_Exhibition()
    {
        ExecuteTutorialPart("Exhibition",
           narratorPhrases: new string[] { "Это выставка. Здесь можно выставить на всеобщее обозрение свои лучшие продукты. Пусть все знают, кто тут настоящий садовод!" });
    }

    /// <summary>
    /// Проигрывает часть туториала
    /// </summary>
    /// <param name="keyPart">Название, по которому идёт сохранение</param>
    /// <param name="activeButtonName">Название кнопки, которую следует сделать активной после окончания части вступления</param>
    /// <param name="activeButtonTag">Тег кнопки, которую следует сделать активной после окончания части вступления</param>
    /// <param name="firstCharacterPhrases">Фразы, которые говорит первый персонаж</param>
    /// <param name="narratorPhrases">Фразы, которые говорит рассказчик</param>
    /// <param name="award">Награда после слов первого персонажа</param>
    private void ExecuteTutorialPart(string keyPart, string activeButtonName = null, string activeButtonTag = null, bool lastPart = false,
        string[] firstCharacterPhrases = null, string[] secondCharacterPhrases = null, string[] narratorPhrases = null, Award award = null)
    {
        var key = $"Tutorial_{keyPart}_Played";
        if (QSReader.Create("TutorialState").Exists(key, "TutorialSkipped")) return;
        SaveTutorialData(key);

        DialogPanel.CreateDialogPanel(FirstCharacterSprite, SecondCharacterSprite, NarratorSprite);

        if (secondCharacterPhrases != null)
        {
            for (var i = 0; i < secondCharacterPhrases.Length; i++)
            {
                try
                {
                    DialogPanel.AddPhrase(NowTalking.First, firstCharacterPhrases[i]);
                    DialogPanel.AddPhrase(NowTalking.Second, secondCharacterPhrases[i]);
                }
                catch
                {
                    Debug.Log("Количество фраз у первого и второго персонажей не совпадает. Некоторые из них были пропущены.");
                }
            }
        }
        else if (firstCharacterPhrases != null)
            foreach (var ph in firstCharacterPhrases)
                DialogPanel.AddPhrase(NowTalking.First, ph);
        if (narratorPhrases != null)
            foreach (var ph in narratorPhrases)
                DialogPanel.AddPhrase(NowTalking.Narrator, ph);

        // добавляет награду после слов первого персонажа
        if (award != null)
            DialogPanel.AddAward(firstCharacterPhrases == null ? 0 : firstCharacterPhrases.Length, award);

        if (lastPart == false)
            HighlightNextButton(activeButtonName: activeButtonName, activeButtonTag: activeButtonTag);
        else DialogPanel.LastAction = () =>
            GameObject.FindGameObjectWithTag("TutorialHandler")
                ?.GetComponent<TutorialHandler>()
                ?.ClearGameAfterTutorial();

        DialogPanel.SkipTutorialBtnActive = true;
        DialogPanel.StartDialog();
    }

    /// <summary>
    /// Подсвечивает нужную кнопку
    /// </summary>
    private void HighlightNextButton(string activeButtonName = null, string activeButtonTag = null)
    {
        // создаёт блокер и дублирует нужную кнопку
        if (BlockerPrefab != null)
        {
            DialogPanel.LastAction = () =>
            {
                GameObject fakeBtn = null;
                GameObject activeButton = null;

                if (activeButtonName != null)
                    activeButton = GameObject.Find(activeButtonName);
                else if (activeButtonTag != null)
                    activeButton = GameObject.FindGameObjectWithTag(activeButtonTag);
                if (activeButton == null) return;

                if (activeButton.name == "SceneButtonField"
                    && !QSReader.Create("TutorialState").Exists("Tutorial_BeginningField_Played", "TutorialSkipped"))
                    ChangeSeedName("Potato", "Обучающий картофель");

                if (activeButton.name == "InventoryLabel"
                    && !QSReader.Create("TutorialState").Exists("Tutorial_ChooseItemToSell_Played", "TutorialSkipped"))
                    ChangeSeedName("Tomato", "Обучающий помидор");

                var canvas = GameObject.FindGameObjectWithTag("Canvas");
                Instantiate(BlockerPrefab, canvas.transform, false);
                fakeBtn = Instantiate(activeButton, activeButton.transform.parent);
                fakeBtn.transform.SetSiblingIndex(activeButton.transform.GetSiblingIndex());
                fakeBtn.name = $"FakeButton_{activeButton.name}";
                fakeButtons.Add(activeButton, fakeBtn);

                activeButton.transform.SetParent(canvas.transform, true);
                activeButton.transform.SetAsLastSibling();
                activeButton.GetComponent<Button>()
                    .onClick.AddListener(() =>
                    {
                        var currentButton = EventSystem.current.currentSelectedGameObject;
                        if (currentButton == null) return;

                        // удаляет блокер и фейковую кнопку
                        GameObject placeholder = null;
                        if (fakeButtons.ContainsKey(currentButton))
                            placeholder = fakeButtons[currentButton];
                        var blocker = GameObject.FindGameObjectWithTag("Blocker");

                        if (placeholder != null)
                        {
                            fakeButtons.Remove(currentButton);
                            currentButton.transform.SetParent(placeholder.transform.parent, true);
                            currentButton.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
                        }
                        Destroy(placeholder);
                        Destroy(blocker);
                    });
            };
        }
        else Debug.Log("Префаб блокера для туториала не указан!");
    }

    /// <summary>
    /// Меняет русское название семечка
    /// </summary>
    /// <param name="nameEnglish">Название семечка на английском</param>
    /// <param name="nameRussian">Будущее название на русском</param>
    private static void ChangeSeedName(string nameEnglish, string nameRussian)
    {
        var tutorSeed = GameObject.FindGameObjectWithTag("Inventory")
               .GetComponent<Inventory>()
               .Elements
               .Where(s => s.Name == nameEnglish)
               .LastOrDefault();

        if (tutorSeed != null)
        {
            tutorSeed.NameInRussian = nameRussian;

            GameObject.FindGameObjectWithTag("Inventory")
                .GetComponent<Inventory>()
                .Save();
        }
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
