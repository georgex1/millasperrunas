using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class loginEmail : MonoBehaviour {

	public GameObject loginEmailPanel;
	public GameObject btnIngresar;

	// Use this for initialization
	void Start () {
		//iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("loginEmail"), "time", 20));
	}

	public void scrollLogin(){
		iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("loginEmail"), "time", 2));
		StartCoroutine (showLogin());
	}

	IEnumerator showLogin(){
		yield return new WaitForSeconds(0.5f);
		btnIngresar.SetActive (true);
		loginEmailPanel.SetActive (true);

		transform.GetComponentInChildren<Text> ().text = "LOGIN EMAIL";
	}

}
