using System;
using CI.QuickSave;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scenario : MonoBehaviour
{
    [SerializeField] public Sprite FirstCharacterSprite;
    [SerializeField] public Sprite SecondCharacterSprite;
    [SerializeField] public Sprite NarratorSprite;
    [SerializeField] private DialogPanelLogic DialogPanel;
    [SerializeField] private DialogPanelLogic DialogPanelPrefab;
    [SerializeField] private GameObject BlockerPrefab;
    [SerializeField] private GameObject BottomTextPrefab;

    private readonly Dictionary<GameObject, GameObject> fakeButtons = new Dictionary<GameObject, GameObject>();

    /// <summary>
    /// В подобных методах нужно создавать диалоги
    /// </summary>
    public void MakeTestDialog()
    {
        // !!! ID НАЧИНАЕТСЯ С ЦИФРЫ 1 !!!
        // ID присваивается фразе автоматически при вызове метода AddPhrase

        DialogPanel.InitDialogPanel(FirstCharacterSprite, SecondCharacterSprite, NarratorSprite);

        DialogPanel.AddPhrase(NowTalking.Second, "Приветствую путник, вижу ты первый раз здесь."); // ID = 1
        DialogPanel.AddPhrase(NowTalking.First, "Эээээ, ну здрасьте!");

        DialogPanel.AddPhrase(NowTalking.Second,
            "Я - Великий Стонкс и у меня есть всевозможные товары, начиная с огурца и заканчивая морковью."); // ID = 3
        DialogPanel.AddPhrase(NowTalking.First, "1. А как же сосисочки?", 3); // ID = 4
        DialogPanel.AddAward(4,
            new Award(AwardType.Achievement, message: "Достижение разблокировано: \"Вернуться в 2007\""));
        DialogPanel.AddPhrase(NowTalking.Second, "Да, и хороший борщик... Не продаём такое, к сожалению.", 4);

        DialogPanel.AddPhrase(NowTalking.First, "2. Вы любите розы?", 3); // ID = 6
        DialogPanel.AddAward(6, new Award(AwardType.Money, money: 500));
        DialogPanel.AddPhrase(NowTalking.Second, "А я на них... не вижу никакого подтекста.", 6);

        DialogPanel.AddPhrase(NowTalking.First, "3. Получить огурчик!", 3); // ID = 8
        DialogPanel.AddAward(8, new Award(AwardType.Seed, seedName: "Cucumber"));

        DialogPanel.AddPhrase(NowTalking.First,
            "Хочется взять что-то необычное... Я думаю, огурец выглядит неплохо, поэтому возьму Иерусалим.");
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

    public void CreateTaskEndDialog(string firstCharacterPhrase, params Award[] awards)
    {
        DialogPanel.InitDialogPanel(FirstCharacterSprite, SecondCharacterSprite, NarratorSprite);
        DialogPanel.AddPhrase(NowTalking.First, firstCharacterPhrase);

        foreach (var award in awards)
            DialogPanel.AddAward(1, award);

        DialogPanel.StartDialog();
    }

    public void Tutorial_Beginning()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                // // тутор для третьего захода в меню выбора
                // if (QSReader.Create("TutorialState").Exists("Tutorial_LabEnding_Played"))
                //     ExecuteTutorialPart("ChoiceThird", lastPart: true, narratorPhrases: new[]
                //     {
                //         "С этого момента вы можете исследовать всё сами! Приятной игры! P. S. Обязательно загляните в К.В.А.Н.Т. При скрещивании разных растений получаются очень смешные названия :)"
                //     });

                // тутор для второго захода в меню выбора
                if (QSReader.Create("TutorialState").Exists("Tutorial_LevelUp2_Played") &&
                    !QSReader.Create("TutorialState").Exists("Tutorial_SideMenuToQuests_Played"))
                    ExecuteTutorialPart("ChoiceSecond", activeButtonName: "SideMenuButton",
                        bottomText: "Нажмите на кнопку бокового меню.");

                // тутор для первого захода в меню выбора
                ExecuteTutorialPart("BeginningChoice", activeButtonName: "SideMenuButton",
                    firstCharacterPhrases: new[]
                    {
                        "Надо вывести новый суперкрутой сорт. В магаз бы заглянуть, вдруг что-нибудь полезное продаётся."
                    },
                    bottomText: "Нажмите на кнопку бокового меню.");

                break;
            case 2:
                ExecuteTutorialPart("BeginningField", activeButtonName: "FarmSpot",
                    narratorPhrases: new[]
                        { "Это поле. Здесь можно посадить семена, которые есть на складе." },
                    bottomText: "Нажмите на грядку.");
                break;
            case 3:
                // ExecuteTutorialPart("BeginningLab", activeButtonName: "Pot",
                //     narratorPhrases: new[]
                //     {
                //         "Это лаборатория. Здесь можно скрестить семена, которые есть на складе. Скрещивать можно только семена одного вида!"
                //     });
                break;
            case 4:
                // ExecuteTutorialPart("BeginningQuantum",
                //     narratorPhrases: new[]
                //     {
                //         "Это К.В.А.Н.Т. Здесь можно скрестить семена, которые есть на складе. Скрещивать можно что угодно, но один раз в день!"
                //     });
                break;
        }
    }

    public void Tutorial_SideMenuToShop()
    {
        ExecuteTutorialPart("SideMenuToShop", activeButtonName: "ShopLabel",
            bottomText: "Нажмите на кнопку \"Магазин\".");
    }

    public void Tutorial_Shop()
    {
        ExecuteTutorialPart("Shop", activeButtonName: "BuyCucumber",
            firstCharacterPhrases: new[]
            {
                "Даровченко, мне бы семян купить!",
                "Эмм, ну да, а откуда вы меня знаете?"
            },
            secondCharacterPhrases: new[]
            {
                "Альберт, верно? Отец того самого Жмышенко Валерия Альбертовича?",
                "Стримы смот... Эээ, не бери в голову, слышал от соседки."
            },
            narratorPhrases: new[]
                { "С повышением уровня в магазине будут появляться новые семена." },
            bottomText: "Нажмите на пакет семян огурца.");
    }

    public void Tutorial_BuyItem()
    {
        ExecuteTutorialPart("BuyItem", activeButtonName: "ProceedButton",
            bottomText: "Нажмите на кнопку \"Купить\".");
    }

    public void Tutorial_AddItem()
    {
        // может быть фейл, если в инвентаре уже заполнено всё место
        ExecuteTutorialPart("AddItem", activeButtonTag: "InventoryPlusBtn",
            narratorPhrases: new[]
                { "Самое время добавить новый пакет семян на склад!" },
            bottomText: "Нажмите на кнопку \"+\".");
    }

    public void Tutorial_ShopExit()
    {
        ExecuteTutorialPart("ShopExit", activeButtonName: "ShopExit",
            firstCharacterPhrases: new[]
            {
                "Ого, как хорошо, что у меня в кармане нашлось немного порублей! Теперь нужно сбегать на поле, да посадить эти семена!"
            },
            bottomText: "Выйдите из магазина.");
    }

    public void Tutorial_GoToField()
    {
        ExecuteTutorialPart("GoToField", activeButtonName: "SceneButtonField",
            bottomText: "Перейдите на вкладку \"Поле\".");
    }

    public void Tutorial_ChooseItemToPlant()
    {
        ExecuteTutorialPart("ChooseItemToPlant", activeButtonName: "0",
            firstCharacterPhrases: new[] { "А теперь самое время посадить огурец!" },
            bottomText: "Нажмите на пакет семян огурца.");
    }

    public void Tutorial_PlantItem()
    {
        ExecuteTutorialPart("PlantItem", activeButtonName: "ProceedButton",
            bottomText: "Нажмите на кнопку \"Посадить\".");
    }

    public void Tutorial_ChooseItemToSpeedUp()
    {
        ExecuteTutorialPart("ChooseItemToSpeedUp", activeButtonName: "FarmSpot",
            firstCharacterPhrases: new[] { "Ну-ка, посмотрим, сколько наши огурчики будут расти." },
            bottomText: "Нажмите на грядку.");
    }

    public void Tutorial_SpeedUpItem()
    {
        ExecuteTutorialPart("SpeedUpItem", activeButtonName: "SpeedUpBtn",
            firstCharacterPhrases: new[]
            {
                "Ого! Многовато времени, я столько ждать не намерен! ",
                "Хорошо, что у меня завалялась в кармане баночка с супер жижей. Сейчас я быстро ускорю рост этих огурчиков."
            },
            bottomText: "Нажмите на кнопку ускорения.");
    }

    public void Tutorial_ConfirmSpeedUp()
    {
        ExecuteTutorialPart("ConfirmSpeedUp", activeButtonName: "YesButton",
            narratorPhrases: new[]
            {
                "Обычно ускорение производится за рекламу, но первый раз мы сделаем это для вас бесплатно."
            },
            bottomText: "Нажмите на кнопку \"Да\".");
    }

    public void Tutorial_WaitForGrowing()
    {
        ExecuteTutorialPart("WaitForGrowing", activeButtonName: "FarmSpot",
            firstCharacterPhrases: new[] { "Вот так-то намного лучше!" },
            bottomText: "Нажмите на грядку, как только огурцы вырастут.");
    }

    public void Tutorial_HarvestPlace()
    {
        ExecuteTutorialPart("HarvestPlace", activeButtonName: "AddToInventory",
            firstCharacterPhrases: new[]
            {
                "Ух ты, сколько огурчиков выросло! Многовато для меня одного. Из одного я семена получу, а остальные продам."
            },
            narratorPhrases: new[]
            {
                "На данной панели отображается ваш урожай. Выросшие огурцы можно превратить в семена для дальнейшего использования или продать."
            },
            bottomText: "Нажмите на кнопку \"+\".");
    }

    public void Tutorial_ChooseItemToReplace()
    {
        ExecuteTutorialPart("ChooseItemToReplace", activeButtonName: "0",
            narratorPhrases: new[]
            {
                "Вы можете не добавлять новые семена на склад, а заменить уже имеющиеся. Так на складе останется больше свободного места."
            },
            bottomText: "Нажмите на пакет семян огурца.");
    }

    public void Tutorial_ReplaceItem()
    {
        ExecuteTutorialPart("ReplaceItem", activeButtonName: "ProceedButton",
            bottomText: "Нажмите на кнопку \"Заменить\".");
    }

    public void Tutorial_HarvestPlaceChoseAll()
    {
        ExecuteTutorialPart("HarvestPlaceChoseAll", activeButtonName: "ChoseAll",
            firstCharacterPhrases: new[]
            {
                "Вот так! Оставшиеся огурчики можно продать!"
            },
            bottomText: "Нажмите на поле для галочки.");
    }

    public void Tutorial_HarvestPlaceSell()
    {
        ExecuteTutorialPart("HarvestPlaceSell", activeButtonName: "Sell",
            bottomText: "Нажмите на кнопку \"Продать выбранное\".");
    }

    public void Tutorial_CheckEnergy()
    {
        ExecuteTutorialPart("CheckEnergy", activeButtonName: "EnergyPanel",
            firstCharacterPhrases: new[]
            {
                "Фух, ну и утомился же я... Надо бы отдохнуть."
            },
            narratorPhrases: new[] { "Когда вы садите растения, у вас тратится энергия." },
            bottomText: "Нажмите на информацию об энергии.");
    }

    public void Tutorial_Energy()
    {
        ExecuteTutorialPart("Energy", activeButtonName: "EnergyExit", narratorPhrases: new[]
            {
                "Энергия сама восстанавливается со временем, но если вы хотите - можно смотреть рекламу и получать энергию БЕСПЛАТНО!"
            },
            bottomText: "Закройте меню энергии.");
    }

    public void Tutorial_FieldEnding()
    {
        ExecuteTutorialPart("FieldEnding",
            narratorPhrases: new[]
            {
                "Мы встретимся с вами вновь, когда вы достигните 2-ого уровня. Пока что осваивайтесь на поле, выращивайте огурцы и получайте опыт."
            });
    }

    public void Tutorial_LevelUp2()
    {
        var buttonName = SceneManager.GetActiveScene().buildIndex switch
        {
            1 => "SideMenuButton",
            2 => "ExitScene",
            _ => ""
        };
        var bottomText = SceneManager.GetActiveScene().buildIndex switch
        {
            1 => "Нажмите на кнопку бокового меню.",
            2 => "Нажмите на кнопку выхода с поля.",
            _ => ""
        };

        ExecuteTutorialPart("LevelUp2", activeButtonName: buttonName,
            narratorPhrases: new[]
            {
                "Поздравляем! Вы достигли второго уровня! В награду за повышение уровня открываются новые предметы в магазине, а также улучшается различные характеристики.",
                "Для вас открылась возможность получать задания! Пройдёмте на доску объявлений."
            },
            bottomText: bottomText);
    }

    public void Tutorial_SideMenuToQuests()
    {
        ExecuteTutorialPart("SideMenuToQuests", activeButtonName: "QuestLabel",
            bottomText: "Нажмите на кнопку \"Задания\".");
    }

    public void Tutorial_GetFirstQuest()
    {
        SecondCharacterSprite = Resources.Load<Sprite>("Characters\\OldLady");
        ExecuteTutorialPart("GetFirstQuest", activeButtonName: "",
            firstCharacterPhrases: new[]
            {
                "Ого! Я чувствую, что как будто стал сильнее, что ли...",
                "Да, так и есть. Вы что-то хотели?",
                "Приятно познакомиться! А меня зовут Альберт.",
                "Хорошо, помогу! Мне всё равно пока заняться нечем."
            },
            secondCharacterPhrases: new[]
            {
                "Здравствуй, милок! Ты недавно в деревне, верно?",
                "Ой, а я не не представилась. Я - Серафима Ивановна, супруга здешнего старосты.",
                "Альберт, мне нужна помощь с посадками. Вырастишь для меня 5 огурцов и 5 помидоров? А я за это словечко перед старостой за тебя замолвлю."
            },
            narratorPhrases: new[]
                { "Овощи для заданий отправляются напрямую с грядки, как только вырастут." });
    }

    public void Tutorial_ShopLevel2()
    {
        FirstCharacterSprite = Resources.Load<Sprite>("Characters\\Salesman");
        ExecuteTutorialPart("ShopLevel2",
            firstCharacterPhrases: new[]
            {
                "Приветствую! У меня в продаже недавно появились семена помидора. Цена небольшая, советую купить."
            });
    }
    
    public void Tutorial_FirstQuestCompleted()
    {
        FirstCharacterSprite = Resources.Load<Sprite>("Characters\\OldMan");
        SecondCharacterSprite = Resources.Load<Sprite>("Characters\\MainHero");
        
        ExecuteTutorialPart("FirstQuestCompleted",
            firstCharacterPhrases: new[]
            {
                "Здравствуй, Альберт! Я - староста этой деревни. Спасибо за то, что помог моей супруге.",
                "Слушай, пойми меня правильно: ты в деревне недавно и я ещё не могу в полной мере доверять тебе. Победи в районной выставке, тогда поговорим."
            },
            secondCharacterPhrases: new[]
            {
                "Приятно познакомиться!",
                "Ух ты, выставка! Надеюсь, я справлюсь!"
            });
    }
    
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void Tutorial_SideMenuToInventory()
    {
        ExecuteTutorialPart("SideMenuInventory", activeButtonName: "InventoryLabel");
    }

    // public void Tutorial_ChooseItemToSell()
    // {
    //     ExecuteTutorialPart("ChooseItemToSell", activeButtonTag: "TutorialPotato",
    //         firstCharacterPhrases: new[] { "Самое время продать выращенную картошку!" });
    // }

    public void Tutorial_SellItem()
    {
        ExecuteTutorialPart("SellItem", activeButtonName: "ProceedButton",
            narratorPhrases: new[]
                { "После продажи семян вы получаете опыт, благодаря которому прокачивается ваш уровень." });
    }

    public void Tutorial_GoToMarket()
    {
        ExecuteTutorialPart("GoToMarket", activeButtonName: "MarketPanel",
            narratorPhrases: new[] { "Теперь пройдёмте на биржу!" });
    }

    // public void Tutorial_Market()
    // {
    //     ExecuteTutorialPart("Market", activeButtonName: "MarketExit",
    //         firstCharacterPhrases: new[] { "Ого! С ценой всё в порядке... У меня первый раз такое!" },
    //         narratorPhrases: new[]
    //         {
    //             "Это биржа. Здесь отображается текущее положение цен на рынке. Цифра справа от семечка означает, насколько изменилась его цена по сравнению с начальной.",
    //             "Если цифра меньше единицы - цена уменьшилась, если равна единице - цена не изменилась, если больше единицы - цена увеличилась. " +
    //             "Цифра будет меняться по ходу игры, следите за ней и продавайте семена вовремя."
    //         });
    // }
    //
    // public void Tutorial_InventoryExit()
    // {
    //     ExecuteTutorialPart("InventoryExit", activeButtonTag: "ExitInventory",
    //         firstCharacterPhrases: new[] { "Теперь можно проведать, как там дела в лаборатории!" });
    // }
    //
    // public void Tutorial_GoToLab()
    // {
    //     ExecuteTutorialPart("GoToLab", activeButtonName: "SceneButtonLab");
    // }
    //
    // public void Tutorial_HybridPanel()
    // {
    //     ExecuteTutorialPart("HybridPanel", activeButtonName: "FirstButton",
    //         narratorPhrases: new[]
    //         {
    //             "Это панель для скрещивания. Выберите семечко слева, затем семечко справа. После этого нажмите \"Скрестить\" и наслаждайтесь результатом."
    //         });
    // }

    public void Tutorial_ChooseItemToCrossFirst()
    {
        ExecuteTutorialPart("ChooseItemToCrossFirst", activeButtonTag: "TutorialTomato",
            firstCharacterPhrases: new[]
                { "Ага! Не зря покупал семена помидора, сейчас их и испробую в скрещивании!" });
    }

    public void Tutorial_ApplyItemToCrossFirst()
    {
        ExecuteTutorialPart("ApplyItemToCrossFirst", activeButtonName: "ProceedButton");
    }

    public void Tutorial_HybridPanelSecond()
    {
        ExecuteTutorialPart("HybridPanelSecond", activeButtonName: "SecondButton",
            firstCharacterPhrases: new[] { "Теперь нужно выбрать второе семечко." });
    }

    public void Tutorial_ChooseItemToCrossSecond()
    {
        ExecuteTutorialPart("ChooseItemToCrossSecond", activeButtonTag: "TutorialTomato",
            firstCharacterPhrases: new[] { "Выбор невелик, поэтому придётся взять семечко из того же пакета." });
    }

    public void Tutorial_ApplyItemToCrossSecond()
    {
        ExecuteTutorialPart("ApplyItemToCrossSecond", activeButtonName: "ProceedButton");
    }

    public void Tutorial_ApplyCrossing()
    {
        ExecuteTutorialPart("ApplyCrossing", activeButtonName: "StartHybrid",
            firstCharacterPhrases: new[] { "Ну, сейчас посмотрим, что получится!" });
    }

    // public void Tutorial_WaitForCrossing()
    // {
    //     ExecuteTutorialPart("WaitForCrossing", activeButtonName: "Pot",
    //         narratorPhrases: new[]
    //         {
    //             "Время ожидания значительно ускорено. Обычно оно складывается из времени роста двух выбранных семян."
    //         });
    // }

    public void Tutorial_MiniGame()
    {
        ExecuteTutorialPart("MiniGame",
            firstCharacterPhrases: new[]
            {
                "Что ж, надеюсь, в этот раз повезёт, ведь вчера мне выпала довольно редкая характеристика. Будет отлично, если это повторится!"
            },
            narratorPhrases: new[]
            {
                "На всех карточках содержатся характеристики выросшего семечка. Шанс выпадения характеристик совпадает с тем, что вы видели на панели скрещивания."
            });
    }

    // public void Tutorial_ReplaceOrAddItem()
    // {
    //     // деактивирует кнопку выхода из инвентаря
    //     if (!QSReader.Create("TutorialState").Exists("Tutorial_ReplaceOrAddItem_Played", "TutorialSkipped"))
    //         GameObject.FindGameObjectWithTag("ExitInventory")?.SetActive(false);
    //
    //     ExecuteTutorialPart("ReplaceOrAddItem",
    //         narratorPhrases: new[]
    //             { "Теперь сами выберите, что сделать с пакетом семян: заменить существующий или добавить новый." });
    // }

    public void Tutorial_LabEnding()
    {
        ExecuteTutorialPart("LabEnding", activeButtonName: "ExitScene",
            firstCharacterPhrases: new[] { "Что ж, в лаборатории я сделал всё, что хотел." });
    }


    //
    // public void Tutorial_Exhibition()
    // {
    //     ExecuteTutorialPart("Exhibition",
    //         narratorPhrases: new[]
    //         {
    //             "Это выставка. Здесь можно выставить на всеобщее обозрение свои лучшие продукты. Пусть все знают, кто тут настоящий садовод!"
    //         });
    // }

    /// <summary>
    /// Проигрывает часть туториала.
    /// </summary>
    /// <param name="keyPart">Название, по которому идёт сохранение.</param>
    /// <param name="activeButtonName">Название кнопки, которую следует сделать активной после окончания части вступления.</param>
    /// <param name="activeButtonTag">Тег кнопки, которую следует сделать активной после окончания части вступления.</param>
    /// <param name="lastPart">True указывается в случае, если данная часть - последняя.</param>
    /// <param name="firstCharacterPhrases">Фразы, которые говорит первый персонаж.</param>
    /// <param name="secondCharacterPhrases">Фразы, которые говорит второй персонаж.</param>
    /// <param name="narratorPhrases">Фразы, которые говорит рассказчик.</param>
    /// <param name="bottomText">Текст, показываемый внизу экрана.</param>
    /// <param name="award">Награда после слов первого персонажа.</param>
    private void ExecuteTutorialPart(string keyPart, string activeButtonName = null, string activeButtonTag = null,
        bool lastPart = false,
        string[] firstCharacterPhrases = null, string[] secondCharacterPhrases = null, string[] narratorPhrases = null,
        string bottomText = null, Award award = null)
    {
        var key = $"Tutorial_{keyPart}_Played";
        if (QSReader.Create("TutorialState").Exists(key, "TutorialSkipped")) return;

        SaveTutorialData(key);

        DialogPanel.InitDialogPanel(FirstCharacterSprite, SecondCharacterSprite, NarratorSprite);

        if (firstCharacterPhrases != null && secondCharacterPhrases != null)
        {
            for (var i = 0; i < Math.Max(firstCharacterPhrases.Length, secondCharacterPhrases.Length); i++)
            {
                if (i < firstCharacterPhrases.Length)
                    DialogPanel.AddPhrase(NowTalking.First, firstCharacterPhrases[i]);

                if (i < secondCharacterPhrases.Length)
                    DialogPanel.AddPhrase(NowTalking.Second, secondCharacterPhrases[i]);
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
            DialogPanel.AddAward(firstCharacterPhrases?.Length ?? 0, award);

        if (lastPart == false)
            HighlightNextButton(activeButtonName: activeButtonName, activeButtonTag: activeButtonTag,
                bottomText: bottomText);
        else
            DialogPanel.LastAction = () =>
                TutorialHandler.ClearGameAfterTutorial();

        DialogPanel.SkipTutorialBtnActive = true;
        DialogPanel.StartDialog();
    }

    /// <summary>
    /// Подсвечивает нужную кнопку
    /// </summary>
    private void HighlightNextButton(string activeButtonName = null, string activeButtonTag = null,
        string bottomText = null)
    {
        // создаёт блокер и дублирует нужную кнопку
        if (BlockerPrefab != null)
        {
            DialogPanel.LastAction = () =>
            {
                GameObject activeButton = null;

                if (activeButtonName != null)
                    activeButton = GameObject.Find(activeButtonName);
                else if (activeButtonTag != null)
                    activeButton = GameObject.FindGameObjectWithTag(activeButtonTag);
                if (activeButton == null) return;

                var canvas = GameObject.FindGameObjectWithTag("Canvas");
                Instantiate(BlockerPrefab, canvas.transform, false);
                var fakeBtn = Instantiate(activeButton, activeButton.transform.parent);
                fakeBtn.transform.SetSiblingIndex(activeButton.transform.GetSiblingIndex());
                fakeBtn.name = $"FakeButton_{activeButton.name}";
                fakeButtons.Add(activeButton, fakeBtn);

                GameObject bottomTextPanel = null;
                if (bottomText != null)
                {
                    bottomTextPanel = Instantiate(BottomTextPrefab, canvas.transform, false);
                    bottomTextPanel.GetComponentInChildren<Text>().text = bottomText;
                }

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
                        Destroy(bottomTextPanel);
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
            .LastOrDefault(s => s.Name == nameEnglish);
        if (tutorSeed == null) return;

        tutorSeed.NameInRussian = nameRussian;
        GameObject.FindGameObjectWithTag("Inventory")
            .GetComponent<Inventory>()
            .Save();
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
