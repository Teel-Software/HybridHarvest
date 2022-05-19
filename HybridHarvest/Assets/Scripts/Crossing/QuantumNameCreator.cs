using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Threading;
using CI.QuickSave;

public class QuantumNameCreator : MonoBehaviour
{
    public string Name1, Name2;
    [SerializeField] Text TextPlace;
    [SerializeField] InputField UserInput;
    [SerializeField] CreateMiniGame MiniGame;

    private string result;

    public void UserEnterName()
    {
        result = UserInput.text;
        result = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(result);
        //TextPlace.text = result;
        UserInput.text = result;
    }

    public void DefaultFill()
    {
        //UserInput.text = "твой вариант";
        TextPlace.text = "Как назовём?";
        //UserInput.text = "твой вариант";
    }

    public void GenerateName()
    {   //помидор картошка огурец горох морковь дебаг 

        //Name1 = "помирец";
        //Name2 = "картошка";
        if(Name1 == "" || Name2 == "")
        {
            var reader = QSReader.Create("QuantumName");
            if (reader.Exists("name1"))
                Name1 = reader.Read<string>("name1");
            if (reader.Exists("name2"))
                Name2 = reader.Read<string>("name2");
        }

        var firstParts = GetSyllables(Name1).ToArray();
        var secondParts = GetSyllables(Name2).ToArray();
        var num = (firstParts.Length + secondParts.Length) / 2;
        var fromFirst = num / 2 + 1;
        var fromSecond = num - fromFirst;
        do
        {
            PlayerPrefs.SetString("QuantPrevName", result);
            result = string.Join("", RandomHalves(firstParts, secondParts, fromFirst, fromSecond));
            result = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(result);
        } while (result == PlayerPrefs.GetString("QuantPrevName"));
        PlayerPrefs.SetString("QuantPrevName", result);
        //TextPlace.text = result;
        UserInput.text = result;

        //result.Add(string.Join("", RandomHalves(firstParts, secondParts, fromFirst, fromSecond)));
        //result.Add(string.Join("",RandomWord(firstParts, secondParts, num)));
    }

    public void FinishChoosing()
    {
        MiniGame.UpdateRussianName(result);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) return;
        var writer = QuickSaveWriter.Create("QuantumName");
        writer.Write("name1", Name1)
            .Write("name2", Name2);
        writer.Commit();
    }

    private List<string> RandomHalves(string[] firstParts, string[] secondParts, int fromFirst, int fromSecond)
    {
        var result = new List<string>();
        int index;
        for (var i = 0; i < fromFirst && i < firstParts.Length;)
        {
            index = Random.Range(0, firstParts.Length);
            if (result.Any(x => x == firstParts[index])) continue;

            i++;
            result.Add(firstParts[index]);
        }
        for (var i = 0; i < fromSecond || i < 1;)
        {
            index = Random.Range(0, secondParts.Length);
            if (result.Any(x => x == secondParts[index])) continue;

            i++;
            result.Add(secondParts[index]);
        }

        return result;
    }

    private static List<string> RandomWord(string[] firstParts, string[] secondParts, int num)
    {
        var result = new List<string>();
        int index;
        for (var i = 0; i < num;)
        {
            if ((int)Random.Range(0, 2) == 1)
            {
                index = Random.Range(0, firstParts.Length);
                if (result.Any(x => x == firstParts[index])) continue;
                result.Add(firstParts[index]);
                i++;
            }
            else
            {
                index = Random.Range(0, secondParts.Length);
                if (result.Any(x => x == secondParts[index])) continue;
                result.Add(secondParts[index]);
                i++;
            }
        }

        return result;
    }

    static IEnumerable<string> GetSyllables(string word, bool english = false)
    {
        var vowelsArr = english
            ? new[] { 'a', 'e', 'i', 'o', 'u' }
            : new[] { 'а', 'о', 'у', 'и', 'э', 'ы', 'я', 'ю', 'е', 'ё' };
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
        result.Add(word);

        return result.Reverse();
    }
}
