using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// CharacterController
public class PlayerController : MonoBehaviour {
    private PlayerCharacter m_Player;    
    private LayerMask m_LayerMask;
    private bool m_Jump, m_CanMove, m_Block, m_Attack;

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

	    m_Block = CrossPlatformInputManager.GetButton("Block");
	    m_Attack = CrossPlatformInputManager.GetButtonDown("Attack");
	    Debug.Log(m_Attack);
	}

    // FixedUpdate
    void FixedUpdate()
    {
        if(m_CanMove && !m_Block)
        {
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            m_Player.Move(h, m_Jump);            
        }
        else
        {
            m_Player.Move(0, false);            
        }
        
        m_Player.Attack(m_Attack, m_Block);

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
