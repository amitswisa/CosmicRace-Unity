using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameUpdating : MonoBehaviour
{
    [SerializeField] Text UpdatePanel;

    public void UpdatePanelText(string message)
    {
        if(message.Equals("CLEAR"))
        {
            UpdatePanel.text = "";
        }
        else
        {
            if(UpdatePanel.text != "")
                UpdatePanel.text += "\n";

            UpdatePanel.text += message;
        }
    }
}
