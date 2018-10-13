using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
using UnityEngine;

// PlayerCharacter
public class PlayerCharacter : MonoBehaviour {    
    public float m_MaxSpeed, m_JumpForce, m_AnimationSpeed;
    public int HP;     
    private bool m_Grounded;    
    private AudioSource m_Audio;
    private Animator m_Animator;  
    private Rigidbody2D m_Rigidbody2D;
    private SpriteRenderer m_SpriteRenderer;
    private Transform m_GroundCheck;
    private CircleCollider2D m_CircleCollider2D;
    private LayerMask m_LayerMask;     
    private Vector2 m_CrouchGroundCheck, m_WalkGroundCheck;
    private Quaternion m_ForwardRotation, m_BackRotation;    
          
    private float lastJumpTime, baseSpeed, blockSpeed, attackSpeed;    
    private float k_GroundedRadius = .5f;   
    private float k_ClimbRadius = 1.0f;
    private float k_UnderRadius = 1f;

    // Awake
    void Awake () {             
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Audio = GetComponent<AudioSource>();
        m_Animator = GetComponent<Animator>();
        m_GroundCheck = transform.Find("GroundCheck");              
        m_ForwardRotation = transform.rotation;              
        m_BackRotation = new Quaternion(0, m_ForwardRotation.y - 1, 0, 0);          
        m_LayerMask = -1;
        m_WalkGroundCheck = m_GroundCheck.localPosition;
        lastJumpTime = Time.time;             

        m_Animator.runtimeAnimatorController = Resources.Load(
            Constants.ANIM_WALK) as RuntimeAnimatorController;

        baseSpeed = m_MaxSpeed;
        blockSpeed = 0f;
        attackSpeed = m_MaxSpeed * (2 / 3f);
    }
	
	// FixedUpdate
	void FixedUpdate () {        
        m_Grounded = false;            
        m_Rigidbody2D.gravityScale = 2;              

        // Check if player is standing on ground by searching for colliders overlapping radius at bottom of player
        Collider2D[] gColliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_LayerMask);
        for (int i = 0; i < gColliders.Length; i++)
        {                      
            if (gColliders[i].gameObject != gameObject)  
            {                
                m_Grounded = true;
            }                   
        }              

        if(m_Grounded)
        {
            m_Animator.speed = Mathf.Abs(m_AnimationSpeed * .2f * m_Rigidbody2D.velocity.x);                    
        }
        else
        {            
            m_Animator.speed = 0;
        }       
    }  

    /*
    Name: Move
    Parameters: float horizontal, bool jump, bool crouch
    */
    public void Move(float horizontal, bool jump)
    {
        if (horizontal < 0)
        {
            transform.rotation = m_BackRotation;
        }
        else if (horizontal > 0)
        {
            transform.rotation = m_ForwardRotation;
        }
                            
        m_Rigidbody2D.velocity = new Vector2(
            horizontal * m_MaxSpeed, m_Rigidbody2D.velocity.y);
        m_GroundCheck.localPosition = m_WalkGroundCheck;

        if(m_Grounded && Time.time - lastJumpTime > .1f)
        {
            // switch to walk animation
            m_Animator.runtimeAnimatorController = Resources.Load(
                Constants.ANIM_WALK) as RuntimeAnimatorController;
        }             

        if (m_Grounded && jump)
        {
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            lastJumpTime = Time.time;
            m_Animator.runtimeAnimatorController = Resources.Load(
                Constants.ANIM_JUMP) as RuntimeAnimatorController;
        }               
    }  
    
    /*
    Name: Attack
    Parameters: bool attack, bool block
    */
    public void Attack(bool attack, bool block)
    {
        if (block && m_Grounded)
        {
            Debug.Log("Loading block sprite...");
            m_Animator.runtimeAnimatorController = Resources.Load(
                Constants.ANIM_EMPTY) as RuntimeAnimatorController;
            m_SpriteRenderer.sprite = Resources.Load<Sprite>(
                Constants.SPRITE_BLOCK);
            m_MaxSpeed = blockSpeed;

            return;
        }

        m_SpriteRenderer.sprite = Resources.Load<Sprite>(
            Constants.SPRITE_IDLE);
        m_MaxSpeed = baseSpeed;
        if (attack)
        {
            
        }
    }
    

    // takeDamage
    void takeDamage()
    {
        HP -= 1;             
        if (HP == 0)
        {            
            SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);            
        }
    }

    // OnTriggerEnter2D
    void OnTriggerEnter2D(Collider2D other)
    {

    }
}
