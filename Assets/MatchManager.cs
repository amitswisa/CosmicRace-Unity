using UnityEngine;

public class MatchManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        // Find player and destroy on friend mode.
        if(GameController.Instance.m_IsFriendMode)
        {
            DestroyPlayer();
        }
        
        GameController.Instance.InitiateRivals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DestroyPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if(player != null)
        {
            Destroy(player);
        }
    }
}
