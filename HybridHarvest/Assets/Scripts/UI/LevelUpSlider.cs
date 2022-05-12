using UnityEngine;
using UnityEngine.UI;

public class LevelUpSlider : MonoBehaviour
{
    public Slider LvlSlider;
    private Inventory _inventory;

    public void SetLevel(int level = 2)
    {
        _inventory.SetLevel(level);
    }

    public void LevelUp()
    {
        SetLevel(_inventory.Level + 1);
    }

    // Start is called before the first frame update
    private void Start()
    {
        LvlSlider = gameObject.GetComponent<Slider>();
        _inventory = GameObject.Find("DataKeeper").GetComponent<Inventory>();
    }

    // Update is called once per frame
    private void Update()
    {
        LvlSlider.value = (float) _inventory.Reputation / _inventory.ReputationLimit * 100;
    }
}
