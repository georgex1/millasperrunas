using UnityEngine;
using System.Collections;

public class notificacionPuntosExtra : MonoBehaviour {

	public GameObject NotifTitulo;

	private MainController GMS;

	/*private string[] anims = new string[]{
		"PanelAnimNoche", "PanelAnimShow", "PanelAnimNieve", "PanelAnimManiana", "PanelAnimLluvia", "PanelAnimFutbol"
	};*/
	public GameObject PanelAnimNoche;
	public GameObject PanelAnimShow;
	public GameObject PanelAnimNieve;
	public GameObject PanelAnimManiana;
	public GameObject PanelAnimLluvia;
	public GameObject PanelAnimFutbol;

	private int lastMotivoKey;

	// Use this for initialization
	void Start () {

		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		lastMotivoKey = GMS.puntosEspecialesMotivoIds.Count;
		/*foreach (string anim_ in anims) {
			GameObject.Find (anim_).SetActive(false);
		}*/

		switch (GMS.puntosEspecialesMotivoIds[lastMotivoKey]) {
		case "1": PanelAnimNoche.SetActive(true); break;
		case "2": PanelAnimShow.SetActive(true); break;
		case "3": PanelAnimNieve.SetActive(true); break;
		case "4": PanelAnimManiana.SetActive(true); break;
		case "5": PanelAnimLluvia.SetActive(true); break;
		case "6": PanelAnimFutbol.SetActive(true); break;
		}

	}
	
	public void cargarEscena(string pageName){
		Application.LoadLevel(pageName);
	}

	public void siguiente(){
		GMS.puntosEspecialesMotivoIds.Remove(lastMotivoKey);
		lastMotivoKey--;
		if (lastMotivoKey > 0) {
			Application.LoadLevel ("notificacion_puntos_especiales");
		} else {
			Application.LoadLevel("home");
		}

	}
}
