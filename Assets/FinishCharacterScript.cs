using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinishCharacterScript : MonoBehaviour
{
    [SerializeField]
    public Image m_skin;
    [SerializeField]
    public TextMeshProUGUI m_username;

    public void setSkin(Sprite sprite)
    {
        m_skin.sprite = sprite;
    }

    public void setUsername(string i_username)
    {
        m_username.text = i_username;
    }
}
