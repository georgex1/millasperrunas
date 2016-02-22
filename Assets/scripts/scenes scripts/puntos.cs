using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class puntos : MonoBehaviour {

	private MainController GMS;

	// Use this for initialization
	void Start () {

		//GameObject GM = GameObject.Find ("MainController");
		//GMS = GM.GetComponent<MainController>();
	}

	public void cargarEscena(string pageName){
		Application.LoadLevel(pageName);
	}
}
