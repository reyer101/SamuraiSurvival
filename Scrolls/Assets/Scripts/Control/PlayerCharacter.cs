using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/*
    Name: Alec Reyerson
    ID: 1826582
    Email: reyer101@mail.chapman.edu
    Course: CPSC-344-01
    Assignment: Gold Milestone

    Description: Script for performing actions based on character input.
    */

// PlayerCharacter
public class PlayerCharacter : MonoBehaviour {
    public GameObject m_LevitateTarget, m_Witch, m_Wizard;
    public LinkedList<GameObject> m_LevitateTargets;    
    public float m_MaxSpeed, m_ClimbSpeed, m_CrouchSpeed, m_JumpForce, 
        m_FireSpellCD, m_LevitateRadius, m_LevitateSpeed;
    public int HP;
    public bool m_DropWhenOutOfRange, m_CanLevitateAndMove;    
    private bool m_Grounded, m_CanClimb, m_HasSpell, m_LeviateDisabled, m_Crouched;    
    private AudioSource m_Audio;
    private Animator m_Animator;  
    private Rigidbody2D m_Rigidbody2D;
    private BoxCollider2D[] m_Colliders;
    private Transform m_GroundCheck, m_ClimbCheck;
    private CircleCollider2D m_CircleCollider2D;
    private LayerMask m_LayerMask;    
    private Vector3 m_SpellSpawnPosition;
    private Vector2 m_NormalSize, m_CrouchSize, m_CrouchGroundCheck, m_WalkGroundCheck;
    private Quaternion m_ForwardRotation, m_BackRotation;
    private Color m_Highlight;
    private Text m_HPText, m_CDText, m_NameText;
    private Image m_FireIcon, m_LevitationIcon, m_CDWheel;

    private LinkedList<string> m_SpellList;
    private String m_AnimPrefix, scene;  
    private int currentSpellIdx, spriteIndex, targetIndex;
    private float lastFireSpellTime, lastLevitateTime, lastToggleTime, lastJumpTime;    
    private float k_GroundedRadius = .5f;   
    private float k_ClimbRadius = 1.0f;
    private float k_UnderRadius = 1f;
    private float m_LevitateCD = 1f;
    public float k_GroundDistance;
         

    // Awake
    void Awake () {                            
        m_Crouched = false;
        m_HasSpell = false;        
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Colliders = GetComponents<BoxCollider2D>();
        m_Audio = GetComponent<AudioSource>();
        m_Animator = GetComponent<Animator>();
        m_GroundCheck = transform.Find("GroundCheck");
        m_Witch = GameObject.FindGameObjectWithTag("WitchToggle");
        m_Wizard = GameObject.FindGameObjectWithTag("WizardToggle");
        m_NameText = GameObject.FindGameObjectWithTag("NameText").GetComponent<Text>();
        m_NameText.text = PlayerPrefs.GetString("Name");               
        m_HPText = GameObject.FindGameObjectWithTag("HPText").GetComponent<Text>();
        m_HPText.text = "HP: " + HP;        
        m_CDWheel = GameObject.FindGameObjectWithTag(
            "CDWheel").transform.Find("CooldownBar").GetComponent<Image>();
        m_FireIcon = GameObject.FindGameObjectWithTag("FireIcon").GetComponent<Image>();
        m_LevitationIcon = GameObject.FindGameObjectWithTag("LevitationIcon").GetComponent<Image>();
        ColorUtility.TryParseHtmlString("#c156f7", out m_Highlight);
        m_ClimbCheck = transform.Find("ClimbCheck");
        m_NormalSize = m_Colliders[0].size;
        m_CrouchSize = new Vector2(m_Colliders[0].size.x, m_Colliders[0].size.y / 2f);
        m_SpellSpawnPosition = transform.Find("SpellSpawner").transform.position;
        m_ForwardRotation = transform.rotation;              
        m_BackRotation = new Quaternion(0, m_ForwardRotation.y - 1, 0, 0);          
        m_LayerMask = -1;
        m_WalkGroundCheck = m_GroundCheck.localPosition;
        m_CrouchGroundCheck = new Vector2(m_WalkGroundCheck.x, m_WalkGroundCheck.y + 2f);
        lastFireSpellTime = -100f;
        lastLevitateTime = -100f;
        lastToggleTime = -100f;
        lastJumpTime = Time.time;
        m_LevitateTargets = new LinkedList<GameObject>();        
        m_SpellList = new LinkedList<string>();              
        currentSpellIdx = 0;
        targetIndex = 0;       

        scene = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetString(Constants.Scene, scene);
        Debug.Log("Scene: " + scene);
        if(scene == "Scolls_Tutorial2")
        {
            m_SpellList.AddLast("Fire");            
            m_LevitationIcon.enabled = false;
            m_HasSpell = true;
        } 
        else if(scene == "1-1KH" || scene == "BossFight")
        {
            m_SpellList.AddLast("Fire");
            m_SpellList.AddLast("Earth");
            m_FireIcon.enabled = false;
            m_HasSpell = true;
        }
        else
        {
            m_LevitationIcon.enabled = false;
            m_FireIcon.enabled = false;
        }

        m_NameText.text = PlayerPrefs.GetString("Name"); 

        // change sprites and animations based on witch or wizard
        if(PlayerPrefs.GetInt("Sprite") == 0)
        {
            m_Witch.SetActive(false);
            m_AnimPrefix = Constants.BoyPrefix;            
        }
        else
        {
            m_Wizard.SetActive(false);
            m_AnimPrefix = Constants.GirlPrefix;           
        }        

        if(scene == "1-1KH")
        {
            if(PlayerPrefs.GetFloat(Constants.CheckpointX, 0) != 0)
            {
                gameObject.transform.position = new Vector2(
                    PlayerPrefs.GetFloat(Constants.CheckpointX),
                    PlayerPrefs.GetFloat(Constants.CheckpointY));
                Debug.Log("X: " + PlayerPrefs.GetFloat(Constants.CheckpointX));
                Debug.Log("Y: " + PlayerPrefs.GetFloat(Constants.CheckpointY));
            }
        }

        m_Animator.runtimeAnimatorController = Resources.Load(
                       m_AnimPrefix + Constants.Walk) as RuntimeAnimatorController;
    }
	
