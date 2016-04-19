using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class tipsPaseoDescripcion : MonoBehaviour {

	private MainController GMS;

	public Text tipTitulo;
	public Text tipDescripcion;
	public Image tipImagen;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		GMS.db.OpenDB("millasperrunas.db");
		ArrayList result = GMS.db.BasicQueryArray ( "select nombre, descripcion, imagen from tips_paseo where id = '"+GMS.tipId+"' " );
		GMS.db.CloseDB();

		if (result.Count > 0) {
			tipTitulo.text = ((string[])result [0]) [0];
			tipDescripcion.text = ((string[])result [0]) [1];
			tipImagen.sprite = GMS.spriteFromFile ( ((string[])result [0]) [2] );
		}

	}
	
	public void cargarEscena(string pageName){
		Debug.Log ("cargar escena: " + pageName);
		Application.LoadLevel(pageName);
	}
}
