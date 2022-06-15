using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System.Linq;
using System.Globalization;
using CI.QuickSave;

public class GeneCrossing : MonoBehaviour
{
    [SerializeField] public Button CurrentPot;
    [SerializeField] Button button1;
    [SerializeField] Button button2;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] QuantumNameCreator nameCreator;

    private List<int> chances;
    private List<int> oppositeSeedStats;

    public int[] Chances
    {
        get => chances.ToArray();
    }

    public int[] OppositeSeedStats
    {
        get => oppositeSeedStats.ToArray();
    }

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

        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            //nameCreator.Name1 = seed1.NameInRussian;
            //nameCreator.Name2 = seed2.NameInRussian;
            var writer = QuickSaveWriter.Create("QuantumName");
            writer.Write("name1", seed1.NameInRussian)
                .Write("name2", seed2.NameInRussian);
            writer.Commit();
            var newSeed = GetQuantumSeed(seed1, seed2);
            CurrentPot.GetComponent<QuantumGrowth>().ApplyLightning(newSeed);
        }
        else
        {
            var newSeed = MixTwoParents(seed1, seed2);
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
        var newSeed = Seed.Create(first.ToString());

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
        (temp, newSeed.MutationChanceGen) =
            CountParameter((int) first.MutationChance, first.MutationChanceGen,
                (int) second.MutationChance, second.MutationChanceGen);
        oppositeSeedStats.Add((int) newSeed.MutationChance == (int) first.MutationChance
            ? (int) second.MutationChance
            : (int) first.MutationChance);
        chancesIterator++;
        newSeed.MutationChance = (MutationChance) temp;

        (newSeed.MinAmount, newSeed.AmountGen) =
            CountParameter(first.MinAmount, first.AmountGen, second.MinAmount, second.AmountGen);
        oppositeSeedStats.Add(newSeed.MinAmount == first.MinAmount ? second.MinAmount : first.MinAmount);
        newSeed.MaxAmount = newSeed.MinAmount == first.MinAmount ? first.MaxAmount : second.MaxAmount;

        //newSeed.Name = MixTwoNames(first.Name, second.Name, english: true);
        newSeed.NameInRussian = MixTwoNames(first.NameInRussian, second.NameInRussian);

        if (CurrentPot != null)
        {
            var writer = QuickSaveWriter.Create("MiniGameStats");
            writer.Write("SelectionChances" + CurrentPot.name, chances)
                .Write("OppositeSeedStats" + CurrentPot.name, oppositeSeedStats);
            writer.Commit();
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
            return (dominant, (Gen) GetNewValueByPossibility((int) gen1, 50, (int) gen2));

        if (value1 != value2)
            chances[chances.Count - 1] = 50;
        var newGen = (Gen) GetNewValueByPossibility((int) gen1, 50, (int) gen2);
        if (gen1 == Gen.Recessive && gen2 == Gen.Mixed || gen2 == Gen.Recessive && gen1 == Gen.Mixed)
            return (newGen == gen1 ? value1 : value2, newGen);

        if (value1 != value2)
            chances[chances.Count - 1] = 75;
        var possibility = (int) Random.value * 100;
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
        var fortune = (int) (Random.value * 100);
        return fortune <= value1Chance ? value1 : value2;
    }

    public Seed GetQuantumSeed(Seed parent1, Seed parent2)
    {
        var newSeed = ScriptableObject.CreateInstance<Seed>();

        newSeed.Parents = parent1.Parents.Concat(parent2.Parents).Distinct().OrderBy(x => x).ToList();

        // место для создания изображения
        ImageMerger.MergeParentImages(newSeed.Parents.OrderBy(x => x).ToList());

        //newSeed.Name = parent1.Name + "-" + parent2.Name;
        //newSeed.NameInRussian = MixTwoNames(parent1.NameInRussian, parent2.NameInRussian);
        newSeed.NameInRussian = "???";
        newSeed.NameInLatin = "";

        newSeed.SeedStats = CSVStatsMerger.GetQuantumStatistics(parent1.Parents.OrderBy(x=>x).ToList(), parent2.Parents.OrderBy(x => x).ToList());
        newSeed.Taste = newSeed.SeedStats.Taste.Keys.ToArray()[0];
        newSeed.TasteGen = Gen.Mixed;

        newSeed.Gabitus = newSeed.SeedStats.Gabitus.Keys.ToArray()[0];
        newSeed.GabitusGen = Gen.Mixed;

        newSeed.GrowTime = newSeed.SeedStats.GrowTime.Keys.ToArray()[0];
        newSeed.GrowTimeGen = Gen.Mixed;

        newSeed.MutationChanceGen = Gen.Mixed;
        newSeed.MutationChance = newSeed.SeedStats.MutationChance.Keys.ToArray()[0];

        newSeed.MinAmount = newSeed.SeedStats.MinAmount.Keys.ToArray()[0];
        newSeed.MaxAmount = newSeed.SeedStats.MaxAmount.Keys.ToArray()[0];
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
