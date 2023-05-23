using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSound : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundSound;
    
    // Start is called before the first frame update
    void Start()
    {
        backgroundSound.loop = true;
        backgroundSound.volume = 0.12f;
        backgroundSound.Play();
    }
}
