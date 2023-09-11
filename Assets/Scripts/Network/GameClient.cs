using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerCommand;

public class GameClient : IDisposable
{
    private TcpClient socket;
    private NetworkStream stream;
    private byte[] receiveBuffer;
    private StringBuilder receivedData = new StringBuilder();
    private ConcurrentQueue<string> messages = new ConcurrentQueue<string>();
    private CancellationTokenSource cancellationTokenSource;
    public event Action<string> OnMessageReceived;
    private bool IsConnected => socket != null && socket.Connected;
    private Task processMessagesTask;
    private Task receiveDataTask;
    private Text m_UpdatesTextObject;

    private GameClient()
    {

    }

    public static GameClient Instance { get; private set; } = new GameClient();
    public Action<string> OnPlayerJoined { get; internal set; }
    public Action<string> OnPlayerLeft { get; internal set; }

    public static void ClearInstance()
    {
        Instance = new GameClient();
    }

    public async Task Connect(bool i_IsFiendMode)
    {
        if (IsConnected)
            this.Disconnect();

        try
        {
            socket = new TcpClient();
            socket.ReceiveBufferSize = 4096;
            socket.SendBufferSize = 4096;
            socket.NoDelay = true;

            receiveBuffer = new byte[socket.ReceiveBufferSize];
            cancellationTokenSource = new CancellationTokenSource();
            Console.WriteLine("Connecting to server...");
            await socket.ConnectAsync(Utils.GAME_SERVER_IP, Utils.GAME_SERVER_PORT);
            Console.WriteLine("Connected to server");
            stream = socket.GetStream();

            if(i_IsFiendMode)
            {
                startFriendModeConnection();
            }
            else
            {
                startOnlineMatchConnection();
            }

            // Start the message processor
            processMessagesTask = ProcessMessagesAsync();
            receiveDataTask = ReceiveDataAsync();
        }
        catch (Exception e)
        {
            OKDialogManager.Instance.ShowDialog("Error", "Unexpected server connection failure occured.");
            Debug.LogError($"{e.Message}");
            GameController.Instance.Disconnect();
        }
    }

    private async void startFriendModeConnection()
    {
        string gameType = "Offline";
        string initGameDataJson = "{\"gameType\": " + gameType + "}\n";
        await SendMessageToServer(initGameDataJson);
    }

    private async void startOnlineMatchConnection()
    {
        string gameType = "Online";
        string initGameDataJson = "{\"gameType\": " + gameType + "}\n";
        await SendMessageToServer(initGameDataJson);

        // Send JSON data after connecting
        int characterId = PlayerPrefs.GetInt("SelectedCharacter", 0) + 1;
        string jsonData = "{\"userid\": " + User.getUserId() + ", \"characterId\": " + characterId + "}\n";
        await SendMessageToServer(jsonData);
    }

