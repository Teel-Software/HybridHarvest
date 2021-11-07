using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenGame : MonoBehaviour
{
    [SerializeField] GameObject RewatchButton;

    /// <summary>
    /// Меняет сцену
    /// </summary>
    /// <param номер следующей сцены ="sceneNum"></param>
    public void ChangeScene(int sceneNum)
    {
        if (!PlayerPrefs.HasKey("GameInitialised") && RewatchButton != null)
            InitializeBeginning();
        else
        {
            if (sceneNum == 0)
            {
                GameObject obj = GameObject.Find("player");
                Destroy(obj);
            }
            SceneManager.LoadScene(sceneNum);
        }
    }

    /// <summary>
    /// Запускает вступительные слайды
    /// </summary>
    private void InitializeBeginning()
    {
        RewatchButton.GetComponent<Button>().onClick.Invoke();
    }
}
