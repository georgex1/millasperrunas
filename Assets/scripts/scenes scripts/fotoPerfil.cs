using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class fotoPerfil : MonoBehaviour {

	private MainController GMS;

	public GameObject nombre;
	public GameObject email;
	public GameObject fecha_nacimiento;
	public GameObject sexo;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController> ();

		nombre.GetComponent<Text> ().text = GMS.userData.nombre;
		email.GetComponent<Text> ().text = GMS.userData.email;
		fecha_nacimiento.GetComponent<Text> ().text = GMS.userData.fecha_nacimiento;
		sexo.GetComponent<Text> ().text = GMS.userData.sexo;
	}

	public void submit(){
		//GMS.upload_user_foto ();
		//Debug.Log ("temp_img: " + GMS.userData.temp_img);
		if (GMS.userData.temp_img != "") {
			if (!GMS.haveInet) {
				GMS.errorPopup ("Verifica tu conexion a internet");
			} else {
				GMS.userData.foto = GMS.userData.temp_img;
				GMS.upload_user_foto ();
				Application.LoadLevel ("cargar-invitar");
			}
		} else {
			Application.LoadLevel ("cargar-invitar");
		}
	}
}
