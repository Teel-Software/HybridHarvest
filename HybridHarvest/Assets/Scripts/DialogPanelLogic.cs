﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Определяет говорящего в текущий момент персонажа
/// </summary>
public enum NowTalking
{
    Narrator,
    First,
    Second
}

/// <summary>
/// Определяет возможный приз
/// </summary>
public enum AwardType
{
    Money,
    Seed,
    Achievement,
    Reputation
}

/// <summary>
/// Содержит персонажа и его речь
/// </summary>
public class Speech
{
    /// <summary>
    /// Содержит персонажа и его речь
    /// </summary>
    public Speech(NowTalking character, string phrase)
    {
        Character = character;
        Phrase = phrase;
    }

    public NowTalking Character { get; private set; }
    public string Phrase { get; private set; }
}

/// <summary>
/// Содержит приз и его компоненты
/// </summary>
public class Award
{
    /// <summary>
    /// Содержит приз и его компоненты
    /// </summary>
    public Award(AwardType currentPrize, string message = "", int money = 0, int reputation = 0, string seedName = "")
    {
        CurrentPrize = currentPrize;
        Message = message;
        Money = money;
        Reputation = reputation;
        SeedName = seedName;
    }

    public AwardType CurrentPrize { get; private set; }
    public string Message { get; private set; }
    public int Money { get; private set; }
    public int Reputation { get; private set; }
    public string SeedName { get; private set; }
}

public class DialogPanelLogic : MonoBehaviour
{
    [SerializeField] Image CharacterSpritePlace;
    [SerializeField] Inventory targetInventory;

    private Sprite FirstCharacterSprite;
    private Sprite SecondCharacterSprite;
    private Sprite NarratorSprite;

    private List<Speech> scenario;
    private Dictionary<string, List<Speech>> answers;
    private Dictionary<string, Award> awards;

    private int speechIndex;
    private string lastPhrase;
    private bool cleaningIsNeeded;

    /// <summary>
    /// Готовит панель к показу диалога, следует вызывать этот метод первым
    /// </summary>
    public void CreateDialogPanel(Sprite firstCharacterSprite, Sprite secondCharacterSprite, Sprite narratorSprite = default)
    {
        scenario = new List<Speech>();
        answers = new Dictionary<string, List<Speech>>();
        awards = new Dictionary<string, Award>();
        FirstCharacterSprite = firstCharacterSprite;
        SecondCharacterSprite = secondCharacterSprite;
        NarratorSprite = narratorSprite;
        cleaningIsNeeded = false;
    }

    /// <summary>
    /// Добавляет фразу в конец диалога, либо добавляет ответ к фразе, указанной в 'answerOn' 
    /// </summary>
    public void AddPhrase(NowTalking character, string phrase, string answerOn = null)
    {
        var speech = new Speech(character, phrase);

        if (answerOn == null)
            scenario.Add(speech);
        else if (!answers.ContainsKey(answerOn))
            answers[answerOn] = new List<Speech> { speech };
        else answers[answerOn].Add(speech);
    }

    /// <summary>
    /// Добавляет награду, появляющуюся после определённого ответа
    /// </summary>
    public void AddAward(string answer, Award award)
    {
        awards.Add(answer, award);
    }

    /// <summary>
    /// Запускает диалог, следует вызывать этот метод последним
    /// </summary>
    public void StartDialog()
    {
        speechIndex = 0;
        LoadNewPhrase();
        transform.gameObject.SetActive(true);
    }

    /// <summary>
    /// Выводит следующую (если присутствует) фразу на диалоговою панель, если фразы нет - деактивирует панель
    /// </summary>
    public void LoadNewPhrase()
    {
        var firstTextComponent = transform.gameObject.GetComponentInChildren<Text>();
        var textPanel = firstTextComponent.transform.parent.gameObject;
        var currentSpeech = new Speech(NowTalking.Narrator, "Ты это не должен был увидеть... Проверь код на баги!");
        List<Speech> currentAnswers;

        if (cleaningIsNeeded)
            firstTextComponent = RedrawText(firstTextComponent, textPanel);

        // Проверка на последнюю фразу
        if (speechIndex >= scenario.Count && !answers.ContainsKey(lastPhrase))
        {
            transform.gameObject.SetActive(false);
            return;
        };

        if (speechIndex < scenario.Count)
            currentSpeech = scenario[speechIndex];

        // Выполняется, если у фразы есть хотя бы один ответ
        if (lastPhrase != null && answers.ContainsKey(lastPhrase))
        {
            currentAnswers = answers[lastPhrase];
            currentSpeech = currentAnswers[0];

            if (currentAnswers.Count > 1)
                DrawAnswers(firstTextComponent, textPanel, currentAnswers);
        }
        else speechIndex++;

        CharacterSpritePlace.sprite =
        currentSpeech.Character == NowTalking.First
        ? FirstCharacterSprite
        : currentSpeech.Character == NowTalking.Second
            ? SecondCharacterSprite
            : NarratorSprite;

        if (!cleaningIsNeeded)
        {
            firstTextComponent.text = currentSpeech.Phrase;
            firstTextComponent.gameObject.name = "Main Text";
            lastPhrase = currentSpeech.Phrase;
        }
    }

