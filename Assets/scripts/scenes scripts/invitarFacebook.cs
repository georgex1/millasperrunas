using UnityEngine;
using System.Collections;
using Facebook.MiniJSON;
using System.Collections.Generic;
using UnityEngine.UI;

public class invitarFacebook : MonoBehaviour {

	private bool enabled = false;

	public GameObject DDFacebook;
	public GameObject FOption;
	
	void Start () {
		FB.Init(SetInit, OnHideUnity);
	}
	
	public void btnLogin(){
		FB.Login("email,user_birthday,user_friends", AuthCallback);
		//Application.LoadLevel("loader");
	}
	
	void AuthCallback(FBResult result) {
		if(FB.IsLoggedIn) {
			FB.API("me?fields=id,name,first_name,last_name,email,birthday,gender,friends.limit(100).fields(first_name,last_name,email,id,gender,birthday)", Facebook.HttpMethod.GET, APICallback);
			
			Debug.Log(FB.UserId);
		} else {
			Debug.Log("User cancelled login");
		}
	}
	
	private void SetInit() {
		enabled = true; 
		print ("fb init");
		// "enabled" is a magic global; this lets us wait for FB before we start rendering
	}
	
	private void OnHideUnity(bool isGameShown) {
	}
	
	void APICallback(FBResult result){
		if (result.Error != null){
			// Let's just try again
			FB.API("me?fields=id,name,first_name,last_name,email,birthday,gender,friends.limit(100).fields(first_name,last_name,email,id,gender,birthday)", Facebook.HttpMethod.GET, APICallback);
			return;
		}
		var dict = Json.Deserialize(result.Text) as Dictionary<string,object>;
		//string userName = dict["name"];
		
		object friendsH;
		var friends = new List<object>();
		string friendName;
		//DDFacebook.transform.fin
		GameObject OptionDefault = FOption;
		OptionDefault.SetActive (true);
		DDFacebook.SetActive(true);

		if(dict.TryGetValue ("friends", out friendsH)) {
			friends = (List<object>)(((Dictionary<string, object>)friendsH) ["data"]);
			if(friends.Count > 0) {
				foreach(object ff in friends){
					var friendDict = ((Dictionary<string,object>)(ff));
					Debug.Log((string)friendDict["first_name"]);

					string nameVar = (string)friendDict["first_name"] + " " + (string)friendDict["last_name"];

					GameObject clone = Instantiate(OptionDefault, OptionDefault.transform.position, OptionDefault.transform.rotation) as GameObject;
					clone.transform.SetParent(OptionDefault.transform.parent);
					clone.transform.localScale = new Vector3(1, 1, 1);
					clone.GetComponentInChildren<Text> ().text = nameVar;
					//clone.GetComponentInChildren<Text> ().name = (string)friendDict["first_name"] + "__" + (string)friendDict["email"];
				}
				/*
				var friendDict = ((Dictionary<string,object>)(friends[0]));
				var friend = new Dictionary<string, string>();
				friend["id"] = (string)friendDict["id"];
				friend["first_name"] = (string)friendDict["first_name"];
				Debug.Log(friend["first_name"]);*/
			}
		}

		OptionDefault.SetActive (false);


	}
}

public class FbUser2 {
	public string fb_user { set; get; }
	
	public FbUser2(){
	}
}

public class FbUser {
	public string id { set; get; }
	public string first_name { set; get; }
	public string last_name { set; get; }
	public string email { set; get; }

	public FbUser(){
		id = "";
		first_name = "";
		last_name = "";
		email = "";
	}
}

public class FbUsers{
	public List<FbUser> FbUsers_ { get; set; }
}
