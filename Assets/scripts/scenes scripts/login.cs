using UnityEngine;
using System.Collections;

public class login : MonoBehaviour {

	/*public Texture2D image;
	public GameObject imageTexture;
	public WebCamTexture mCamera = null;
	WebCamTexture webCamTexture;*/


	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		MainController GMS = GM.GetComponent<MainController>();

		//auto login usuario
		GMS.db.OpenDB("millasperrunas.db");
		ArrayList result = GMS.db.BasicQueryArray ("select id, email, nombre, fbid, fecha_nacimiento, sexo from usuarios limit 1");
		if (result.Count > 0) {
			GMS.userData.populateUser(  ((string[])result [0]) );
			/*foreach (string[] row_ in result) {
				GMS.userData.populateUser(row_);
			}*/

			Application.LoadLevel ("cargar-invitar");
		}
		GMS.db.CloseDB();


		/*mCamera = new WebCamTexture (768,1200,40);
		imageTexture.renderer.material.mainTexture = mCamera;
		mCamera.Play ();*/
	}

}
