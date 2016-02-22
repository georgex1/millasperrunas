using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class invitar : MonoBehaviour {
	//private dbAccess db ;

	public GameObject nombre;
	public GameObject email;

	private MainController GMS;
	public GameObject buttonSubmit;

	// Use this for initialization
	void Start () {
		//db = GetComponent<dbAccess>();

		buttonSubmit.SetActive (false);
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		DopMascotas ();
	}

	// Update is called once per frame
	void Update(){
		if (nombre.GetComponent<Text> ().text != "" && validEmail (email.GetComponent<Text> ().text) 
		    && GMS.invitado.parentescos_id != "" && GMS.invitado.perros_id != "" ) {
			if(!buttonSubmit.activeSelf){
				buttonSubmit.SetActive (true);
			}
		} else {
			if(buttonSubmit.activeSelf){
				buttonSubmit.SetActive (false);
			}
		}
	}

	public void cargarEscena(string escena){
		Application.LoadLevel (escena);
	}

	public bool validEmail(string emailaddress){
		return System.Text.RegularExpressions.Regex.IsMatch(emailaddress, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
	}

	public void DopMascotas(){

		GMS.db.OpenDB("millasperrunas.db");
		ArrayList result = GMS.db.BasicQueryArray ( GMS.perro.queryPerrosUsuario(GMS.userData.id) );
		GMS.db.CloseDB();
		Debug.Log (result);
		//string[] ArrayRows = new string[result.Count];
		/*foreach(string[] row_ in result){
			Debug.Log ( row_[1] +"|*|"+ row_[1] );
		}*/
		
		GameObject OptionDefault = GameObject.Find("DDMascota/PanelMask/PanelScroll/Option");
		
		foreach(string[] row_ in result){

			GameObject clone = Instantiate(OptionDefault, OptionDefault.transform.position, OptionDefault.transform.rotation) as GameObject;
			clone.transform.SetParent(OptionDefault.transform.parent);
			clone.transform.localScale = new Vector3(1, 1, 1);
			clone.GetComponentInChildren<Text> ().text = row_[1];
			clone.GetComponentInChildren<Text> ().name = "opt_" + row_[0];
		}
		
		Destroy (OptionDefault);
		
		GameObject.Find("DDMascota").SetActive(false);
	}

	/*public void selectParentesco(string parentesco_){
		GMS.invitado.parentescos_id = parentesco_;
	}*/

	public void selectParentesco(GameObject opcion){
		GameObject.Find("ButtonSelParentesco").GetComponentInChildren<Text> ().text = opcion.GetComponentInChildren<Text> ().text;
		string[] opcionName = opcion.name.Split ('_');
		Debug.Log ("selecciono parentesco: " + opcionName[1]);
		GMS.invitado.parentescos_id = opcionName[1];
		GameObject.Find("DDParentesco").SetActive (false);
	}

	public void selectMascota(GameObject opcion){

		GameObject.Find("ButtonSelMascota").GetComponentInChildren<Text> ().text = opcion.GetComponentInChildren<Text> ().text;
		string[] mascotaId = opcion.GetComponentInChildren<Text> ().name.Split('_');
		Debug.Log ("selecciono mascota: " + mascotaId[1]);

		GMS.invitado.perros_id = mascotaId [1];

		GameObject.Find("DDMascota").SetActive (false);
	}

	public void selectfbUser(GameObject opcion){
		nombre.transform.parent.GetComponent<InputField>().text = opcion.GetComponentInChildren<Text> ().text;;
		//nombre.GetComponent<Text> ().text = opcion.GetComponentInChildren<Text> ().text;
		GameObject.Find("DDFacebook").SetActive (false);
	}

	public void submit(){
		string nombre_ = nombre.GetComponent<Text> ().text;
		string email_ = email.GetComponent<Text> ().text;

		if(!GMS.haveInet){
			GMS.errorPopup("Verifica tu conexion a internet");
		}else{
			
			GMS.invitado.email = email_;
			GMS.invitado.nombre = nombre_;

			GMS.invitar();
		}
	}

}
