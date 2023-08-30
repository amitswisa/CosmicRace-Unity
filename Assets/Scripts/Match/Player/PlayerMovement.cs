using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private LayerMask floorLayerMask;
    [SerializeField] private AudioSource jumpSoundEffect;
    private enum MovementState {idle, running, jumping, falling, attack, attacked}
    private bool isPoweUpOn = false;
    private bool isEnumerate = false;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _boxCollider2D;
    private Vector2 _velocity;
    private readonly float _extraHeightCheck = .1f;
    private float _dirX = 0f;
    private long lastLocationSend;
    private PlayerCommand command;
    private bool isAttacked = false;
    public GameObject lighteningAttack;
    public AudioClip lightningSound;
    private AudioSource lightningAudioSource;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        this.command = new PlayerCommand(MessageType.COMMAND, User.getUsername()
                    , PlayerCommand.PlayerAction.IDLE, new Location(getX(), getY()));

        this.lastLocationSend = getCurrentTimeInMilliseconds();

        GetComponent<PlayerData>()._selected_charecter = PlayerPrefs.GetInt("SelectedCharacter", 0);

        GetComponentInChildren<TextMeshPro>().SetText(User.getUsername());
        SetLightingVisibility(false);
        lightningAudioSource = lighteningAttack.GetComponent<AudioSource>();
        if (lightningAudioSource == null)
        {
            lightningAudioSource = lighteningAttack.GetComponent<AudioSource>();
        }
    }

    void Update() 
    {
        if (!GameController.Instance.m_IsGameRunning) return;
        if (GameController.Instance.m_IsPlayerFinished) return;

        PlayerCommand currentCommand = null;

        _dirX = Input.GetAxisRaw("Horizontal");
        _rigidbody2D.velocity = new Vector2( _dirX * speed, _rigidbody2D.velocity.y);
        
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            currentCommand = command;
            GameController.Instance.PlayerQuit();
        }

        if (IsOnGround() && Input.GetKeyDown(KeyCode.Space))
        {
            jumpSoundEffect.Play();
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce);
            
            currentCommand = new PlayerCommand(MessageType.COMMAND, User.getUsername()
                , PlayerCommand.PlayerAction.JUMP, new Location(getX(), getY()));
        }

        if((Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D)) && !command.isEqual(PlayerCommand.PlayerAction.RUN_RIGHT))
        {
             currentCommand = new PlayerCommand(MessageType.COMMAND, User.getUsername()
                                        , PlayerCommand.PlayerAction.RUN_RIGHT, new Location(getX(), getY()));
        } else if((Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A)) && !command.isEqual(PlayerCommand.PlayerAction.RUN_LEFT))
        {
             currentCommand = new PlayerCommand(MessageType.COMMAND, User.getUsername()
                                        , PlayerCommand.PlayerAction.RUN_LEFT, new Location(getX(), getY()));
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentCommand = new PlayerCommand(MessageType.COMMAND, User.getUsername(),
                PlayerCommand.PlayerAction.ATTACK, new Location(getX(), getY()));
            // StartCoroutine((Attack(1.5f)));
        }

        UpdateLoctionAtServerTask();
        UpdateAnimationState(currentCommand);
    }

    public void setDirX(float i_dirx)
    {
        this._dirX = i_dirx;
    }

    private long getCurrentTimeInMilliseconds()
    {
        return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    private bool isLocationUpdateRequired()
    {
        return (getCurrentTimeInMilliseconds() - this.lastLocationSend) >= 4000;
    }

    private void UpdateLoctionAtServerTask()
    {
        if(isLocationUpdateRequired() && !GameController.Instance.m_IsFriendMode)
        {
            PlayerCommand updateLocationCommand = new PlayerCommand(MessageType.COMMAND, User.getUsername()
                ,PlayerCommand.PlayerAction.UPDATE_LOCATION, new Location(getX(), getY()));
                
            GameClient.Instance.SendMessageToServer(updateLocationCommand.ToJson()  + "\n");

            lastLocationSend = getCurrentTimeInMilliseconds();
            Debug.Log("UpdateLocationAtServerTask triggered!");
        }
    }

    private bool IsOnGround()
    {
        return Physics2D.BoxCast(_boxCollider2D.bounds.center, _boxCollider2D.size, 0f, Vector2.down, .1f, floorLayerMask);
    }

    public void ResetIsSpecialPowerOn()
    {
        isPoweUpOn = false;
    }

    public void UpdateAnimationState(PlayerCommand currentCommand)
    {
        MovementState state;
        if (_dirX > 0f)
        {
            state = MovementState.running;
            _spriteRenderer.flipX = false;
        }
        else if (_dirX < 0f)
        {
            state = MovementState.running;
            _spriteRenderer.flipX = true;
        }
        else
        {
            try
            {
                if (!command.isEqual(PlayerCommand.PlayerAction.IDLE))
                {
                    currentCommand = new PlayerCommand(MessageType.COMMAND, User.getUsername()
                        , PlayerCommand.PlayerAction.IDLE, new Location(getX(), getY()));
                }
            }
            catch (Exception e)
            {
            }

            state = MovementState.idle;
        }

        if (_rigidbody2D.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if(_rigidbody2D.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }
        _animator.SetInteger("state", (int)state);

        if(currentCommand != null)
        {
            if(!currentCommand.isEqual(command.m_Action))
            {
                Debug.Log("-----------------\n"+currentCommand.ToJson()+"\n-------------");
                GameController.Instance.SendMessageToServer(currentCommand.ToJson()+"\n");
                command = currentCommand;
            }
        }
    }

    IEnumerator PowerUp(float duration)
    {
        Debug.Log("PowerUp started!");
        isPoweUpOn = true;
        yield return new WaitForSeconds(duration);
        speed = speed/2f;
        isPoweUpOn = false;
        Debug.Log("PowerUp Off!");

    }


    public void AttackedByLighting(float duration)
    {
        StartCoroutine(Attacked(duration));
    }

    private IEnumerator Attacked(float duration)
    {
        SetLightingVisibility(true);
        PlayLightningSound(lightningSound);
        OnLightingAttacked();
        yield return new WaitForSeconds(duration);
        SetLightingVisibility(false);
    }
    
    private void SetLightingVisibility(bool attacked)
    {
        isAttacked = attacked;
       
        if (lighteningAttack != null)
        {
            lighteningAttack.SetActive(attacked);
        }
    }
    
    private void PlayLightningSound(AudioClip lightningSoundClip)
    {
        if (lightningAudioSource != null && lightningSoundClip != null)
        {
            lightningAudioSource.PlayOneShot(lightningSoundClip);
            
        }
    }

    private void OnLightingAttacked()
    {
        float exp = GetComponent<PlayerData>().exp;
        exp -= 35;
        exp = exp < 0 ? 0 : exp;
        GetComponent<PlayerData>().exp = exp;
        _rigidbody2D.bodyType = RigidbodyType2D.Static;
        ResetIsSpecialPowerOn();

        // Send death notification.
        PlayerCommand currentCommand = new PlayerCommand(MessageType.COMMAND, User.getUsername()
            , PlayerCommand.PlayerAction.DEATH, new Location(transform.position.x, transform.position.y));
        GameController.Instance.SendMessageToServer(currentCommand.ToJson()+"\n");
        Debug.Log("Death trigger sent!");
        GetComponentInChildren<TextMeshPro>().SetText("");
        _animator.SetTrigger("death");
        GetComponent<Transform>().transform.position = transform.position;
    }

    private Transform respawnPoint = null;

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Trap") && !GameController.Instance.m_IsPlayerFinished)
        {
            float exp = GetComponent<PlayerData>().exp;
            exp -= 35;
            exp = exp < 0 ? 0 : exp;
            GetComponent<PlayerData>().exp = exp;
            _rigidbody2D.bodyType = RigidbodyType2D.Static;
            ResetIsSpecialPowerOn();
            
            respawnPoint = col.gameObject.GetComponent<TrapData>().respawnPoint;

            // Send death notification.
            PlayerCommand currentCommand = new PlayerCommand(MessageType.COMMAND, User.getUsername()
                , PlayerCommand.PlayerAction.DEATH, new Location(respawnPoint.position.x, respawnPoint.position.y));

            GameController.Instance.SendMessageToServer(currentCommand.ToJson()+"\n");

            Debug.Log("Current Command: " + currentCommand.ToJson());
            Debug.Log("Death trigger sent!");

            GetComponentInChildren<TextMeshPro>().SetText("");

            _animator.SetTrigger("death");

            GetComponent<Transform>().transform.position = respawnPoint.position;

            respawnPoint = null;
            
            Debug.Log("for user: "+ User.getUsername()+"the for player is Name: "+  GetComponentInChildren<TextMeshPro>().text);
        }
    }

    private void RestartLevel()
    {
        _animator.ResetTrigger("death");
        GetComponentInChildren<TextMeshPro>().SetText(User.getUsername());
        Debug.Log("Name: "+  GetComponentInChildren<TextMeshPro>().text);
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _animator.Play("Idle", 0,0f);
    }

    private float getX()
    {
        return gameObject.transform.position.x;
    }

    private float getY()
    {
        return gameObject.transform.position.y;
    }
    
    public void DeathByProjectile(String bullet_id)
    {
        PlayerCommand currentCommand = new PlayerCommand(MessageType.COMMAND, User.getUsername()
            , PlayerCommand.PlayerAction.DEATH, new Location(transform.position.x, transform.position.y));
        GameController.Instance.SendMessageToServer(currentCommand.ToJson()+"\n");
        GameController.Instance.CollideBullet(bullet_id, User.getUsername());
    }
}
