using System.Collections;
using TMPro;
using UnityEngine;

public class RivalMovement : MonoBehaviour
{
    private struct Action
    {
        public bool isInAction;
        public bool isActionActivated;
        
        public Action(bool isInAction, bool isActionActivated)
        {
            this.isInAction = isInAction;
            this.isActionActivated = isActionActivated;
        }
    }
    private float speed = 7f;
    private float jumpForce = 14f;
    [SerializeField] private LayerMask floorLayerMask;
    private enum MovementState {idle, running, jumping, falling, attacked}
    private bool isPoweUpOn = false;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _boxCollider2D;
    private Vector2 _velocity;
    private float _dirX = 0f;
    // Rival data members for performing action inside the game.
    private Action isJumping;
    private float desiredHorizontalInput = 0f;
    private bool m_MovementLock = false;
    public GameObject lighteningAttack;
    private Transform respawnPoint = null;
    private string rival_name;


    // Start is called before the first frame update
    void Start()
    {
        this.isJumping = new Action(false,false);

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _animator = gameObject.GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        rival_name = GetComponent<PlayerData>().playerName;
        GetComponentInChildren<TextMeshPro>().SetText(rival_name);
        SetLightingVisibility(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.Instance.m_IsGameRunning
            || this.m_MovementLock) return;
        
        if(m_MovementLock) return;
        
        if(GameController.Instance.m_Rivals[rival_name].isFinish) return;

        _dirX = desiredHorizontalInput;
        _rigidbody2D.velocity = new Vector2( _dirX * speed, _rigidbody2D.velocity.y);
        
        if(isJumping.isInAction)
        {
            if(IsOnGround())
            {
                if(!isJumping.isActionActivated)
                {
                    isJumping.isActionActivated = true;
                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce);
                } else {
                    isJumping.isInAction = false;
                    isJumping.isActionActivated = false;
                }
            }
        }
        
        UpdateAnimationState();
    }

// Method to initiate a jump
    public void PerformJump()
    {
        if (!isJumping.isInAction) // Ensure the player can't jump again while already jumping
        {
            isJumping.isInAction = true;
        }
    }

    public void MoveRight()
    {
        desiredHorizontalInput = 1f;
    }

    public void MoveLeft()
    {
        desiredHorizontalInput = -1f;
    }

    public void StopMoving()
    {
        desiredHorizontalInput = 0f;
    }

    private bool IsOnGround()
    {
        return Physics2D.BoxCast(_boxCollider2D.bounds.center, _boxCollider2D.size, 0f, Vector2.down, .1f, floorLayerMask);
    }

    public void ResetIsSpecialPowerOn()
    {
        isPoweUpOn = false;
    }

    public void setDirX(float dirX)
    {
        this._dirX = dirX;
    }

    public void UpdateAnimationState()
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

    public void AttackedByLighting(Location location, float duration)
    {
        StartCoroutine(Attacked(location, duration));
    }

    private IEnumerator Attacked(Location location, float duration)
    {
        SetLightingVisibility(true);
        OnAttacked();
        yield return new WaitForSeconds(duration);
        SetLightingVisibility(false);
    }
    
    private void SetLightingVisibility(bool attacked)
    {
        if (lighteningAttack != null)
        {
            lighteningAttack.SetActive(attacked);
        }
    }
    

    private void OnAttacked()
    {
        // Send death notification.
        Debug.Log("Death trigger sent!");
        GetComponentInChildren<TextMeshPro>().SetText("");
        _animator.SetTrigger("death");
        GetComponent<Transform>().transform.position = transform.position;
    }

    public void TriggerDeath(Location location)
    {
        m_MovementLock = true;
        _dirX = 0;
        _rigidbody2D.bodyType = RigidbodyType2D.Static;
        GetComponentInChildren<TextMeshPro>().SetText("");
        _animator.SetTrigger("death");
        
    }
    
    public void OnCollisionEnter2D(Collision2D col)
    {
        if (GameController.Instance.m_IsFriendMode && col.gameObject.CompareTag("Trap"))
        {
            _rigidbody2D.bodyType = RigidbodyType2D.Static;
            ResetIsSpecialPowerOn();
            
            respawnPoint = col.gameObject.GetComponent<TrapData>().respawnPoint;

            GetComponentInChildren<TextMeshPro>().SetText("");

            TriggerDeath(new Location(0, 0));

            GetComponent<Transform>().transform.position = respawnPoint.position;

            respawnPoint = null;
            
            Debug.Log("for user: "+ User.getUsername()+"the for player is Name: "+  GetComponentInChildren<TextMeshPro>().text);
        }
    }

    public void RestartLevel()
    {        
        _animator.ResetTrigger("death");
        GetComponentInChildren<TextMeshPro>().SetText(GetComponent<PlayerData>().playerName);
        Debug.Log("Is Dynamic Now");
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _animator.Play("Idle", 0,0f);
        m_MovementLock = false;
        
        if(GameController.Instance.m_IsFriendMode)
            GameController.Instance.OfflineMatch_NotifyBackToLife(rival_name);
    }
}