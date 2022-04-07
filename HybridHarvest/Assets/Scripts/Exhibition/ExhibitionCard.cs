using Exhibition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitionCard : MonoBehaviour
{
    public Image portrait;
    public TextMeshProUGUI name;
    public Button showSeedsBtn;
    public void SetOpponent(Opponent opponent)
    {
        portrait.sprite = opponent.Portrait();
        name.text = opponent.Name;
    }
}