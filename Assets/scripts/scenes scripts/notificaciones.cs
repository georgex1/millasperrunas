using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class notificaciones : MonoBehaviour {

	private MainController GMS;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		getNotificaciones ();
	}

	/*private IEnumerator updateNotifs(){
		yield return new WaitForSeconds (4);
		getNotificaciones ();
	}
*/
	private void getNotificaciones(){
		GMS.db.OpenDB("millasperrunas.db");
		ArrayList result = GMS.db.BasicQueryArray ( "select id, titulo, descripcion, tipo from notificaciones where visto = '0'" );
		GMS.db.CloseDB();
		Debug.Log (result);

		GameObject OptionDefault = GameObject.Find("Notificacion1");

		foreach(string[] row_ in result){
			
			GameObject clone = Instantiate(OptionDefault, OptionDefault.transform.position, OptionDefault.transform.rotation) as GameObject;
			
			clone.transform.FindChild("Text").GetComponent<Text>().text = row_[1] ;
			
			clone.transform.SetParent(OptionDefault.transform.parent);
			clone.transform.localScale = new Vector3(1, 1, 1);
			clone.transform.name = "opt_" + row_[0] ;
		}
		
		Destroy (OptionDefault);

		//StartCoroutine (updateNotifs());
	}

	public void selectNotificacion(GameObject tip){
		string[] opcionName = tip.name.Split ('_');
		GMS.notificacionId = opcionName[1];

		/*if (opcionName [2] == "foto") {//sacar foto al perro
			Application.LoadLevel ("msg-tomar-foto");
		} else{
			Application.LoadLevel ("notificacion");
		}*/
		Application.LoadLevel ("notificacion");
	}

	public void backBtn(){
		if (GMS.paseando) {
			Application.LoadLevel ("paseo");
		} else {
			Application.LoadLevel ("home");
		}
	}

	public void cargarEscena(string escena){
		Application.LoadLevel (escena);
	}
}
