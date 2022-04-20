using Exhibition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitionCard : MonoBehaviour
{
    public Image portrait;
    public TextMeshProUGUI opponentName;
    public Button showSeedsBtn;
    public GameObject seedPanel;
    private Opponent _opponent;
    public void SetOpponent(Opponent opponent, Transform parent)
    {
        _opponent = opponent;
        portrait.sprite = opponent.Portrait();
        opponentName.text = opponent.Name;
        showSeedsBtn.onClick.AddListener(() =>
        {
            Instantiate(seedPanel, parent);
        }); 
    }
}