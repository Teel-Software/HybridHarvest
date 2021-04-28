using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundCreation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var height = Camera.main.orthographicSize * 2f;
        var width = height * Screen.width / Screen.height;//так и только так
        //var height = Screen.height;
        //var width = Screen.width;
        transform.localScale = new Vector3(width, height, 0);
    }
}
