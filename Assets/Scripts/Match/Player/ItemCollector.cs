using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    private int coinsCounter = 0;

    [SerializeField] private Text coinsText;
    [SerializeField] private AudioSource coinCollectSoundEffect;
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.CompareTag("Coin"))
        {
            coinCollectSoundEffect.Play();
            Destroy(collider2D.gameObject);
            coinsCounter++;
            coinsText.text = "Coins: " + coinsCounter;

            // Update server.
            GameController.Instance.UpdateServerAfterCoinCollection();

            Debug.Log("Coins Counter: " + coinsCounter);
        }
    }
}
