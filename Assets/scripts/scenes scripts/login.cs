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
		ArrayList result = GMS.db.BasicQueryArray ("select id, email, nombre, fbid, fecha_nacimiento, sexo, foto from usuarios limit 1");
		if (result.Count > 0) {
			GMS.userData.populateUser2(  ((string[])result [0]) );

			StartCoroutine(GMS.updateRegId());

			/*foreach (string[] row_ in result) {
				GMS.userData.populateUser(row_);
			}*/
			if(GMS.userData.foto != "" && GMS.userData.foto != null ){
				GMS.toRedirectLogin = "cargar-invitar";
				Application.LoadLevel ("initGPS");
			}else{
				GMS.toRedirectLogin = "subir-foto";
				Application.LoadLevel ("initGPS");
			}


		}
		GMS.db.CloseDB();


		/*mCamera = new WebCamTexture (768,1200,40);
		imageTexture.renderer.material.mainTexture = mCamera;
		mCamera.Play ();*/
	}

}
