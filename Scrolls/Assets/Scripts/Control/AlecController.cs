using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// CharacterController
public class AlecController : MonoBehaviour {
    private Transform m_CrouchCheck;
    private PlayerCharacter m_Player;
    private Vector3 m_CrouchScale, m_NormalScale;
    private LayerMask m_LayerMask;
    private bool m_Jump, m_Crouch, m_CanMove;

    private float k_CrouchRadius = 1.5f;

    // Awake
    void Awake () {
        m_Player = GetComponent<PlayerCharacter>();
        m_CrouchCheck = transform.Find("ClimbCheck");
        m_LayerMask = -1;
        m_CanMove = true;
        m_Crouch = false;                             		
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
            m_Player.Move(h, m_Jump, m_Crouch);            
        }
        else
        {
            m_Player.Move(0, false, m_Crouch);            
        }

        m_Jump = false;

        float v = CrossPlatformInputManager.GetAxis("Vertical");      
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
