using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class paseoData : MonoBehaviour {

	private MainController GMS;
	public GameObject puntos;
	public GameObject kms;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();
	}
	
	void FixedUpdate () {
		
		//if (GPSC.actSec != GPSC.displaySeconds) {
		puntos.GetComponent<Text>().text = GMS.gps_calcPuntos.ToString ("n2") + " PUNTOS";
		kms.GetComponent<Text>().text = GMS.gps_partialVeloc.ToString ("n1") + "KM/H";
		//}
	}
}
