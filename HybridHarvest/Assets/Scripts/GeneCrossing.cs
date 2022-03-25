using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System.Linq;
using System.Globalization;

public class GeneCrossing : MonoBehaviour
{
    [SerializeField] public Button CurrentPot;
    [SerializeField] Button button1;
    [SerializeField] Button button2;
    [SerializeField] Sprite defaultSprite;

    private List<int> chances;
    private List<int> oppositeSeedStats;

    public int[] Chances { get => chances.ToArray(); }
    public int[] OppositeSeedStats { get => oppositeSeedStats.ToArray(); }

    int chancesIterator;
    private Image textBGImage;

    private void Awake()
    {
        chances = new List<int>();
        oppositeSeedStats = new List<int>();
    }

    public void Clicked()
    {
        var seed1 = button1.GetComponent<LabButton>().NowSelected;
        var seed2 = button2.GetComponent<LabButton>().NowSelected;
        if (seed1 == null || seed2 == null)
            return;
        if (SceneManager.GetActiveScene().buildIndex == 4) {
            var newSeed = GetQuantumSeed(seed1, seed2);
            CurrentPot.GetComponent<QuantumGrowth>().ApplyLightning(newSeed);
        }
        else
        {
            var newSeed = MixTwoParents(seed1, seed2);
            if (SceneManager.GetActiveScene().buildIndex == 4)
                CurrentPot.GetComponent<QuantumGrowth>().ApplyLightning(newSeed);
            else
                CurrentPot.GetComponent<LabGrowth>().PlantIt(newSeed, seed1.GrowTime + seed2.GrowTime);
        }

        ExitHybridMenu();
    }

    /// <summary>
    /// clears menue when closed
    /// </summary>
    public void ExitHybridMenu()
    {
        button2.GetComponent<LabButton>().ClearButton();
        button1.GetComponent<LabButton>().ClearButton();
        button1.GetComponent<LabButton>().PlaceForResult.GetComponent<LabButton>().ClearButton();
        button1.GetComponent<LabButton>().PlaceForResult.gameObject.SetActive(false);
        button1.GetComponent<LabButton>().InventoryFrame.Redraw();
        GameObject.FindGameObjectWithTag("HybridPanel").SetActive(false); // deactivates hybrid panel
    }

    /// <summary>
    /// creates new seed, based on two parents
    /// </summary>
    /// <returns> new seed</returns>
    public Seed MixTwoParents(Seed first, Seed second)
    {
        chancesIterator = 0;
        var newSeed = ScriptableObject.CreateInstance<Seed>();
        newSeed.SetValues(first.ToString());

        (newSeed.Taste, newSeed.TasteGen) =
            CountParameter(first.Taste, first.TasteGen, second.Taste, second.TasteGen);
        oppositeSeedStats.Add(newSeed.Taste == first.Taste ? second.Taste : first.Taste);
        chancesIterator++;

        (newSeed.Gabitus, newSeed.GabitusGen) =
            CountParameter(first.Gabitus, first.GabitusGen, second.Gabitus, second.GabitusGen);
        oppositeSeedStats.Add(newSeed.Gabitus == first.Gabitus ? second.Gabitus : first.Gabitus);
        chancesIterator++;

        (newSeed.GrowTime, newSeed.GrowTimeGen) =
            CountParameter(first.GrowTime, first.GrowTimeGen, second.GrowTime, second.GrowTimeGen);
        oppositeSeedStats.Add(newSeed.GrowTime == first.GrowTime ? second.GrowTime : first.GrowTime);
        chancesIterator++;

        int temp = 0;
        (temp, newSeed.MutationPossibilityGen) =
            CountParameter((int)first.MutationPossibility, first.MutationPossibilityGen,
            (int)second.MutationPossibility, second.MutationPossibilityGen);
        oppositeSeedStats.Add((int)newSeed.MutationPossibility == (int)first.MutationPossibility ? (int)second.MutationPossibility : (int)first.MutationPossibility);
        chancesIterator++;
        newSeed.MutationPossibility = (MutationChance)temp;

        (newSeed.minAmount, newSeed.AmountGen) =
            CountParameter(first.minAmount, first.AmountGen, second.minAmount, second.AmountGen);
        oppositeSeedStats.Add(newSeed.minAmount == first.minAmount ? second.minAmount : first.minAmount);
        newSeed.maxAmount = newSeed.minAmount == first.minAmount ? first.maxAmount : second.maxAmount;

        //newSeed.Name = MixTwoNames(first.Name, second.Name, english: true);
        newSeed.NameInRussian = MixTwoNames(first.NameInRussian, second.NameInRussian);

        if (CurrentPot != null)
        {
            PlayerPrefs.SetString("SelectionChances" + CurrentPot.name, string.Join(" ", chances));
            PlayerPrefs.SetString("OppositeSeedStats" + CurrentPot.name, string.Join(" ", oppositeSeedStats));
        }

        return newSeed;
    }

