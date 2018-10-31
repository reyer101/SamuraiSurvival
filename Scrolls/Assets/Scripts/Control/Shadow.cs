using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
	private GameObject m_Player;
	private SpriteRenderer m_SpriteRenderer;
	private Rigidbody2D m_Rigidbody2D;
	private AudioSource m_Audio;
	private Animator m_Animator;
	private Quaternion m_ForwardRotation, m_BackRotation;
	private String[] m_Attacks;
	
	public float m_MaxSpeed, m_AttackDelay;
	private float baseSpeed, blockSpeed = 0f,
		lastAttackTime = -999f;

	private int m_AttackIdx = 0, m_AttackSoundIdx;
	private bool m_Attacking;

	void Start ()
	{
		m_Player = GameObject.FindGameObjectWithTag("Player");
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		m_Audio = GetComponent<AudioSource>();
		m_Animator = GetComponent<Animator>();
		m_Attacks = new[] {Constants.ANIM_ATTACK1, Constants.ANIM_ATTACK2};
		
		m_ForwardRotation = transform.rotation;              
		m_BackRotation = new Quaternion(0, m_ForwardRotation.y - 1, 0, 0);
	}
	
	void Update () {
		
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
	
	// stopAttacking
	private void StopAttacking()
	{
		m_Attacking = false;
	}
}
