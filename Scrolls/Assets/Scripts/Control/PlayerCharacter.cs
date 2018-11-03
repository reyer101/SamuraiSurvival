﻿using System;
using UnityEngine;

// PlayerCharacter
public class PlayerCharacter : MonoBehaviour {    
    public float m_MaxSpeed, m_JumpForce, m_AnimationSpeed, m_AttackDelay;

    [Range(0, 10)] 
    public float m_AttackRange;
    
    [Range(1, 20)]
    public int m_HP;
    private int m_AttackIdx, m_AttackSoundIdx, m_BlockSoundIdx;
    private bool m_Grounded, m_Attacking, m_Blocking, m_HasSword;    
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
        blockSpeed, throwAnimDuration;    
    private float k_GroundedRadius = .5f;   

    // Awake
    void Awake () {             
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Audio = GetComponent<AudioSource>();
        m_Animator = GetComponent<Animator>();
        m_HasSword = true;
        m_GroundCheck = transform.Find("GroundCheck");              
        m_ForwardRotation = transform.rotation;              
        m_BackRotation = new Quaternion(0, m_ForwardRotation.y - 1, 0, 0);          
        m_LayerMask = -1;
        m_WalkGroundCheck = m_GroundCheck.localPosition;
        lastJumpTime = -999f;
        lastAttackTime = -999f;
        lastThrowTime = -999f;
        m_Attacks = new[] {Constants.ANIM_ATTACK1, Constants.ANIM_ATTACK2};

        m_Animator.runtimeAnimatorController = Resources.Load(
            Constants.ANIM_WALK) as RuntimeAnimatorController;

        baseSpeed = m_MaxSpeed;
        blockSpeed = 0f;
        throwAnimDuration = .25f;
    }
	
	// FixedUpdate
	void FixedUpdate () {        
        m_Grounded = false;                         

        // Check if player is standing on ground by searching for
        // colliders overlapping radius at bottom of player
        Collider2D[] gColliders = Physics2D.OverlapCircleAll(
            m_GroundCheck.position, k_GroundedRadius, m_LayerMask);
        for (int i = 0; i < gColliders.Length; i++)
        {                      
            if (gColliders[i].gameObject != gameObject)  
            {                
                m_Grounded = true;
            }                   
        }              

        if(m_Grounded)
        {
            m_Animator.speed = Mathf.Abs(m_AnimationSpeed * .2f 
                * m_Rigidbody2D.velocity.x);  
        } 
        else
        {            
            m_Animator.speed = 0;
        }

	    if (m_Attacking || Time.time - lastThrowTime <= throwAnimDuration)
	    {
	        m_Animator.speed = Mathf.Abs(m_AnimationSpeed);
	    }
	}  

