using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonClicked : MonoBehaviour
{
    [SerializeField] private Button exitBtn;

    // Start is called before the first frame update
    void Start()
    {
        // On clicking exit button.
        exitBtn.onClick.AddListener(() => {
            
            // Reset logged in user's data.
            User.Logout();

            // Switch to game home display scene.
            SceneManager.LoadScene("LoginScene"); 
            //SceneSwitcher.LoadScene("LoginScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
