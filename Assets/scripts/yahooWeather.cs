using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class yahooWeather{

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

		string query = String.Format("http://weather.yahooapis.com/forecastrss?w=" + weid);
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

		WindSpeed = channel.SelectSingleNode("item").SelectSingleNode("yweather:wind", manager).Attributes["speed"].Value;

		/*
		Humidity = channel.SelectSingleNode("yweather:atmosphere", manager).Attributes["humidity"].Value;
		WindSpeed = channel.SelectSingleNode("yweather:wind", manager).Attributes["speed"].Value;
		Town = channel.SelectSingleNode("yweather:location", manager).Attributes["city"].Value;
		TFCond = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["text"].Value;
		TFHigh = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["high"].Value;
		TFLow = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["low"].Value; */
	}

	public void GetWeatherXml()
	{
		XmlDocument doc = new XmlDocument();
		//obtener woeid con lat y lng
		Debug.Log ("http://query.yahooapis.com/v1/public/yql?q=select%20woeid%20from%20geo.placefinder%20where%20text=%27"+lat+",%20"+lng+"%27%20and%20gflags=%27R%27");

		doc.Load ("http://query.yahooapis.com/v1/public/yql?q=select%20woeid%20from%20geo.placefinder%20where%20text=%27"+lat+",%20"+lng+"%27%20and%20gflags=%27R%27");

		//doc.Load("http://query.yahooapis.com/v1/public/yql?q=select%20woeid%20from%20geo.places%281%29%20%20where%20text=%27Posadas%20Misiones,%20Argentina%27&format=xml");
		string weid = "";
		foreach(XmlNode node in doc.DocumentElement.ChildNodes){
			weid = node.InnerText; //or loop through its children as well
		}
		Debug.Log ("WOEID: " + weid);
		GetWeather (weid);
	}

}
