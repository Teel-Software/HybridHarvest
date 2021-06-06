using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        GameObject[] obj = GameObject.FindGameObjectsWithTag("GameMusic");
        if (obj.Length > 1)
            Destroy(this.gameObject);
        DontDestroyOnLoad(this?.gameObject);
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    GameObject obj = GameObject.Find("player");
    //    if ((scene.buildIndex == 0)&& (!obj.Equals(this.gameObject)))
    //        GameObject.Destroy(obj);
    //}
}
