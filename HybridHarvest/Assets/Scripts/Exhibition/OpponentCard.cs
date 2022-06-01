using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Exhibition
{
    public class OpponentCard : MonoBehaviour
    {
        public Image Portrait;
        public TextMeshProUGUI OpponentName;
        public Button ShowSeedsBtn;
        public GameObject SeedPanel;
        
        private Opponent opp;
        public void SetOpponent(Opponent opponent, Transform parent)
        {
            opp = opponent;
            Portrait.sprite = opponent.Portrait;
            OpponentName.text = opponent.Name;
            ShowSeedsBtn.onClick.AddListener(() =>
            {
                var panel = Instantiate(SeedPanel, parent);
                panel.GetComponent<OpponentSeedPanel>().SetSeeds(opp.Seeds);
            });
        }
    }
}