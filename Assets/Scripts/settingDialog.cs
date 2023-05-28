using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settingDialog : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    [SerializeField] private ToggleButton musicToggle;

    [SerializeField] private ToggleButton SoundToggle;

    [SerializeField] private Button okButton;
    
    private SoundManager soundManager;
    
    private void Start()
    {
        musicToggle.isOn = !soundManager.isMute("Background");
        SoundToggle.isOn = !soundManager.isMute("click");
    }

    private void Update()
    {
        musicToggle.isOn = !soundManager.isMute("Background");
        SoundToggle.isOn = !soundManager.isMute("click");
    }

    public void ShowDialog()
    {
        gameObject.SetActive(true);
        soundManager = GameObject.Find("SceneSounds").GetComponent<SoundManager>();
        musicToggle.GetComponent<Button>().onClick.AddListener(_toggleMusic);
        SoundToggle.GetComponent<Button>().onClick.AddListener(_toggleSound);
        volumeSlider.onValueChanged.AddListener(_changeVolume);
        okButton.onClick.AddListener(onButtonClicked);
    }
    
    private void onButtonClicked()
    {
        musicToggle.GetComponent<Button>().onClick.RemoveAllListeners();
        SoundToggle.GetComponent<Button>().onClick.RemoveAllListeners();
        volumeSlider.onValueChanged.RemoveAllListeners();
        gameObject.SetActive(false);
    }

    private void _toggleMusic()
    {
        soundManager.toggleMute("Background", musicToggle.isOn);
    }
    private void _toggleSound()
    {
        soundManager.toggleMute("click", SoundToggle.isOn);
    }

    private void _changeVolume(float value)
    {
        soundManager.changeVolume(value);
    }
}