	// FixedUpdate
	void FixedUpdate () {        
        m_Grounded = false;
        m_CanClimb = false;
        m_LeviateDisabled = false;       
        m_Rigidbody2D.gravityScale = 2;
        checkCanLevitateVert();
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

        Collider2D[] cColliders = Physics2D.OverlapCircleAll(m_ClimbCheck.position, k_ClimbRadius, m_LayerMask);
        for (int i = 0; i < cColliders.Length; i++)
        {          
            if (cColliders[i].gameObject.tag.Contains("Climb"))
            {                  
                m_CanClimb = true;
                m_Rigidbody2D.gravityScale = 0;
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

        if (m_HasSpell)
        {
            float cd = 1;
            switch (m_SpellList.ElementAt(currentSpellIdx))
            {
                case "Fire":
                    m_CDWheel.color = new Color32(0xFF, 0, 0, 0xFF);
                    m_LevitationIcon.enabled = false;
                    m_FireIcon.enabled = true;
                    float fireCD = m_FireSpellCD - (Time.time - lastFireSpellTime);
                    if (fireCD > 0) 
                    {
                        cd = (m_FireSpellCD - fireCD) / m_FireSpellCD;                        
                    }                    
                    break;
                case "Earth":
                    m_CDWheel.color = new Color32(0, 0, 0xFF, 0xFF);
                    m_LevitationIcon.enabled = true;
                    m_FireIcon.enabled = false;
                    float levCD = m_LevitateCD - (Time.time - lastLevitateTime);
                    if (levCD > 0)
                    {
                        cd = (m_LevitateCD - levCD) / m_LevitateCD;
                    }                    
                    break;
            }
            m_CDWheel.fillAmount = cd;
        }        
    }  

    /*
    Name: Move
    Parameters: float horizontal, bool jump, bool crouch
    */
    public void Move(float horizontal, bool jump, bool crouch)
    {
        m_Crouched = crouch;
        if(m_CanLevitateAndMove || (!m_CanLevitateAndMove && m_LevitateTarget == null))
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
            
            if(Mathf.Abs(horizontal) < .1)
            {                
                // Prevents the player from being pushed by levitating object
                m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX
                      | RigidbodyConstraints2D.FreezeRotation;
            }            

            if(horizontal <= 0 && m_Rigidbody2D.velocity.x > 0
                || horizontal >= 0 && m_Rigidbody2D.velocity.x < 0)
            {
                m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX
                     | RigidbodyConstraints2D.FreezeRotation;
            }            

            if (!crouch)
            {              
                m_Rigidbody2D.velocity = new Vector2(
                    horizontal * m_MaxSpeed, m_Rigidbody2D.velocity.y);
                m_GroundCheck.localPosition = m_WalkGroundCheck;

                if(m_Grounded && Time.time - lastJumpTime > .1f)
                {
                    // switch to walk animation
                    m_Animator.runtimeAnimatorController = Resources.Load(
                       m_AnimPrefix + Constants.Walk) as RuntimeAnimatorController;
                }               

                foreach (BoxCollider2D collider in m_Colliders)
                {
                    collider.size = m_NormalSize;
                }                 
            }
            else
            {                
                m_Animator.runtimeAnimatorController = Resources.Load(
                   m_AnimPrefix + Constants.Crouch) as RuntimeAnimatorController;
                m_Rigidbody2D.velocity = new Vector2(horizontal * m_CrouchSpeed, m_Rigidbody2D.velocity.y);
                m_GroundCheck.localPosition = m_CrouchGroundCheck;

                foreach (BoxCollider2D collider in m_Colliders)
                {
                    collider.size = m_CrouchSize;
                }
            }        

            if (m_Grounded && jump && !m_Crouched)
            {
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                lastJumpTime = Time.time;
                m_Animator.runtimeAnimatorController = Resources.Load(
                    m_AnimPrefix + Constants.Jump) as RuntimeAnimatorController;                
            }
        }
        else
        {
            m_Rigidbody2D.velocity = Vector2.zero;
        }        
    }

    /*
    Name: Climb
    Parameters: float vertical
    */
    public void Climb(float vertical)
    {        
        if (m_CanClimb)
        {            
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, vertical * m_ClimbSpeed);            
        }
    }

