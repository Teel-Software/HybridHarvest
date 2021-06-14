using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    public string ObjectTag;
    void Awake()
    {
        if (ObjectTag != null)
        {
            GameObject[] obj = GameObject.FindGameObjectsWithTag(ObjectTag);
            if (obj.Length > 1)
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }
}
