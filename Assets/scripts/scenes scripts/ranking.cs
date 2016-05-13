using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ranking : MonoBehaviour {

	private MainController GMS;
	public GameObject ItemDefault;
	public Text perroNombre;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		perroNombre.text = GMS.perro.nombre;

		GMS.db.OpenDB("millasperrunas.db");
		
		ArrayList result = GMS.db.BasicQueryArray ( GMS.userData.queryFamiliaRanking(GMS.perro.id.ToString()) );
		//familia.id, familia.email, familia.nombre, familia.foto, perros_usuarios.puntos_semana
		GMS.db.CloseDB();
		bool isFirst = true;
		float startOpacity = 1f;
		float xPosition = 0f;

		float maxXRestPos = ItemDefault.GetComponent<LayoutElement> ().preferredWidth;
		int maxPuntaje = 0;

		if (result.Count > 0) {
			foreach (string[] row_ in result) {
				if( int.Parse( row_[4] ) > maxPuntaje ){
					maxPuntaje = int.Parse( row_[4] );
				}
			}
		}

		if (result.Count > 0) {
			foreach (string[] row_ in result) {
				GameObject clone = Instantiate(ItemDefault, ItemDefault.transform.position, ItemDefault.transform.rotation) as GameObject;
				clone.transform.SetParent(ItemDefault.transform.parent);
				clone.transform.localScale = new Vector3(1, 1, 1);
				clone.name = "usuario_" + row_[0];

				Debug.Log("cargar imagen de usuario: " + row_[3]);
				clone.transform.Find("Panel/Panel/PerfilMask/ImagePerfil").GetComponent<Image> ().sprite = GMS.spriteSquareFromFile ( row_[3] );
				clone.transform.Find("Panel/Panel/PerfilNombre").GetComponent<Text>().text = row_[2];
				clone.transform.Find("Panel/PanelP/PerfilPuntos").GetComponent<Text>().text = row_[4]+" PTS.";

				if(isFirst){
					isFirst = false;
				}else{
					startOpacity = startOpacity*0.8f;

					int puntosInt = int.Parse(row_[4]);

					if(puntosInt == 0){
						xPosition = maxXRestPos * -1f;
					}else{
						float PuntosPos = maxXRestPos * ( 100 - ( 100 * puntosInt / maxPuntaje ) ) / 100;
						xPosition = (float)PuntosPos * -1f;
						//xPosition = xPosition*0.8f;
					}
					Debug.Log("Puntaje: " + puntosInt + " | screen width: " + Screen.width + " | maxPuntaje: " + maxPuntaje + " | xPosition: " + xPosition);

					//0 ------ 1

					//0 -----261--------- 4118 = 6.3%
					//0 ----------------- 397

					// 100 x 200 / 400 = % puntos (50%)
					// 1080 x 50 / 100 = 50%

					// maxXRestPos x ( 100 * puntosInt / maxPuntaje ) / 100
					// maxXRestPos x ( 100 - ( 100 * puntosInt / maxPuntaje ) ) / 100



					GameObject PanelRelleno = clone.transform.Find("PanelRelleno").gameObject;
					Color cloneColor = PanelRelleno.GetComponent<Image>().color;
					PanelRelleno.GetComponent<Image>().color = new Color(cloneColor.r, cloneColor.g, cloneColor.b, startOpacity );

					//PanelRelleno.transform.localScale = new Vector3(PanelRelleno.transform.localScale.x * startOpacity,PanelRelleno.transform.localScale.y,PanelRelleno.transform.localScale.z);
					//xPosition = xPosition / PanelRelleno.transform.parent.position.x;
					//xPosition = -198f;

					//xPosition = ItemDefault.GetComponent<LayoutElement> ().preferredWidth;
					//xPosition = xPosition*-1f;

					//PanelRelleno.transform.position = new Vector3(xPosition, PanelRelleno.transform.position.y, PanelRelleno.transform.position.z);

					PanelRelleno.transform.localPosition = new Vector3(xPosition, 0, 0);

					Debug.Log("preferredWidth: " + ItemDefault.GetComponent<LayoutElement> ().preferredWidth );
					Debug.Log("PanelRelleno.transform.parent.position.x" + PanelRelleno.transform.parent.position.x);

					clone.transform.Find("Panel/Panel/IcoCorona").gameObject.SetActive(false);
				}

				if(int.Parse(row_[0]) == GMS.userData.id){
					clone.transform.Find("Panel/IcoMgs").gameObject.SetActive(false);
				}

			}
		}
		ItemDefault.SetActive (false);
	}
	
	public void gotoChat(GameObject option_){
		string[] idUsuario = option_.name.Split('_');
		if (int.Parse (idUsuario [1]) != GMS.userData.id) {
			PlayerPrefs.SetString("chatUserId", idUsuario [1]);
			Application.LoadLevel("chat");
		}
	}

	public void gotoScene(string scene){
		Application.LoadLevel(scene);
	}
}
