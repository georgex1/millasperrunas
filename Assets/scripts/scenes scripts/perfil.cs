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

		headerImage.GetComponent<Image> ().sprite = GMS.spriteFromFile (GMS.userData.foto);
		UserName.text = GMS.userData.nombre;

		//badges
		GMS.db.OpenDB("millasperrunas.db");
		
		ArrayList result = GMS.db.BasicQueryArray ( GMS.userData.queryBadgesUsuario(GMS.userData.id) );
		if (result.Count > 0) {

			foreach (string[] row_ in result) {
				GameObject clone = Instantiate(defaultBadge, defaultBadge.transform.position, defaultBadge.transform.rotation) as GameObject;
				clone.transform.SetParent(defaultBadge.transform.parent);
				clone.transform.localScale = new Vector3(1, 1, 1);
				clone.name = "badge_" + row_[0];
				
				Sprite sprite_ = GMS.spriteFromFile ( row_[2] );
				clone.GetComponent<Image> ().sprite = sprite_;

			}

		}
		GMS.db.CloseDB();
	}

	public void goBack(){
		Application.LoadLevel ("home");
	}

	public void goBadge(GameObject badgeItem){
		string[] idBadge = badgeItem.name.Split('_');
		PlayerPrefs.SetString ("badgeId", idBadge[1]);
		Application.LoadLevel ("badgeDetail");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
