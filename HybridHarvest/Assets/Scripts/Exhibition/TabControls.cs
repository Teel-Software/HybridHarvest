using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Exhibition
{
    public class TabControls : MonoBehaviour
    {
        [SerializeField] 
        private Button[] tabButtons;
        [SerializeField] 
        private Button mainTabButton;
        [SerializeField] 
        private GameObject[] tabPanels;

        [SerializeField] 
        private int defaultActive = 1;
        public void Start()
        {
            foreach (var panel in tabPanels)
            {
                panel.SetActive(false);
            }

            void disableClickedButton(int index)
            {
                for (var i = 0; i < tabButtons.Length; i++)
                    tabButtons[i].interactable = i != index;
            }

            mainTabButton.onClick.AddListener(() =>
            {
                mainTabButton.interactable = false;
                disableClickedButton(-1);
                CloseAllTabs();
            });
        
            for (var i = 0; i < tabButtons.Length; i++)
            {
                var index = i;
                tabButtons[index].onClick.AddListener(() =>
                {
                    mainTabButton.interactable = true;
                    disableClickedButton(index);
                    CloseAllTabs();
                    tabPanels[index].SetActive(true);
                });
            }

            mainTabButton.interactable = true;
            tabPanels[defaultActive].SetActive(true);
            tabButtons[defaultActive].interactable = false;
        }
    
                    
        private void CloseAllTabs()
        {
            foreach (var panel in tabPanels)
                panel.SetActive(false);
        }
        void Update()
        {
            //tabButtons[currentTab].interactable = false;
        }
    }
}
