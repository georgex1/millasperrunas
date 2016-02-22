using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class loginEmailSC : MonoBehaviour {

	private MainController GMS;
	public GameObject Email;
	public GameObject Password;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();
	}

	public void login(){

		string email_ = Email.GetComponent<Text> ().text;
		string pass_ = Password.GetComponent<InputField> ().text;

		GMS.userData.email = email_;
		GMS.userData.password = pass_;

		GMS.loginEmail ();
	}

	public void cargarEscena(string escena){
		Application.LoadLevel (escena);
	}

}
