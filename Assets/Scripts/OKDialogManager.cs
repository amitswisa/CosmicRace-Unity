using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class OKDialogManager : MonoBehaviour
{
    // OKDialog singleton instance
    public OKDialog DialogInstance { get; private set; }

    private static OKDialogManager _instance;

    // Static instance of the manager
    public static OKDialogManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Create new GameObject and add OKDialogManager
                GameObject singletonObject = new GameObject("OKDialogManager");
                _instance = singletonObject.AddComponent<OKDialogManager>();
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

    public void ShowDialog(string title, string description)
    {
        // Create instance of OKDialog from prefab if it doesn't exist
        if (DialogInstance == null)
        {
            // Load the prefab from the Resources folder
            OKDialog dialogPrefab = Resources.Load<OKDialog>("Prefabs/Dialogs/OKDialog");

            if (dialogPrefab == null)
            {
                Debug.LogError("Couldn't find OKDialog prefab in Resources folder.");
                return;
            }

            DialogInstance = Instantiate(dialogPrefab);
            DontDestroyOnLoad(DialogInstance.gameObject);
        }

        // Show the dialog
        DialogInstance.ShowDialog(title, description);
    }
}