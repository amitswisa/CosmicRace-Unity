using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private Sprite isOnSprite;
    [SerializeField] private Sprite isOffSprite;
    public bool isOn = false;
    
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(_onClick);
        GetComponent<Image>().sprite = isOn ? isOnSprite : isOffSprite;
    }

    private void Update()
    {
        GetComponent<Image>().sprite = isOn ? isOnSprite : isOffSprite;
    }

    void _onClick()
    {
        toggle();
    }

    public void toggle()
    {
        isOn = !isOn;
        GetComponent<Image>().sprite = isOn ? isOnSprite : isOffSprite;
    }
}
