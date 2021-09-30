using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioClip ClickSound;
    
    public void Click() 
    {
        var source = GameObject.FindGameObjectWithTag("GameSFX").GetComponent<AudioSource>();
        source.PlayOneShot(ClickSound);
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        
    }
}
