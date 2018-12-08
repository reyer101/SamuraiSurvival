using System;
using UnityEngine;

// PlayerCharacter
public class PlayerCharacter : AbsCharacter {    
    
    public float m_JumpForce;
    
    private bool m_Grounded, m_HasSword;
    private Transform m_GroundCheck;
    private CircleCollider2D m_CircleCollider2D;
    private LayerMask m_LayerMask;     
    private Vector2 m_CrouchGroundCheck, m_WalkGroundCheck;

    private float lastJumpTime = -999f, lastThrowTime = -999f,
        throwAnimDuration;    
    private float k_GroundedRadius = .5f;   

    // Awake
    void Start ()
    {
        m_HasSword = true;
        m_GroundCheck = transform.Find("GroundCheck");  
        m_DamageColor = new Color(1f, 0.3f, 0.24f);
        m_LayerMask = -1;
        m_WalkGroundCheck = m_GroundCheck.localPosition;
        m_Animator.runtimeAnimatorController = Resources.Load(
            Constants.ANIM_WALK) as RuntimeAnimatorController;
        throwAnimDuration = .25f;
    }
	
	// FixedUpdate
	void FixedUpdate () {        
	    base.FixedUpdate();
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
            m_HealthBar.transform.localRotation = m_BackRotation;
        }
        else if (horizontal > 0)
        {
            transform.rotation = m_ForwardRotation;
            m_HealthBar.transform.localRotation = m_ForwardRotation;
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

    // StopThrowing
    void StopThrowing()
    {
        m_HasSword = true;
    }
    
    // ProcessDead
    protected override void ProcessDead()
    {
        // TODO: do player dead stuff here
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
                go.GetComponent<ShadowCharacter>().ProcessAttack(false);
            }
        }
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