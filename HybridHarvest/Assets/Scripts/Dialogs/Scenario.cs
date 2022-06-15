using System;
using CI.QuickSave;
using System.Collections.Generic;
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
        DialogPanel.AddAward(6, new Award(AwardType.Money, amount: 500));
        DialogPanel.AddPhrase(NowTalking.Second, "А я на них... не вижу никакого подтекста.", 6);

        DialogPanel.AddPhrase(NowTalking.First, "3. Получить огурчик!", 3); // ID = 8
        DialogPanel.AddAward(8, new Award(AwardType.Seed, seedName: "Cucumber"));

        DialogPanel.AddPhrase(NowTalking.First,
            "Хочется взять что-то необычное... Я думаю, огурец выглядит неплохо, поэтому возьму Иерусалим.");
        DialogPanel.AddPhrase(NowTalking.Second, "???????????????\n[Великий Стонкс помер от такого]");
        DialogPanel.AddPhrase(NowTalking.Narrator, "Со смертью этого персонажа нить повествования обрывается.");

        DialogPanel.AddPhrase(NowTalking.Narrator, "Хочешь бесплатную репутацию?"); // ID = 12
        DialogPanel.AddPhrase(NowTalking.First, "1. Да!", 12); // ID = 13
        DialogPanel.AddAward(13, new Award(AwardType.Reputation, amount: 500));
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

    public void CreateFirstTaskDialog(params Award[] awards)
    {
        DialogPanel.InitDialogPanel(FirstCharacterSprite, SecondCharacterSprite, NarratorSprite);
        DialogPanel.AddPhrase(NowTalking.First,
            "Ох, большое спасибо тебе, милок! Я вот тебе немного денюжек насобирала, авось пригодятся!");

        foreach (var award in awards)
            DialogPanel.AddAward(1, award);

        DialogPanel.AddPhrase(NowTalking.First,
            "Кстати, я о тебе рассказала муженьку своему - старосте, так он сразу за мной увязался: сказал, мол, на тебя взглянуть хочет.");

        DialogPanel.StartDialog();

        // тутор для выполнения первого задания
        DialogPanel.LastAction = () => FirstQuestCompleted();

        // активирует кнопку выставки
        var writer = QuickSaveWriter.Create("PurchasedEnhancements");
        writer.Write("Exhibition", true);
        writer.Commit();
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
                if (QSReader.Create("LevelState").Exists("LevelUp2"))
                    GetFirstQuest();

                // тутор для первого захода в меню выбора
                ExecuteTutorialPart("BeginningChoice", activeButtonName: "SideMenuButton",
                    firstCharacterPhrases: new[]
                    {
                        "Наконец-то я понял, чего мне не хватало всё это время! Огромное количество энергии - вот ответ на все вопросы!",
                        "Теперь мне бы не помешало раздобыть немного семян для экспериментов. Да и деньги тоже бы пригодились.",
                        "Ну и конечно, мне нужно доказать всем, кто в меня не верил, что моё изобретение работает.",
                        "Начнём с простого - нужно достать семена для экспериментов. Кажется, здесь неподалёку есть один магазинчик, где их продают..."
                    },
                    bottomText: "Нажмите на кнопку бокового меню.");

                break;
            case 2:
                FirstCharacterSprite = Resources.Load<Sprite>("Characters\\Narrator");
                SecondCharacterSprite = Resources.Load<Sprite>("Characters\\MainHero");

                ExecuteTutorialPart("BeginningField", activeButtonName: "FarmSpot",
                    firstCharacterPhrases: new[] { "Это поле. Здесь можно посадить семена, которые есть на складе." },
                    secondCharacterPhrases: new[]
                    {
                        "Самое время посадить семена огурца, которые я только что купил. Я как раз подготовил несколько грядок!"
                    },
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
                "Здравствуйте, а вы не знаете, где тут можно прикупить семян?",
                "Ох, а я как-то сразу и не заметил.",
                "Ого, всё верно! А откуда вы моего деда знаете?",
                "Да мне бы семян каких-нибудь, любые подойдут.",
            },
            secondCharacterPhrases: new[]
            {
                "Да как не знать? Знаю. Это ж я их и продаю. Видишь - написано: \"Магазин\"?",
                "А, погоди, малой! Ты же внук того самого профессора, который у себя дома безвылазно сидел? Тебя вроде Альбертом кличут, верно?",
                "Он ко мне тоже захаживал и прям как ты семена просил. Эх, хороший мужик был... Ну, так чего ты там хотел?",
                "Ну, всё, что есть - на прилавке. Покупай - не хочу. И не забудь: у нас в деревне только порублями расплачиваются.",
            },
            narratorPhrases: new[]
                { "С повышением уровня в магазине будут появляться новые виды семян и различные улучшения." },
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
                { "Чтобы добавить новый пакет семян на склад, нажмите на кнопку с плюсом." },
            bottomText: "Нажмите на \"+\".");
    }

    public void Tutorial_ShopExit()
    {
        ExecuteTutorialPart("ShopExit", activeButtonName: "ShopExit",
            firstCharacterPhrases: new[]
            {
                "Как хорошо, что у меня в кармане нашлось немного порублей! Теперь осталось только посадить эти семена и у меня уже будет несколько образцов."
            },
            bottomText: "Нажмите на кнопку выхода из магазина.");
    }

    public void Tutorial_GoToField()
    {
        ExecuteTutorialPart("GoToField", activeButtonName: "SceneButtonField",
            bottomText: "Нажмите на кнопку \"Поле\".");
    }

    public void Tutorial_ChooseItemToPlant()
    {
        FirstCharacterSprite = Resources.Load<Sprite>("Characters\\MainHero");

        ExecuteTutorialPart("ChooseItemToPlant", activeButtonName: "0",
            narratorPhrases: new[] { "Выберите пакет семян, который хотите посадить." },
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
            firstCharacterPhrases: new[] { "Хм, что-то долго они растут. Интересно, сколько ещё нужно ждать?" },
            bottomText: "Нажмите на грядку.");
    }

    public void Tutorial_SpeedUpItem()
    {
        ExecuteTutorialPart("SpeedUpItem", activeButtonName: "SpeedUpBtn",
            firstCharacterPhrases: new[]
            {
                "Чего?! Три минуты?! Я столько ждать не намерен!",
                "Хорошо, что я вчера изобрёл способ ускорения роста семян! А ну-ка..."
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
            firstCharacterPhrases: new[] { "Вот так намного лучше!" },
            bottomText: "Нажмите на грядку, как только огурцы вырастут.");
    }

    public void Tutorial_HarvestPlace()
    {
        ExecuteTutorialPart("HarvestPlace", activeButtonName: "AddToInventory",
            firstCharacterPhrases: new[]
            {
                "Ух ты, огурчиков выросло больше, чем я ожидал. Тогда один я использую для получения семян, а остальные продам."
            },
            narratorPhrases: new[]
            {
                "На данной панели отображается ваш урожай. Выросшие плоды можно превратить в семена для дальнейшего использования или продать."
            },
            bottomText: "Нажмите на кнопку \"+\".");
    }

    public void Tutorial_ChooseItemToReplace()
    {
        ExecuteTutorialPart("ChooseItemToReplace", activeButtonName: "0",
            narratorPhrases: new[]
            {
                "Вы можете не занимать ещё одно место новыми семенами, а заменить уже имеющиеся. Так на складе останется больше свободного места."
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
            bottomText: "Поставьте галочку в квадрате рядом с кнопкой \"Продать выбранное\".");
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
                "Фух, а это тяжелее, чем я думал... Надо бы отдохнуть!"
            },
            narratorPhrases: new[] { "Когда вы садите растения, у вас тратится энергия." },
            bottomText: "Нажмите на информацию об энергии.");
    }

    public void Tutorial_Energy()
    {
        ExecuteTutorialPart("Energy", activeButtonName: "EnergyExit", narratorPhrases: new[]
            {
                "Энергия сама восстанавливается со временем, но если вы хотите - можно смотреть рекламу и получать энергию абсолютно бесплатно! :)"
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

    public void GetFirstQuest()
    {
        FirstCharacterSprite = Resources.Load<Sprite>("Characters\\OldLady");
        SecondCharacterSprite = Resources.Load<Sprite>("Characters\\MainHero");

        ExecuteStoryPart("GetFirstQuest", activeButtonName: "SideMenuButton",
            firstCharacterPhrases: new[]
            {
                "Ох ты, милок, здравствуй! Я тебя искала-искала, а ты как в воду канул.",
                "Ой, ну конечно ты меня не помнишь. Ты же тогда ещё пешком под стол ходил. Хорошо, что ты сюда приехал, милок, а то бы стояла изба твоего деда никому не нужная.",
                "Знавала, конечно. Ох, ну и работящий человек был. Я к нему иногда захаживала, огурчики, да помидорчики брала. У него они всегда самые вкусные были. \nМожет и у тебя парочка найдётся?",
                "Ой, милок, балуешь ты меня... Ах да, чуть не забыла: зовут меня Серафима Ивановна. \nНу а о награде не беспокойся, уж кого-кого, а благодетелей своих я вовек не забуду!"
            },
            secondCharacterPhrases: new[]
            {
                "Добрый вечер, бабушка! Вы наверное меня с кем-то перепутали. Я ведь только недавно сюда приехал.",
                "Ничего себе, вы моего деда знали?",
                "Для вас - найдётся! Только подождать немного надо, пока они поспеют."
            },
            bottomText: "Нажмите на кнопку бокового меню.");
    }

    public void SideMenuToQuests()
    {
        ExecuteStoryPart("SideMenuToQuests", activeButtonName: "QuestLabel",
            bottomText: "Нажмите на кнопку \"Задания\".");
    }

    public void QuestDescription()
    {
        ExecuteStoryPart("QuestDescription",
            narratorPhrases: new[]
            {
                "Чтобы выполнить задание, вам необходимо вырастить огурцы и помидоры и собрать с них урожай. \n(Семена помидора продаются у торговца)"
            },
            bottomText: "Нажмите на кнопку \"Задания\".");
    }

    public void ShopLevel2()
    {
        FirstCharacterSprite = Resources.Load<Sprite>("Characters\\Salesman");
        ExecuteStoryPart("ShopLevel2",
            firstCharacterPhrases: new[]
            {
                "Приветствую! У меня в продаже недавно появились семена помидора. Цена небольшая, советую купить."
            },
            narratorPhrases: new[]
            {
                "При повышении уровня у вас появляется возможность покупать различные улучшения, которые находятся во вкладке \"Улучшения\". (Она расположена справа снизу)"
            });
    }

    public void Tutorial_()
    {
        ExecuteStoryPart("",
            narratorPhrases: new[]
            {
                "На панели справа ототбражаются активные задания. При нажатии на кнопку \"+\" под портретом персонажа все выбранные плоды будут использованы для выполнения задания."
            });
    }

    public void FirstQuestCompleted()
    {
        FirstCharacterSprite = Resources.Load<Sprite>("Characters\\OldMan");
        SecondCharacterSprite = Resources.Load<Sprite>("Characters\\MainHero");

        ExecuteStoryPart("FirstQuestCompleted",
            firstCharacterPhrases: new[]
            {
                "Здравствуй, милок! Я - староста этой деревни. Зовут меня Максимилиан Вениаминович, но детвора меня дедом Максимом кличет. А тебя как звать?",
                "Хорошее имечко! Ну так, а что я собственно хотел-то... \nАх да, спасибо за то, что супруге моей помог. Она всё хотела салат вкусный приготовить, да овощей подходящих найти никак не могла.",
                "Да, насчёт этого... У меня есть для тебя парочка заданий, да вот не знаю, осилишь ли. Давай так поступим: у нас тут иногда люди выставки устраивают, как займёшь первое место на такой - тогда и поговорим."
            },
            secondCharacterPhrases: new[]
            {
                "Приятно познакомиться! Я - Альберт. Родители назвали меня так в честь одного великого учёного!",
                "Всегда рад помочь! Если ещё что нужно будет - обращайтесь, помогу с радостью!",
                "Выставки? То, что надо! Я как раз хотел всем показать, какие чудо-растения можно получить с помощью научного подхода!"
            });
    }

    public void ExhibitionWin()
    {
        ExecuteStoryPart("ExhibitionWin",
            narratorPhrases: new[]
            {
                "Поздравляем! Вы победили на выставке!"
            }, isLastPhrase: true);
    }

    public void StoryEnd()
    {
        ExecuteStoryPart("StoryEnd",
            narratorPhrases: new[]
            {
                "Поздравляем! Вам удалось добраться до конца сюжета в этой версии игры. Но не беспокойтесь, история Альберта на этом не заканчивается.",
                "Ждите новых заданий и персонажей в следующем обновлении! \nP.S. Вы можете и дальше скрещивать и выращивать новые удивительные растения!",
                "С наилучшими пожеланиями, Teel@Software!"
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
    /// Проигрывает часть сюжета.
    /// </summary>
    /// <param name="keyPart">Название, по которому идёт сохранение.</param>
    /// <param name="activeButtonName">Название кнопки, которую следует сделать активной после окончания части вступления.</param>
    /// <param name="activeButtonTag">Тег кнопки, которую следует сделать активной после окончания части вступления.</param>
    /// <param name="firstCharacterPhrases">Фразы, которые говорит первый персонаж.</param>
    /// <param name="secondCharacterPhrases">Фразы, которые говорит второй персонаж.</param>
    /// <param name="narratorPhrases">Фразы, которые говорит рассказчик.</param>
    /// <param name="bottomText">Текст, показываемый внизу экрана.</param>
    /// <param name="award">Награда после слов первого персонажа.</param>
    private void ExecuteStoryPart(string keyPart, string activeButtonName = null, string activeButtonTag = null,
        string[] firstCharacterPhrases = null, string[] secondCharacterPhrases = null, string[] narratorPhrases = null,
        string bottomText = null, Award award = null, bool isLastPhrase = false)
    {
        var fileNamePart = "Story";
        var key = $"{fileNamePart}_{keyPart}_Played";
        if (QSReader.Create($"{fileNamePart}State").Exists(key)) return;

        SavePlayedPart($"{fileNamePart}State", key);

        PrepareDialog(activeButtonName, activeButtonTag, firstCharacterPhrases, secondCharacterPhrases, narratorPhrases,
            bottomText, award, isLastPhrase: isLastPhrase);

        DialogPanel.StartDialog();
    }

    /// <summary>
    /// Проигрывает часть туториала.
    /// </summary>
    /// <param name="keyPart">Название, по которому идёт сохранение.</param>
    /// <param name="activeButtonName">Название кнопки, которую следует сделать активной после окончания части вступления.</param>
    /// <param name="activeButtonTag">Тег кнопки, которую следует сделать активной после окончания части вступления.</param>
    /// <param name="firstCharacterPhrases">Фразы, которые говорит первый персонаж.</param>
    /// <param name="secondCharacterPhrases">Фразы, которые говорит второй персонаж.</param>
    /// <param name="narratorPhrases">Фразы, которые говорит рассказчик.</param>
    /// <param name="bottomText">Текст, показываемый внизу экрана.</param>
    /// <param name="award">Награда после слов первого персонажа.</param>
    private void ExecuteTutorialPart(string keyPart, string activeButtonName = null, string activeButtonTag = null,
        string[] firstCharacterPhrases = null, string[] secondCharacterPhrases = null, string[] narratorPhrases = null,
        string bottomText = null, Award award = null)
    {
        var fileNamePart = "Tutorial";
        var key = $"{fileNamePart}_{keyPart}_Played";
        if (QSReader.Create($"{fileNamePart}State").Exists(key, "TutorialSkipped")) return;

        PrepareDialog(activeButtonName, activeButtonTag, firstCharacterPhrases, secondCharacterPhrases, narratorPhrases,
            bottomText, award, fileNamePart, keyPart);

        DialogPanel.SkipTutorialBtnActive = true;
        DialogPanel.StartDialog();

        SaveNowPlaying($"{fileNamePart}State", $"Tutorial_{keyPart}");
    }

    /// <summary>
    /// Подготавливает и запускает диалог по заданным параметрам.
    /// </summary>
    /// <param name="activeButtonName">Название кнопки, которую следует сделать активной после окончания части вступления.</param>
    /// <param name="activeButtonTag">Тег кнопки, которую следует сделать активной после окончания части вступления.</param>
    /// <param name="firstCharacterPhrases">Фразы, которые говорит первый персонаж.</param>
    /// <param name="secondCharacterPhrases">Фразы, которые говорит второй персонаж.</param>
    /// <param name="narratorPhrases">Фразы, которые говорит рассказчик.</param>
    /// <param name="bottomText">Текст, показываемый внизу экрана.</param>
    /// <param name="award">Награда после слов первого персонажа.</param>
    private void PrepareDialog(string activeButtonName, string activeButtonTag, string[] firstCharacterPhrases,
        string[] secondCharacterPhrases, string[] narratorPhrases, string bottomText, Award award,
        string fileName = null, string partName = null, bool isLastPhrase = false)
    {
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

        HighlightNextButton(activeButtonName, activeButtonTag, bottomText, fileName, partName);

        if (isLastPhrase)
            DialogPanel.LastAction = StoryEnd;
    }

    /// <summary>
    /// Подсвечивает нужную кнопку
    /// </summary>
    private void HighlightNextButton(string activeButtonName = null, string activeButtonTag = null,
        string bottomText = null, string fileName = null, string partName = null)
    {
        // создаёт блокер и дублирует нужную кнопку
        if (BlockerPrefab != null)
        {
            DialogPanel.LastAction = () =>
            {
                SavePlayedPart($"{fileName}State", $"{fileName}_{partName}_Played");
                GameObject activeButton = null;

                if (activeButtonName != null)
                    activeButton = GameObject.Find(activeButtonName);
                else if (activeButtonTag != null)
                    activeButton = GameObject.FindGameObjectWithTag(activeButtonTag);
                if (activeButton == null)
                {
                    SaveNowPlaying($"{fileName}State");
                    return;
                }

                var canvas = GameObject.FindGameObjectWithTag("Canvas");
                var blocker = Instantiate(BlockerPrefab, canvas.transform, false);
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

                        var reader = QSReader.Create($"{fileName}State");
                        if (reader.Exists("NowPlaying")
                            && reader.Read<string>("NowPlaying") == $"{fileName}_{partName}")
                            SaveNowPlaying($"{fileName}State");

                        // удаляет блокер и фейковую кнопку
                        GameObject placeholder = null;
                        if (fakeButtons.ContainsKey(currentButton))
                            placeholder = fakeButtons[currentButton];

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
    /// Сохраняет данные о том, какая часть уже проигрывалась.
    /// </summary>
    private static void SavePlayedPart(string fileName, string partName)
    {
        var writer = QuickSaveWriter.Create(fileName);
        writer.Write(partName, true);
        writer.Commit();
    }

    /// <summary>
    /// Сохраняет данные о том, какая часть проигрывается сейчас.
    /// </summary>
    private static void SaveNowPlaying(string fileName, string partName = "")
    {
        var writer = QuickSaveWriter.Create(fileName);
        writer.Write("NowPlaying", partName);
        writer.Commit();
    }
}
