using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Exhibition
{
    public class ExhibitionCard : MonoBehaviour
    {
        public Image portrait;
        public TextMeshProUGUI opponentName;
        public Button showSeedsBtn;
        public GameObject seedPanel;
        private Opponent opp;
        public void SetOpponent(Opponent opponent, Transform parent)
        {
            opp = opponent;
            portrait.sprite = opponent.Portrait();
            opponentName.text = opponent.Name;
            showSeedsBtn.onClick.AddListener(() =>
            {
                var panel = Instantiate(seedPanel, parent);
                panel.GetComponent<OpponentSeedPanel>().SetSeeds(opp.Seeds);
            });
        }
    }
}