    /// <summary>
    /// Выводит на экран все ответы к текущей фразе
    /// </summary>
    private void DrawAnswers(Text firstTextComponent, GameObject textPanel, List<Speech> currentAnswers)
    {
        var gridLay = textPanel.GetComponent<GridLayoutGroup>();
        gridLay.enabled = true;
        var paddingVal = 10;
        gridLay.cellSize = new Vector2(textPanel.GetComponent<RectTransform>().rect.width - 2 * paddingVal,
            (textPanel.GetComponent<RectTransform>().rect.height - 4 * paddingVal) / 3);

        for (var i = 0; i < currentAnswers.Count; i++)
        {
            var newText = Instantiate(firstTextComponent);
            newText.text = currentAnswers[i].Phrase;
            newText.transform.SetParent(firstTextComponent.transform.parent, false);
            newText.gameObject.name = $"Text {i + 1}";

            newText.gameObject.AddComponent<Button>();
            newText.GetComponent<Button>().onClick.AddListener(OnButtonClicked);
            newText.GetComponent<Button>().targetGraphic = newText;
        }

        cleaningIsNeeded = true;
        Destroy(firstTextComponent.gameObject);

        // Выключает возможность переключать фразу по нажатию в любом месте экрана
        transform.GetComponentInChildren<Button>().enabled = false; // finds Blocker
        textPanel.GetComponent<Button>().enabled = false;
    }

    /// <summary>
    /// Приводит текст к стандартному отображению после выбора варианта ответа
    /// </summary>
    private Text RedrawText(Text firstTextComponent, GameObject textPanel)
    {
        var newText = Instantiate(firstTextComponent);
        var CGD = gameObject.GetComponent<ClearGameData>() ?? gameObject.AddComponent<ClearGameData>();

        textPanel.GetComponent<GridLayoutGroup>().enabled = false;
        CGD.DeleteChildren(textPanel);
        newText.transform.SetParent(textPanel.transform, false);

        var paddingVal = 10;
        var newTextRT = newText.gameObject.GetComponent<RectTransform>();
        newTextRT.anchorMin = Vector2.zero;
        newTextRT.anchorMax = Vector2.one;
        newTextRT.sizeDelta = new Vector2(-2 * paddingVal, -2 * paddingVal);

        firstTextComponent = newText;
        cleaningIsNeeded = false;

        // Позволяет переключать фразу по нажатию в любом месте экрана
        transform.GetComponentInChildren<Button>().enabled = true; // finds Blocker
        textPanel.GetComponent<Button>().enabled = true;
        Destroy(newText.GetComponent<Button>());
        return firstTextComponent;
    }

    /// <summary>
    /// Вызывается при нажатии на один из ответов
    /// </summary>
    private void OnButtonClicked()
    {
        var answer = EventSystem.current.currentSelectedGameObject;
        if (answer == null) return;

        lastPhrase = answer.GetComponent<Text>().text;

        if (awards.ContainsKey(lastPhrase))
        {
            var pr = awards[lastPhrase];
            var message = "";

            switch (pr.CurrentPrize)
            {
                case AwardType.Achievement:
                    message = pr.Message;
                    break;
                case AwardType.Money:
                    message = $"Вы получили монеты ({pr.Money})";
                    targetInventory.ChangeMoney(pr.Money);
                    targetInventory.SaveAllData();
                    break;
                case AwardType.Seed:
                    var seed = (Seed)Resources.Load("Seeds\\" + pr.SeedName);
                    message = $"Вы получили {seed.NameInRussian.ToLower()}!";
                    targetInventory.AddItem(seed, true);
                    targetInventory.SaveAllData();
                    break;
                case AwardType.Reputation:
                    message = $"Вы получили очки репутации ({pr.Reputation})";
                    targetInventory.ChangeReputation(pr.Reputation);
                    targetInventory.SaveAllData();
                    break;
            }

            GetComponent<NotificationCenter>().Show(message);
        }

        LoadNewPhrase();
    }
}
