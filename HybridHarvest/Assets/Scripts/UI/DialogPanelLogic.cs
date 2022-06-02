using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

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

    public NowTalking Character { get; }
    public string Phrase { get; }
}

public class DialogPanelLogic : MonoBehaviour
{
    [SerializeField] Image CharacterSpritePlace;

    public bool SkipTutorialBtnActive { get; set; }
    public Action LastAction { get; set; } // действие активируется после исчезновения диалога с экрана

    private Sprite FirstCharacterSprite;
    private Sprite SecondCharacterSprite;
    private Sprite NarratorSprite;

    private List<Speech> speechByID;
    private Dictionary<string, int> IDByPhrase;
    private List<Speech> scenario;
    private Dictionary<int, List<Speech>> answers;
    private Dictionary<int, HashSet<Award>> awards;
    private HashSet<int> hideTriggers;

    private int speechIndex;
    private int lastPhraseID;
    private bool cleaningIsNeeded;

    /// <summary>
    /// Готовит панель к показу диалога, следует вызывать этот метод первым
    /// </summary>
    public void InitDialogPanel(Sprite firstCharacterSprite, Sprite secondCharacterSprite,
        Sprite narratorSprite = default)
    {
        speechByID = new List<Speech> { new Speech(NowTalking.Narrator, "fill zero slot") };
        IDByPhrase = new Dictionary<string, int>();
        scenario = new List<Speech>();
        answers = new Dictionary<int, List<Speech>>();
        awards = new Dictionary<int, HashSet<Award>>();
        hideTriggers = new HashSet<int>();
        lastPhraseID = 0;

        FirstCharacterSprite = firstCharacterSprite;
        SecondCharacterSprite = secondCharacterSprite;
        NarratorSprite = narratorSprite;
        cleaningIsNeeded = false;
        SkipTutorialBtnActive = false;
        LastAction = null;
    }

    /// <summary>
    /// Добавляет фразу в конец диалога, либо добавляет ответ к фразе с ID, указанным в 'answerOnID'.
    /// Скрывает панель после фразы, если включен hideTrigger.
    /// </summary>
    public void AddPhrase(NowTalking character, string phrase, int answerOnID = 0, bool hideTrigger = false)
    {
        if (scenario is null)
            throw new MethodAccessException("Call method \"CreateDialogPanel\" first!");

        var speech = new Speech(character, phrase);
        speechByID.Add(speech);
        IDByPhrase.Add(speech.Phrase, ++lastPhraseID);

        if (hideTrigger)
            hideTriggers.Add(lastPhraseID);

        if (answerOnID == 0)
            scenario.Add(speech);
        else if (!answers.ContainsKey(answerOnID))
            answers[answerOnID] = new List<Speech> { speech };
        else answers[answerOnID].Add(speech);
    }

    /// <summary>
    /// Добавляет награду, появляющуюся после определённого ответа
    /// </summary>
    public void AddAward(int answerID, Award award)
    {
        if (!awards.ContainsKey(answerID))
            awards.Add(answerID, new HashSet<Award>());
        awards[answerID].Add(award);
    }

    /// <summary>
    /// Запускает диалог, следует вызывать этот метод последним
    /// </summary>
    public void StartDialog()
    {
        speechIndex = 0;
        lastPhraseID = 0;
        Show();

        var skipTutBtn = transform.Find("SkipTutorial").gameObject;
        if (skipTutBtn != null)
            skipTutBtn.SetActive(SkipTutorialBtnActive);
        else Debug.Log("Кнопку SkipTutorial не нашёл: Active = " + SkipTutorialBtnActive);

        LoadNextPhrase();
    }

    /// <summary>
    /// Заканчивает диалог.
    /// </summary>
    public void EndDialog()
    {
        Hide();
        LastAction?.Invoke();
    }

    /// <summary>
    /// Показывает панель наград, либо отображает следующую фразу
    /// </summary>
    public void ExecuteNextMove()
    {
        if (awards.ContainsKey(lastPhraseID))
            GetComponent<AwardsCenter>().Show(awards[lastPhraseID]);
        else LoadNextPhrase();
    }