    /*
    Name: MoveLevitationTarget
    Parameters: float h, float v
    */
    public void MoveLevitationTarget(float h, float v)
    {                
        if(m_LevitateTarget != null && !m_LeviateDisabled)
        {            
            Rigidbody2D rb = m_LevitateTarget.GetComponent<Rigidbody2D>();
            Vector3 direction = new Vector3(h, -v, 0);
            if (!(Mathf.Abs(m_LevitateTarget.transform.position.y - transform.position.y) >= m_LevitateRadius
                || Mathf.Abs(m_LevitateTarget.transform.position.x - transform.position.x) >= m_LevitateRadius))
            {
                if(m_Grounded)
                {
                    rb.velocity = direction * 20f * m_LevitateSpeed * Time.deltaTime;                    
                }                                                                
            }
            else
            {
                rb.velocity = Vector2.zero;
                Vector3 position1 = m_LevitateTarget.transform.position;
                Vector3 position2 = position1 - .01f * (position1 - transform.position);
                m_LevitateTarget.transform.position = position2;                                
                if (m_DropWhenOutOfRange)
                {
                    rb.gravityScale = 1;
                    m_LevitateTarget.GetComponent<SpriteRenderer>().material.color = Color.white;                    
                    m_LevitateTarget = null;
                }                
            }
        }            
    }
    
    // castSpell
    public void castSpell()
    {
        if(m_SpellList.Count > 0)
        {
            switch (m_SpellList.ElementAt(currentSpellIdx))
            {
                case "Fire":
                    if ((Time.time - lastFireSpellTime) > m_FireSpellCD)
                    {
                        Quaternion spawnRotation = new Quaternion(0, transform.rotation.y, 0, 0);
                        GameObject spell = (GameObject)Instantiate(Resources.Load(
                            "Spells/FireSpell"), m_SpellSpawnPosition, spawnRotation);
                        lastFireSpellTime = Time.time;
                        m_Audio.clip = (AudioClip)Resources.Load(Constants.FireSpellAudio);
                        m_Audio.Play();
                    }
                    break;
                case "Earth":                    
                    if((Time.time - lastLevitateTime) > m_LevitateCD)
                    {
                        if(m_LevitateTarget == null)
                        {
                            m_Audio.clip = (AudioClip)Resources.Load("Audio/Levitate");
                            m_Audio.Play();
                            findLevitateTargets();                            
                        }
                        else
                        {                            
                            m_LevitateTarget.GetComponent<Rigidbody2D>().gravityScale = 1;
                            m_LevitateTarget.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                            m_LevitateTarget.GetComponent<SpriteRenderer>().material.color = Color.white;                            
                            m_LevitateTarget = null;
                            m_LevitateTargets.Clear();
                            targetIndex = 0;
                            lastLevitateTime = Time.time;                            
                        }                        
                    }                    
                    break;                 
            }
        }          
    }   

    // toggleSpell
    public void toggleSpell()
    {
        if((Time.time - lastToggleTime) > .1f && m_SpellList.Count != 0)
        {
            if(currentSpellIdx + 1 <= m_SpellList.Count -1)
            {
                ++currentSpellIdx;
            }
            else
            {
                currentSpellIdx = 0;
                if(m_LevitateTarget != null)
                {
                    m_LevitateTarget.GetComponent<Rigidbody2D>().gravityScale = 1;
                    m_LevitateTarget.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    m_LevitateTarget.GetComponent<SpriteRenderer>().material.color = Color.white;
                    m_LevitateTarget = null;
                    m_LevitateTargets.Clear();
                    targetIndex = 0;
                }                           
            }            
        }
        lastToggleTime = Time.time;        
    }