    public async Task SendMessageToServer(string message)
    {
        if (IsConnected)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            try
            {
                if (stream != null)
                {
                    await stream.WriteAsync(data, 0, data.Length);
                    //Debug.Log("Message sent to server: " + message);
                }
                else
                {
                    throw new Exception("Stream is null!");
                }
            }
            catch (Exception ex)
            {   
                OKDialogManager.Instance.ShowDialog("Error", "Unexpected error was occured.");
                Debug.LogError($"Error while writing to stream: {ex.Message}");
                GameController.Instance.Disconnect();
            }
        }
    }

    // Throws IOException.
    private async Task ReceiveDataAsync()
    {
        while (IsConnected && !cancellationTokenSource.Token.IsCancellationRequested)
        {
            int bytesRead = await stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length, cancellationTokenSource.Token);
            if (bytesRead <= 0)
                continue;

            string receivedMessage = Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);
            receivedData.Append(receivedMessage);

            ProcessReceivedData();
        }
    }

    private void ProcessReceivedData()
    {
        while (receivedData.ToString().IndexOf('\n') != -1)
        {
            int newlineIndex = receivedData.ToString().IndexOf('\n');
            string completeMessage = receivedData.ToString().Substring(0, newlineIndex);
            receivedData.Remove(0, newlineIndex + 1);

            messages.Enqueue(completeMessage);
        }
    }

    private async Task ProcessMessagesAsync()
    {
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            if (messages.TryDequeue(out string message))
                await HandleMessageAsync(message);
            else
                await Task.Delay(1);
        }
    }

    private async Task HandleMessageAsync(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        try
        {
            JObject json = JObject.Parse(message);
            string type = (string)json["m_MessageType"];

            if (type.Equals(MessageType.MESSAGE))
            {
                await HandleServerMessageAsync(json);
            }
            else
            {
                HandlePlayerCommand(message);
            }
        }
        catch (Exception e)
        {
            Debug.Log(message);
            Debug.LogError($"Error while handling message: {e.Message}");
            OKDialogManager.Instance.ShowDialog("Error", "Unexpected error was occured.");
            GameController.Instance.Disconnect();
        }
    }

    private async Task HandleServerMessageAsync(JObject json)
    {
        string type = (string)json["ActionType"];
        string content = (string)json["MessageContent"];

        switch (type)
        {
            case "CONFIRMATION":
                if (content.Trim().Equals("isAlive"))
                {
                    await SendMessageToServer("ALIVE\n");
                    Debug.Log("Server validation check - SUCCESS!");
                }
                else if (content.Trim().Equals("READY?"))
                {
                    await SendMessageToServer("READY\n");
                    GameController.Instance.UpdateTextObject("Server is ready to start the game");
                }
                break;

            case "NOTIFICATION":
                GameController.Instance.UpdateTextObject(content);
                Debug.Log(content);
                break;

            case "ACTION":
                if (content.Trim().Equals("START"))
                {
                    GameController.Instance.UpdateTextObject("Starting Game...");
                    Debug.Log("Starting Game...");
                    
                    await Task.Delay(1000); // Delay before scene transition
                    await UnityMainThreadDispatcher.Instance.EnqueueAsync(() =>
                    {
                        SceneManager.LoadSceneAsync("MatchScene", LoadSceneMode.Single);
                        GameController.Instance.SetMatchStarted();
                    });
                }
                break;
            
            case "DATA":
                JObject resJsonData = JObject.Parse(content);
                GameController.Instance.UpdateRivalesData(resJsonData);
                break;
            
            case "MATCH_TERMINATION":
                GameController.Instance.Disconnect(content);
                break;

            case PlayerAction.COMPLETE_LEVEL:
                Debug.Log("COMPLETE_LEVEL");
                Debug.Log(content);
                JObject completeLevelJson = JObject.Parse(content);
                
                // Extract Username and Poisition keys from content
                string finishedPlayerName = (string)completeLevelJson["Username"];
                int finishedPlayerPosition = (int)completeLevelJson["Position"];
                int coinsColl = (int)completeLevelJson["Coins"];
                
                // Check if key exists before.
                if(GameController.Instance.m_finish_players.ContainsKey(finishedPlayerName))
                    GameController.Instance.m_finish_players.Remove(finishedPlayerName);

                GameController.Instance.m_finish_players.Add(finishedPlayerName, finishedPlayerPosition);

                GameController.Instance.NotificationEnqueue(finishedPlayerName + " finished in #" + finishedPlayerPosition + " position!");

                if(finishedPlayerName.Equals(User.getUsername()))
                {
                    User.updateCoinsAmount(coinsColl);

                    int xpUpdateAmount = 0;
                    
                    if(finishedPlayerPosition == 1)
                    {
                        xpUpdateAmount = 10;
                    } else if(finishedPlayerPosition == 2) {
                        xpUpdateAmount = 5;
                    }


                    User.updateXp(xpUpdateAmount);
                }
                
                break;

            case "COMPLETE_MATCH":
                Debug.Log("COMPLETE_MATCH Occured.");
                await UnityMainThreadDispatcher.Instance.EnqueueAsync(() =>
                {
                    /// TODO go to finish scene
                    SceneManager.LoadScene("FinishScene", LoadSceneMode.Single);
                    OKDialogManager.Instance.ShowDialog("Match Finish!", content);
                });
                break;

            case "ROOM_CREATED":
                GameController.Instance.SetMatchIdentifier(content);
                Debug.Log("Room created: " + content);
                
                await UnityMainThreadDispatcher.Instance.EnqueueAsync(() =>
                    {
                        SceneManager.LoadSceneAsync("FriendModeWaitingScene", LoadSceneMode.Single);
                    });
                break;

            case "PLAYER_JOINED":
                Debug.Log("Player joined: " + content);
                OnPlayerJoined?.Invoke(content);
                break;

            case "MATCH_ENDED":
                GameController.Instance.EndMatch();
                break;
        }
    }

    private void HandlePlayerCommand(string command)
    {
        
        PlayerCommand playerCommand = JsonConvert.DeserializeObject<PlayerCommand>(command);

        switch (playerCommand.m_Action)
        {
            case PlayerAction.JUMP:
                GameController.Instance.m_Rivals[playerCommand.m_Username].PerformJump(playerCommand);
                break;

            case PlayerAction.RUN_LEFT:
                GameController.Instance.m_Rivals[playerCommand.m_Username].MoveLeft(playerCommand);
                break;

            case PlayerAction.RUN_RIGHT:
                GameController.Instance.m_Rivals[playerCommand.m_Username].MoveRight(playerCommand);
                break;

            case PlayerAction.IDLE:
                GameController.Instance.m_Rivals[playerCommand.m_Username].StopMoving(playerCommand);
                break;
            case PlayerAction.ATTACK:
                if (playerCommand.m_AttackInfo != null
                    && playerCommand.m_AttackInfo.m_AttackerName != null
                    && playerCommand.m_AttackInfo.m_Victim != null)
                {
                    AttackInfo attackInfo = playerCommand.m_AttackInfo;
                    if (attackInfo.m_Victim != User.getUsername())
                    {
                        GameController.Instance.m_Rivals[attackInfo.m_Victim].AttackedByLighting(playerCommand);
                    }
                    else
                    {
                        GameObject player = GameObject.FindWithTag("Player");
                        player.GetComponent<PlayerMovement>().AttackedByLighting(1.5f);
                    }

                    GameController.Instance.NotificationEnqueue(playerCommand.m_AttackInfo.m_AttackerName + " attacked " + playerCommand.m_AttackInfo.m_Victim + "!");
                }

                break;

            case PlayerAction.UPDATE_LOCATION:
                GameController.Instance.m_Rivals[playerCommand.m_Username].PositionCorrection(playerCommand);
                break;

            case PlayerAction.DEATH:
                GameController.Instance.m_Rivals[playerCommand.m_Username].ActivateDeath(playerCommand.m_Location);
                break;

            case PlayerAction.RIVAL_QUIT:

                GameController.Instance.NotificationEnqueue(playerCommand.m_Username + "quited the match");
                
                if(GameController.Instance.m_IsMatchStarted)
                {
                    GameController.Instance.m_Rivals[playerCommand.m_Username].Quit(playerCommand);
                    GameController.Instance.m_Rivals.Remove(playerCommand.m_Username);
                }
                
                if(GameController.Instance.m_IsFriendMode && !GameController.Instance.m_IsMatchStarted)
                    OnPlayerLeft?.Invoke(playerCommand.m_Username);

                break;
            case PlayerAction.COMPLETE_LEVEL:
                GameController.Instance.NotificationEnqueue(playerCommand.m_Username + " got to the final line");
                break;
            default:
                Debug.Log("Player " + playerCommand.m_Username + " action: " + playerCommand.m_Action);
                break;
        }
    }

    public async void Dispose()
    {
        cancellationTokenSource?.Cancel();

        if (processMessagesTask != null && receiveDataTask != null)
        {
            await Task.WhenAll(processMessagesTask, receiveDataTask);
        }

        OnMessageReceived = null;

        cancellationTokenSource?.Dispose();
        cancellationTokenSource = null;

        receivedData.Clear();
        messages.Clear();

        stream?.Close();
        stream = null;

        socket?.Close();
        socket = null;

        processMessagesTask = null;
        receiveDataTask = null;
    }

    public bool IsConnectionAlive()
    {
        return this.IsConnected;
    }

    public void Disconnect()
    {
        Dispose();
    }
}