    /// <summary>
    /// count new seed`s parameter based on biological laws
    /// </summary>
    public (int, Gen) CountParameter(int value1, Gen gen1, int value2, Gen gen2)
    {
        var dominant = Mathf.Min(value1, value2);
        var recessive = Mathf.Max(value1, value2);
        if (SceneManager.GetActiveScene().buildIndex == 4)
            (dominant, recessive) = (recessive, dominant);

        chances.Add(100);
        if (gen1 == Gen.Dominant && gen2 == Gen.Dominant)
            return (dominant, gen1);
        if (gen1 == Gen.Recessive && gen2 == Gen.Recessive)
            return (recessive, gen1);
        if (gen1 == Gen.Dominant && gen2 == Gen.Recessive || gen1 == Gen.Recessive && gen2 == Gen.Dominant)
            return (dominant, Gen.Mixed);
        if (gen1 == Gen.Dominant && gen2 == Gen.Mixed || gen2 == Gen.Dominant && gen1 == Gen.Mixed)
            return (dominant, (Gen)GetNewValueByPossibility((int)gen1, 50, (int)gen2));

        if (value1 != value2)
            chances[chances.Count - 1] = 50;
        var newGen = (Gen)GetNewValueByPossibility((int)gen1, 50, (int)gen2);
        if (gen1 == Gen.Recessive && gen2 == Gen.Mixed || gen2 == Gen.Recessive && gen1 == Gen.Mixed)
            return (newGen == gen1 ? value1 : value2, newGen);

        if (value1 != value2)
            chances[chances.Count - 1] = 75;
        var possibility = (int)Random.value * 100;
        newGen = possibility <= 25
            ? Gen.Dominant
            : possibility < 75
                ? Gen.Mixed
                : Gen.Recessive;

        return (dominant, newGen);
    }

    /// <summary>
    /// returns value with possibility
    /// </summary>
    public int GetNewValueByPossibility(int value1, int value1Chance, int value2)
    {
        var fortune = (int)(Random.value * 100);
        return fortune <= value1Chance ? value1 : value2;
    }

    private Seed GetQuantumSeed(Seed parent1, Seed parent2)
    {
        var newSeed = ScriptableObject.CreateInstance<Seed>();

        newSeed.Name = parent1.Name + "-" + parent2.Name;
        newSeed.NameInRussian = MixTwoNames(parent1.NameInRussian, parent2.NameInRussian);
        newSeed.NameInLatin = "";

        newSeed.LevelData = CSVStatsMerger.GetQuantumStatistics(parent1.Name, parent2.Name);
        newSeed.Taste = newSeed.LevelData.Taste.Keys.ToArray()[0];
        newSeed.TasteGen = Gen.Mixed;

        newSeed.Gabitus = newSeed.LevelData.Gabitus.Keys.ToArray()[0];
        newSeed.GabitusGen = Gen.Mixed;

        newSeed.GrowTime = newSeed.LevelData.GrowTime.Keys.ToArray()[0];
        newSeed.GrowTimeGen = Gen.Mixed;

        newSeed.MutationPossibilityGen = Gen.Mixed;
        newSeed.MutationPossibility = newSeed.LevelData.MutationChance.Keys.ToArray()[0];

        newSeed.minAmount = newSeed.LevelData.MinAmount.Keys.ToArray()[0];
        newSeed.maxAmount = newSeed.LevelData.MaxAmount.Keys.ToArray()[0];
        newSeed.AmountGen = Gen.Mixed;

        return newSeed;
    }

    /// <summary>
    /// Gets first syllables from first word, last syllable from second word and returns mixed word
    /// </summary>
    private string MixTwoNames(string firstName, string secondName, bool english = false)
    {
        var firstSyllables = GetSyllables(firstName, english);
        var secondSyllables = GetSyllables(secondName, english);
        var result = string.Join("", firstSyllables.Take(firstSyllables.Count() - 1))
            + string.Join("", secondSyllables.Skip(secondSyllables.Count() - 1));

        return result;
    }

    /// <summary>
    /// Breaks a word into syllables
    /// </summary>
    /// <returns>IEnumerable of syllables</returns>
    private static IEnumerable<string> GetSyllables(string word, bool english = false)
    {
        var vowels = english
            ? "aeiou"
            : "аоуиэыяюеё";
        var vowelsArr = vowels.ToCharArray();
        var vowelsIndexes = new List<int>();
        var result = new HashSet<string>();
        word = word.ToLower();

        for (var i = 0; i < word.Length; i++)
            if (vowelsArr.Contains(word[i]))
                vowelsIndexes.Add(i);

        for (var i = vowelsIndexes.Count - 1; i > 0; i--)
        {
            if (vowelsIndexes[i] - vowelsIndexes[i - 1] == 1)
                continue;
            var consonantCount = vowelsIndexes[i] - vowelsIndexes[i - 1] - 1;
            var startIndex = vowelsIndexes[i - 1] + 1 + consonantCount / 2;
            result.Add(word.Substring(startIndex));
            word = word.Remove(startIndex);
        }
        result.Add(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word));

        return result.Reverse();
    }
}
