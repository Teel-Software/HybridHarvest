using UnityEngine;

namespace Exhibition
{
    public class OpponentPanel : MonoBehaviour
    {
        [SerializeField] private GameObject[] cards;

        private Opponent[] opponents;
        
        public void Awake()
        {
            foreach (var card in cards)
                card.SetActive(false);

            opponents = GetComponentInParent<Exhibition>().Opponents;
            for (var i = 0; i < opponents.Length; i++)
            {                
                cards[i].SetActive(true);
                var exhibitionCard = cards[i].GetComponent<OpponentCard>();
                exhibitionCard.SetOpponent(opponents[i], gameObject.transform.parent);
            }
        }
    
        public void OnEnable()
        {
            gameObject.SetActive(GetComponentInParent<Exhibition>().State == ExhibitionState.Inactive);
        }
    }
}