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
            DestroyCoins();
        }
        
        GameController.Instance.InitiateRivals();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.Instance.m_IsFriendMode 
            && Input.GetKeyDown(KeyCode.Escape))
                GameController.Instance.PlayerQuit();
    }

    private void DestroyPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if(player != null)
        {
            Destroy(player);
        }
    }

    private void DestroyCoins()
    {
        GameObject coin = GameObject.FindWithTag("Coin");

        if(coin != null)
        {
            Destroy(coin);
        }
    }
}
