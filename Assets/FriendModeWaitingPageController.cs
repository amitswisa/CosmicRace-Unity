using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendModeWaitingPageController : MonoBehaviour
{
    public TextMeshProUGUI m_PageTitle;
    public GameObject itemPrefab; // UI Text prefab
    public Button m_StartGameBtn;
    public Transform contentPanel; // Content of the ScrollView
    public TextMeshProUGUI waitingText;
    public List<string> m_WaitingPlayers {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        this.m_WaitingPlayers = new List<string>();

        m_StartGameBtn.onClick.AddListener(onStartGameBtnClicked);

        GameClient.Instance.OnPlayerJoined += AddWaitingPlayer;
        GameClient.Instance.OnPlayerLeft += RemoveWaitingPlayer;

        if(GameController.Instance.m_IsFriendMode
            && GameController.Instance.IsConnected())
            {
                m_PageTitle.text = "#" + GameController.Instance.GetMatchIdentifier();
            }

        UpdateListDisplay();
    }

    // Call this function whenever the list changes
    public void UpdateListDisplay()
    {
        // Check if the list is empty.
        if (m_WaitingPlayers.Count == 0)
        {
            // If it is, show the waiting text and return.
            waitingText.gameObject.SetActive(true);
            contentPanel.gameObject.SetActive(false);
            return;
        }
        else
        {
            // If not, make sure the waiting text is hidden.
            waitingText.gameObject.SetActive(false);
            contentPanel.gameObject.SetActive(true);
        }

        // Remove old items
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Add new items
        for (int i = 0; i < m_WaitingPlayers.Count; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            newItem.GetComponent<TextMeshProUGUI>().text = (i+1).ToString() + ". " + m_WaitingPlayers[i] + "\n";
        }
    }

    public void onStartGameBtnClicked()
    {
        if(this.m_WaitingPlayers.Count < 2)
        {
            OKDialogManager.Instance.ShowDialog("Not enough players", "You need at least 2 players to start the game.");
            return;
        }

        GameController.Instance.SendMessageToServer("START\n");
    }

    public void AddWaitingPlayer(string playerName)
    {
        this.m_WaitingPlayers.Add(playerName);
        UpdateListDisplay();
    }

    public void RemoveWaitingPlayer(string playerName)
    {
        this.m_WaitingPlayers.Remove(playerName);
        UpdateListDisplay();
    }

    void Update()
    {
        
    }
}
