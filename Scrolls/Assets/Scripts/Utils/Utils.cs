using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Utils {

	// returns all children of a given game object
	public static List<GameObject> getChildren(GameObject gameObject) {
		List<GameObject> retval = new List<GameObject> ();
		for (int i = 0; i < gameObject.transform.childCount; ++i) {
			GameObject child = gameObject.transform.GetChild (i).gameObject;
			retval.Add (child);
		}

		return retval;
	}
}
