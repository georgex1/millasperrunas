using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class registro : MonoBehaviour {

	public GameObject nombre;
	public GameObject email;
	public GameObject pass;
	public string sexo;

	public Toggle tyc;

	private MainController GMS;
	public GameObject buttonSubmit;

	public GameObject panelSeguridad;
	private int passStreng;

	// Use this for initialization
	void Start () {
		passStreng = 0;
	//al final a subir-foto
		buttonSubmit.SetActive (false);
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		panelSeguridad.transform.Find("baja").gameObject.SetActive(false);
		panelSeguridad.transform.Find("mediana").gameObject.SetActive(false);
		panelSeguridad.transform.Find("alta").gameObject.SetActive(false);

		//precargar campos
		if (GMS.userData.nombre != "") {
			nombre.GetComponent<InputField> ().text = GMS.userData.nombre;
		}
		if (GMS.userData.email != "") {
			email.GetComponent<InputField> ().text = GMS.userData.email;
		}
		if (GMS.userData.sexo != "") {
			sexo = GMS.userData.sexo;
			if(sexo == "Hombre"){
				GameObject.Find ("Hombre").GetComponent<Toggle>().isOn = true;
			}
			if(sexo == "Mujer"){
				GameObject.Find ("Mujer").GetComponent<Toggle>().isOn = true;
			}
		}
	}

	void Update(){

		if (GMS.userData.nombre != nombre.GetComponent<InputField> ().text) {
			GMS.userData.nombre = nombre.GetComponent<InputField> ().text;
		}
		if (GMS.userData.email != email.GetComponent<InputField> ().text) {
			GMS.userData.email = email.GetComponent<InputField> ().text;
		}
		if (GMS.userData.sexo != sexo) {
			GMS.userData.sexo = sexo;
		}

		if (nombre.GetComponent<InputField> ().text != "" && validEmail (email.GetComponent<InputField> ().text) && pass.GetComponent<InputField> ().text != "" 
		    && sexo != "" && GMS.userData.date_day != "" && GMS.userData.date_month != "" && GMS.userData.date_year != "" && tyc.isOn && passStreng == 3) {

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
		InputField nombre_ = nombre.GetComponent<InputField> ();
		InputField email_ = email.GetComponent<InputField> ();
		InputField pass_ = pass.GetComponent<InputField> ();

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

	public void cargar_escena(string scena){
		Application.LoadLevel (scena);
	}

	public void onChangePass(){
		string passString = pass.GetComponent<InputField> ().text;
		int countStreng = 0;

		if (Regex.IsMatch (passString, @"[a-zA-Z]")) {
			countStreng++;
		}

		if (Regex.IsMatch(passString, @"\d")) {
			countStreng++;
		}

		if (Regex.IsMatch(passString, "^.*[&!$|/].*$")) {
			countStreng++;
		}

		passStreng = countStreng;

		panelSeguridad.transform.Find("baja").gameObject.SetActive(false);
		panelSeguridad.transform.Find("mediana").gameObject.SetActive(false);
		panelSeguridad.transform.Find("alta").gameObject.SetActive(false);

		if (countStreng == 1 || countStreng == 0) {
			panelSeguridad.transform.Find("baja").gameObject.SetActive(true);
		}
		if (countStreng == 2) {
			panelSeguridad.transform.Find("mediana").gameObject.SetActive(true);
		}
		if (countStreng == 3) {
			panelSeguridad.transform.Find("alta").gameObject.SetActive(true);
		}
	}
}
