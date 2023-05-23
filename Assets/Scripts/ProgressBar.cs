using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public static ProgressBar instance;
    [SerializeField] private PlayerData _playerData;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameController.Instance.m_IsGameRunning)
        {
            if (Input.GetKeyDown(KeyCode.R) && _playerData.IsExpFull())
            {
                Debug.Log("hp before decrease: " + _playerData.exp);
                _playerData.exp -= 50f;
                Debug.Log("hp after decrease: " + _playerData.exp);
            }

            GetComponent<Slider>().value = _playerData.exp / _playerData._maxExp;
        }
    }

}
