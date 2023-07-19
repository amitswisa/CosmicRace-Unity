using TMPro;
using UnityEngine;

public class FriendModeWaitingPageController : MonoBehaviour
{
    public TextMeshProUGUI m_PageTitle;

    // Start is called before the first frame update
    void Start()
    {
        if(GameController.Instance.m_IsFriendMode
            && GameController.Instance.IsConnected())
            {
                m_PageTitle.text = "#" + GameController.Instance.GetMatchIdentifier();
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
