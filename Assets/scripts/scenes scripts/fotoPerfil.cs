using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class fotoPerfil : MonoBehaviour {

	private MainController GMS;

	public GameObject nombre;
	public GameObject email;
	public GameObject fecha_nacimiento;
	public GameObject sexo;
	public GameObject emailIf;

	public GameObject rotL;
	public GameObject rotR;

	public GameObject loadingF;
	public GameObject PanelIcosUpd;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController> ();

		nombre.GetComponent<Text> ().text = GMS.userData.nombre;

		emailIf.SetActive(false);
		if (GMS.userData.email == "") {
			emailIf.SetActive(true);
		}

		email.GetComponent<Text> ().text = GMS.userData.email;
		fecha_nacimiento.GetComponent<Text> ().text = GMS.userData.fecha_nacimiento;
		sexo.GetComponent<Text> ().text = GMS.userData.sexo;

		if(GMS.userData.foto != ""){
			loadingF.SetActive(true);
			GMS.userData.temp_img = GMS.userData.foto;
			string filepath = Application.persistentDataPath + "/" + GMS.userData.foto;
			if (File.Exists (filepath)) {
				GameObject.Find ("backImage").GetComponent<Image>().sprite = GMS.spriteFromFile(GMS.userData.foto);
				loadingF.SetActive(false);
				PanelIcosUpd.SetActive(false);
			}else{//intentar cargar de nuevo en 2 segs....
				StartCoroutine( tryGetPicture() );
			}
		}

		rotL.SetActive(false);
		rotR.SetActive(false);
	}

	void Update () {
		if(GMS.userData.temp_img != "" && !rotL.activeSelf){
			rotL.SetActive(true);
			rotR.SetActive(true);
		}
	}

	private IEnumerator tryGetPicture(){
		yield return new WaitForSeconds (2);
		
		Debug.Log ("tryGetPicture ...");
		
		string filepath = Application.persistentDataPath + "/" + GMS.userData.foto;
		if (File.Exists (filepath)) {
			GameObject.Find ("backImage").GetComponent<Image> ().sprite = GMS.spriteFromFile (GMS.userData.foto);
			loadingF.SetActive(false);
			PanelIcosUpd.SetActive(false);
		} else {
			StartCoroutine( tryGetPicture() );
		}
		
	}

	public void rotateLeft(string direct){
		GameObject.Find ("backImage").GetComponent<Image>().sprite = GMS.saveTextureRotate (GMS.userData.temp_img, direct);
	}

	public void submit(){

		GMS.toRedirectLogin = "cargar-invitar";

		//bool updateUData = false;

		if (GMS.userData.email == "") {
			GMS.userData.email = emailIf.GetComponent<InputField>().text;
			//updateUData = true;
		}
		string format_birthDate = GMS.userData.date_year + '-' + GMS.userData.date_month + '-' + GMS.userData.date_day;
		GMS.userData.fecha_nacimiento = format_birthDate;
		//if (updateUData) {

		//}

		//GMS.upload_user_foto ();
		//Debug.Log ("temp_img: " + GMS.userData.temp_img);
		if (GMS.userData.temp_img != "") {
			if (!GMS.haveInet) {
				GMS.errorPopup ("Verifica tu conexion a internet");
			} else {
				GMS.updateUserData();

				GMS.userData.foto = GMS.userData.temp_img;
				GMS.upload_user_foto ();

				Application.LoadLevel ("initGPS");
			}
		} else {
			GMS.errorPopup ("Debes subir una foto de perfil");
			//Application.LoadLevel ("initGPS");
		}
	}
}
