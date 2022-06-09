using UnityEngine;
using UnityEngine.UI;

namespace Exhibition
{
    public class DayIcon : MonoBehaviour
    {
        public Image Background;
        [SerializeField] private Image MedalIcon;

        [SerializeField] private Text DayText;
        [SerializeField] private Text PlacementText;

        public void SetPlace(int place)
        {
            MedalIcon.gameObject.SetActive(place > 0);
            if (place > 0)
            {
                PlacementText.text = place.ToString();
                MedalIcon.sprite = Resources.Load<Sprite>($"Medals/Medal{place}");
            }
            else
            {
                PlacementText.text = "";
            }
        }
    }
}
