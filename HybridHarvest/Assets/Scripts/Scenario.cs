using UnityEngine;

public class Scenario : MonoBehaviour
{
    [SerializeField] DialogPanelLogic DialogPanel;
    [SerializeField] Sprite FirstCharacterSprite;
    [SerializeField] Sprite SecondCharacterSprite;
    [SerializeField] Sprite NarratorSprite;

    /// <summary>
    /// В подобных методах нужно создавать диалоги
    /// </summary>
    public void MakeTestDialog()
    {
        // Пока что лучше не добавлять больше трёх ответов, будут проблемы с отображением

        DialogPanel.CreateDialogPanel(FirstCharacterSprite, SecondCharacterSprite, NarratorSprite);

        DialogPanel.AddPhrase(NowTalking.Second, "Приветствую путник, вижу ты первый раз здесь.");
        DialogPanel.AddPhrase(NowTalking.First, "Эээээ, ну здрасьте!");

        var ph1 = "Я - Великий Стонкс и у меня есть всевозможные товары, начиная с огурца и заканчивая морковью.";
        DialogPanel.AddPhrase(NowTalking.Second, ph1);
        var ph1ans1 = "1. А как же сосисочки?";
        DialogPanel.AddPhrase(NowTalking.First, ph1ans1, ph1);
        DialogPanel.AddAward(ph1ans1, new Award(AwardType.Achievement, message: "Достижение разблокировано: \"Вернуться в 2007\""));
        var ph1ans1ph1 = "Да, и хороший борщик... Не продаём такое, к сожалению.";
        DialogPanel.AddPhrase(NowTalking.Second, ph1ans1ph1, ph1ans1);
        var ph1ans2 = "2. Вы любите розы?";
        DialogPanel.AddPhrase(NowTalking.First, ph1ans2, ph1);
        DialogPanel.AddAward(ph1ans2, new Award(AwardType.Money, money: 500));
        var ph1ans2ph1 = "А я на них... не вижу никакого подтекста.";
        DialogPanel.AddPhrase(NowTalking.Second, ph1ans2ph1, ph1ans2);
        var ph1ans3 = "3. Получить огурчик!";
        DialogPanel.AddPhrase(NowTalking.First, ph1ans3, ph1);
        DialogPanel.AddAward(ph1ans3, new Award(AwardType.Seed, seedName: "Cucumber"));

        DialogPanel.AddPhrase(NowTalking.First, "Хочется взять что-то необычное... Я думаю, огурец выглядит неплохо, поэтому возьму Иерусалим.");
        DialogPanel.AddPhrase(NowTalking.Second, "???????????????\n[Великий Стонкс помер от такого]");
        DialogPanel.AddPhrase(NowTalking.Narrator, "Со смертью этого персонажа нить повествования обрывается.");

        var ph2 = "Хочешь бесплатную репутацию?";
        DialogPanel.AddPhrase(NowTalking.Narrator, ph2);
        DialogPanel.AddPhrase(NowTalking.First, "1. Да!", ph2);
        DialogPanel.AddAward("1. Да!", new Award(AwardType.Reputation, reputation: 500));
        DialogPanel.AddPhrase(NowTalking.First, "2. Нет!", ph2);
        DialogPanel.AddPhrase(NowTalking.First, "3. Четыре!", ph2);

        DialogPanel.StartDialog();
    }
}
