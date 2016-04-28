using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.UI;
using Facebook.MiniJSON;

public class puntos : MonoBehaviour {

	private MainController GMS;
	private string URLGmaps;
	private string URLGmapsShare;
	private byte[] ImgBytesShare;
	public GameObject GmapImage;
	private string localImageName;

	public Image GmapImageS;
	public GameObject shareBtn;

	public Text perroNombre;

	private string shareImg = "";
	public GameObject textCargando;

	// Use this for initialization
	void Start () {

		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		perroNombre.text = GMS.perro.nombre;

		int mapH = (int)Math.Truncate (Screen.height * 0.19f);

		Debug.Log ("Map size: " + Screen.width + " x " + mapH);

		GMS.db.OpenDB("millasperrunas.db");
		ArrayList result = GMS.db.BasicQueryArray("select lat, lng, id from ultimo_recorrido where perros_id = '"+GMS.perro.id+"' ");
		GMS.db.CloseDB();

		if (result.Count > 0) {
			string latList = "";

			foreach (string[] row_ in result) {
				latList += row_[0] + "," + row_[1] + "|";
				shareImg = row_[2];
			}
			latList = latList.Remove(latList.Length - 1);

			URLGmaps = "https://maps.googleapis.com/maps/api/staticmap?maptype=satellite&size=" + Screen.width + "x" + mapH + "&path=color:0x00CC00|weight:5|"+latList;
			URLGmapsShare = "https://maps.googleapis.com/maps/api/staticmap?maptype=satellite&size=400x400&path=color:0x00CC00|weight:5|"+latList;

			Debug.Log(URLGmaps);

			//URLGmaps = "https://maps.googleapis.com/maps/api/staticmap?size=" + Screen.width + "x" + mapH + "&path=color:0x0000ff|weight:5|40.737102,-73.990318|40.749825,-73.987963|40.752946,-73.987384|40.755823,-73.986397";
			StartCoroutine (setGoogleMaps());

		}

		if (shareImg == "" || !GMS.haveInet) {
			shareBtn.SetActive(false);
			textCargando.SetActive (false);
		}

		FB.Init(SetInit, OnHideUnity);
	}

	public void shareFacebookBtn(){
		GMS.showLoading (true);
		GMS.upload_recorrido(URLGmapsShare, shareImg);
		StartCoroutine (shareFacebookw ());
	}

	private IEnumerator shareFacebookw(){
		yield return new WaitForSeconds (3);
		GMS.showLoading (false);
		shareFacebook ();
	}

	private void shareFacebook()
	{
		if (FB.IsLoggedIn) {
			FB.Feed (
				linkCaption: "Purina Walk",
				picture: GMS.getShareImage(shareImg),
				linkName: "Mi ultimo paseo con " + GMS.perro.nombre,
				link: "https://play.google.com/store"
				);
		} else {
			CallFBLogin();
		}
	}
	private void CallFBLogin()
	{
		FB.Login("email", LoginCallback); //publish_actions
	}
	
	void LoginCallback(FBResult result)
	{
		if (result.Error != null) {
			GMS.errorPopup("Ocurrio un error, intentalo nuevamente: " + result.Error);
		}else if (!FB.IsLoggedIn){

		}else
		{
			shareFacebook();
		}
	}

	private void SetInit() {
		print ("fb init");
	}
	private void OnHideUnity(bool isGameShown) {}

	/*public void shareFacebook(){
		FB.Login("publish_actions,user_photos", AuthCallback);
	}

	void AuthCallback(FBResult result) {
		if(FB.IsLoggedIn) {

			var wwwForm = new WWWForm();
			wwwForm.AddBinaryData("image", ImgBytesShare, "InteractiveConsole.png");
			
			FB.API("me/photos", Facebook.HttpMethod.POST, APICallback, wwwForm);
			
			Debug.Log(FB.UserId);
		} else {
			GMS.errorPopup("Ocurrio un error con el login de facebook, por favor intentalo nuevamente.");
			Debug.Log("User cancelled login");
		}
	}

	void APICallback(FBResult result){
		if (result.Error != null) {
			GMS.errorPopup("Ocurrio un error, intentalo nuevamente.");
			//FB.API("me/photos", Facebook.HttpMethod.POST, APICallback, wwwForm);
			return;
		}
	}*/

	public void cargarEscena(string pageName){
		Application.LoadLevel(pageName);
	}

	private IEnumerator setGoogleMaps(){

		int timestamp = (int)Math.Truncate((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
		localImageName = timestamp.ToString () + ".png";

		Texture2D texture = new Texture2D (1, 1);

		WWW www = new WWW (URLGmaps);

		yield return www;
		www.LoadImageIntoTexture (texture);
		
		byte[] ImgBytes = texture.EncodeToPNG ();
		Sprite sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0f, 0f));

		GmapImageS.sprite = sprite;
		textCargando.SetActive (false);
	}
}
