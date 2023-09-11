using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameUpdating : MonoBehaviour
{
    [SerializeField] Text UpdatePanel;
    [SerializeField] Image selfImageProperty;

    void Start()
    {
        gameObject.SetActive(false);
        GameController.Instance.AddUpdateViewListener(UpdatePanel, gameObject);
    }
}