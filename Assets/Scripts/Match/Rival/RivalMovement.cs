using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private enum MovementState {idle, running, jumping, falling}
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

    // Start is called before the first frame update
    void Start()
    {
        this.isJumping = new Action(false,false);

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _animator = gameObject.GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponentInChildren<TextMeshPro>().SetText(GetComponent<PlayerData>().playerName);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.Instance.m_IsGameRunning) return;
        if(m_MovementLock) return;
        
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

    private void UpdateAnimationState()
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

    public void TriggerDeath(Location location)
    {
        m_MovementLock = true;
        _dirX = 0;
        _rigidbody2D.bodyType = RigidbodyType2D.Static;
        GetComponentInChildren<TextMeshPro>().SetText("");
        _animator.SetTrigger("death");
        
    }
    public void RestartLevel()
    {        
        _animator.ResetTrigger("death");
        GetComponentInChildren<TextMeshPro>().SetText(GetComponent<PlayerData>().playerName);
        Debug.Log("Is Dynamic Now");
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _animator.Play("Idle", 0,0f);
        m_MovementLock = false;
    }
}