using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FriendModePageController : MonoBehaviour
{
    public void onCreateRoomBtnClicked()
    {
        GameController.Instance.ConnectInFriendMode();
    }
}
