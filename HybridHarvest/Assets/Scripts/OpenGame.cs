using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenGame : MonoBehaviour
{
    [SerializeField] GameObject RewatchButton;

    public void ChangeScene(int sceneNum)
    {
        if (!PlayerPrefs.HasKey("GameInitialised") && RewatchButton != null)
            InitializeBeginning();
        else
        {
            if (sceneNum == 0)
            {
                GameObject obj = GameObject.Find("player");
                GameObject.Destroy(obj);
            }
            SceneManager.LoadScene(sceneNum);
        }
    }

    private void InitializeBeginning()
    {
        RewatchButton.GetComponent<Button>().onClick.Invoke();
    }
}
