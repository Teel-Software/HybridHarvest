using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DontDestroySFX : MonoBehaviour
{
    public static GameObject instance;
    //public string objectTag;
    void Awake() 
    {
        // if (objectTag != null) 
        // {
        //     GameObject[] obj = GameObject.FindGameObjectsWithTag(objectTag);
        //     if (obj.Length > 1)
        //         Destroy(gameObject);
        //     DontDestroyOnLoad(gameObject);
        // }
        if (instance == null)
            instance = gameObject;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
