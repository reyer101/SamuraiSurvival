using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	private static readonly string DUEL = "Duel";
	private static readonly string MAIN = "Main";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void showOptions(string menuType) {
		
		// iterare through menu children game objects
		foreach (GameObject go in Utils.getChildren(gameObject)) {
			if (go.name.Contains (menuType)) {
				go.SetActive (true);
				continue;
			} 

			go.SetActive (false);
		}
	}

	public void startDuel(string difficulty) {
		setDifficulty (difficulty);
		SceneManager.LoadScene(1);
	}

	public void exit() {
		Application.Quit ();
	}

	void setDifficulty(string difficulty) {
		PlayerPrefs.SetString (Constants.KEY_DIFFICULTY, difficulty);
	}
}
