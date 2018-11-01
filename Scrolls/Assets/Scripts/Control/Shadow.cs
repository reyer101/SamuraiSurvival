using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shadow : MonoBehaviour
{
	private GameObject m_Player;
	private SpriteRenderer m_SpriteRenderer;
	private Rigidbody2D m_Rigidbody2D;
	private AudioSource m_Audio;
	private Animator m_Animator;
	private Quaternion m_ForwardRotation, m_BackRotation;
	private Vector3 m_PlayerDirection;
	private String[] m_Attacks;

	// Percent chance to attack when in range
	[Range(0, 100f)]
	public float m_AttackChance; 
	public float m_MaxSpeed, m_AttackDelay, m_BlockDelay;
	private float baseSpeed, playerDistanceX, playerDistanceY,
		blockSpeed = 0f, lastAttackTime = -999f;

	private int m_AttackIdx = 0, m_AttackSoundIdx;
	private bool m_Attacking, m_Blocking;

	void Start ()
	{
		m_Player = GameObject.FindGameObjectWithTag("Player");
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_Audio = GetComponent<AudioSource>();
		m_Animator = GetComponent<Animator>();
		m_Attacks = new[] {Constants.ANIM_ATTACK1, Constants.ANIM_ATTACK2};
		
		m_ForwardRotation = transform.rotation;              
		m_BackRotation = new Quaternion(0, m_ForwardRotation.y - 1, 0, 0);
	}
	
	void Update ()
	{
		// calculate player direction and distance
		m_PlayerDirection = m_Player.transform.position - transform.position;
		playerDistanceX = Math.Abs(m_PlayerDirection.x);
		playerDistanceY = Math.Abs(m_PlayerDirection.y);

		float horizontal = 0;
		bool attack = false, block = false;
		
		m_Animator.speed = 0;
		if (playerDistanceX > 4f)
		{
			// normalize the direction vector for velocity
			horizontal = (float) Math.Round(m_PlayerDirection.normalized.x);
			m_Animator.speed = Mathf.Abs(m_Rigidbody2D.velocity.x) / 8f;
		}
		else
		{
			attack = true;
			if (Random.Range(0f, 100f) >= m_AttackChance 
					&& playerDistanceY < 4f)
			{
				block = true;
				attack = false;
			}
		}
		Move(horizontal);
	}
	
	/*
    Name: Move
    Parameters: float horizontal
    */
	public void Move(float horizontal)
	{
		//Debug.Log("Horizontal: " + horizontal);
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

		// default no animation 
		String animPath = Constants.ANIM_EMPTY;
		if(!m_Attacking) 
		{
			m_Animator.runtimeAnimatorController = Resources.Load(
				Constants.ANIM_WALK) as RuntimeAnimatorController;
		}             
	}  
	
	/*
    Name: Attack
    Parameters: bool attack, bool block
    */
	public void Attack(bool attack, bool block)
	{		
		m_MaxSpeed = baseSpeed;
		if (block)
		{
			m_Animator.runtimeAnimatorController = Resources.Load(
				Constants.ANIM_EMPTY) as RuntimeAnimatorController;
			m_SpriteRenderer.sprite = Resources.Load<Sprite>(
				Constants.SPRITE_BLOCK);
			m_MaxSpeed = blockSpeed;

			return;
		}

		if (attack && Time.time - lastAttackTime > m_AttackDelay)
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

			Invoke("StopAttacking", m_AttackDelay);
		}
	}
	
	// StopAttacking
	private void StopAttacking()
	{
		m_Attacking = false;
	}
	
	// StopBlocking
	private void StopBlocking()
	{
		m_Blocking = false;
	}
}
