using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("GameMusic");
        if (obj.Length > 1)
            Destroy(this.gameObject);
        DontDestroyOnLoad(this?.gameObject);
    }
}
