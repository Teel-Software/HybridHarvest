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

    public int[] Chances = new int[3];
    public int[] OppositeSeedStats = new int[3];

    int chancesIterator;
    private Image textBGImage;

    public void Clicked()
    {
        var seed1 = button1.GetComponent<LabButton>().NowSelected;
        var seed2 = button2.GetComponent<LabButton>().NowSelected;
        if (seed1 == null || seed2 == null)
            return;
        var newSeed = MixTwoParents(seed1, seed2);

        if (SceneManager.GetActiveScene().buildIndex == 4)
            CurrentPot.GetComponent<QuantumGrowth>().ApplyLightning(newSeed);
        else
            CurrentPot.GetComponent<LabGrowth>().PlantIt(newSeed);

        ExitHybridMenu();
    }

    public void ExitHybridMenu()
    {
        button2.GetComponent<LabButton>().ClearButton();
        button1.GetComponent<LabButton>().ClearButton();
        button1.GetComponent<LabButton>().PlaceForResult.GetComponent<LabButton>().ClearButton();
        button1.GetComponent<LabButton>().PlaceForResult.gameObject.SetActive(false);
        button1.transform.parent.gameObject.SetActive(false); // deactivates hybrid panel
        button1.GetComponent<LabButton>().InventoryFrame.Redraw();
    }

    public Seed MixTwoParents(Seed first, Seed second)
    {
        chancesIterator = 0;
        var newSeed = ScriptableObject.CreateInstance<Seed>();
        newSeed.SetValues(first.ToString());
        (newSeed.Taste, newSeed.TasteGen) =
            CountParameter(first.Taste, first.TasteGen, second.Taste, second.TasteGen);
        OppositeSeedStats[0] = newSeed.Taste == first.Taste ? second.Taste : first.Taste;
        chancesIterator++;

        (newSeed.Gabitus, newSeed.GabitusGen) =
            CountParameter(first.Gabitus, first.GabitusGen, second.Gabitus, second.GabitusGen);
        OppositeSeedStats[1] = newSeed.Gabitus == first.Gabitus ? second.Gabitus : first.Gabitus;
        chancesIterator++;

        (newSeed.GrowTime, newSeed.GrowTimeGen) =
            CountParameter(first.GrowTime, first.GrowTimeGen, second.GrowTime, second.GrowTimeGen);
        OppositeSeedStats[2] = newSeed.GrowTime == first.GrowTime ? second.GrowTime : first.GrowTime;

        newSeed.NameInRussian = MixTwoNames(first.NameInRussian, second.NameInRussian);

        if (CurrentPot != null)
        {
            PlayerPrefs.SetString("SelectionChances" + CurrentPot.name, string.Join(" ", Chances));
            PlayerPrefs.SetString("OppositeSeedStats" + CurrentPot.name, string.Join(" ", OppositeSeedStats));
        }

        return newSeed;
    }

    public (int, Gen) CountParameter(int value1, Gen gen1, int value2, Gen gen2)
    {
        var dominant = Mathf.Min(value1, value2);
        var recessive = Mathf.Max(value1, value2);
        if (SceneManager.GetActiveScene().buildIndex == 4)
            (dominant, recessive) = (recessive, dominant);

        Chances[chancesIterator] = 100;
        if (gen1 == Gen.Dominant && gen2 == Gen.Dominant)
            return (dominant, gen1);
        if (gen1 == Gen.Recessive && gen2 == Gen.Recessive)
            return (recessive, gen1);
        if (gen1 == Gen.Dominant && gen2 == Gen.Recessive || gen1 == Gen.Recessive && gen2 == Gen.Dominant)
            return (dominant, Gen.Mixed);
        if (gen1 == Gen.Dominant && gen2 == Gen.Mixed || gen2 == Gen.Dominant && gen1 == Gen.Mixed)
            return (dominant, (Gen)GetNewValueByPossibility((int)gen1, 50, (int)gen2));

        if (value1 != value2)
            Chances[chancesIterator] = 50;
        var newGen = (Gen)GetNewValueByPossibility((int)gen1, 50, (int)gen2);
        if (gen1 == Gen.Recessive && gen2 == Gen.Mixed || gen2 == Gen.Recessive && gen1 == Gen.Mixed)
            return (newGen == gen1 ? value1 : value2, newGen);

        if (value1 != value2)
            Chances[chancesIterator] = 75;
        var possibility = (int)Random.value * 100;
        newGen = possibility <= 25
            ? Gen.Dominant
            : possibility < 75
                ? Gen.Mixed
                : Gen.Recessive;

        return (dominant, newGen);
    }

    public int GetNewValueByPossibility(int value1, int value1Chance, int value2)
    {
        var fortune = (int)(Random.value * 100);
        return fortune <= value1Chance ? value1 : value2;
    }

    /// <summary>
    /// Gets first syllables from first word, last syllable from second word and returns mixed word
    /// </summary>
    private string MixTwoNames(string firstName, string secondName)
    {
        var firstSyllables = GetSyllables(firstName);
        var secondSyllables = GetSyllables(secondName);
        var result = string.Join("", firstSyllables.Take(firstSyllables.Count() - 1))
            + string.Join("", secondSyllables.Skip(secondSyllables.Count() - 1));

        return result;
    }

    /// <summary>
    /// Breaks a word into syllables
    /// </summary>
    /// <returns>IEnumerable of syllables</returns>
    private static IEnumerable<string> GetSyllables(string word)
    {
        var vowels = "аоуиэыяюеё".ToCharArray();
        var vowelsIndexes = new List<int>();
        var result = new HashSet<string>();
        word = word.ToLower();

        for (var i = 0; i < word.Length; i++)
            if (vowels.Contains(word[i]))
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
