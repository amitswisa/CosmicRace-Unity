using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSummaries : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(SendAPIRequest());
    }
    
    private IEnumerator SendAPIRequest()
    {
        string url = Utils.GAME_SERVER_IP+"https://api.example.com/data";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;
                Debug.Log("Response: " + responseText);
            }
        }
    }
}
