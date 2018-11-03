using System;
using System.Xml.Serialization;
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

	[Range(0, 100f)] 
	public float m_VulnerableChance;

	[Range(0, 4f)] 
	public float m_VulnerableDuration;

	[Range(0, 10f)]
	public float m_AttackRange;
	
	public float m_MaxSpeed, m_AttackDelay, m_BlockDuration;

	[Range(0, 4)] 
	public int m_MaxAttackChain;
	
	private float baseSpeed, playerDistanceX, playerDistanceY,
		blockSpeed = 0f, lastAttackTime = -999f, lastChoiceTime = -99f,
		choiceDelay;

	private int m_AttackIdx = 0, m_AttackSoundIdx, attackChain = 0;
	private bool m_Attacking, m_Blocking, m_LastBlock, m_Vulnerable,
		m_LastVulnerable;

	void Start ()
	{
		m_Player = GameObject.FindGameObjectWithTag("Player");
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_Audio = GetComponent<AudioSource>();
		m_Animator = GetComponent<Animator>();
		m_Attacks = new[] {Constants.ANIM_ATTACK1, Constants.ANIM_ATTACK2};
		baseSpeed = m_MaxSpeed;
		choiceDelay = m_AttackDelay;
		
		m_ForwardRotation = transform.rotation;              
		m_BackRotation = new Quaternion(0, m_ForwardRotation.y - 1, 0, 0);
	}
	
	void FixedUpdate ()
	{
		// calculate player direction and distance
		m_PlayerDirection = m_Player.transform.position - transform.position;
		playerDistanceX = Math.Abs(m_PlayerDirection.x);
		playerDistanceY = Math.Abs(m_PlayerDirection.y);

		float horizontal = 0;
		bool attack = false, block = false;
		
		if (playerDistanceX >= m_AttackRange)
		{
			// normalize the direction vector for velocity
			horizontal = (float) Math.Round(m_PlayerDirection.normalized.x);
			m_Animator.speed = Mathf.Abs(m_Rigidbody2D.velocity.x) / 8f;
		}
		else
		{
			if (m_Blocking || m_Attacking || m_Vulnerable)
			{
				return;
			}
			
			if (!m_LastVulnerable && Random.Range(0f, 100f) <= 
					m_VulnerableChance)
			{
				m_Vulnerable = true;
				m_LastVulnerable = true;
				attackChain = 0;
				m_LastBlock = false;
				m_SpriteRenderer.sprite = Resources.Load<Sprite>(
					Constants.SPRITE_VULNERABLE);
				Invoke("StopVulnerability", m_VulnerableDuration);

				return;
			}

			m_LastVulnerable = false;
			
			if (Time.time - lastChoiceTime >= choiceDelay && !m_Vulnerable)
			{
				attack = true;
				m_Animator.speed = .5f;
				float rand = Random.Range(0f, 100f);
				if (rand >= m_AttackChance && playerDistanceY < 4f 
					&& !m_LastBlock || attackChain == m_MaxAttackChain)
				{
					m_Animator.speed = 0;
					choiceDelay = m_BlockDuration;
					block = true;
					attack = false;
				}
				lastChoiceTime = Time.time;
			}
		}
		
		Attack(attack, block);
		Move(horizontal);
	}
	
	/*
    Name: Move
    Parameters: float horizontal
    */
	public void Move(float horizontal)
	{
		if (horizontal < 0)
		{
			transform.rotation = m_BackRotation;
		}
		else if (horizontal > 0)
		{
			transform.rotation = m_ForwardRotation;
		}

		if (!m_Blocking && !m_Attacking && Time.time - choiceDelay - .5f > lastChoiceTime)
		{
			m_Rigidbody2D.velocity = new Vector2(
				horizontal * m_MaxSpeed * 10f, m_Rigidbody2D.velocity.y);
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
		if (block || m_Blocking && !m_Attacking)
		{
			m_Animator.runtimeAnimatorController = Resources.Load(
				Constants.ANIM_EMPTY) as RuntimeAnimatorController;
			m_SpriteRenderer.sprite = Resources.Load<Sprite>(
				Constants.SPRITE_BLOCK);
			m_MaxSpeed = blockSpeed;
			m_Blocking = true;

			Debug.Log("Block");
			attackChain = 0;
			m_LastBlock = true;
			choiceDelay = m_BlockDuration;
			Invoke("StopBlocking", m_BlockDuration);

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
			
			Debug.Log("Attack");
			++attackChain;
			m_LastBlock = false;
			choiceDelay = m_AttackDelay;
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
	
	// StopVulnerability
	private void StopVulnerability()
	{
		m_Vulnerable = false;
	}
}