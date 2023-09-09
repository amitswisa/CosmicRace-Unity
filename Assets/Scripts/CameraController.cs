using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 1;
    public float minZoom = 8;
    public float maxZoom = 50;  // Adjust if needed
    public float safetyMargin = 2.0f;  // Additional margin to ensure all players are inside camera view
    public float zoomFactor = 0.5f;  // This is a new addition, adjust for zoom scale

    [SerializeField] private Transform player;

    private GameController gameController;
    private float targetZoom;
    private Camera cam;
    private float eliminationMargin = 2.0f;  // A margin beyond the camera view to eliminate a player
    private float warningTime = 3.0f;  // Time given for a player to come back into the view before elimination


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
            CheckForElimination();

            float maxDistance = 0;
            foreach (var p1 in gameController.m_Rivals)
            {
                foreach (var p2 in gameController.m_Rivals)
                {
                    float distance = Vector3.Distance(p1.Value.m_rivalInstance.transform.position, p2.Value.m_rivalInstance.transform.position);
                    if (distance > maxDistance)
                        maxDistance = distance;
                }
            }

            targetZoom = maxDistance * zoomFactor + safetyMargin;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

            // Adjust camera position to be centered between all players
            Vector3 averagePosition = Vector3.zero;
            int count = 0;
            foreach (var rival in gameController.m_Rivals)
            {
                averagePosition += rival.Value.m_rivalInstance.transform.position;
                count++;
            }
            averagePosition /= count;
            averagePosition.z = transform.position.z;
            transform.position = averagePosition;
        }
        else if(!gameController.m_IsFriendMode && gameController.m_IsGameRunning)
        {
            // Follow single player
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
    }

    void CheckForElimination()
    {
        float leftBound = transform.position.x - cam.orthographicSize * cam.aspect - eliminationMargin;
        float rightBound = transform.position.x + cam.orthographicSize * cam.aspect + eliminationMargin;

        KeyValuePair<string, MatchRival> lastPlayer = DetermineLastPlayer(); // Assuming Rival is the type of m_Rivals values

        if (lastPlayer.Value == null)
            return;

        Transform lastPlayerTransform = lastPlayer.Value.m_rivalInstance.transform;

        if (lastPlayerTransform.position.x < leftBound || lastPlayerTransform.position.x > rightBound)
        {
            EliminatePlayer(lastPlayer.Key);
        }
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

    void EliminatePlayer(string playerUsername)
    {
        GameController.Instance.m_Rivals[playerUsername].EliminateRival(playerUsername);
    }
}
