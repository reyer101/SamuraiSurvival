using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DuelController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Vector2 shadowSpawn = GameObject.FindGameObjectWithTag ("ShadowSpawn")
			.gameObject.transform.position;
		string difficulty = PlayerPrefs.GetString (Constants.KEY_DIFFICULTY);
		difficulty = difficulty.Equals("") ? Constants.EASY : difficulty;
		Debug.Log ("Start Duel: " + difficulty);
		GameObject shadow = Resources.Load<GameObject>(String.Format (
			Constants.OBJECT_SHADOW, difficulty));

		Instantiate (shadow, shadowSpawn, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// load title scene
	public void stopDuel() {
		SceneManager.LoadScene (0);
	}
}
