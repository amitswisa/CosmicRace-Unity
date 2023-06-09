using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BackBtnScript : MonoBehaviour
{
    public Button backBtnPrefab;

    void Start()
    {
        // Add a listener to the Button component's onClick event
        backBtnPrefab.onClick.AddListener(MyOnClickFunction);
    }

    public void MyOnClickFunction()
    {
        SceneManager.LoadScene("HomeScene");
    }
}
