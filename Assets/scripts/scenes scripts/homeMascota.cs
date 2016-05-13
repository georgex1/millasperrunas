using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class homeMascota : MonoBehaviour {

	private MainController GMS;
	public GameObject TPuntos;
	public GameObject TKm;

	public GameObject perro_bck;
	public GameObject familiaItem;

	private ArrayList famliaList;

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

				Sprite sprite_ = GMS.spriteSquareFromFile ( row_[4] );
				clone.transform.Find("PanelImage").GetComponent<Image> ().sprite = sprite_;

				Debug.Log("perro: " + row_[1]);

				GMS.perro.populatePerro (row_);
			}
			changePerroData ();
			StartCoroutine ( updatePuntosPerro() );

		}else {
			Application.LoadLevel ("cargar-invitar");
		}
		
		Destroy (perrosItem);

		//populateFamilia ();

	}

	private IEnumerator updatePuntosPerro(){
		yield return new WaitForSeconds (1);

		TPuntos.GetComponent<Text> ().text = GMS.perro.puntos + " PUNTOS";
		TKm.GetComponent<Text> ().text = float.Parse( GMS.perro.kilometros ).ToString ("n2") + " KM";

		StartCoroutine ( updatePuntosPerro() );
	}

	private void populateFamilia(){

		familiaItem.SetActive (false);

		//borrar la familia anterior
		GameObject[] panels = GameObject.FindGameObjectsWithTag("familiaTag");
		for (int i = 0; i < panels.Length; i++) {
			if(panels[i].activeSelf){
				Destroy(panels[i]);
			}
		}

		//cargar los familiares del perro seleccionado
		GMS.db.OpenDB("millasperrunas.db");
		
		ArrayList result = GMS.db.BasicQueryArray ( GMS.userData.queryFamiliaRanking(GMS.perro.id.ToString()) );
		GMS.db.CloseDB();
		
		familiaItem.SetActive (true);

		if (result.Count > 0) {
			foreach (string[] row_ in result) {
				if(row_[3] != ""){
					GameObject clone = Instantiate(familiaItem, familiaItem.transform.position, familiaItem.transform.rotation) as GameObject;
					clone.transform.SetParent(familiaItem.transform.parent);
					clone.transform.localScale = new Vector3(1, 1, 1);
					clone.name = "familia_" + row_[0];
					
					Sprite sprite_ = GMS.spriteSquareFromFile ( row_[3] );
					clone.transform.Find("PanelImage").GetComponent<Image> ().sprite = sprite_;
					//clone.GetComponent<Image> ().sprite = sprite_;
				}
			}
		}
		
		familiaItem.SetActive (false);
	}

	private void changePerroData(){

		Sprite sprite_ = GMS.spriteFromFile (GMS.perro.foto);
		perro_bck.GetComponent<Image> ().sprite = sprite_;

		//TPuntos.GetComponent<Text> ().text = float.Parse( GMS.perro.puntos ).ToString ("n2") + " PUNTOS";
		TPuntos.GetComponent<Text> ().text = GMS.perro.puntos + " PUNTOS";
		TKm.GetComponent<Text> ().text = float.Parse( GMS.perro.kilometros ).ToString ("n2") + " KM";

		GMS.paseoPerroId = GMS.perro.id;

		populateFamilia ();
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

	public void deletePerroUsuario(){
		GMS.QestionPopup ("deletePerroUsuario", "¿Estás seguro que querés dejar de pasear a "+GMS.perro.nombre+"?");
	}
}
