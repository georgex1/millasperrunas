using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using OAuth;

public class yahooWeather : MonoBehaviour{

	public string Condition;
	public string Temperature;
	public string WindSpeed;

	private string lat;
	private string lng;

	public yahooWeather(){

	}

	public yahooWeather(string lat_, string lng_){
		lat = lat_;
		lng = lng_;
	}

	public void GetWeather(string weid)
	{
		Debug.Log ("weid: " + weid);

//#if !UNITY_EDITOR

		string consumerKey = "dj0yJmk9ZHFMeWdhTUtYQ083JmQ9WVdrOVVITnNTV1JCTjJjbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD0yNA--";  
		string consumerSecret = "6ecda0d47f3249db309d82f63559a75a7edd6fcb";

		var uri = new Uri("http://weather.yahooapis.com/forecastrss?w=" + weid);  
		string url, param;  
		var oAuth = new OAuthBase();  
		var nonce = oAuth.GenerateNonce();  
		var timeStamp = oAuth.GenerateTimeStamp();  
		var signature = oAuth.GenerateSignature(uri, consumerKey,  
		                                        consumerSecret, string.Empty, string.Empty, "GET", timeStamp, nonce,  
		                                        OAuthBase.SignatureTypes.HMACSHA1, out url, out param);  

		string query = string.Format ("{0}?{1}&oauth_signature={2}", url, param, signature);
		Debug.Log (query);
		XmlDocument wData = new XmlDocument();
		wData.Load(query);
		
		XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
		manager.AddNamespace("yweather","http://xml.weather.yahoo.com/ns/rss/1.0");
		
		XmlNode channel = wData.SelectSingleNode("rss").SelectSingleNode("channel");
		XmlNodeList nodes = wData.SelectNodes("/rss/channel/item/yweather:forecast", manager);
		
		Condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;
		float temp = (float.Parse (channel.SelectSingleNode ("item").SelectSingleNode ("yweather:condition", manager).Attributes ["temp"].Value) - 32f) / 1.8f;
		//Temperature = temp.ToString ("n2");
		Temperature = string.Format ("{0:0}", temp);
		
		WindSpeed = channel.SelectSingleNode("yweather:wind", manager).Attributes["speed"].Value;

		Debug.Log ("se consiguio clima yahoo: viento: " + WindSpeed);

//#endif

		/*
		string consumerKey = "dj0yJmk9ZHFMeWdhTUtYQ083JmQ9WVdrOVVITnNTV1JCTjJjbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD0yNA--";  
		string consumerSecret = "6ecda0d47f3249db309d82f63559a75a7edd6fcb";
		
		var uri = new Uri("http://weather.yahooapis.com/forecastrss?w=" + weid);  
		string url, param;  
		var oAuth = new OAuthBase();  
		var nonce = oAuth.GenerateNonce();  
		var timeStamp = oAuth.GenerateTimeStamp();  
		var signature = oAuth.GenerateSignature(uri, consumerKey,  
		                                        consumerSecret, string.Empty, string.Empty, "GET", timeStamp, nonce,  
		                                        OAuthBase.SignatureTypes.HMACSHA1, out url, out param);  
		
		string query = string.Format ("{0}?{1}&oauth_signature={2}", url, param, signature);

		WWWForm form = new WWWForm();
		string responseURL = query;
		WWW www = new WWW(responseURL, form);
		StartCoroutine(yahooData(www));

		*/

		//using (WebRequest.Create(string.Format("{0}?{1}&oauth_signature={2}", url, param, signature)).GetResponse()) { }

		//comentado hasta arreglar...
		/*string query = String.Format("http://weather.yahooapis.com/forecastrss?w=" + weid);
		XmlDocument wData = new XmlDocument();
		wData.Load(query);
		
		XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
		manager.AddNamespace("yweather","http://xml.weather.yahoo.com/ns/rss/1.0");
		
		XmlNode channel = wData.SelectSingleNode("rss").SelectSingleNode("channel");
		XmlNodeList nodes = wData.SelectNodes("/rss/channel/item/yweather:forecast", manager);

		Condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;
		float temp = (float.Parse (channel.SelectSingleNode ("item").SelectSingleNode ("yweather:condition", manager).Attributes ["temp"].Value) - 32f) / 1.8f;
		//Temperature = temp.ToString ("n2");
		Temperature = string.Format ("{0:0}", temp);

		WindSpeed = channel.SelectSingleNode("yweather:wind", manager).Attributes["speed"].Value;*/





		/*  NO VA!
		Humidity = channel.SelectSingleNode("yweather:atmosphere", manager).Attributes["humidity"].Value;
		WindSpeed = channel.SelectSingleNode("yweather:wind", manager).Attributes["speed"].Value;
		Town = channel.SelectSingleNode("yweather:location", manager).Attributes["city"].Value;
		TFCond = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["text"].Value;
		TFHigh = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["high"].Value;
		TFLow = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["low"].Value; */
	}

	IEnumerator yahooData(WWW www){
		yield return www;
		if (www.error == null) {

			XmlDocument wData = new XmlDocument();
			wData.Load(www.text);
			
			XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
			manager.AddNamespace("yweather","http://xml.weather.yahoo.com/ns/rss/1.0");
			
			XmlNode channel = wData.SelectSingleNode("rss").SelectSingleNode("channel");
			XmlNodeList nodes = wData.SelectNodes("/rss/channel/item/yweather:forecast", manager);
			
			Condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;
			float temp = (float.Parse (channel.SelectSingleNode ("item").SelectSingleNode ("yweather:condition", manager).Attributes ["temp"].Value) - 32f) / 1.8f;
			//Temperature = temp.ToString ("n2");
			Temperature = string.Format ("{0:0}", temp);
			
			WindSpeed = channel.SelectSingleNode("yweather:wind", manager).Attributes["speed"].Value;

			//www.text
		}
	}

	public void GetWeatherXml()
	{
/*#if UNITY_EDITOR
		string weid = "466882";
#else*/
		XmlDocument doc = new XmlDocument();
		//obtener woeid con lat y lng
		string yql = "http://query.yahooapis.com/v1/public/yql?q=SELECT woeid FROM geo.places WHERE text='("+lat+", "+lng+")'";
		Debug.Log (yql);
		
		doc.Load (yql);
		string weid = "";
		foreach(XmlNode node in doc.DocumentElement.ChildNodes){
			weid = node.InnerText; //or loop through its children as well
		}
		
		Debug.Log ("WOEID: " + weid);
//#endif

		GetWeather (weid);
	}

}
