using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    private AudioSource finishSound;
    private bool levelCompleted = false;
    void Start()
    {
        finishSound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && !levelCompleted)
        {
            finishSound.Play();
            levelCompleted = true;
            Invoke(nameof(CompleteLevel), 2f);
        }
    }

    private void CompleteLevel()
    {
        SceneManager.LoadScene("FinishScene", LoadSceneMode.Single);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
