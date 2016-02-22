using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class notificacion : MonoBehaviour {

	public GameObject NotifTitulo;
	public GameObject NotifDescripcion;
	public GameObject NotifButtons;

	private string perros_id;

	private MainController GMS;

	/*private string[] anims = new string[]{
		"PanelAnimCazador", "PanelAnimFeliz", "PanelAnimCorrea", "PanelAnimAnsioso"
	};*/

	// Use this for initialization
	void Start () {

		Debug.Log (this.gameObject.name);

		//random anim
		/*int randAnim = Random.Range (0, 3);

		foreach (string anim_ in anims) {
			if(anim_ != anims[randAnim]){
				GameObject.Find (anim_).SetActive(false);
			}
		}*/

		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		GMS.db.OpenDB("millasperrunas.db");
		ArrayList result = GMS.db.BasicQueryArray ("select titulo, descripcion, tipo, perros_id from notificaciones where id = '"+GMS.notificacionId+"' ");
		if (result.Count > 0) {
			NotifTitulo.GetComponent<Text>().text = ((string[])result [0])[0] ;
			NotifDescripcion.GetComponent<Text>().text = ((string[])result [0])[1] ;

			if(((string[])result [0])[2] == "contenido"){
				GMS.db.UpdateSingle("notificaciones", "visto", "1", "id", GMS.notificacionId);
			}
			if(((string[])result [0])[2] == "invitacion"){
				NotifButtons.SetActive(true);
				perros_id = ((string[])result [0])[3];
			}
		}
		GMS.db.CloseDB();
	}

	public void aceptar_invitacion(string aceptado){
		string[] fields = {"aceptado", "perros_id", "usuarios_id"};
		string[] values = {aceptado, perros_id, GMS.userData.id.ToString()};
		GMS.insert_sync(fields, values, "perros_invitacion_respuesta");

		GMS.db.UpdateSingle("perros_usuarios", "aceptado", aceptado, "perros_id" , perros_id );
		Application.LoadLevel ("home-mascota");
	}


	public void cargarEscena(string escena){
		Application.LoadLevel (escena);
	}
}
