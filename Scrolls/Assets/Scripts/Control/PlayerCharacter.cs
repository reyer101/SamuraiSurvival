using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// PlayerCharacter
public class PlayerCharacter : MonoBehaviour {
    public GameObject m_LevitateTarget, m_Witch, m_Wizard;
    public LinkedList<GameObject> m_LevitateTargets;    
    public float m_MaxSpeed, m_ClimbSpeed, m_CrouchSpeed, m_JumpForce, 
        m_FireSpellCD, m_LevitateRadius, m_LevitateSpeed;
    public int HP;
    public bool m_DropWhenOutOfRange, m_CanLevitateAndMove;    
    private bool m_Grounded;    
    private AudioSource m_Audio;
    private Animator m_Animator;  
    private Rigidbody2D m_Rigidbody2D;
    private BoxCollider2D[] m_Colliders;
    private Transform m_GroundCheck, m_ClimbCheck;
    private CircleCollider2D m_CircleCollider2D;
    private LayerMask m_LayerMask;    
    private Vector3 m_SpellSpawnPosition;
    private Vector2 m_WalkGroundCheck;
    private Quaternion m_ForwardRotation, m_BackRotation;
    
    private String m_AnimPrefix;  
    private float lastJumpTime;    
    private float k_GroundedRadius = .5f;   
    private float k_ClimbRadius = 1.0f;
    private float k_UnderRadius = 1f;
    public float k_GroundDistance;
         

    // Awake
    void Awake () {                                   
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Colliders = GetComponents<BoxCollider2D>();
        m_Audio = GetComponent<AudioSource>();
        m_Animator = GetComponent<Animator>();
        m_GroundCheck = transform.Find("GroundCheck");
            
        m_Animator.runtimeAnimatorController = Resources.Load(
            m_AnimPrefix + Constants.Walk) as RuntimeAnimatorController;
    }
	
	// FixedUpdate
	void FixedUpdate () {        
        m_Grounded = false;    
        m_Rigidbody2D.gravityScale = 2;
        m_SpellSpawnPosition = transform.Find("SpellSpawner").transform.position;       

        // Check if player is standing on ground by searching for colliders overlapping radius at bottom of player
        Collider2D[] gColliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_LayerMask);
        for (int i = 0; i < gColliders.Length; i++)
        {                      
            if (gColliders[i].gameObject != gameObject 
                && gColliders[i].gameObject.tag != "Checkpoint")  
            {                
                m_Grounded = true;
               
            }                   
        }    

        if(m_Grounded)
        {
            m_Animator.speed = Mathf.Abs(.2f * m_Rigidbody2D.velocity.x);                    
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
    public void Move(float horizontal, bool jump, bool crouch)
    {
       
        m_Rigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezePositionX;

        if (horizontal < 0)
        {
            transform.rotation = m_BackRotation;
        }
        else if (horizontal > 0)
        {
            transform.rotation = m_ForwardRotation;
        }
                        
        if(horizontal <= 0 && m_Rigidbody2D.velocity.x > 0
            || horizontal >= 0 && m_Rigidbody2D.velocity.x < 0) {
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX
                | RigidbodyConstraints2D.FreezeRotation;
        }
                             
        m_Rigidbody2D.velocity = new Vector2(
            horizontal * m_MaxSpeed, m_Rigidbody2D.velocity.y);
        m_GroundCheck.localPosition = m_WalkGroundCheck;

        if(m_Grounded && Time.time - lastJumpTime > .1f)
        {
            // switch to walk animation
            m_Animator.runtimeAnimatorController = Resources.Load(
                m_AnimPrefix + Constants.Walk) as RuntimeAnimatorController;
        }                             

        if (m_Grounded && jump)
        {
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            lastJumpTime = Time.time;
            m_Animator.runtimeAnimatorController = Resources.Load(
                m_AnimPrefix + Constants.Jump) as RuntimeAnimatorController;                
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
