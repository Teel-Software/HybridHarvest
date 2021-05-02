using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        //GameObject obj = GameObject.Find("player");
        //if (!obj.Equals(this.gameObject))
        //    GameObject.Destroy(obj); (scene.buildIndex == 0)
        DontDestroyOnLoad(this.gameObject);
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    GameObject obj = GameObject.Find("player");
    //    if ((scene.buildIndex == 0)&& (!obj.Equals(this.gameObject)))
    //        GameObject.Destroy(obj);
    //}
}
