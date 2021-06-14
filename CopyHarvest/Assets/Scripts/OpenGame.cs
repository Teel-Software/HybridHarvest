using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenGame : MonoBehaviour
{
    public void ChangeScene(int sceneNum)
    {
        //print("it`s fine");
        if (sceneNum == 0)
        {
            GameObject obj = GameObject.Find("player");
            GameObject.Destroy(obj);
        }
        SceneManager.LoadScene(sceneNum);
    }
}
