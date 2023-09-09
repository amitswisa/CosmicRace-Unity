using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationText;
    public float displayDuration = 3.0f;

    private Queue<string> messageQueue = new Queue<string>();
    private bool isDisplayingMessage = false;

    void Start()
    {
        notificationText.transform.parent.gameObject.SetActive(false);
    }

    public void EnqueueMessage(string message)
    {
        messageQueue.Enqueue(message);
        if (!isDisplayingMessage)
        {
            StartCoroutine(DisplayMessage());
        }
    }

    private IEnumerator DisplayMessage()
    {
        isDisplayingMessage = true;

        while (messageQueue.Count > 0)
        {
            notificationText.text = messageQueue.Dequeue();
            notificationText.transform.parent.gameObject.SetActive(true);  // Activating the parent (the Image)
            yield return new WaitForSeconds(displayDuration);
            notificationText.transform.parent.gameObject.SetActive(false); // Deactivating the parent
        }

        isDisplayingMessage = false;
    }
}