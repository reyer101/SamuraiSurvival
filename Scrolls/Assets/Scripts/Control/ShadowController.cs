using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShadowController : MonoBehaviour
{
	private GameObject m_PlayerObject;
	private ShadowCharacter m_Shadow;
	private Vector3 m_PlayerDirection;
	private bool m_CanMove = true;

	private float playerDistanceX, playerDistanceY;

	// Percent chance to attack when in range
	[Range(0, 100f)]
	public float m_AttackChance;

	[Range(0, 100f)] 
	public float m_VulnerableChance;

	[Range(0, 10f)]
	public float m_ReadyRange;

	public float m_BlockDuration;

	[Range(0, 4)] 
	public int m_MaxAttackChain;

	//private float lastChoiceTime = -99f, lastSwordHitTime = -99f, choiceDelay;

	private int attackChain;
	private bool m_LastBlock, m_LastVulnerable;

	// Start
	void Start ()
	{
		m_Shadow = GetComponent<ShadowCharacter>();
		m_PlayerObject = GameObject.FindGameObjectWithTag("Player");
	}

	// Fixed Update
	void FixedUpdate()
	{
		// do nothing if dead
		if (m_Shadow.IsDead()) {
			return;
		}

		// calculate player direction and distance
		m_PlayerDirection = m_PlayerObject.transform.position 
			- transform.position;
		playerDistanceX = Math.Abs(m_PlayerDirection.x);
		playerDistanceY = Math.Abs (m_PlayerDirection.y);

		float horizontal = 0;
		bool attack = false, block = false, vulnerable = false;

		if (playerDistanceX >= m_ReadyRange)
		{
			// normalize the direction vector for velocity
			m_CanMove = true;
			horizontal = (float) Math.Round(m_PlayerDirection.normalized.x);
		}
		else
		{
			m_CanMove = false;
			if (m_Shadow.IsBlocking() || m_Shadow.IsAttacking() 
					|| m_Shadow.isVulnerable())
			{
				return;
			}

			if (!m_LastVulnerable && Random.Range(0f, 100f) <= 
				m_VulnerableChance)
			{
				vulnerable = true;
			}

			m_LastVulnerable = false;

			if (!m_Shadow.isVulnerable())
			{
				attack = true;
				float rand = Random.Range(0f, 100f);
				if (rand >= m_AttackChance && playerDistanceY < 4f 
					&& !m_LastBlock || attackChain == m_MaxAttackChain)
				{
					block = true;
					attack = false;
				}
			}
		}

		m_Shadow.Attack(attack, block, vulnerable);
		m_Shadow.Move(horizontal);
	}

	// GetPlayerDirection
	public Vector3 GetPlayerDirection() {
		return m_PlayerDirection;
	}
}
