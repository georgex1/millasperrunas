using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class badgeDetail : MonoBehaviour {

	private MainController GMS;
	public Image badgeImage;
	public Text badgeTitle;
	public Text badgeDescription;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		string badgeId = PlayerPrefs.GetString ("badgeId");

		ArrayList result = GMS.db.BasicQueryArray ( "select id, nombre, foto, descripcion from badges where id = '"+badgeId+"' " );
		if (result.Count > 0) {
			badgeTitle.text = ((string[])result [0]) [1];
			badgeDescription.text = ((string[])result [0]) [3];
			badgeImage.sprite = GMS.spriteFromFile ( ((string[])result [0]) [2] );
		}
	}

	public void goBack(){
		Application.LoadLevel ("perfil");
	}
}
