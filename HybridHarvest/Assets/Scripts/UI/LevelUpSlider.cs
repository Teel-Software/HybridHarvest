using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelUpSlider : MonoBehaviour
    {

        public Slider LvlSlider;
        private Inventory _inventory;
        
        // Start is called before the first frame update
        void Start()
        {
            LvlSlider = gameObject.GetComponent<Slider>();
            _inventory = GameObject.Find("DataKeeper").GetComponent<Inventory>();
        }

        // Update is called once per frame
        void Update()
        {
            LvlSlider.value = (float)_inventory.Reputation / _inventory.ReputationLimit * 100;
        }
    }
}
