using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private int _selected_charecter;
    public float exp = 0;
    [SerializeField] private float _expIncreasePerSec = 5f;
    public readonly float _maxExp = 100f;
    public string playerName = "";
    private bool flag = false;
    
    // Start is called before the first frame update
    void Start()
    {
        //gameClient = GameClient.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!flag)
        {
            GetComponentInChildren<TextMeshPro>().SetText(playerName);
            flag = true;
        }
        
        if (!GameController.Instance.m_IsGameRunning)
            return;
        if (exp < _maxExp)
        {
            exp += _expIncreasePerSec * Time.deltaTime;
        }
        else if (exp > _maxExp)
        {
            exp = _maxExp;
        }
    }
    
    public bool IsExpFull()
    {
        return exp >= _maxExp;
    }
}
