using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 1;
    public float minZoom = 8;
    public float maxZoom = 80; 
    public float safetyMargin = 2.0f;
    public float zoomFactor = 0.5f;

    [SerializeField] private Transform player;

    private GameController gameController;
    private float targetZoom;
    private Camera cam;
    private float eliminationMargin = 1.0f;

    private void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
        this.gameController = GameController.Instance;
    }

    private void Update()
    {
        if (gameController.m_IsFriendMode && gameController.m_IsGameRunning && gameController.m_Rivals.Count > 1)
        {
            Transform firstPlayerTransform = DetermineFirstPlayer().Value.m_rivalInstance.transform;
            
            // Calculate max distance from the first player to all other players
            float maxDistanceFromFirstPlayer = 0;
            foreach (var p in gameController.m_Rivals)
            {
                float distance = Vector3.Distance(firstPlayerTransform.position, p.Value.m_rivalInstance.transform.position);
                if (distance > maxDistanceFromFirstPlayer)
                    maxDistanceFromFirstPlayer = distance;
            }

            targetZoom = maxDistanceFromFirstPlayer * zoomFactor + safetyMargin;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

            // Adjust camera position to be a blend between average position and the first player's position
            Vector3 averagePosition = Vector3.zero;
            int count = 0;
            foreach (var rival in gameController.m_Rivals)
            {
                averagePosition += rival.Value.m_rivalInstance.transform.position;
                count++;
            }
            averagePosition /= count;
            Vector3 focusPosition = Vector3.Lerp(firstPlayerTransform.position, averagePosition, 0.5f);  // 0.5f gives equal weight to the first player and the average position
            focusPosition.z = transform.position.z;
            transform.position = focusPosition;

            CheckForElimination();
        }
        else if (!gameController.m_IsFriendMode && gameController.m_IsGameRunning)
        {
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
    }


    void CheckForElimination()
    {
        float leftBound = transform.position.x - cam.orthographicSize * cam.aspect - eliminationMargin;
        float rightBound = transform.position.x + cam.orthographicSize * cam.aspect + eliminationMargin;

        KeyValuePair<string, MatchRival> lastPlayer = DetermineLastPlayer();

        if (lastPlayer.Value == null)
            return;

        Transform lastPlayerTransform = lastPlayer.Value.m_rivalInstance.transform;

        if (lastPlayerTransform.position.x < leftBound || lastPlayerTransform.position.x > rightBound)
        {
            EliminateRival(lastPlayer.Key);
        }
    }

    KeyValuePair<string, MatchRival> DetermineFirstPlayer()
    {
        float maxX = float.MinValue;
        KeyValuePair<string, MatchRival> firstPlayer = new KeyValuePair<string, MatchRival>();

        foreach (var rival in gameController.m_Rivals)
        {
            if (rival.Value.m_rivalInstance.transform.position.x > maxX)
            {
                maxX = rival.Value.m_rivalInstance.transform.position.x;
                firstPlayer = rival;
            }
        }

        return firstPlayer;
    }

    KeyValuePair<string, MatchRival> DetermineLastPlayer()
    {
        float minX = float.MaxValue;
        KeyValuePair<string, MatchRival> lastPlayer = new KeyValuePair<string, MatchRival>();

        foreach (var rival in gameController.m_Rivals)
        {
            if (rival.Value.m_rivalInstance.transform.position.x < minX)
            {
                minX = rival.Value.m_rivalInstance.transform.position.x;
                lastPlayer = rival;
            }
        }

        return lastPlayer;
    }

    void EliminateRival(string playerUsername)
    {
        GameController.Instance.EliminateRival(playerUsername);
    }
}