using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserStatsScript : MonoBehaviour
{

    [SerializeField] TMP_Text usernameText;
    [SerializeField] TMP_Text coinsValue;

    // Start is called before the first frame update
    void Start()
    {
        usernameText.text = User.getUsername();
        coinsValue.text = User.getCoinsAmount().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
