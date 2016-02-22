using UnityEngine;
using System.Collections;

public class notificacionPuntosExtra : MonoBehaviour {

	public GameObject NotifTitulo;

	private MainController GMS;

	private string[] anims = new string[]{
		"PanelAnimNoche", "PanelAnimShow", "PanelAnimNieve", "PanelAnimManiana", "PanelAnimLluvia", "PanelAnimFutbol"
	};

	// Use this for initialization
	void Start () {

		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		foreach (string anim_ in anims) {
			GameObject.Find (anim_).SetActive(false);
		}

		switch (GMS.puntosEspecialesMotivoId) {
		case "1": GameObject.Find ("PanelAnimNoche").SetActive(true); break;
		case "2": GameObject.Find ("PanelAnimShow").SetActive(true); break;
		case "3": GameObject.Find ("PanelAnimNieve").SetActive(true); break;
		case "4": GameObject.Find ("PanelAnimManiana").SetActive(true); break;
		case "5": GameObject.Find ("PanelAnimLluvia").SetActive(true); break;
		case "6": GameObject.Find ("PanelAnimFutbol").SetActive(true); break;
		}

	}
	
	public void cargarEscena(string pageName){
		Application.LoadLevel(pageName);
	}
}
