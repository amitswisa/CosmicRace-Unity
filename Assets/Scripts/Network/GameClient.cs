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

    private GameClient()
    {

    }

    public static GameClient Instance { get; } = new GameClient();
    public Action<string> OnPlayerJoined { get; internal set; }
    public Action<string> OnPlayerLeft { get; internal set; }

    public async Task Connect(bool i_IsFiendMode)
    {
        if (IsConnected)
            return;

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
            Dispose();
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
                Dispose();
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
            Debug.LogError($"Error while handling message: {e.Message}");
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
                    Debug.Log("Server is ready to start the game");
                }
                break;

            case "NOTIFICATION":
                Debug.Log(content);
                break;

            case "ACTION":
                if (content.Trim().Equals("START"))
                {
                    Debug.Log("Starting Game...");
                    await Task.Delay(1000); // Delay before scene transition
                    await UnityMainThreadDispatcher.Instance.EnqueueAsync(() =>
                    {
                        GameController.Instance.SetMatchStarted();
                        SceneManager.LoadSceneAsync("MatchScene", LoadSceneMode.Single);
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

            case "COMPLETE_MATCH":
                    await UnityMainThreadDispatcher.Instance.EnqueueAsync(() =>
                    {
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
        Debug.Log(command);
        PlayerCommand playerCommand = JsonConvert.DeserializeObject<PlayerCommand>(command);

        if (playerCommand.m_AttackInfo != null &&
            playerCommand.m_AttackInfo.m_AttackerName != null &&
            playerCommand.m_AttackInfo.m_Victim != null
            )
        {
            AttackInfo attackInfo = playerCommand.m_AttackInfo;
            
            Debug.Log("GameClient.cs: " + "Attack - attackInfo: " + attackInfo.ToString());
            Debug.Log("GameClient.cs: " + "Attack - m_AttackerName: " + attackInfo.m_AttackerName);
            Debug.Log("GameClient.cs: " + "Attack - m_Victim: " + attackInfo.m_Victim);

            // TODO check if playerCommand.m_Username is the killer or the victim
            // TODO check if we need to access to m_Rivals or even when i attack (need to check)
            Debug.Log("GameClient.cs: " + "Attack - m_Rivals: " +GameController.Instance.m_Rivals.Count);
            foreach (var rival in GameController.Instance.m_Rivals)
            {
                Debug.Log("GameClient.cs: " + "Attack - m_Rivals rival.key: " + rival.Key);
                Debug.Log("GameClient.cs: " + "Attack - m_Rivals rival.Value.m_Username: " + rival.Value.m_Username);
                Debug.Log("GameClient.cs: " + "Attack - m_Rivals rival.Value.mCharacterId: " + rival.Value.mCharacterId);
            }
            Debug.Log("GameClient.cs: " + "Attack - User.getUsername: " +User.getUsername());
            if (attackInfo.m_Victim != User.getUsername())
            {
                GameController.Instance.m_Rivals[attackInfo.m_Victim].Attacked(playerCommand);
            }
            else
            {
                GameObject player = GameObject.FindWithTag("Player");
                player.GetComponent<PlayerMovement>().Attacked(1.5f);
            }

            return; // TODO CHECK IF VALID RRETURN HERE
        }

        BulletInfo bulletInfo;
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
                // TODO
                Debug.Log(command);
                //Debug.Log("GameClient.cs: " + "Attack - command: " + command);
                
                
                
                break;

            case PlayerAction.UPDATE_LOCATION:
                GameController.Instance.m_Rivals[playerCommand.m_Username].PositionCorrection(playerCommand);
                break;

            case PlayerAction.DEATH:
                GameController.Instance.m_Rivals[playerCommand.m_Username].ActivateDeath(playerCommand.m_Location);
                break;

            case PlayerAction.RIVAL_QUIT:
                Debug.Log(playerCommand.m_Username + "quited the match.");
                
                if(GameController.Instance.m_IsMatchStarted)
                    GameController.Instance.m_Rivals[playerCommand.m_Username].Quit(playerCommand);
                
                if(GameController.Instance.m_IsFriendMode && !GameController.Instance.m_IsMatchStarted)
                    OnPlayerLeft?.Invoke(playerCommand.m_Username);

                break;
            case PlayerAction.BULLET_CREATED:
                bulletInfo = playerCommand.m_bulletInfo;
                GameController.Instance.NewBullet(bulletInfo.id,bulletInfo.owner,bulletInfo.position,bulletInfo.isToRight);
                break;
            case PlayerAction.BULLET_UPDATED:
                bulletInfo = playerCommand.m_bulletInfo;
                GameController.Instance.UpdateBullet(bulletInfo.id,bulletInfo.position);
                break;
            case PlayerAction.BULLET_DESTROY:
                bulletInfo = playerCommand.m_bulletInfo;
                GameController.Instance.DestroyBullet(bulletInfo.id);
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