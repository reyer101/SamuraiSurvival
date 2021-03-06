﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

// CharacterController
public class PlayerController : MonoBehaviour {
    private PlayerCharacter m_Player;    
    private LayerMask m_LayerMask;
    private bool m_Jump, m_CanMove, m_Block, m_Attack, m_Throw;

    private float k_CrouchRadius = 1.5f;

    // Awake
    void Awake () {
        m_Player = GetComponent<PlayerCharacter>();        
        m_LayerMask = -1;
        m_CanMove = true;                                  		
	}
	
	// Update
	void Update () {
        if (!m_Jump && m_CanMove)
        {
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }

		if (CrossPlatformInputManager.GetButtonDown("Cancel"))
		{
			SceneManager.LoadScene(0);
		}

	    m_Block = CrossPlatformInputManager.GetButton("Block");
	    m_Attack = CrossPlatformInputManager.GetButtonDown("Attack");
	    m_Throw = CrossPlatformInputManager.GetButtonDown("Throw");
	}

    // FixedUpdate
    void FixedUpdate()
    {
        if(m_CanMove)
        {
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            m_Player.Move(h, m_Jump);            
        }
        else
        {
            m_Player.Move(0, false);            
        }
        
        m_Player.Attack(m_Attack, m_Block, m_Throw);

        m_Jump = false;             
    }

    /*
    Name: setCanMove
    Parameters: bool canMove
    */
    public void setCanMove(bool canMove)
    {
        m_CanMove = canMove;
    }
}
