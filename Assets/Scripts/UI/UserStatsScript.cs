using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserStatsScript : MonoBehaviour
{

    [SerializeField] TMP_Text usernameText;
    [SerializeField] TMP_Text coinsValue;
    [SerializeField] Image progressBarImage;
    [SerializeField] TMP_Text levelValue;
    [SerializeField] Image xpValue;

    // Start is called before the first frame update
    void Start()
    {
        usernameText.text = User.getUsername();
        coinsValue.text = User.getCoinsAmount().ToString();
        progressBarImage.fillAmount = User.getProgress();
        levelValue.text = User.getLevel().ToString();
        xpValue.fillAmount = User.getProgress();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
