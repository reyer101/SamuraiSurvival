using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordMover : MonoBehaviour
{
	public float m_MoveSpeed, m_RotationSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.right * m_MoveSpeed / 10f);
		transform.Rotate(Vector3.forward * Time.deltaTime * 360f
			 * m_RotationSpeed, Space.World);
	}
}