    /// <summary>
    /// Выводит следующую (если присутствует) фразу на диалоговою панель, если фразы нет - деактивирует панель.
    /// В переменной wasHided указывается true, если на последней фразе стоял hideTrigger.
    /// </summary>
    public void LoadNextPhrase(bool wasHided = false)
    {
        var firstTextComponent = transform.gameObject.GetComponentInChildren<Text>();
        var textPanel = firstTextComponent.transform.parent.gameObject;
        var currentSpeech = new Speech(NowTalking.Narrator, "Ты это не должен был увидеть... Проверь код на баги!");

        if (cleaningIsNeeded)
            firstTextComponent = RedrawMainText(firstTextComponent, textPanel);

        // Проверка на последнюю фразу
        if (speechIndex >= scenario.Count && !answers.ContainsKey(lastPhraseID)
            || !wasHided && hideTriggers.Contains(lastPhraseID))
        {
            EndDialog();
            return;
        }

        if (speechIndex < scenario.Count)
            currentSpeech = scenario[speechIndex];

        // Выполняется, если у предыдущей фразы есть хотя бы один ответ
        if (lastPhraseID != 0 && answers.ContainsKey(lastPhraseID))
        {
            var currentAnswers = answers[lastPhraseID];
            currentSpeech = currentAnswers[0];

            if (currentAnswers.Count > 1)
                DrawAnswers(firstTextComponent, textPanel, currentAnswers);
        }
        else speechIndex++;

        ChangeCharacterSprite(currentSpeech);

        // Выполняется, когда нет выбора ответов
        if (!cleaningIsNeeded)
        {
            firstTextComponent.text = currentSpeech.Phrase;
            firstTextComponent.gameObject.name = "Main Text";
            firstTextComponent.GetComponent<AnimateText>().RestartAnimation();
            lastPhraseID = IDByPhrase[currentSpeech.Phrase];
        }
    }

    /// <summary>
    /// Продолжает показ диалога с того места, на котором панель была скрыта
    /// </summary>
    public void Continue()
    {
        Show();
        LoadNextPhrase(true);
    }

    /// <summary>
    /// Показывает панель
    /// </summary>
    private void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// Скрывает панель
    /// </summary>
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Меняет спрайт персонажа
    /// </summary>
    private void ChangeCharacterSprite(Speech currentSpeech)
    {
        CharacterSpritePlace.sprite = currentSpeech.Character switch
        {
            NowTalking.First => FirstCharacterSprite,
            NowTalking.Second => SecondCharacterSprite,
            _ => NarratorSprite,
        };
    }

    /// <summary>
    /// Выводит на экран все ответы к текущей фразе
    /// </summary>
    private void DrawAnswers(Text firstTextComponent, GameObject textPanel, List<Speech> currentAnswers)
    {
        var gridLay = textPanel.GetComponent<GridLayoutGroup>();
        gridLay.enabled = true;
        var paddingVal = (firstTextComponent.transform.parent.GetComponent<RectTransform>().rect.width
                          - firstTextComponent.GetComponent<RectTransform>().rect.width) / 2;

        for (var i = 0; i < currentAnswers.Count; i++)
        {
            var newText = Instantiate(firstTextComponent, firstTextComponent.transform.parent, false);
            newText.text = currentAnswers[i].Phrase;
            newText.gameObject.name = $"Text {i + 1}";

            newText.gameObject.AddComponent<Button>();
            newText.GetComponent<Button>().onClick.AddListener(OnButtonClicked);
            newText.GetComponent<Button>().targetGraphic = newText;
        }

        gridLay.cellSize = new Vector2(textPanel.GetComponent<RectTransform>().rect.width - 2 * paddingVal,
            (textPanel.GetComponent<RectTransform>().rect.height - 2 * paddingVal
                                                                 - (currentAnswers.Count - 1) * gridLay.spacing.y) /
            (currentAnswers.Count > 3
                ? currentAnswers.Count
                : 3));

        foreach (var animText in textPanel.GetComponentsInChildren<AnimateText>().Reverse())
            animText.RestartAnimation();

        cleaningIsNeeded = true;
        Destroy(firstTextComponent.gameObject);

        // Выключает возможность переключать фразу по нажатию в любом месте экрана
        transform.GetComponentInChildren<Button>().enabled = false; // находит блокер
        textPanel.GetComponent<Button>().enabled = false;
    }

    /// <summary>
    /// Приводит текст к стандартному отображению после выбора варианта ответа
    /// </summary>
    private Text RedrawMainText(Text firstTextComponent, GameObject textPanel)
    {
        var newText = Instantiate(firstTextComponent, textPanel.transform, false);
        textPanel.GetComponent<GridLayoutGroup>().enabled = false;
        ClearGameData.ClearChildren(textPanel);

        var paddingVal = (firstTextComponent.transform.parent.GetComponent<RectTransform>().rect.width
                          - firstTextComponent.GetComponent<RectTransform>().rect.width) / 2;
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

        // защита от не прогрузившихся полностью строк
        var txt = answer.GetComponent<Text>().text;
        if (IDByPhrase.ContainsKey(txt))
            lastPhraseID = IDByPhrase[txt];
        else return;

        // отображает награды за ответ при их наличии
        if (awards.ContainsKey(lastPhraseID))
            GetComponent<AwardsCenter>().Show(awards[lastPhraseID]);
        else LoadNextPhrase();
    }
}
