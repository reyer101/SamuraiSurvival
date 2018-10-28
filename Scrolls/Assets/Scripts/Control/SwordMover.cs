using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordMover : MonoBehaviour
{
	public float m_MoveSpeed, m_RotationSpeed, m_ReturnTime;
	private float m_SpawnTime;
	private Boolean m_HitTerrain;
	private Rigidbody2D m_RigidBody;
	private GameObject m_Player;

	// Use this for initialization
	void Start ()
	{
		m_RigidBody = GetComponent<Rigidbody2D>();
		m_Player = GameObject.FindGameObjectWithTag("Player");
		m_SpawnTime = Time.time;
		m_HitTerrain = false;
		Debug.Log("Rotation: " + transform.rotation);
	}
	
	// Update is called once per frame
	void Update ()
	{
		float step;
		if (Time.time - m_SpawnTime <= m_ReturnTime / 2f && !m_HitTerrain)
		{
			Vector3 velocity = transform.rotation.x == 0 ? Vector3.right 
				: Vector3.left;
			m_RigidBody.velocity = velocity * m_MoveSpeed * 2f;
		}
		else
		{
			step = m_MoveSpeed * Time.deltaTime * 4f;
			transform.position = Vector3.MoveTowards(transform.position,
				m_Player.transform.position, step);
		}
		
		transform.Rotate(Vector3.forward  * Time.deltaTime * 360f
			 * m_RotationSpeed, Space.Self);
		
	}
	
	// OnTriggerEnter2D
	private void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("Hit game collider: " + other.gameObject.name);
		if (other.gameObject.tag == "Terrain")
		{
			m_HitTerrain = true;
		}
	}
}
