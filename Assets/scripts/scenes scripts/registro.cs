using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class registro : MonoBehaviour {

	public GameObject nombre;
	public GameObject email;
	public GameObject pass;
	public string sexo;

	private MainController GMS;
	public GameObject buttonSubmit;

	// Use this for initialization
	void Start () {
	//al final a subir-foto
		buttonSubmit.SetActive (false);
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();
	}

	void Update(){
		if (nombre.GetComponent<Text> ().text != "" && validEmail (email.GetComponent<Text> ().text) && pass.GetComponent<Text> ().text != "" 
			&& sexo != "" && GMS.userData.date_day != "" && GMS.userData.date_month != "" && GMS.userData.date_year != "") {
			Debug.Log("active button");
			if(!buttonSubmit.activeSelf){
				buttonSubmit.SetActive (true);
			}
		} else {
			if(buttonSubmit.activeSelf){
				buttonSubmit.SetActive (false);
			}
		}
	}


	public void submit(){
		Text nombre_ = nombre.GetComponent<Text> ();
		Text email_ = email.GetComponent<Text> ();
		Text pass_ = pass.GetComponent<Text> ();

		string format_birthDate = GMS.userData.date_year + '-' + GMS.userData.date_month + '-' + GMS.userData.date_day;

		if(!GMS.haveInet){
			GMS.errorPopup("Verifica tu conexion a internet");
		}else{

			GMS.userData.email = email_.text;
			GMS.userData.nombre = nombre_.text;
			GMS.userData.password = pass_.text;
			GMS.userData.fecha_nacimiento = format_birthDate;
			GMS.userData.sexo = sexo;
			GMS.userData.email = email_.text;

			GMS.register();
		}

	}

	public void changeSexo(string sexo_){
		sexo = sexo_;
	}

	public void showFechaNac(GameObject fechaNac){
		if(fechaNac.activeSelf == true){
			fechaNac.SetActive(false);
		}else{
			fechaNac.SetActive(true);
		}
	}

	public bool validEmail(string emailaddress){
		return System.Text.RegularExpressions.Regex.IsMatch(emailaddress, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
	}

	public void back_login(){
		Application.LoadLevel ("login");
	}
}
