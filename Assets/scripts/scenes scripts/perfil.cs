using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class perfil : MonoBehaviour {

	private MainController GMS;
	public GameObject headerImage;

	public GameObject defaultBadge;
	public Text UserName;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		headerImage.GetComponent<Image> ().sprite = GMS.spriteFromFile (GMS.perro.foto);
		UserName.text = GMS.perro.nombre;

		//badges
		GMS.db.OpenDB("millasperrunas.db");
		
		ArrayList result = GMS.db.BasicQueryArray ( GMS.userData.queryBadgesUsuario(GMS.userData.id, GMS.perro.id) );
		if (result.Count > 0) {
			int i = 1;
			string parentObj = "";
			foreach (string[] row_ in result) {
				GameObject clone = Instantiate(defaultBadge, defaultBadge.transform.position, defaultBadge.transform.rotation) as GameObject;

				if(i == 1 || i == 4 || i == 7 || i == 10){
					parentObj = "PanelV1";
				}
				if(i == 2 || i == 5 || i == 8 || i == 11){
					parentObj = "PanelV2";
				}
				if(i == 3 || i == 6 || i == 9 || i == 12){
					parentObj = "PanelV3";
				}
				//clone.transform.SetParent(defaultBadge.transform.parent);
				clone.transform.SetParent(GameObject.Find (parentObj).transform);

				clone.transform.localScale = new Vector3(1, 1, 1);
				clone.name = "badge_" + row_[0];
				
				Sprite sprite_ = GMS.spriteFromFile ( row_[2] );
				clone.GetComponent<Image> ().sprite = sprite_;
				i++;
			}

		}
		GMS.db.CloseDB();

		Destroy (defaultBadge);
	}

	public void goBack(){
		Application.LoadLevel ("home");
	}

	public void goBadge(GameObject badgeItem){
		string[] idBadge = badgeItem.name.Split('_');
		PlayerPrefs.SetString ("badgeId", idBadge[1]);
		Application.LoadLevel ("badge-detail");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
