using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Rider.Unity.Editor;
using UnityEditorInternal;
using UnityEngine.SceneManagement;
using UnityEngine;

// PlayerCharacter
public class PlayerCharacter : MonoBehaviour {    
    public float m_MaxSpeed, m_JumpForce, m_AnimationSpeed, m_AttackDelay,
        m_ThrowDelay;
    public int HP;
    private int m_AttackIdx;
    private bool m_Grounded, m_Attacking, m_Threw;    
    private AudioSource m_Audio;
    private Animator m_Animator;  
    private Rigidbody2D m_Rigidbody2D;
    private SpriteRenderer m_SpriteRenderer;
    private Transform m_GroundCheck;
    private CircleCollider2D m_CircleCollider2D;
    private LayerMask m_LayerMask;     
    private Vector2 m_CrouchGroundCheck, m_WalkGroundCheck;
    private Quaternion m_ForwardRotation, m_BackRotation;
    private string[] m_Attacks;
          
    private float lastJumpTime, lastAttackTime, lastThrowTime, baseSpeed, 
        blockSpeed, attackSpeed;    
    private float k_GroundedRadius = .5f;   
    private float k_ClimbRadius = 1.0f;
    private float k_UnderRadius = 1f;

    // Awake
    void Awake () {             
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Audio = GetComponent<AudioSource>();
        m_Animator = GetComponent<Animator>();
        m_AttackIdx = 0;
        m_GroundCheck = transform.Find("GroundCheck");              
        m_ForwardRotation = transform.rotation;              
        m_BackRotation = new Quaternion(0, m_ForwardRotation.y - 1, 0, 0);          
        m_LayerMask = -1;
        m_WalkGroundCheck = m_GroundCheck.localPosition;
        lastJumpTime = Time.time;
        lastAttackTime = Time.time;
        lastThrowTime = Time.time;
        m_Attacks = new[] {Constants.ANIM_ATTACK1, Constants.ANIM_ATTACK2};

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

	    if (m_Attacking)
	    {
	        m_Animator.speed = Mathf.Abs(m_AnimationSpeed);
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

        if(m_Grounded && Time.time - lastJumpTime > .1f && !m_Attacking)
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
    Parameters: bool attack, bool block, bool
    */
    public void Attack(bool attack, bool block, bool t)
    {
        if (!m_Threw)
        {
            if (block && m_Grounded)
            {
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
            if (attack && (Time.time - lastAttackTime > m_AttackDelay))
            {
                m_Attacking = true;
                m_Animator.runtimeAnimatorController = Resources
                    .Load(m_Attacks[m_AttackIdx % 2]) as RuntimeAnimatorController;
                ++m_AttackIdx;
                lastAttackTime = Time.time;
            
                Invoke("stopAttacking", m_AttackDelay);
                return;
            }
        }
        
        
    }
    
    // isAttacking
    public bool isAttacking()
    {
        return m_Attacking;
    }

    // stopAttacking
    void stopAttacking()
    {
        m_Attacking = false;
    }

    // stopThrowing
    void stopThrowing()
    {
        m_Threw = false;
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
