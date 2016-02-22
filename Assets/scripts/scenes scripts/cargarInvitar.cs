using UnityEngine;
using System.Collections;

public class cargarInvitar : MonoBehaviour {

	private bool hasPerro;
	private MainController GMS;

	// Use this for initialization
	void Start () {
		hasPerro = false;
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();


		GMS.db.OpenDB("millasperrunas.db");
		ArrayList result = GMS.db.BasicQueryArray ( GMS.perro.queryPerrosUsuario(GMS.userData.id) );

		GMS.db.CloseDB();
		
		if (result.Count > 0) {
			hasPerro = true;
			foreach (string[] row_ in result) {
				GMS.perro.populatePerro (row_);
			}

			Application.LoadLevel ("home-mascota");
		}
	}
	
	public void cargarEscena(string escena){
		if (escena == "invitar-email" && !hasPerro) {
			GMS.errorPopup ("No tiene cargada ninguna mascota todavia", "0");
		} else {
			Application.LoadLevel (escena);
		}
	}
}
