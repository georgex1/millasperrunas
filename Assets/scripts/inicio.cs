using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class inicio : MonoBehaviour {

	public GameObject btnRaza;
	public GameObject btnEdad;
	public GameObject btnParentesco;
	public GameObject btnMascota;

	public GameObject DDRaza;
	public GameObject DDEdad;
	public GameObject DDParentezco;
	public GameObject DDMascota;


	private MainController GMS;
	// Use this for initialization
	void Start () {
//		generarCamposFecha ();
		/*GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void cargarEscena(string pageName){
		Application.LoadLevel(pageName);
	}


	public void showFechaNac(GameObject fechaNac){
		if(fechaNac.activeSelf == true){
			fechaNac.SetActive(false);
		}else{
			fechaNac.SetActive(true);
		}
	}

	public void selectRaza(GameObject opcion){
		btnRaza.GetComponentInChildren<Text> ().text = opcion.GetComponentInChildren<Text> ().text;
		Debug.Log (opcion.GetComponentInChildren<Text> ().text);
		DDRaza.SetActive (false);
	}

	public void selectEdad(GameObject opcion){
		btnEdad.GetComponentInChildren<Text> ().text = opcion.GetComponentInChildren<Text> ().text;
		Debug.Log (opcion.GetComponentInChildren<Text> ().text);
		DDEdad.SetActive (false);
	}

	public void selectParentesco(GameObject opcion){
		btnParentesco.GetComponentInChildren<Text> ().text = opcion.GetComponentInChildren<Text> ().text;
		Debug.Log (opcion.GetComponentInChildren<Text> ().text);
		DDParentezco.SetActive (false);
	}

	public void selectMascota(GameObject opcion){
		btnMascota.GetComponentInChildren<Text> ().text = opcion.GetComponentInChildren<Text> ().text;
		Debug.Log (opcion.GetComponentInChildren<Text> ().text);
		DDMascota.SetActive (false);
	}

	/*public void fbLogin(){
		GMS.SendMessage ("fbLogin");
	}*/

}
