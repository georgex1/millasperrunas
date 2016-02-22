using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ranking : MonoBehaviour {

	private MainController GMS;
	public GameObject ItemDefault;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		GMS.db.OpenDB("millasperrunas.db");
		
		ArrayList result = GMS.db.BasicQueryArray ( GMS.userData.queryFamiliaRanking() );
		//familia.id, familia.email, familia.nombre, familia.foto, perros_usuarios.puntos_semana
		GMS.db.CloseDB();
		bool isFirst = true;
		float startOpacity = 0.9f;

		if (result.Count > 0) {
			foreach (string[] row_ in result) {
				GameObject clone = Instantiate(ItemDefault, ItemDefault.transform.position, ItemDefault.transform.rotation) as GameObject;
				clone.transform.SetParent(ItemDefault.transform.parent);
				clone.transform.localScale = new Vector3(1, 1, 1);
				clone.name = "usuario_" + row_[0];

				clone.transform.FindChild("ImagePerfil").GetComponent<Image> ().sprite = GMS.spriteFromFile ( row_[3] );
				clone.transform.FindChild("PerfilNombre").GetComponent<Text>().text = row_[2];
				clone.transform.FindChild("PerfilPuntos").GetComponent<Text>().text = row_[4]+" PTS.";

				if(isFirst){
					isFirst = false;
				}else{
					startOpacity = startOpacity*0.9f;

					GameObject PanelRelleno = clone.transform.FindChild("PanelRelleno").gameObject;
					PanelRelleno.renderer.material.color = new Color(0, 164, 79, startOpacity);
					PanelRelleno.transform.localScale = new Vector3(PanelRelleno.transform.localScale.x * startOpacity,PanelRelleno.transform.localScale.y,PanelRelleno.transform.localScale.z);

					clone.transform.FindChild("IcoCorona").gameObject.SetActive(false);
				}

				if(int.Parse(row_[0]) == GMS.userData.id){
					clone.transform.FindChild("IcoMgs").gameObject.SetActive(false);
				}

			}
		}
		Destroy (ItemDefault);
	}
	
	public void gotoChat(GameObject option_){
		string[] idUsuario = option_.name.Split('_');
		if (int.Parse (idUsuario [1]) != GMS.userData.id) {
			PlayerPrefs.SetString("chatUserId", idUsuario [1]);
			Application.LoadLevel("chat");
		}
	}
}
