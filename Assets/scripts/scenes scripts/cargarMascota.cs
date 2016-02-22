using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class cargarMascota : MonoBehaviour {

	public GameObject btnRaza;
	public GameObject btnEdad;

	public GameObject DDRaza;
	public GameObject DDEdad;

	public GameObject Nombre;
	public GameObject Edad;
	public GameObject Raza;

	public string peso_;
	public string edad_;
	public string raza_ = "0";

	public GameObject buttonSubmit;

	private MainController GMS;
	private bool isDebugScreen;

	// Use this for initialization
	void Start () {
		isDebugScreen = false;

		buttonSubmit.SetActive (false);
		if (!isDebugScreen) {
			GameObject GM = GameObject.Find ("MainController");
			GMS = GM.GetComponent<MainController> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Nombre.GetComponent<Text> ().text != "" && peso_ != "" && edad_ != "" && raza_ != "0" && GMS.perro.temp_img != "") {
			if (!buttonSubmit.activeSelf) {
				buttonSubmit.SetActive (true);
			}
		} else {
			if (buttonSubmit.activeSelf) {
				buttonSubmit.SetActive (false);
			}
		}
	}

	public void submit(){
		Text nombre_ = Nombre.GetComponent<Text> ();
		if (!isDebugScreen) {
			if (!GMS.haveInet) {
				GMS.errorPopup ("Verifica tu conexion a internet");
			} else {

				GMS.perro.nombre = nombre_.text;
				GMS.perro.peso = peso_;
				GMS.perro.edad = edad_;
				GMS.perro.razas_id = raza_;
				GMS.perro.parentescos_id = "1";
				GMS.perro.foto = GMS.perro.temp_img;

				GMS.registerPerro ();
			}
		}

	}

	public void changePeso(string peso){
		peso_ = peso;
	}

	/*public void selectRaza(string razas_id){
		raza_ = razas_id;
		DDRaza.SetActive (false);
	}*/
	public void selectRaza(GameObject opcion){

		btnRaza.GetComponentInChildren<Text> ().text = opcion.GetComponentInChildren<Text> ().text;

		string[] opcionName = opcion.name.Split ('_');
		raza_ = opcionName[1];
		DDRaza.SetActive (false);
	}

	/*public void selectEdad(int Sedad_){
		edad_ = Sedad_.ToString();
		DDEdad.SetActive (false);
	}*/

	public void selectEdad(GameObject opcion){
		btnEdad.GetComponentInChildren<Text> ().text = opcion.GetComponentInChildren<Text> ().text;
		string[] opcionName = opcion.name.Split ('_');
		edad_ = opcionName[1];
		DDEdad.SetActive (false);
	}

	public void cargarEscena(string escena){
		Application.LoadLevel (escena);
	}
}
