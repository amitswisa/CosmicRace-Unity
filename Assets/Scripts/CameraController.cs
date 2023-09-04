using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 1;
    public float minZoom = 8;
    public float maxZoom = 17;

    [SerializeField] private Transform player;

    private GameController gameController;

    private float targetZoom;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
        this.gameController = GameController.Instance;
    }

    private void Update()
    {
        if (gameController.m_IsFriendMode)
        {
            // Find two most distant players
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

            // Adjust camera zoom to encompass all players
            targetZoom = Mathf.Clamp(maxDistance, minZoom, maxZoom);
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
        else
        {
            // Follow player
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
    }
}