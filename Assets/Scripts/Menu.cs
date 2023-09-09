using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Button charactersScene;
    [SerializeField] Button exitBtn;

    void Start() {
        charactersScene.onClick.AddListener(() => {
            SceneManager.LoadScene("HomeCharactersScene");
        });

        exitBtn.onClick.AddListener(Logout);
    }

    public void OnClickOnlineGame()
    {
        SceneManager.LoadScene("OnlineMatchScene");
    }

    public void OnClickCustomRoomBtn()
    {
        SceneManager.LoadScene("OfflineModeScene");
    }

    public void Logout()
    {
        User.Logout();
        SceneManager.LoadScene("LoginScene");
    }
    
}
