﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;
using VoxelBusters.AssetStoreProductUtility.Demo;

public class chatTest : MonoBehaviour {
	SimChat sc;
	string sender;
	protected Vector2 sp = Vector2.zero;
	public InputField Mgs;
	float rt1=-3f;
	private MainController GMS;
	
	private Sprite userImage;
	private Sprite amigoImage;

	private UserData amigoData;
	
	public GameObject chatYo;
	public GameObject chatEl;
	private float yChatCount = 0f;
	private float sumScroll = 0;

	public GameObject ScrollObj;
	private string MgsTextNotif;
	
	// Use this for initialization
	void Start () {
		amigoData = new UserData();
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		string amigoId = PlayerPrefs.GetString ("chatUserId");

		GMS.db.OpenDB("millasperrunas.db");
		ArrayList result = GMS.db.BasicQueryArray ("select familia.id, familia.email, familia.nombre, familia.foto, perros_usuarios.chat_group from familia " +
		                                           "inner join perros_usuarios on perros_usuarios.usuarios_id = familia.id where familia.id = "+amigoId+" ");
		GMS.db.CloseDB ();

		Debug.Log ("amigo id: " + ((string[])result [0]) [0]);

		amigoData.id = int.Parse( ((string[])result [0]) [0] );
		amigoData.email = ((string[])result [0]) [1];
		amigoData.nombre = ((string[])result [0]) [2];
		amigoData.foto =  ((string[])result [0]) [3];

		amigoImage = GMS.spriteSquareFromFile (((string[])result [0]) [3]);
		userImage = GMS.spriteSquareFromFile ( GMS.userData.foto );

		Application.runInBackground = true;
		//string chatGroup = ((string[])result [0]) [4];

		string chatGroup = multipKeys ( ((string[])result [0]) [4], GMS.perro.chat_group );

		/*Debug.Log ("chat group vals: " + ((string[])result [0]) [4] + " | " + GMS.perro.chat_group);
		Debug.Log ("chat group: " + chatGroup);*/
		
		sender = GMS.userData.nombre;
		sc = new SimChat(chatGroup, gameObject.GetComponent<MonoBehaviour>(), sender);
		
		sc.continueCheckMessages();
		sc.setReceiveFunction(receiveMessage1);

		chatYo.SetActive (false);
		chatEl.SetActive (false);
		//displayChat2 ();

		//myScrollRect.verticalNormalizedPosition = 0.5f;
		yChatCount = chatYo.transform.position.y;
		//RectTransform rectTransform = GetComponent<RectTransform>();
		//rectTransform.pivot = new Vector2(transform.pivot.x, 0);
	}

	private string multipKeys(string str1, string str2){
		double newKey = double.Parse(str1) + double.Parse(str2);
		return newKey.ToString();
	}

	//functions to call when a new message is received
	void receiveMessage1(SimpleMessage[] sm){
		rt1 = Time.time;
		Debug.Log (sm.Length);

		foreach(SimpleMessage smIn in sm){

			GameObject clone;
			//check if the sender had the same name as me, and change the color
			if(smIn.sender == sender){ //yo
				clone = Instantiate(chatYo, chatYo.transform.position, chatYo.transform.rotation) as GameObject;
				clone.transform.SetParent(chatYo.transform.parent);
				clone.transform.Find("PanelC/Panel/PerfilMask/ImagePerfil").GetComponent<Image> ().sprite = userImage;

			}else{
				clone = Instantiate(chatEl, chatEl.transform.position, chatEl.transform.rotation) as GameObject;
				clone.transform.SetParent(chatEl.transform.parent);
				clone.transform.Find("PanelC/Panel/PerfilMask/ImagePerfil").GetComponent<Image> ().sprite = amigoImage;

			}


			//get date:
			System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
			dateTime = dateTime.AddSeconds( Double.Parse(smIn.time.ToString()) );
			string printDate = dateTime.ToShortDateString() +" "+ dateTime.ToLongTimeString();

			//sumScroll += clone.transform.GetComponent<Text> ().preferredHeight;
			//Debug.Log("preferredHeight: " +clone.transform.GetComponent<Text> ().preferredHeight);
			//yChatCount -= 0.5f;

			//clone.GetComponent<RectTransform>().pivot = new Vector2(GameObject.Find("PanelContent").GetComponent<RectTransform>().pivot.y, 0);
			clone.transform.localScale = new Vector3(1, 1, 1);
			clone.SetActive(true);
			//Debug.Log(smIn.message);
			clone.transform.Find("PanelC/PanelP/TextMgs").GetComponent<Text> ().text = smIn.message.Replace("&#10;"," ");
			clone.transform.Find("PanelC/PanelP/TextTime").GetComponent<Text> ().text = smIn.sender + " " + printDate;


			Debug.Log("dice q fecha: " + printDate);


			//ScrollObj.GetComponent<ScrollRect>().verticalNormalizedPosition = -1;
			//normalizedPosition
			//ScrollObj.GetComponent<RectTransform>().sizeDelta = new Vector2(ScrollObj.GetComponent<RectTransform>().rect.width, sumScroll); 
		}



		//string _nID = ScheduleLocalNotification (CreateNotification (1, eNotificationRepeatInterval.NONE));

		StartCoroutine (scrollDown ());
	}
	private IEnumerator scrollDown(){
		yield return new WaitForSeconds (0.1f);
		ScrollObj.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
	}

