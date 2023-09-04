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
        GameController.Instance.AddUpdateViewListener(UpdatePanel);
    }
}
