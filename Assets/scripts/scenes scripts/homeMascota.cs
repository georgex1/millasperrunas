﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class homeMascota : MonoBehaviour {

	private MainController GMS;
	public GameObject TPuntos;
	public GameObject TKm;

	public GameObject perro_bck;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		Debug.Log ("perros puntos: " + GMS.perro.puntos);
		Debug.Log ("perros kilometros: " + GMS.perro.kilometros);

		GMS.db.OpenDB("millasperrunas.db");
		
		ArrayList result = GMS.db.BasicQueryArray ( GMS.perro.queryPerrosUsuario(GMS.userData.id) );
		GMS.db.CloseDB();


		//cargar los perros en el header
		GameObject perrosItem = GameObject.Find ("perro_item");
		if (result.Count > 0) {
			foreach (string[] row_ in result) {
				GameObject clone = Instantiate(perrosItem, perrosItem.transform.position, perrosItem.transform.rotation) as GameObject;
				clone.transform.SetParent(perrosItem.transform.parent);
				clone.transform.localScale = new Vector3(1, 1, 1);
				clone.name = "perro_" + row_[0];

				Sprite sprite_ = GMS.spriteFromFile ( row_[4] );
				clone.GetComponent<Image> ().sprite = sprite_;

				Debug.Log("perro: " + row_[1]);

				GMS.perro.populatePerro (row_);
			}
			changePerroData ();
		}else {
			Application.LoadLevel ("cargar-invitar");
		}
		
		Destroy (perrosItem);

		//cargar los familiares del perro seleccionado

	}

	private void changePerroData(){

		Sprite sprite_ = GMS.spriteFromFile (GMS.perro.foto);
		perro_bck.GetComponent<Image> ().sprite = sprite_;

		TPuntos.GetComponent<Text> ().text = float.Parse( GMS.perro.puntos ).ToString ("n2") + " PUNTOS";
		TKm.GetComponent<Text> ().text = float.Parse( GMS.perro.kilometros ).ToString ("n2") + " KM";

		GMS.paseoPerroId = GMS.perro.id;
	}

	void Awake(){

	}

	// Update is called once per frame
	void Update () {
	
	}

	public void cargarEscena(string pageName){
		Application.LoadLevel(pageName);
	}

	public void changePerro(GameObject perroItem){
		string[] idPerro = perroItem.name.Split('_');

		GMS.db.OpenDB("millasperrunas.db");
		ArrayList result = GMS.db.BasicQueryArray ( GMS.perro.queryPerro(idPerro[1], GMS.userData.id) );
		GMS.db.CloseDB();
		
		foreach (string[] row_ in result) {
			Debug.Log(row_[1]);
			GMS.perro.populatePerro (row_);
		}
		changePerroData ();

	}
}