	private TouchScreenKeyboard keyboard;
	
	// Update is called once per frame
	void Update () {
		/*if (Input.GetKeyDown("enter")){
			sendMgs();
			keyboard.active = false;
		}*/

		/*Event e = Event.current;
		if (e.type == EventType.keyDown && e.keyCode == KeyCode.Return) {
			sendMgs();
		}*/
	}

	void OnGUI(){
		/*GUI.skin.textField.fontSize = GUI.skin.button.fontSize = GUI.skin.label.fontSize = 10;
		displayChat(new Rect(0, Screen.height * 0.1f, Screen.width, Screen.height*0.8f),sc,sp);*/
	}
	
	/*void displayChat(Rect area,SimChat sc,Vector2 sp){
		sp.y = Mathf.Infinity;
		GUILayout.BeginArea(area);
		
		GUILayout.BeginVertical("box");
		sp = GUILayout.BeginScrollView(sp);
		Color c = GUI.contentColor;
		//loop through each of the messages contained in allMessages
		foreach(SimpleMessage sm in sc.allMessages){
			GUILayout.BeginHorizontal();
			//check if the sender had the same name as me, and change the color
			if(sm.sender == sender){
				GUI.contentColor = Color.red;
				GUILayout.FlexibleSpace();
				GUILayout.Label(sm.message);
			}else{
				GUI.contentColor = Color.green;
				GUILayout.Label(sm.sender+": "+sm.message);
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}
		GUI.contentColor = c;
		GUILayout.EndScrollView();
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}*/
	
	public void sendMgs(){
		MgsTextNotif = Mgs.text;

		if (MgsTextNotif != "") {

			Debug.Log (Mgs.text);
			sc.message = Mgs.text;
			sc.sendMessage ();
			sc.message = "";
			Mgs.text = "";
		
			/*sync para notificaciones*/
			string[] fields = {"usuarios_id", "amigos_id", "texto", "perros_id"};
			string[] values = {GMS.userData.id.ToString (), amigoData.id.ToString (), MgsTextNotif, GMS.perro.id.ToString ()};
			GMS.insert_sync (fields, values, "notificacion_chat");
		
			Debug.Log ("mgs enviado..");
		}
	}

	/*#region API Calls
	private string ScheduleLocalNotification(CrossPlatformNotification _notification)
	{
		return NPBinding.NotificationService.ScheduleLocalNotification(_notification);
	}
	#endregion
	
	#region Misc. Methods
	
	private CrossPlatformNotification CreateNotification (long _fireAfterSec, eNotificationRepeatInterval _repeatInterval)
	{
		// User info
		IDictionary _userInfo			= new Dictionary<string, string>();
		_userInfo["data"]				= "add what is required";
		
		CrossPlatformNotification.iOSSpecificProperties _iosProperties			= new CrossPlatformNotification.iOSSpecificProperties();
		_iosProperties.HasAction		= true;
		_iosProperties.AlertAction		= GMS.appName;
		
		CrossPlatformNotification.AndroidSpecificProperties _androidProperties	= new CrossPlatformNotification.AndroidSpecificProperties();
		_androidProperties.ContentTitle	= amigoData.nombre;
		_androidProperties.TickerText	= GMS.appName;
		_androidProperties.CustomSound	= "Notification.mp3"; //Keep the files in Assets/StreamingAssets/VoxelBusters/NativePlugins/Android folder.
		//_androidProperties.LargeIcon	= "NativePlugins.png"; //Keep the files in Assets/StreamingAssets/VoxelBusters/NativePlugins/Android folder.
		if (GMS.checkImageExists (amigoData.foto)) {
			_androidProperties.LargeIcon = Application.persistentDataPath + "/" + amigoData.foto;
		} else {
			_androidProperties.LargeIcon	= "default.jpg";
		}
		
		CrossPlatformNotification _notification	= new CrossPlatformNotification();
		_notification.AlertBody			= MgsTextNotif; //On Android, this is considered as ContentText
		_notification.FireDate			= System.DateTime.Now.AddSeconds(_fireAfterSec);
		_notification.RepeatInterval	= _repeatInterval;
		_notification.UserInfo			= _userInfo;
		_notification.iOSProperties		= _iosProperties;
		_notification.AndroidProperties	= _androidProperties;
		
		return _notification;
	}
	
	#endregion
*/

	public void goBack() {
		Application.LoadLevel("ranking");
	}

}
