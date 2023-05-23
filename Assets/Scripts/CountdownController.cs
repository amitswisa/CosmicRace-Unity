using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    public int countDownTime;

    public TextMeshProUGUI countdownDisplay;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(CountdownToStart());
    }
    
    IEnumerator CountdownToStart()
    {
        GameController.Instance.m_IsGameRunning = false;
        while (countDownTime > 0)
        {
            countdownDisplay.text = countDownTime.ToString();
            yield return new WaitForSeconds(1f);
            countDownTime--;
        }

        countdownDisplay.text = "GO!";
        yield return new WaitForSeconds(1f);
        countdownDisplay.gameObject.SetActive(false);

        GameController.Instance.m_IsGameRunning = true;
        
        Destroy(this);
    }
    
}
