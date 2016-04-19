using UnityEngine;
using System.Collections;

public class backButton : MonoBehaviour {

	private bool changeScene = false;
	private MainController GMS;

	// Use this for initialization
	void Start () {
		GMS = GameObject.Find ("MainController").GetComponent<MainController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!changeScene) {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				if (Application.loadedLevelName == "badge-detail") {
					Application.LoadLevel ("perfil");
				}
				if (Application.loadedLevelName == "chat") {
					Application.LoadLevel ("ranking");
				}
				if (Application.loadedLevelName == "chat") {
					Application.LoadLevel ("ranking");
				}
				if (Application.loadedLevelName == "about" || Application.loadedLevelName == "configuracion") {
					Application.LoadLevel (GMS.antScene);
				}
				if (Application.loadedLevelName == "home") {
					Application.LoadLevel ("home-mascota");
				}
				if (Application.loadedLevelName == "home-mascota" || Application.loadedLevelName == "ingresar" || Application.loadedLevelName == "login" ) {
					qstSalir();
				}
				if (Application.loadedLevelName == "invitar-email" || Application.loadedLevelName == "invitar-gracias") {
					Application.LoadLevel ("home-mascota");
				}
				if (Application.loadedLevelName == "msg-foto-tomada") {
					Application.LoadLevel ("paseo");
				}
				if (Application.loadedLevelName == "msg-pass") {
					Application.LoadLevel ("registro");
				}
				if (Application.loadedLevelName == "msg-tomar-foto" || Application.loadedLevelName == "notificacion") {
					Application.LoadLevel ("notificaciones");
				}
				if (Application.loadedLevelName == "notificacion_puntos_especiales" || Application.loadedLevelName == "perfil" 
				    || Application.loadedLevelName == "puntos" || Application.loadedLevelName == "ranking" ) {
					Application.LoadLevel ("home");
				}
				if (Application.loadedLevelName == "notificaciones") {
					if (GMS.paseando) {
						Application.LoadLevel ("paseo");
					} else {
						Application.LoadLevel ("home");
					}
				}
				if (Application.loadedLevelName == "registro") {
					Application.LoadLevel ("login");
				}
				if (Application.loadedLevelName == "tips-paseo") {
					Application.LoadLevel ("paseo");
				}
				if (Application.loadedLevelName == "tips-paseo-descripcion") {
					Application.LoadLevel ("tips-paseo");
				}

				changeScene = true;
			}
		}
	}

	private void qstSalir(){
		#if !UNITY_EDITOR
		#if UNITY_ANDROID

		string[] m_buttons = new string[] { "NO", "SI" };
		NPBinding.UI.ShowAlertDialogWithMultipleButtons ("Walk", "Cerrar?", m_buttons, (string _buttonPressed)=>{
			if(_buttonPressed == "SI"){
				Application.Quit();
			}
		});
		#else
		
		#endif
		#endif
		changeScene = false;
	}

	public void cargarEscena(string pageName){
		if (Application.loadedLevelName == "about") {
			Application.LoadLevel (GMS.antScene);
		} else {
			Application.LoadLevel (pageName);
		}
	}
}
