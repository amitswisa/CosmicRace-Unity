using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource backgroundSound;
    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private AudioSource typeSound;
    [SerializeField] private Image soundSettingsBtn;

    void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MuteGame(float volume)
    {
        if(backgroundSound.volume > 0)
        {
            backgroundSound.volume = 0f;
            buttonSound.volume = 0f;
            typeSound.volume = 0f;
        } else {
            backgroundSound.volume = 1f;
            buttonSound.volume = 1f;
            typeSound.volume = 1f;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