    /*
    Name: Move
    Parameters: float horizontal, bool jump
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

        // default no animation 
        String animPath = Constants.ANIM_EMPTY;
        if(m_Grounded && Time.time - lastJumpTime > .1f && !m_Attacking && 
           !(Time.time - lastThrowTime < m_AttackDelay))
        {
            // switch to walk animation
            animPath = m_HasSword ? Constants.ANIM_WALK : Constants.ANIM_WALKN; 
            m_Animator.runtimeAnimatorController = Resources.Load(
                animPath) as RuntimeAnimatorController;
        }             

        if (m_Grounded && jump)
        {
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            lastJumpTime = Time.time;
            String spritePath = m_HasSword ? Constants.SPRITE_JUMP 
                : Constants.SPRITE_JUMPN;
            m_Animator.runtimeAnimatorController = Resources.Load(
                Constants.ANIM_EMPTY) as RuntimeAnimatorController;
            Sprite sprite = Resources.Load<Sprite>(spritePath);

            m_SpriteRenderer.sprite = sprite;
        }
    }  
    
    /*
    Name: Attack
    Parameters: bool attack, bool block, bool threw
    */
    public void Attack(bool attack, bool block, bool threw)
    {
        m_Blocking = false;
        if (m_HasSword)
        {
            m_MaxSpeed = baseSpeed;
            if (threw && m_HasSword)
            {
                // start the sword throw animation 
                m_Animator.runtimeAnimatorController = Resources.Load(
                    Constants.ANIM_THROW) as RuntimeAnimatorController;
                m_HasSword = false;
                lastThrowTime = Time.time;

                // play the sword throw audio clip
                m_Audio.clip = Resources.Load<AudioClip>(String.Format(
                    Constants.CLIP_SWING, 0));
                m_Audio.Play();

                // throw the sword once animation completes
                Invoke("ThrowSword", throwAnimDuration);
                return;
            }
            
            if (block && m_Grounded)
            {
                // load the block sprite
                m_Animator.runtimeAnimatorController = Resources.Load(
                    Constants.ANIM_EMPTY) as RuntimeAnimatorController;
                m_SpriteRenderer.sprite = Resources.Load<Sprite>(
                    Constants.SPRITE_BLOCK);
                
                // prevent movement while blocking
                m_MaxSpeed = blockSpeed;
                m_Blocking = true;

                return;
            }

            if (attack && (Time.time - lastAttackTime > m_AttackDelay) && m_HasSword)
            {
                // start attack animation
                m_Attacking = true;
                m_Animator.runtimeAnimatorController = Resources
                    .Load(m_Attacks[m_AttackIdx % 2]) as RuntimeAnimatorController;
                ++m_AttackIdx;
                lastAttackTime = Time.time;

                // play attack audio clip
                string clip = String.Format(Constants.CLIP_SWING,
                    m_AttackSoundIdx % 3);
                m_Audio.clip = Resources.Load<AudioClip>(clip);
                m_Audio.Play();
                ++m_AttackSoundIdx;
                
                // damage all enemies in range
                Invoke("DamageTargets", .2f);

                // stop attacking after attack delay
                Invoke("StopAttacking", m_AttackDelay);
            }
        }
    }
    
    // ThrowSword
    void ThrowSword()
    {
        GameObject sword = Resources.Load<GameObject>(Constants.OBJECT_SWORD);
        Vector3 offset = transform.rotation.y < m_ForwardRotation.y ? 
            new Vector2(-3.5f, 0) : new Vector2(3, 0);
        Instantiate(sword, transform.position + offset, transform.rotation);
    }

    // StopAttacking
    void StopAttacking()
    {
        m_Attacking = false;
    }

    // StopThrowing
    void StopThrowing()
    {
        m_HasSword = true;
    }
    
    // DamageTargets
    void DamageTargets()
    {
        bool facingForward = transform.rotation == m_ForwardRotation;
        Vector2 direction = facingForward ? Vector2.right : Vector2.left;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position,
            direction, m_AttackRange);

        foreach (RaycastHit2D hit in hits)
        {
            GameObject go = hit.transform.gameObject;
            if (go.CompareTag(Constants.TAG_SHADOW))
            {
                go.GetComponent<Shadow>().TakeDamage();
            }
        }
    }

    // TakeDamage
    public void TakeDamage()
    {
        string clip;
        if (!m_Blocking) 
        {
            // lose hp and set impact clip path
            m_HP -= 1;
            clip = Constants.CLIP_IMPACT;
            if (m_HP == 0)
            {
                // player dead, do dead stuff here
                //SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
            }
        }
        else
        {
            // set block clip path
            clip = String.Format(Constants.CLIP_BLOCK,
                m_BlockSoundIdx % 2);
            ++m_BlockSoundIdx;
        }

        // play either block or impact audio clip
        m_Audio.clip = Resources.Load<AudioClip>(clip);
        m_Audio.Play();
    }

    // OnTriggerEnter2D
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.TAG_SWORD))
        {
            // "catch" the sword
            Destroy(other.gameObject);
            StopThrowing();

            if (!m_Grounded)
            {
                // toggle jump animation
                m_Animator.runtimeAnimatorController = Resources.Load(
                    Constants.ANIM_EMPTY) as RuntimeAnimatorController;
                m_SpriteRenderer.sprite = Resources.Load<Sprite>(
                    Constants.SPRITE_JUMP);
            }
            else
            {
                // toggle walk animation
                m_Animator.runtimeAnimatorController = Resources.Load(
                    Constants.ANIM_WALK) as RuntimeAnimatorController;
            }
        }
    }
}
