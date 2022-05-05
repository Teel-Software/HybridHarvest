using System.Linq;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Exhibition
{
    public class OpponentSeedInfo : MonoBehaviour
    {
        public GameObject seedListing;
        public VerticalLayoutGroup scrollContent;
        public Seed[] Seeds;

        public void Awake()
        {
            const int testCount = 3;
            Seeds = new Seed[testCount];
            foreach (var i in Enumerable.Range(0, testCount))
            {
                Seeds[i] = Seed.Create("Pea|_|900|1|1|1|2|1|9|11|Горох|Písum|0");
            }
            foreach (var seed in Seeds)
            {
                var listing = Instantiate(seedListing, scrollContent.transform);
                // change tp text mesh pro
                var text = listing.GetComponentInChildren<TextMeshProUGUI>();
                text.text = SeedStatFormatter.FormatSmall(seed);
                // idk what to call this
                var listingClass = listing.GetComponentInChildren<OpponentSeedListing>();
                var img = listingClass.plantImage;
                img.sprite = seed.PlantSprite;
            }
        }

        public void OnDisable()
        {
            Destroy(gameObject);
        }
    }
}