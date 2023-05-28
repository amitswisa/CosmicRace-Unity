using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Modify OKDialog to work with the manager
public class OKDialog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Description;
    [SerializeField] private Button m_OKButton;

    private void Start()
    {
        m_OKButton.onClick.AddListener(this.onButtonClicked);
    }

    public void ShowDialog(string title, string description)
    {
        m_Title.text = title;
        m_Description.text = description;
        gameObject.SetActive(true);
    }

    private void onButtonClicked()
    {
        Destroy(gameObject);
    }
}