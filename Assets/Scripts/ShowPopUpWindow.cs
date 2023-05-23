using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPopUpWindow : MonoBehaviour
{
    [SerializeField] private GameObject popUpWindow;
    public void OnShowCharacterStatisticWindowClick()
    {
        popUpWindow.gameObject.SetActive(true);
    }
}
