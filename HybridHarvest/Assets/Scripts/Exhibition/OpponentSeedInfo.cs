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
            Seeds = new Seed[1];
            Seeds[0] = ScriptableObject.CreateInstance<Seed>();
            Seeds[0].SetValues("Pea|_|900|1|1|1|2|1|9|11|Горох|Písum|0");
            foreach (var seed in Seeds)
            {
                var listing = Instantiate(seedListing, scrollContent.transform);
                // change tp text mesh pro
                var text = listing.GetComponentInChildren<Text>();
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