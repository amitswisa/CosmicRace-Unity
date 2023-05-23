using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cancelPopUpWindow : MonoBehaviour
{
    [SerializeField] private GameObject popUpWindow;
    
    public void OnCancelPopUpWindowClick()
    {
        popUpWindow.gameObject.SetActive(false);
    }
}
