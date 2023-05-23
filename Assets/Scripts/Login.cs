
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class Login : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button loginBtn;
    [SerializeField] private Button registerBtn;
    [SerializeField] private TMP_Text errorText;

    // Start is called before the first frame update
   void Start()
    {        
        loginBtn.onClick.AddListener(HandleLoginPressed);
        registerBtn.onClick.AddListener(() => {
            Application.OpenURL("http://cosmicrace.tech/register");
        });
    }

    private void HandleLoginPressed() {

        string username = this.usernameInputField.text;
        string password = this.passwordInputField.text;

        
        // Check that username and password inputs are filled.
        if(username == string.Empty || password == string.Empty) {
            changeErrorPanelText("Please fill all fields.");
            return;
        }

        // Disable login button.
        loginBtn.interactable = false;

        // Create the raw data to be sent in the request
        var data = new { username = username, password = password };

        HttpRequest httpRequest = new HttpRequest("login");

        try {
            JObject res = httpRequest.post(data);

            // Display the response data
            if(User.setUser(username, res))
            {
                SceneSwitcher.LoadScene("HomeScene");  // Switch to game home display scene.
            } else {
                changeErrorPanelText("Some error occured!");
            }
            
        } catch(System.Net.WebException err) {
            changeErrorPanelText(err.Message);
        } finally {
            loginBtn.interactable = true; // Enable button again.
        }
    }

    private void changeErrorPanelText(string text) {
        OKDialogManager.Instance.ShowDialog("Error", text);
    }
}
