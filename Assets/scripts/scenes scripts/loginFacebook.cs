using UnityEngine;
using System.Collections;
using Facebook.MiniJSON;

public class loginFacebook : MonoBehaviour {
	private bool enabled = false;
	private MainController GMS;

	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		FB.Init(SetInit, OnHideUnity);
	}

	public void btnLogin(){
		FB.Login("email,user_birthday,user_friends", AuthCallback);
		//Application.LoadLevel("loader");
	}

	void AuthCallback(FBResult result) {
		if(FB.IsLoggedIn) {
			FB.API("/me?fields=id,name,first_name,last_name,email,birthday,gender", Facebook.HttpMethod.GET, APICallback);


			/*GameObject GM = GameObject.FindGameObjectWithTag ("mainController");
			mainController GMS = GM.GetComponent<mainController>();
			GMS.FbUserId = FB.UserId;
			print (GMS.FbUserId);*/

			Debug.Log(FB.UserId);
		} else {
			GMS.errorPopup("Ocurrio un error con el login de facebook, por favor intentalo nuevamente.");
			Debug.Log("User cancelled login");
		}
	}
	
	private void SetInit() {
		enabled = true; 
		print ("fb init");
		// "enabled" is a magic global; this lets us wait for FB before we start rendering
	}
	
	private void OnHideUnity(bool isGameShown) {
		/*if (!isGameShown) {
			// pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// start the game back up - we're getting focus again
			Time.timeScale = 1;
		}*/
	}

	void APICallback(FBResult result){
		if (result.Error != null){
			// Let's just try again
			FB.API("/me?fields=id,name,first_name,last_name,email,birthday,gender,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, APICallback);
			return;
		}
		GameObject GM = GameObject.Find ("MainController");
		MainController GMS = GM.GetComponent<MainController>();

		IDictionary search = (IDictionary) Json.Deserialize (result.Text);

		GMS.userData.fbid = (string)search ["id"];
		GMS.userData.nombre = (string)search ["first_name"] + ' '+ (string)search ["last_name"];
		GMS.userData.email = (string)search ["email"]+"";

		if ((string)search ["gender"]+"" == "female") {
			GMS.userData.sexo = "Mujer";
		} else {
			GMS.userData.sexo = "Hombre";
		}

		//GMS.userData.sexo = (string)search ["gender"]+"";
		GMS.userData.fecha_nacimiento = (string)search ["birthday"]+"";

		if (GMS.userData.fecha_nacimiento != "" && GMS.userData.fecha_nacimiento != null) {
			string[] fn = GMS.userData.fecha_nacimiento.Split('/');
			GMS.userData.date_day = fn[1];
			GMS.userData.date_month = int.Parse(fn[0]).ToString();
			GMS.userData.date_year = fn[2];
		}

		Debug.Log ("fbid: " + GMS.userData.fbid + " | nombre: " + GMS.userData.nombre + " | email: " + GMS.userData.email + " | sexo: " + GMS.userData.sexo + "fecha nacimiento: " + GMS.userData.fecha_nacimiento );

		GMS.showLoading(true);
		FB.API("me/picture?type=large", Facebook.HttpMethod.GET, GetPicture);
	}

	private void GetPicture(FBResult result)
	{
		if (result.Error == null) {
			
			GMS.userData.temp_img = GMS.generateId ().ToString ()  + ".png";
			StartCoroutine (GMS.saveTextureToFile (result.Texture, GMS.userData.temp_img, 'u'));
			
			StartCoroutine (loginFacebook_ ());
			
			/*Image img = UIFBProfilePic.GetComponent<Image>();
			img.sprite = Sprite.Create(result.Texture, new Rect(0,0, 128, 128), new Vector2());*/
		} else {
			GMS.showLoading(false);
			GMS.errorPopup("Ocurrio un error con el login de facebook, por favor intentalo nuevamente.");
		}
		
	}
	
	private IEnumerator loginFacebook_(){
		yield return new WaitForSeconds (3);
		//GMS.showLoading(false);
		GMS.loginFacebook ();
	}
}

