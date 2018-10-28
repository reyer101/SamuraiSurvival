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
    private int m_AttackIdx, m_AttackSoundIdx;
    private bool m_Grounded, m_Attacking, m_HasSword;    
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
    private float k_ClimbRadius = 1.0f;
    private float k_UnderRadius = 1f;

    // Awake
    void Awake () {             
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Audio = GetComponent<AudioSource>();
        m_Animator = GetComponent<Animator>();
        m_HasSword = true;
        m_AttackIdx = 0;
        m_AttackSoundIdx = 0;
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

	    if (m_Attacking || Time.time - lastThrowTime <= throwAnimDuration)
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
    Parameters: bool attack, bool block, bool
    */
    public void Attack(bool attack, bool block, bool threw)
    {
        if (m_HasSword)
        {
            m_MaxSpeed = baseSpeed;
            if (threw && m_HasSword) //Time.time - lastThrowTime > m_ThrowDelay)
            {
                m_Animator.runtimeAnimatorController = Resources.Load(
                    Constants.ANIM_THROW) as RuntimeAnimatorController;
                m_HasSword = false;
                lastThrowTime = Time.time;

                m_Audio.clip = Resources.Load<AudioClip>(String.Format(
                    Constants.CLIP_SWING, 0));
                m_Audio.Play();

                Invoke("throwSword", throwAnimDuration);
                return;
            }
            
            if (block && m_Grounded)
            {
                m_Animator.runtimeAnimatorController = Resources.Load(
                    Constants.ANIM_EMPTY) as RuntimeAnimatorController;
                m_SpriteRenderer.sprite = Resources.Load<Sprite>(
                    Constants.SPRITE_BLOCK);
                m_MaxSpeed = blockSpeed;

                return;
            }

            if (attack && (Time.time - lastAttackTime > m_AttackDelay) && m_HasSword)
            {
                m_Attacking = true;
                m_Animator.runtimeAnimatorController = Resources
                    .Load(m_Attacks[m_AttackIdx % 2]) as RuntimeAnimatorController;
                ++m_AttackIdx;
                lastAttackTime = Time.time;

                string clip = String.Format(Constants.CLIP_SWING,
                    m_AttackSoundIdx % 3);
                m_Audio.clip = Resources.Load<AudioClip>(clip);
                m_Audio.Play();
                ++m_AttackSoundIdx;

                Invoke("stopAttacking", m_AttackDelay);
            }
        }
    }
    
    // throwSword
    public void throwSword()
    {
        GameObject sword = Resources.Load<GameObject>(Constants.OBJECT_SWORD);
        Vector3 offset = transform.rotation.y < m_ForwardRotation.y ? 
            new Vector2(-3.5f, 0) : new Vector2(3, 0);
        Instantiate(sword, transform.position + offset, transform.rotation);
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
        m_HasSword = true;
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
        if (other.tag == "Sword")
        {
            Destroy(other.gameObject);
            m_HasSword = true;

            if (!m_Grounded)
            {
                m_Animator.runtimeAnimatorController = Resources.Load(
                    Constants.ANIM_EMPTY) as RuntimeAnimatorController;
                m_SpriteRenderer.sprite = Resources.Load<Sprite>(
                    Constants.SPRITE_JUMP);
            }
            else
            {
                m_Animator.runtimeAnimatorController = Resources.Load(
                    Constants.ANIM_WALK) as RuntimeAnimatorController;
            }
        }
    }
}
