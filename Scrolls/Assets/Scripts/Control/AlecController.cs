using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// CharacterController
public class AlecController : MonoBehaviour {
    private PlayerCharacter m_Player;    
    private LayerMask m_LayerMask;
    private bool m_Jump, m_CanMove;

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
