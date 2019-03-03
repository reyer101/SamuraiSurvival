using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Pulse : MonoBehaviour {
	private Text m_Text;
	private Color m_TextColor;
	private float startTime, glowDiff = .001f, glowAlpha = .6f;
	private bool dim;

	public string m_DisplayText;

	[Range(0, 5f)]
	public float m_PulseStrength;

	public float m_PulseSpeed;

	// Use this for initialization
	void Start () {
		m_Text = GetComponent<Text> ();
		m_PulseSpeed = m_PulseSpeed / 1000f;
		m_TextColor = m_Text.color;
		startTime = Time.time;

		m_Text.text = m_DisplayText;
	}
	
	// Update is called once per frame
	void Update () {

		// make prompt text pulse
		if (glowAlpha <= .6f)
		{
			glowDiff = m_PulseSpeed * m_PulseStrength;
		} 
		
		if (glowAlpha - 1.0f > 0f)
		{
			glowDiff = -(m_PulseSpeed * m_PulseStrength);
		}

		glowAlpha = glowAlpha + glowDiff;
		m_Text.color = new Color(255f, 255f, 255f, glowAlpha);
		
	}
}
