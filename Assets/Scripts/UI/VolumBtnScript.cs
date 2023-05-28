using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumBtnScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Button volumeButton;
    [SerializeField] private Sprite soundOnImage;
    [SerializeField] private Sprite soundOffImage;
    private SoundManager soundManager;
    private void Start()
    {
        soundManager = GameObject.Find("SceneSounds").GetComponent<SoundManager>();
        volumeButton = GetComponent<Button>();

        // Add the onClick event listener to the volume button
        volumeButton.onClick.AddListener(OnVolumeButtonClick);
        
        if (soundManager.isMute("Background"))
        {
            GetComponent<Image>().sprite = soundOffImage;
        }
        else
        {
            GetComponent<Image>().sprite = soundOnImage;
        }
    }
    
    public void OnVolumeButtonClick()
    {
        // Implement your volume control logic here
        // For example, you can toggle the volume on/off or adjust the volume level

        // You can access the AudioSource component or any other volume-related components in your scene
        // Example:

        // Toggle the volume on/off
        soundManager.toggleMute("Background", !soundManager.isMute("Background"));
        if (soundManager.isMute("Background"))
        {
            GetComponent<Image>().sprite = soundOffImage;
        }
        else
        {
            GetComponent<Image>().sprite = soundOnImage;
        }
        

        // Alternatively, adjust the volume level
        // audioSource.volume = 0.5f; // Set volume level to 50%
    }
    // Update is called once per frame
    void Update()
    {
        if (soundManager.isMute("Background"))
        {
            GetComponent<Image>().sprite = soundOffImage;
        }
        else
        {
            GetComponent<Image>().sprite = soundOnImage;
        }
    }
}
