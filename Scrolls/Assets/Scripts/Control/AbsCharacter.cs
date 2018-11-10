using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsCharacter : MonoBehaviour {
	
	protected int m_AttackIdx, m_AttackSoundIdx, m_BlockSoundIdx;
	
	protected GameObject m_HealthBar, m_HealthForeground;
	protected SpriteRenderer m_SpriteRenderer;
	protected Rigidbody2D m_Rigidbody2D;
	protected AudioSource m_Audio;
	protected Animator m_Animator;
	protected Quaternion m_ForwardRotation, m_BackRotation;
	protected Color m_SpriteColor;
	protected readonly string[] m_Attacks = {Constants.ANIM_ATTACK1, 
		Constants.ANIM_ATTACK2}; 
	protected string m_Anim = "";
	protected bool m_Attacking, m_Blocking, m_Dead, dim;
	protected float lastAttackTime = -999f, baseSpeed, lastDamageTime = -999f,
		blockSpeed, initialHealthScale, maxHp; 
	
	public float m_MaxSpeed, m_AnimationSpeed, m_AttackDelay;
	
	[Range(1, 50)]
	public float m_HP;
	
	[Range(0, 10f)]
	public float m_AttackRange;

	// Awake
	void Awake () {
		m_HealthBar = transform.Find(Constants.OBJECT_HEALTHBAR).gameObject;
		m_HealthForeground = m_HealthBar.transform.Find(Constants
			.OBJECT_HEALTHFOREGROUND).gameObject;
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		m_SpriteColor = m_SpriteRenderer.color;
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_Audio = GetComponent<AudioSource>();
		m_Animator = GetComponent<Animator>();
		m_ForwardRotation = transform.rotation;              
		m_BackRotation = new Quaternion(0, m_ForwardRotation.y - 1, 0, 0);

		baseSpeed = m_MaxSpeed;
		initialHealthScale = m_HealthForeground.transform.localScale.x;
		maxHp = m_HP;
	}

	// FixedUpdate
	protected void FixedUpdate()
	{
		if (Time.time - lastDamageTime > 3f)
		{
			m_HealthBar.SetActive(false);
		}
	}

	// LateUpdate
	protected void LateUpdate()
	{
		float r, g, b, a;
		Color color;
		switch (m_Anim)
		{
			case "Pulse":
				a = m_SpriteRenderer.color.a;
				if (a >= m_SpriteColor.a)
				{
					dim = true;
				}

				if (a <= .6f)
				{
					dim = false;
				}

				color = m_SpriteColor;
				r = color.r;
				g = color.g;
				b = color.b;
				if (dim)
				{
					m_SpriteRenderer.color = new Color(r, g, b,
						a - a * Time.deltaTime * 5f);
				}
				else
				{
					m_SpriteRenderer.color = new Color(r, g, b,
						a + a * Time.deltaTime * 5f);
				}

				break;
		}
	}
	
	// ProcessAttack
	public void ProcessAttack(bool swordThrow)
	{
		// do nothing if already dead
		if (m_Dead)
		{
			return;
		}
		
		string clip;
		float hpLoss = swordThrow ? .5f : 1f;
		if (!m_Blocking) 
		{
			// lose hp and set impact clip path
			m_HP -= hpLoss;
			clip = Constants.CLIP_IMPACT;
			lastDamageTime = Time.time;
			m_HealthBar.SetActive(true);
			DropHealthBar();
			if (m_HP < .5f)
			{
				ProcessDead();
			}
			else
			{
				m_Anim = Constants.ANIM_SHADOW_PULSE;
				Invoke("StopAnim", .25f);
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
	
	// ProcessDead
	protected abstract void ProcessDead();
	
	// StopAnim
	void StopAnim()
	{
		m_Anim = "";
		m_SpriteRenderer.color = m_SpriteColor;
	}

	// StopAttacking
	void StopAttacking()
	{
		m_Attacking = false;
	}
	
	// DropHealthBar
	protected void DropHealthBar()
	{
		Vector3 healthScale = m_HealthForeground.transform.localScale;
		healthScale = new Vector3((initialHealthScale * m_HP) / maxHp,
			healthScale.y);
		m_HealthForeground.transform.localScale = healthScale;

		lastDamageTime = Time.time;
		m_HealthBar.SetActive(true);
	}
}
