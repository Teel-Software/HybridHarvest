using System.Collections.Generic;
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
/// Содержит персонажа и его речь
/// </summary>
internal class Speech
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

public class DialogPanelLogic : MonoBehaviour
{
    [SerializeField] Image CharacterSpritePlace;

    private Sprite FirstCharacterSprite;
    private Sprite SecondCharacterSprite;
    private Sprite NarratorSprite;

    private List<Speech> scenario;
    private Dictionary<string, List<Speech>> answers;
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
        FirstCharacterSprite = firstCharacterSprite;
        SecondCharacterSprite = secondCharacterSprite;
        NarratorSprite = narratorSprite;
        cleaningIsNeeded = false;
    }

    /// <summary>
    /// Добавляет фразу в конец диалога либо добавляет ответ к фразе, указанной в 'answerOn' 
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

        // Приводит текст к стандартному отображению после выбора варианта ответа
        if (cleaningIsNeeded)
        {
            var newText = Instantiate(firstTextComponent);
            var CGD = gameObject.AddComponent<ClearGameData>();

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
        }

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
    /// Called when user clicks on button
    /// </summary>
    private void OnButtonClicked()
    {
        var button = EventSystem.current.currentSelectedGameObject;
        if (button == null) return;

        lastPhrase = button.GetComponent<Text>().text;
        LoadNewPhrase();
    }
}
