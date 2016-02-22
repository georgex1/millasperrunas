using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class configuracion : MonoBehaviour {

	private MainController GMS;

	public GameObject check;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		string config_alerts = PlayerPrefs.GetString ("config_alerts");

		if (config_alerts == "false") {
			check.SetActive(false);
		}
	}

	public void config_alerts(){
		string config_alerts = PlayerPrefs.GetString ("config_alerts");

		if (config_alerts == "false") {
			PlayerPrefs.SetString ("config_alerts", "true");
			check.SetActive (true);
			GMS.notificationsScript.enableNotifs();
		} else {
			PlayerPrefs.SetString ("config_alerts", "false");
			check.SetActive (false);
			GMS.notificationsScript.disableNotifs();
		}
	}

	public void cargarEscena(string pageName){
		Application.LoadLevel(pageName);
	}
}
