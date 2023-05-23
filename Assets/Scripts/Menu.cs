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
    [SerializeField] Button matchesHistoryBtn;

    void Start() {
        charactersScene.onClick.AddListener(() => {
            SceneManager.LoadScene("HomeCharactersScene");
        });

        matchesHistoryBtn.onClick.AddListener(() => {
            SceneManager.LoadScene("MatchesHistoryScene");
        });

        exitBtn.onClick.AddListener(Logout);
    }

    public void OnClickOnlineGame()
    {
        SceneManager.LoadScene("CharacterSelectScene");
    }

    public void OnClickCustomRoomBtn()
    {
        SceneManager.LoadScene("RoomScene");
    }

    public void Logout()
    {
        User.Logout();
        SceneManager.LoadScene("LoginScene");
    }
    
}