    // toggleTarget
    public void toggleTarget()
    {
        if(m_LevitateTargets.Count > 0)
        {
            if(m_LevitateTarget != null)
            {
                m_LevitateTarget.GetComponent<Rigidbody2D>().gravityScale = 1;
                m_LevitateTarget.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                m_LevitateTarget.GetComponent<SpriteRenderer>().material.color = Color.white;               
            }
            else
            {
                m_LevitateTargets.Remove(m_LevitateTarget);
            }                               

            try
            {
                m_LevitateTargets.ElementAt(targetIndex).GetComponent<Rigidbody2D>().constraints
                       &= ~RigidbodyConstraints2D.FreezePositionY;
                m_LevitateTarget = m_LevitateTargets.ElementAt(targetIndex + 1); 
                if(m_LevitateTarget != null)
                {
                    m_LevitateTarget.GetComponent<Rigidbody2D>().gravityScale = 0;
                    m_LevitateTarget.GetComponent<SpriteRenderer>().material.color = m_Highlight;
                    targetIndex = targetIndex + 1;
                } 
                else
                {
                    toggleTarget();
                }                 
            }
            catch(ArgumentOutOfRangeException e)
            {                
                targetIndex = 0;      
                if(m_LevitateTargets.Count > 0)
                {
                    m_LevitateTarget = m_LevitateTargets.ElementAt(targetIndex);
                }
                else
                {
                    return;
                }                          
                
                if (m_LevitateTarget != null)
                {
                    m_LevitateTarget.GetComponent<Rigidbody2D>().gravityScale = 0;
                    m_LevitateTarget.GetComponent<SpriteRenderer>().material.color = m_Highlight;
                }
                else
                {
                    toggleTarget();
                }               
            }

            if (Mathf.Abs(m_LevitateTarget.transform.position.y - transform.position.y) >= m_LevitateRadius
                || Mathf.Abs(m_LevitateTarget.transform.position.x - transform.position.x) >= m_LevitateRadius)
            {                
                m_LevitateTargets.Remove(m_LevitateTarget);
                m_LevitateTarget.GetComponent<Rigidbody2D>().gravityScale = 1;
                m_LevitateTarget.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                m_LevitateTarget.GetComponent<SpriteRenderer>().material.color = Color.white;
                toggleTarget();
            }
        }
        else
        {
            m_LevitateTarget = null;
        }       
    }

    // checkCanLevitateVert
    private void checkCanLevitateVert()
    {
        if(m_LevitateTarget != null)
        {
            m_LevitateTarget.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            Vector2 checkPosition = m_LevitateTarget.transform.position + new Vector3(0, 1.4f);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPosition, k_UnderRadius, m_LayerMask);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject != m_LevitateTarget.gameObject && (collider.gameObject.tag == "Liftable"
                    || collider.gameObject.tag == "Player"))
                {                    
                    m_LevitateTarget.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY
                       | RigidbodyConstraints2D.FreezeRotation;
                }
            }            
        }
    }

    // findLevitateTargets
    public void findLevitateTargets()
    {
        m_LevitateTargets = new LinkedList<GameObject>();
        targetIndex = 0;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
                                transform.position, m_LevitateRadius, m_LayerMask);
        for (int i = 0; i < colliders.Length; ++i)
        {
            if (colliders[i].gameObject.tag == "Liftable"
                && !m_LevitateTargets.Contains(colliders[i].gameObject))
            {
                m_LevitateTargets.AddLast(colliders[i].gameObject);
                lastLevitateTime = Time.time;
            }
        }

        if (m_LevitateTargets.Count > 0)
        {
            m_LevitateTarget = m_LevitateTargets.ElementAt(targetIndex);
            m_LevitateTarget.GetComponent<Rigidbody2D>().gravityScale = 0;
            m_LevitateTarget.GetComponent<SpriteRenderer>().material.color = m_Highlight;
        }
    }

    // takeDamage
    void takeDamage()
    {
        HP -= 1;
        m_HPText.text = "HP: " + HP;       
        if (HP == 0)
        {            
            SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);            
        }
    }
    
    // OnTriggerEnter2D
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.Contains("EnemyProjectile"))
        {
            takeDamage();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.name.Contains("Hair"))
        {
            Vector2 hairballVelocity = other.gameObject.GetComponent<Rigidbody2D>().velocity;
            float maxVelocity = Mathf.Max(
                Mathf.Abs(hairballVelocity.x), Mathf.Abs(hairballVelocity.y));
            
            if(maxVelocity > 6f)
            {
                takeDamage(); 
                Destroy(other.gameObject);
            }
        }
        else if(other.gameObject.tag == "Ghost")
        {
            takeDamage();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.name == "FireScroll")
        {
            m_HasSpell = true;
            Destroy(other.gameObject);
            m_SpellList.AddLast("Fire");
            currentSpellIdx = 0;            
        }
        else if (other.gameObject.name == "EarthScroll")
        {
            Destroy(other.gameObject);
            m_SpellList.AddLast("Earth");            
            currentSpellIdx = 1;
        }
    }  
}
