using UnityEngine;
using UnityEngine.UI;

namespace Exhibition
{
    public class TabControls : MonoBehaviour
    {
        [SerializeField] private Button[] tabButtons;
        [SerializeField] private Button mainTabButton;
        [SerializeField] private GameObject[] tabPanels;
        [SerializeField] private int defaultActive = 1;
        [SerializeField] private bool reloadTabsOnDisable;

        public void Start()
        {
            foreach (var panel in tabPanels)
            {
                panel.SetActive(false);
            }

            void disableClickedEnableOthers(int index)
            {
                for (var i = 0; i < tabButtons.Length; i++)
                    tabButtons[i].interactable = i != index;
            }

            mainTabButton.onClick.AddListener(() =>
            {
                mainTabButton.interactable = false;
                disableClickedEnableOthers(-1);
                CloseAllTabs();
            });

            for (var i = 0; i < tabButtons.Length; i++)
            {
                var index = i;
                tabButtons[index].onClick.AddListener(() =>
                {
                    mainTabButton.interactable = true;
                    disableClickedEnableOthers(index);
                    CloseAllTabs();
                    tabPanels[index].SetActive(true);
                });
            }

            if (defaultActive >= 0)
            {
                mainTabButton.interactable = true;
                tabPanels[defaultActive].SetActive(true);
                tabButtons[defaultActive].interactable = false;
            }
        }


        private void CloseAllTabs()
        {
            foreach (var panel in tabPanels)
                panel.SetActive(false);
        }

        private void OnDisable()
        {
            if (!reloadTabsOnDisable) return;

            if (defaultActive >= 0 && defaultActive < tabButtons.Length)
                tabButtons[defaultActive].onClick.Invoke();
            else mainTabButton.onClick.Invoke();
        }

        void Update()
        {
            //tabButtons[currentTab].interactable = false;
        }
    }
}
