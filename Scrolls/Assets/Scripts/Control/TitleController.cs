using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class TitleController : MonoBehaviour
{
	public float m_ScrollSpeed, m_TerrainDistance, m_BackgroundDistance,
		m_FadeRate, m_TitleRate, m_GlowStrength, m_GlowSpeed;

	private static float TERRAIN_DIFF = 380f;
	private static float BACKGROUND_DIFF = 5825f;
	
	private GameObject m_Player;
	private GameObject m_Terrain, m_Terrain2, m_Background, m_Background2;
	private Camera m_Camera;
	
	// ui stuff
	private Image m_Fade, m_Title;
	private Text m_PromptText;
	
	private float startTerrainX, startBackgroundX, startTime, glowAlpha = .6f,
		visibleAlpha = 255f, glowDiff = .001f;

	private bool inOrderTerrain, inOrderBackground;
	

	void Start () {
		m_Player = GameObject.FindGameObjectWithTag("Player");
		m_Terrain = GameObject.Find("Terrain");
		m_Terrain2 = GameObject.Find("Terrain 2");
		m_Background = GameObject.Find("Background");
		m_Background2 = GameObject.Find("Background 2");
		m_Fade = GameObject.FindGameObjectWithTag("Fade")
			.GetComponent<Image>();
		m_Title = GameObject.FindGameObjectWithTag("Title")
			.GetComponent<Image>();
		m_PromptText = GameObject.Find("Prompt").GetComponent<Text>();
		m_Camera = Camera.main;

		inOrderTerrain = true;
		inOrderBackground = true;

		m_GlowSpeed = m_GlowSpeed / 1000f;

		// initial starting positions for terrain and background
		startTerrainX = m_Player.transform.position.x;
		startBackgroundX = m_Player.transform.position.x;
	}
	
	void Update ()
	{
		if (Time.time - startTime > 5.5)
		{
			// fade out black 
			m_Fade.color = new Color(0, 0, 0, m_Fade.color.a - 
				(m_FadeRate / 100f) * Time.deltaTime);
		}

		if (Time.time - startTime > 24)
		{
			// fade in title
			float alpha = m_Title.color.a + (m_TitleRate *
				Time.deltaTime) / 100;
			m_Title.color = new Color(255f, 255f, 255f, alpha);
		}

		// make prompt text pulse
		if (glowAlpha <= .6f)
		{
			glowDiff = m_GlowSpeed * m_GlowStrength;
		} 
		
		Debug.Log("Glow subtract: " + (glowAlpha - 1.0f));
		
		if (glowAlpha - 1.0f > 0f)
		{
			glowDiff = -(m_GlowSpeed * m_GlowStrength);
		}

		glowAlpha = glowAlpha + glowDiff;
		m_PromptText.color = new Color(255f, 255f, 255f, glowAlpha);
		

		if (m_Title.color.a > .9f)
		{
			m_PromptText.text = "Press 'space' to begin";

			if (CrossPlatformInputManager.GetButtonDown("Jump"))
			{
				// load the main scene
				SceneManager.LoadScene(1);
			}
		}
		
		// move the player at the scroll speed
		m_Player.GetComponent<Rigidbody2D>().velocity = new Vector2(
			m_ScrollSpeed, 0);
		
		// adjust the animation speed 
		m_Player.GetComponent<Animator>().speed = m_ScrollSpeed / 10f;
		
		// make the camera follow the player
		Vector3 cameraPos = m_Camera.transform.position;
		Vector3 playerPos = m_Player.transform.position;
		m_Camera.transform.position = new Vector3(playerPos.x,
			cameraPos.y, cameraPos.z);
		
		// check to see if terrain needs to be moved
		if (m_Player.transform.position.x - startTerrainX > m_TerrainDistance)
		{
			// move the first terrain object
			GameObject terrain;
			if (inOrderTerrain)
			{
				terrain = m_Terrain;
				inOrderTerrain = false;
			}
			else
			{
				terrain = m_Terrain2;
				inOrderTerrain = true;
			}
			
			terrain.transform.position += new Vector3(TERRAIN_DIFF, 0f, 0);
			startTerrainX = m_Player.transform.position.x;
		}
		
		// check to see if background needs to be moved
		if (m_Player.transform.position.x - startBackgroundX 
		    > m_BackgroundDistance)
		{
			// move the first terrain object
			GameObject background;
			if (inOrderBackground)
			{
				background = m_Background;
				inOrderBackground = false;
			}
			else
			{
				background = m_Background2;
				inOrderBackground = true;
			}
			
			background.transform.position += new Vector3(BACKGROUND_DIFF, 0, 0);
			startBackgroundX = m_Player.transform.position.x;
		}
	}
}
