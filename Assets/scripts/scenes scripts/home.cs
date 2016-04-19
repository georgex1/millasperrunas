using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class home : MonoBehaviour {
	//private dbAccess db ;
	private MainController GMS;

	//public GameObject sumPuntos;
	//public GameObject sumKm;
	public GameObject indPuntos;
	public GameObject indKm;
	public GameObject famPuntos;
	public GameObject famKm;
	public GameObject perroNombre;

	public GameObject perro_bck;

	// Use this for initialization
	void Start () {

		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();


		GMS.db.OpenDB("millasperrunas.db");
		ArrayList result = GMS.db.BasicQueryArray ( GMS.perro.queryPerro(GMS.perro.id.ToString(), GMS.userData.id) );
		
		GMS.db.CloseDB();
		
		if (result.Count > 0) {
			foreach (string[] row_ in result) {
				GMS.perro.populatePerro (row_);
			}
		} else {
			Application.LoadLevel("cargar-invitar");
		}


		//db = GetComponent<dbAccess>();
		/*GMS.db.OpenDB("millasperrunas.db");

		ArrayList result = GMS.db.BasicQueryArray ( GMS.perro.queryPerrosUsuario(GMS.userData.id) );
		GMS.db.CloseDB();

		if (result.Count > 0) {
			foreach (string[] row_ in result) {
				GMS.perro.populatePerro (row_);
			}*/

		Debug.Log ("km perro: "+GMS.perro.kilometros+ " | puntos perro: "+GMS.perro.puntos);

		indPuntos.GetComponent<Text>().text = float.Parse( GMS.perro.puntos ).ToString("n2")+ " PTS.";
		indKm.GetComponent<Text>().text = float.Parse( GMS.perro.kilometros ).ToString("n2")+ " KM";

		GMS.db.OpenDB("millasperrunas.db");
		ArrayList resultF = GMS.db.BasicQueryArray ( GMS.perro.queryPerrosFamilia(GMS.perro.id.ToString()) );
		GMS.db.CloseDB();

		if (resultF.Count > 0) {

			Debug.Log("famKm: " + ((string[])resultF [0]) [0] + " | famPuntos: " + ((string[])resultF [0]) [1]);

			famKm.GetComponent<Text>().text = float.Parse( ((string[])resultF [0]) [0] ).ToString("n2")+ " KM";
			famPuntos.GetComponent<Text>().text = float.Parse( ((string[])resultF [0]) [1] ).ToString("n2")+ " PTS.";

			/*foreach (string[] row2_ in resultF) {
				famKm.GetComponent<Text>().text = float.Parse( row2_[0] ).ToString("n2")+ " KM";
				famPuntos.GetComponent<Text>().text = float.Parse( row2_[1] ).ToString("n2")+ " PTS.";

				float sumPuntos_ = float.Parse( row2_[1] ) + float.Parse( GMS.perro.puntos );
				float sumKm_ = float.Parse( row2_[0] ) + float.Parse( GMS.perro.kilometros );

				sumPuntos.GetComponent<Text>().text = sumPuntos_.ToString("n2")+ " PTS.";
				sumKm.GetComponent<Text>().text = sumKm_.ToString("n2")+ " KM";
			}*/
		}

		perroNombre.GetComponent<Text>().text = GMS.perro.nombre;

		Sprite sprite_ = GMS.spriteFromFile (GMS.perro.foto);

		perro_bck.GetComponent<Image> ().sprite = sprite_;

		/*} else {
			Application.LoadLevel ("cargar-invitar");
		}*/
	}

	public void cargarEscena(string pageName){
		Application.LoadLevel(pageName);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
