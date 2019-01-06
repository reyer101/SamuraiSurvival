using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Move = Constants.Move;

public class ShadowController : MonoBehaviour
{
	private GameObject m_PlayerObject;
	private ShadowCharacter m_Shadow;
	private Vector3 m_PlayerDirection;
	private List<int> m_MovePool; 

	private float playerDistanceX, playerDistanceY, moveSum, startTime;
	private bool m_LastBlock, m_LastVulnerable;

	// Percent chance to attack when in range
	[Range(0, 100)]
	public int m_AttackChance;

	[Range(0, 100)] 
	public int m_VulnerableChance;

	[Range(0, 100)]
	public int m_BlockChance;

	[Range(0, 10f)]
	public float m_AttackRange;

	public float m_BlockDuration;

	[Range(0, 4)] 
	public int m_MaxAttackChain;

	// Start
	void Start ()
	{
		m_Shadow = GetComponent<ShadowCharacter>();
		m_PlayerObject = GameObject.FindGameObjectWithTag("Player");
		moveSum = m_AttackChance + m_BlockChance + m_VulnerableChance;
		InitMovePool ();

		Debug.Log("After InitMovePool()");
		foreach (int move in m_MovePool) {
			Debug.Log ("Move: " + move);
		}
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

		//Debug.Log(String.Format("({0:F}, {1:F})", playerDistanceX, m_ReadyRange));
		if (playerDistanceX > m_AttackRange)
		{
			// normalize the direction vector for velocity
			horizontal = (float) Math.Round(m_PlayerDirection.normalized.x);
			GetComponent<ShadowCharacter> ().StopAction ();
		}
		else
		{
			if (!m_Shadow.IsIdle())
			{
				return;
			}
				
//			if (!m_LastVulnerable && Random.Range(0f, 100f) <= 
//				m_VulnerableChance)
//			{
//				vulnerable = true;
//				m_LastVulnerable = true;
//			}
//
//			if (!m_Shadow.isVulnerable())
//			{
//				m_LastVulnerable = false;
//				attack = true;
//				vulnerable = false;
//				block = false;
//				float rand = Random.Range(0f, 100f);
//				if (rand >= m_AttackChance
//					&& !m_LastBlock || m_Shadow.GetAttackChain() == m_MaxAttackChain)
//				{
//					m_LastBlock = true;
//					block = true;
//					attack = false;
//				}
//			}

			m_Shadow.Attack(ComputeMove());
		}
			
		m_Shadow.Move(horizontal);
	}

	// GetPlayerDirection
	public Vector3 GetPlayerDirection() {
		return m_PlayerDirection;
	}

	// InitMovePool
	private void InitMovePool() {
		m_MovePool = new List<int>();

		// add all basic attack moves
		for (int i = 0; i < m_AttackChance; ++i) {
			m_MovePool.Add ((int) Move.MOVE_ATTACK);
		}

		// add all block moves
		for (int i = 0; i < m_BlockChance; ++i) {
			m_MovePool.Add ((int) Move.MOVE_BLOCK);
		}

		// add all powerup moves
		for (int i = 0; i < m_VulnerableChance; ++i) {
			m_MovePool.Add ((int) Move.MOVE_POWERUP);
		}
	}

	// ComputeMove
	private int ComputeMove() {
		int move = -1;
		try {
			move = m_MovePool [(int) (Random.Range (0, moveSum - 1))];
		} catch (ArgumentOutOfRangeException e) {
			Debug.Log ("Move pool error");
		}

		// compute initial move
		switch (move) 
		{
			case (int)Move.MOVE_ATTACK:
				if (m_Shadow.GetAttackChain () == m_MaxAttackChain) {
				move = (int) (Random.Range (1, 2) == 1 ? Move.MOVE_BLOCK 
					: Move.MOVE_POWERUP);
				}
				break;
			case (int) Move.MOVE_BLOCK:
				if (m_LastBlock) {
				move = (int) (Random.Range (1, 2) == 1 ? Move.MOVE_ATTACK 
					: Move.MOVE_POWERUP);
				}
				break;
			case (int) Move.MOVE_POWERUP:
				if (m_LastVulnerable) {
					move = (int)(Random.Range (1, 2) == 1 ? Move.MOVE_ATTACK 
						: Move.MOVE_BLOCK);
				}
				break;
		}

		// make additional move adjustments (last block, powerup, etc.)
		switch (move) 
		{
			case (int) Move.MOVE_ATTACK:
				m_LastBlock = false;
				m_LastVulnerable = false;
				break;
			case (int) Move.MOVE_BLOCK:
				m_LastBlock = true;
				m_LastVulnerable = false;
				break;
			case (int) Move.MOVE_POWERUP:
				m_LastBlock = false;
				m_LastVulnerable = true;
				break;
		}

		return move;
	}
}