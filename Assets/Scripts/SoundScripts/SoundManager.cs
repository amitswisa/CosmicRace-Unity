using System;
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

    private bool _isMute = false;
    

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
    
    private void Start()
    {
        _isMute = false;
    }
    

    public void changeVolume(float volume)
    {
        backgroundSound.volume = volume;
        buttonSound.volume = volume;
        typeSound.volume = volume;
    }

    public void Mute()
    {
        backgroundSound.mute = true;
        buttonSound.mute = true;
        typeSound.mute = true;
    }

    public void unMute()
    {
        backgroundSound.mute = false;
        buttonSound.mute = false;
        typeSound.mute = false;
    }

    public void toggleMute()
    {
        _isMute = !_isMute;
        if (_isMute)
        {
            Mute();
        }
        else
        {
            unMute();
        }
    }

    public bool isMuted()
    {
        return _isMute;
    }
    
    public void toggleMute(String name, bool isMute)
    {
        switch (name)
        {
            case "Background":
                backgroundSound.mute = isMute;
                break;
            case "click":
                buttonSound.mute = typeSound.mute = isMute;
                break;
        }

        _isMute = backgroundSound.mute && buttonSound.mute && typeSound.mute;
    }

    public bool isMute(String name)
    {
        switch (name)
        {
            case "Background":
                return backgroundSound.mute;
            case "click":
                return buttonSound.mute && typeSound.mute;
        }

        return false;
    }


}
