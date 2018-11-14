using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShadowCharacter : AbsCharacter
{
	private PlayerCharacter m_Player;
	private ShadowController m_ShadowController;

	// Percent chance to attack when in range
	[Range(0, 100f)]
	public float m_AttackChance;

	[Range(0, 100f)] 
	public float m_VulnerableChance;

	[Range(0, 4f)] 
	public float m_VulnerableDuration;

	[Range(0, 10f)]
	public float m_ReadyRange;
	
	public float m_BlockDuration;

	[Range(0, 4)] 
	public int m_MaxAttackChain;
	
	private float lastChoiceTime = -99f, lastSwordHitTime = -99f, choiceDelay;

	private int attackChain;
	private bool m_LastBlock, m_Vulnerable, m_LastVulnerable;

	void Start ()
	{
		m_Player = GameObject.FindGameObjectWithTag("Player")
			.GetComponent<PlayerCharacter>();
		m_ShadowController = GetComponent<ShadowController>();
		m_DamageColor = m_SpriteColor;
		choiceDelay = m_AttackDelay;
	}
	
	void FixedUpdate ()
	{
		base.FixedUpdate();

		//float horizontal = 0;
		//bool attack = false, block = false, vulnerable = false;
		
		// always face the player's direction
		if (m_ShadowController.GetPlayerDirection().x < 0)
		{
			transform.rotation = m_BackRotation;
			m_HealthBar.transform.localRotation = m_BackRotation;
		}
		else
		{
			transform.rotation = m_ForwardRotation;
			m_HealthBar.transform.localRotation = m_ForwardRotation;
		}
		
		/*if (playerDistanceX >= m_ReadyRange)
		{
			// normalize the direction vector for velocity
			m_CanMove = true;
			m_MaxSpeed = baseSpeed;
			horizontal = (float) Math.Round(m_PlayerDirection.normalized.x);
			m_Animator.speed = Mathf.Abs(m_Rigidbody2D.velocity.x) / 8f;
		}
		else
		{
			m_CanMove = false;
			m_Animator.speed = .5f;
			if (m_Blocking || m_Attacking || m_Vulnerable)
			{
				return;
			}
			
			if (!m_LastVulnerable && Random.Range(0f, 100f) <= 
					m_VulnerableChance)
			{
				vulnerable = true;
			}

			m_LastVulnerable = false;
			
			if (Time.time - lastChoiceTime >= choiceDelay 
					&& !m_Vulnerable)
			{
				attack = true;
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
		
		Attack(attack, block, vulnerable);
		Move(horizontal);*/
	}
	
	// LateUpdate
	void LateUpdate()
	{
		base.LateUpdate();
		float r, g, b, a;
		Color color;
		switch (m_Anim)
		{
			case "Fade":
				color = m_SpriteColor;
				a = m_SpriteRenderer.color.a;
				r = color.r;
				g = color.g;
				b = color.b;
				
				m_SpriteRenderer.color = new Color(r, g, b,
					a - a * Time.deltaTime * .75f);

				if (m_SpriteRenderer.color.a <= .1)
				{
					DestroyImmediate(gameObject);
				}
				
				break;
		}
	}

	/*
    Name: Move
    Parameters: float horizontal
    */
	public void Move(float horizontal)
	{
		m_Rigidbody2D.velocity = new Vector2(
			horizontal * m_MaxSpeed * 10f, m_Rigidbody2D.velocity.y);
		m_Animator.runtimeAnimatorController = Resources.Load(
			Constants.ANIM_WALK) as RuntimeAnimatorController;
		m_Animator.speed = Mathf.Abs(m_Rigidbody2D.velocity.x) / 100f;
	}  
	
	/*
    Name: Attack
    Parameters: bool attack, bool block
    */
	public void Attack(bool attack, bool block, bool vulnerable)
	{
		if (m_Attacking || m_Blocking || m_Vulnerable)
		{
			return;
		}
		
		if (vulnerable)
		{
			m_Vulnerable = true;
			m_LastVulnerable = true;
			attackChain = 0;
			m_LastBlock = false;
			m_Animator.runtimeAnimatorController = Resources.Load(
				Constants.ANIM_EMPTY) as RuntimeAnimatorController;
			m_SpriteRenderer.sprite = Resources.Load<Sprite>(
				Constants.SPRITE_VULNERABLE);
			m_MaxSpeed = blockSpeed;
			Invoke("StopVulnerability", m_VulnerableDuration);

			return;
		}
		
		m_MaxSpeed = baseSpeed;
		if (block)
		{
			m_Animator.runtimeAnimatorController = Resources.Load(
				Constants.ANIM_EMPTY) as RuntimeAnimatorController;
			m_SpriteRenderer.sprite = Resources.Load<Sprite>(
				Constants.SPRITE_BLOCK);
			m_MaxSpeed = blockSpeed;
			m_Blocking = true;

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
			
			// damage the player if they are in range
			StartCoroutine(DamagePlayer(new Vector2(), .2f));
			
			++attackChain;
			m_LastBlock = false;
			choiceDelay = m_AttackDelay;
			Invoke("StopAttacking", m_AttackDelay);
		}
	}
	
	// DamagePlayer
	IEnumerator DamagePlayer(Vector2 playerDistance, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (playerDistance.x <= m_AttackRange && playerDistance.y
		    <= m_AttackRange)
		{
			m_Player.ProcessAttack(false);
			m_Audio.Stop();
		}
	}

	// ProcessDead
	protected override void ProcessDead()
	{
		m_Dead = true;
		m_Animator.speed = 0;
		m_Anim = Constants.ANIM_SHADOW_FADE;	
	}

	// IsVulnerable
	public bool isVulnerable() {
		return m_Vulnerable;
	}

	// IsIdle
	public bool IsIdle() {
		return !m_Blocking && !m_Attacking && !m_Vulnerable;
	}

	// StopBlocking
	void StopBlocking()
	{
		m_Blocking = false;
	}
	
	// StopVulnerability
	void StopVulnerability()
	{
		m_Vulnerable = false;
	}
	
	// OnTriggerEnter2D
	private void OnTriggerEnter2D(Collider2D other)
	{
		// if hit by a thrown sword
		if (other.gameObject.CompareTag(Constants.TAG_SWORD) 
			&& Time.time - lastSwordHitTime >= .5f)
		{
			lastSwordHitTime = Time.time;
			ProcessAttack(true);
		}
	}
}