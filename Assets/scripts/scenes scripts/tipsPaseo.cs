using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class tipsPaseo : MonoBehaviour {
	
	private MainController GMS;
	//private gpsController GPSC;

	public GameObject UserNombre;
	public GameObject UserFoto;

	public GameObject PerroNombre;
	public GameObject PerroFoto;

	public GameObject puntos;
	public GameObject kms;
	public GameObject tiempo;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		/*GameObject GPS_ = GameObject.Find ("GPSController");
		GPSC = GPS_.GetComponent<gpsController>();*/

		if (GMS.userData.foto != "") {
			Sprite sprite_ = GMS.spriteFromFile ( GMS.userData.foto );
			UserFoto.GetComponent<Image> ().sprite = sprite_;
		}
		UserNombre.GetComponent<Text> ().text = GMS.userData.nombre;

		if (GMS.perro.foto != "") {
			Sprite sprite_ = GMS.spriteFromFile ( GMS.perro.foto );
			PerroFoto.GetComponent<Image> ().sprite = sprite_;
		}
		PerroNombre.GetComponent<Text> ().text = GMS.perro.nombre;

		DopTips ();
	}

	void FixedUpdate () {
		tiempo.GetComponent<Text>().text = GMS.gps_displayHours.ToString () + "H - " + GMS.gps_displayMinutes.ToString () + "M - " +GMS.gps_displaySeconds.ToString () + "S";
		
		//if (GPSC.actSec != GPSC.displaySeconds) {
		puntos.GetComponent<Text>().text = GMS.gps_calcPuntos.ToString ("n2") + " PUNTOS";
		kms.GetComponent<Text>().text = GMS.gps_partialVeloc.ToString ("n2") + "KM/H";
		//}
	}
	
	public void DopTips(){
		
		GMS.db.OpenDB("millasperrunas.db");
		ArrayList result = GMS.db.BasicQueryArray ( "select id, nombre, descripcion from tips_paseo where habilitado = '1' order by orden ASC" );
		GMS.db.CloseDB();
		Debug.Log (result);
		//string[] ArrayRows = new string[result.Count];
		/*foreach(string[] row_ in result){
			Debug.Log ( row_[1] +"|*|"+ row_[1] );
		}*/
		
		GameObject OptionDefault = GameObject.Find("PanelTip1");

		int tnro = 1;
		foreach(string[] row_ in result){
			
			GameObject clone = Instantiate(OptionDefault, OptionDefault.transform.position, OptionDefault.transform.rotation) as GameObject;

			clone.transform.FindChild("TextNro").GetComponent<Text>().text = "TIP Nº" + tnro.ToString() ;
			clone.transform.FindChild("TextTipo").GetComponent<Text>().text = row_[1];

			clone.transform.SetParent(OptionDefault.transform.parent);
			clone.transform.localScale = new Vector3(1, 1, 1);
			clone.transform.name = "opt_" + row_[0];

			tnro++;
		}
		
		Destroy (OptionDefault);
	}

	public void selectTip(GameObject tip){
		string[] opcionName = tip.name.Split ('_');
		GMS.tipId = opcionName[1];

		Application.LoadLevel("tips-paseo-descripcion");
	}

	public void cargarEscena(string pageName){
		Application.LoadLevel(pageName);
	}
}
