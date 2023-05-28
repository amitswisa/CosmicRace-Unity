using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SettingDialogManager : MonoBehaviour
{
    // OKDialog singleton instance
    public settingDialog DialogInstance { get; private set; }

    private static SettingDialogManager _instance;

    // Static instance of the manager
    public static SettingDialogManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Create new GameObject and add OKDialogManager
                GameObject singletonObject = new GameObject("settingDialogManager");
                _instance = singletonObject.AddComponent<SettingDialogManager>();
                DontDestroyOnLoad(singletonObject);
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ShowDialog()
    {
        // Create instance of OKDialog from prefab if it doesn't exist
        if (DialogInstance == null)
        {
            // Load the prefab from the Resources folder
            settingDialog dialogPrefab = Resources.Load<settingDialog>("Prefabs/Dialogs/settingDialog");

            if (dialogPrefab == null)
            {
                Debug.LogError("Couldn't find settingDialog prefab in Resources folder.");
                return;
            }

            DialogInstance = Instantiate(dialogPrefab);
            DontDestroyOnLoad(DialogInstance.gameObject);
        }

        // Show the dialog
        DialogInstance.ShowDialog();
    }
}