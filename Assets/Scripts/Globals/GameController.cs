using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using static PlayerCommand;

public class GameController : MonoBehaviour
{
    //[SerializeField] GameObject m_ClientUpdatesPanel;
    private StartGameUpdating updatePanelScript;
    private GameObject m_RivalPrefab;
    private string m_MatchIdentifier;
    public bool m_IsGameRunning = false;
    public bool m_IsMatchStarted {get; private set;}
    public bool m_IsQuit {get; set;}
    public JObject RivalsData {get; private set;}
    public Dictionary<string, MatchRival> m_Rivals {get; private set;}
    private static GameController instance; // Singleton instance
    public static GameController Instance // Public instance property
    {
        get
        {
            // Create instance if it doesn't exist
            if (instance == null)
            {
                instance = FindObjectOfType<GameController>();

                // If no instance found, create a new game object and attach the GameController script to it
                if (instance == null)
                {
                    GameObject gameControllerObject = new GameObject("GameController");
                    instance = gameControllerObject.AddComponent<GameController>();
                }
            }

            return instance;
        }
    }
    
    private void Awake()
    {
        // Ensure only one instance exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set instance to this if it doesn't exist
        if (instance == null)
        {
            instance = this;
        }

        // Ensure that this GameController and all of its children are not destroyed
        // when loading a new scene
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        this.m_IsQuit = false;
        this.m_IsMatchStarted = false;
        // try {
        //     this.m_ClientUpdatesPanel.SetActive(false);
        // } catch(Exception e) {
        //     Debug.Log("Update panel destroyed");
        // }

        GameObject[] rivalPrefabs = Resources.LoadAll<GameObject>("Prefabs/Match/Rival");

        if (rivalPrefabs.Length > 0)
        {
            m_RivalPrefab = rivalPrefabs[0];
            Debug.Log("Rival prefab loaded successfully.");
        }
        else
        {
            Debug.LogError("Rival prefab not found in Resources folder.");
        }
        
        this.m_IsGameRunning = false;
    }
    
    public async void Connect()
    {
        // this.m_ClientUpdatesPanel.SetActive(true);
        
        // updatePanelScript = m_ClientUpdatesPanel.GetComponent<StartGameUpdating>();
        // if(updatePanelScript != null)
        // {
        //     GameClient.Instance.OnMessageReceived += updatePanelScript.UpdatePanelText;
        //     await GameClient.Instance.Connect();
        // }

         await GameClient.Instance.Connect();

    }
    
    public void SendMessageToServer(string message)
    {
        GameClient.Instance.SendMessageToServer(message);
    }
    
    public void PlayerQuit()
    {
        this.m_IsQuit = true;

        PlayerCommand quitCommand
            = new PlayerCommand(MessageType.COMMAND, User.getUsername()
                    , PlayerAction.QUIT, new Location(0,0));
        
        this.SendMessageToServer(quitCommand.ToJson()+"\n");
        
        this.Disconnect();
    }

    public void InitiateRivals()
    {
        this.m_MatchIdentifier = (string) RivalsData["MatchIdentifier"];

        JObject playerList = (JObject)RivalsData["Players"];

        foreach (var player in playerList)
        {
            string playerName = player.Key.Trim('"');
            
            if(!playerName.Equals(User.getUsername()))
            {

                if(this.m_Rivals == null)
                    this.m_Rivals = new Dictionary<string, MatchRival>();

                GameObject newRival = Instantiate(m_RivalPrefab);
                
                // Convert CharacterData to JObject
                JObject characterData = JObject.Parse((string)player.Value["CharacterData"]);
                int characterID = (int)characterData["characterID"];

                MatchRival matchRival = new MatchRival(playerName, newRival, characterID);
                this.m_Rivals.Add(playerName, matchRival);
            }
        }
    }

    public void UpdateServerAfterCoinCollection()
    {
        // TODO - Fix Location to original location of player.
        PlayerCommand coinCollectCommand
            = new PlayerCommand(MessageType.COMMAND, User.getUsername()
                    , PlayerAction.COIN_COLLECT, new Location(0,0));
        this.SendMessageToServer(coinCollectCommand.ToJson()+"\n");
    }

    public void SetMatchStarted()
    {
        this.m_IsMatchStarted = true;
    }

    public void UpdateRivalesData(JObject i_RivalsData)
    {
        this.RivalsData = i_RivalsData;
    }

    public void Disconnect()
    {
        GameClient.Instance.Disconnect();
        
        if(this.m_IsMatchStarted)
        {
            UnityMainThreadDispatcher.Instance().EnqueueAsync(() => {
                SceneManager.LoadScene("HomeScene", LoadSceneMode.Single);

                if(this.m_IsQuit)
                    OKDialogManager.Instance.ShowDialog("Error", "You quit the game, no progress has been saved.");
                else
                    OKDialogManager.Instance.ShowDialog("Notification", "Match ended due to some reason.");
            });
        }
    }

    private void OnApplicationQuit()
    {
        if (GameClient.Instance != null)
        {
            GameClient.Instance.Disconnect();
        }
    }
}
