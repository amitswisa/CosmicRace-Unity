using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSettingsWindow : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(_onClick);
    }

    void _onClick()
    {
        SettingDialogManager.Instance.ShowDialog();
    }
}
