using System.Collections.Generic;
using UnityEngine;
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
    private Dictionary<string, Speech> answers;
    private int speechIndex;

    /// <summary>
    /// Готовит панель к показу диалога, следует вызывать этот метод первым
    /// </summary>
    public void CreateDialogPanel(Sprite firstCharacterSprite, Sprite secondCharacterSprite, Sprite narratorSprite = null)
    {
        scenario = new List<Speech>();
        answers = new Dictionary<string, Speech>();
        FirstCharacterSprite = firstCharacterSprite;
        SecondCharacterSprite = secondCharacterSprite;
        NarratorSprite = narratorSprite;
    }

    /// <summary>
    /// Добавляет фразу в конец диалога
    /// </summary>
    public void AddPhrase(NowTalking character, string phrase, string answerOn = "")
    {
        var speech = new Speech(character, phrase);

        if (answerOn == "")
            scenario.Add(speech);
        else answers[answerOn] = speech;
    }

    /// <summary>
    /// Запускает диалог, следует вызывать этот метод последним
    /// </summary>
    public void StartDialog()
    {
        speechIndex = -1;
        LoadNewPhrase();
        transform.gameObject.SetActive(true);
    }

    /// <summary>
    /// Выводит следующую (если присутствует) фразу на диалоговою панель, если фразы нет - деактивирует панель
    /// </summary>
    public void LoadNewPhrase()
    {
        if (++speechIndex >= scenario.Count)
        {
            transform.gameObject.SetActive(false);
            return;
        };

        var currentSpeech = scenario[speechIndex];
        CharacterSpritePlace.sprite =
            currentSpeech.Character == NowTalking.First
            ? FirstCharacterSprite
            : currentSpeech.Character == NowTalking.Second
                ? SecondCharacterSprite
                : NarratorSprite;
        transform.gameObject.GetComponentInChildren<Text>().text = currentSpeech.Phrase;
    }
}
