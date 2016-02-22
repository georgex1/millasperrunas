using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class gpsController : MonoBehaviour {

	//public bool gps_active;
	public float ant_lat;
	public float ant_lng;
	private float timing;

	public double sumDistance;
	private int sumTime;

	private int partialTime;
	private double partialDistance;

	//public double partialVeloc;

	private int displaySeconds2;
	private float loadTime;

	private float refreshTime;
	private float refreshTimeVeloc;

	private bool isCalcDist;
	private bool isCalcVeloc;

	private string debugGps;
	private string debugVeloc;

	private MainController GMS;

	public GameObject beep;


	//private int refreshGPS;
	private int trychangeVel;

	public float act_lat;
	public float act_lng;


	//variables cuando se esta corriendo...
	public float sumDistanceAct;
	public float sumPuntosPaseo;
	public int actSec;
	//public float calcPuntos;
	/*public int displaySeconds;
	public int displayMinutes;
	public int displayHours;*/

	// Use this for initialization
	void Start () {

		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		GMS.gps_active = false;


		actSec = 0;
		GMS.gps_calcPuntos = 0f;

		GMS.gps_displaySeconds = 0;
		GMS.gps_displayMinutes = 0;
		GMS.gps_displayHours = 0;
		trychangeVel = 0;
		//refreshGPS = 2;
		/*displaySeconds = 0;
		displayMinutes = 0;
		displayHours = 0;*/

		resetVars ();
		
		StartCoroutine (init_gps());
		
		// Stop service if there is no need to query location updates continuously
		//Input.location.Stop();
	}

	private IEnumerator init_gps(){
		yield return new WaitForSeconds (2);

		// First, check if user has location service enabled
		if (!Input.location.isEnabledByUser) {
			Debug.Log ("GPS deshabilitado");
			debugGps = "GPS deshabilitado";

			if(!GMS.popup.activeSelf){
				GMS.errorPopup ("GPS Deshabilitado... por favor habilitelo para usar la app");
			}

			StartCoroutine (init_gps());
			// remind user to enable GPS
			// As far as I know, there is no way to forward user to GPS setting menu in Unity
		} else {
			// Start service before querying location
			Input.location.Start();

			// Wait until service initializes
			int maxWait = 20;
			while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
				yield return new WaitForSeconds(1);
				maxWait--;
				Debug.Log("veces reintentando: " + maxWait);
				debugGps = "veces reintentando: " + maxWait;
			}
			
			// Service didn't initialize in 20 seconds
			if (maxWait < 1) {
				Debug.Log("error");
				GMS.gps_active = false;
			}
			
			// Connection has failed
			if (Input.location.status == LocationServiceStatus.Failed) {
				Debug.Log ("conexion fallida");
				debugGps = "conexion fallida";
				GMS.gps_active = false;
			}
			
			// Access granted and location value could be retrieved
			else {
				Debug.Log ("ubicacion: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

				#if !UNITY_EDITOR
				act_lat = ant_lat = Input.location.lastData.latitude;
				act_lng = ant_lng = Input.location.lastData.longitude;
				#else
				act_lat = ant_lat = -27.36836f;
				act_lng = ant_lng = -55.90231f;
				#endif
				
				if(ant_lat != 0f){
					//Debug.Log("cargar lat y lng de GPS para clima...");
					GMS.init_lat = ant_lat.ToString();
					GMS.init_lng = ant_lng.ToString();
					
					PlayerPrefs.SetString("init_lat", ant_lat.ToString());
					PlayerPrefs.SetString("init_lng", ant_lng.ToString());
				}
				debugGps = "esperar 3 segs para inicializar lectura GPS...";
				StartCoroutine( checkToStart () );
				
			}
		}
	}

	public void resetVars(){
		sumTime = 0;
		sumDistance = 0;
		partialTime = 0;
		partialDistance = 0;
		GMS.gps_partialVeloc = 0;
		isCalcDist = false;
		isCalcVeloc = false;
		refreshTime = 0f;
		refreshTimeVeloc = 0f;
		sumPuntosPaseo = 0f;
		
		Debug.Log("iniciando GPS...");
	}

	void OnApplicationPause(bool pauseStatus) {
		if (pauseStatus) {
			GMS.gps_active = false;
		}
	}
	void Awake () {
		StartCoroutine (init_gps());
	}
	
	IEnumerator checkToStart (){
		yield return new WaitForSeconds (3);
		GMS.gps_active = true;
		debugGps = " gps_active = true ";
		GMS.showLoading (false);

	}
	
	// Update is called once per frame
	void Update () {

		if (GMS.paseando) {

			//Input.location.Start();
#if !UNITY_EDITOR
			if (GMS.gps_active ) {
#endif

				GMS.paseoTime += Time.deltaTime;
				GMS.paseoKm += Time.deltaTime;

				//calculos de paseo
				int roundedRestSeconds = Mathf.CeilToInt (GMS.paseoTime);
				GMS.gps_displaySeconds = roundedRestSeconds % 60;
				GMS.gps_displayMinutes = roundedRestSeconds / 60;
				GMS.gps_displayHours = GMS.gps_displayMinutes / 60;

				refreshTime += Time.deltaTime;
				int refreshTimeS = Mathf.CeilToInt (refreshTime) % 60;
				//refreshTimeVeloc += Time.deltaTime;

				loadTime += Time.deltaTime;

				/*int roundedRestSeconds2 = Mathf.CeilToInt (refreshTimeVeloc);
				displaySeconds2 = roundedRestSeconds2 % 60;

				partialTime = displaySeconds2;*/

				#if !UNITY_EDITOR
				act_lat = Input.location.lastData.latitude;
				act_lng = Input.location.lastData.longitude;
				#endif

				if (refreshTimeS == GMS.gps_refreshGPS && !isCalcDist) { //calculo cada 2 segs

					isCalcDist = true;

					double getDistance = Math.Round ((double)distance (ant_lat, ant_lng, act_lat, act_lng, 'K'), 5);
					if (!Double.IsNaN (getDistance) && (ant_lat != act_lat || ant_lng != act_lng)) {
						sumDistance += (getDistance * 1000); //sumo la distancia en mts
						partialDistance = getDistance;

						transform.GetComponent<AudioSource> ().Play ();
					}
					ant_lat = act_lat;
					ant_lng = act_lng;


					Debug.Log ("distancia sumada: " + sumDistance + " KM");

					if(partialDistance == 0){
						trychangeVel++;
					}

					if(trychangeVel > 2 || partialDistance != 0){
						//calculo para metros sobre segundos
						GMS.gps_partialVeloc = (partialDistance * 1000) / GMS.gps_refreshGPS;
						GMS.gps_partialVeloc = GMS.gps_partialVeloc * 3.6;
						Debug.Log ("Velocidad: " + GMS.gps_partialVeloc + " KM/H");

						trychangeVel = 0;
					}

					
					//debugVeloc = partialDistance.ToString () + " * 1000 / " + GMS.gps_refreshGPS.ToString ();


					if(sumDistanceAct != float.Parse(sumDistance.ToString())){
						
						Debug.Log(" sumDistanceAct != sumDistance ");
						
						sumDistanceAct = float.Parse(sumDistance.ToString());
						Debug.Log(" sumDistanceAct : " + sumDistanceAct);

						GMS.db.OpenDB ("millasperrunas.db");

						sumPuntosPaseo = sumPuntosPaseo + (sumDistanceAct * GMS.puntosCalc);

						GMS.gps_calcPuntos = GMS.gps_calcPuntos + ( sumDistanceAct * GMS.puntosCalc );
						
						Debug.Log(" calcPuntos : " + GMS.gps_calcPuntos);
						
						//textPuntos_.text = calcPuntos.ToString ("n2") + " PUNTOS";
						//textKm_.text = GMS.gps_partialVeloc.ToString ("n2") + "KM/H";
						
						float sumPuntos = float.Parse (GMS.perro.puntos) + GMS.gps_calcPuntos;
						//float sumKms = float.Parse (GMS.perro.kilometros) + float.Parse(GMS.gps_partialVeloc.ToString());
						float sumKms = float.Parse (GMS.perro.kilometros) + float.Parse(sumDistance.ToString());

						GMS.db.BasicQueryInsert ("update perros_usuarios set puntos = " + sumPuntos.ToString () + ", kilometros = " + sumKms.ToString () + " " +
						                         "where perros_id = " + GMS.perro.id + " and usuarios_id = " + GMS.userData.id + " ");


						//cargar paseo
						string[] cols = new string[]{ "id", "perros_id", "usuarios_id", "puntos", "kilometros", "fecha_entrada"};
						string[] colsVals = new string[]{ GMS.idPaseo, GMS.perro.id.ToString(), GMS.userData.id.ToString(), sumPuntosPaseo.ToString(), sumDistance.ToString(), GMS.getActualDate() };
						
						GMS.db.InsertIgnoreInto("paseos", cols, colsVals, GMS.idPaseo);

						GMS.db.CloseDB ();
						
						string[] fields = {"puntos", "kilometros", "perros_id", "usuarios_id", "paseos_id", "kilometros_paseo", "puntos_paseo"};
						string[] values = {sumPuntos.ToString (), sumKms.ToString (), GMS.perro.id.ToString() , GMS.userData.id.ToString(), GMS.idPaseo, sumDistance.ToString(), sumPuntosPaseo.ToString()};
						GMS.insert_sync(fields, values, "perros_puntos");

					}

					/*partialVeloc = (partialDistance * 1000) / partialTime;
					Debug.Log ("Velocidad: " + partialVeloc + " M/S");
					
					debugVeloc = partialDistance.ToString() + " * 1000 / " + partialTime.ToString();*/
				}
				if (isCalcDist && refreshTimeS > GMS.gps_refreshGPS) {
					isCalcDist = false;
					refreshTime = 0f;
					partialDistance = 0;

					//partialDistance = 0;
					//partialTime = 0;
				}

				/*if (partialTime == GMS.gps_refreshGPS && !isCalcVeloc) {//calculo de velocidad cada 7 segundos

					isCalcVeloc = true;

					//calculo para metros sobre segundos
					//GMS.gps_partialVeloc = (partialDistance * 1000) / partialTime;
					//Debug.Log ("Velocidad: " + GMS.gps_partialVeloc + " M/S");

					//debugVeloc = partialDistance.ToString () + " * 1000 / " + partialTime.ToString ();
				}
				if (isCalcVeloc) {
					isCalcVeloc = false;
					//refreshTimeVeloc = 0f;

					partialDistance = 0;
					partialTime = 0;
				}*/
				
				//textTime_.text = displayHours.ToString () + "H - " + displayMinutes.ToString () + "M - " + displaySeconds.ToString () + "S";
				
				//guardo en la db
				/*if (actSec != GMS.gps_displaySeconds) {
					actSec = GMS.gps_displaySeconds;

					Debug.Log("entro para calcular...");

					if(sumDistanceAct != float.Parse(sumDistance.ToString())){

						Debug.Log(" sumDistanceAct != sumDistance");
						
						sumDistanceAct = float.Parse(sumDistance.ToString());
						GMS.db.OpenDB ("millasperrunas.db");
						
						GMS.gps_calcPuntos = GMS.gps_calcPuntos + ( sumDistanceAct * GMS.puntosCalc );

						Debug.Log(" calcPuntos : " + GMS.gps_calcPuntos);
						
						//textPuntos_.text = calcPuntos.ToString ("n2") + " PUNTOS";
						//textKm_.text = GMS.gps_partialVeloc.ToString ("n2") + "KM/H";
						
						float sumPuntos = float.Parse (GMS.perro.puntos) + GMS.gps_calcPuntos;
						float sumKms = float.Parse (GMS.perro.kilometros) + float.Parse(GMS.gps_partialVeloc.ToString());
						
						GMS.db.BasicQueryInsert ("update perros_usuarios set puntos = " + sumPuntos.ToString () + ", kilometros = " + sumKms.ToString () + " " +
						                         "where perros_id = " + GMS.perro.id + " and usuarios_id = " + GMS.userData.id + " ");

						GMS.db.CloseDB ();

						string[] fields = {"puntos", "kilometros", "perros_id", "usuarios_id"};
						string[] values = {sumPuntos.ToString (), sumKms.ToString (), GMS.perro.id.ToString() , GMS.userData.id.ToString()};
						GMS.insert_sync(fields, values, "perros_puntos");
					}
				}*/




			#if !UNITY_EDITOR
			}
			#endif

		}
		//Input.location.Stop();
	}

	//debug
	void OnGUI(){

		if (GMS.isDebug) {
			Double tometers = sumDistance * 1000;
			GUI.skin.label.fontSize = 32;
			#if UNITY_EDITOR
			GUI.skin.label.fontSize = 18;
			#endif
			GUI.Label (new Rect (0, 0, Screen.width, Screen.height * 0.05f), "distancia sumada : " + sumDistance + " KM" + " | en metros: " + tometers);
			GUI.Label (new Rect (0, Screen.height * 0.05f, Screen.width, Screen.height * 0.05f), "Velocidad : " + GMS.gps_partialVeloc + " M/S");

			GUI.Label (new Rect (0, Screen.height * 0.10f, Screen.width, Screen.height * 0.05f), "Debug GPS : " + debugGps);

			GUI.Label (new Rect (0, Screen.height * 0.15f, Screen.width, Screen.height * 0.05f), "GPS info: " + act_lat + " | " + act_lng);
			GUI.Label (new Rect (0, Screen.height * 0.20f, Screen.width, Screen.height * 0.05f), "GPS info Ant: " + ant_lat + " | " + ant_lng);

			GUI.Label (new Rect (0, Screen.height * 0.25f, Screen.width, Screen.height * 0.05f), "GPS Status : " + Input.location.status);

			GUI.Label (new Rect (0, Screen.height * 0.30f, Screen.width, Screen.height * 0.05f), "Calculo velocidad : " + debugVeloc);
		}
	}
	
	private double distance(double lat1, double lon1, double lat2, double lon2, char unit) {
		double theta = lon1 - lon2;
		double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
		dist = Math.Acos(dist);
		dist = rad2deg(dist);
		dist = dist * 60 * 1.1515;
		if (unit == 'K') {
			dist = dist * 1.609344;
		} else if (unit == 'N') {
			dist = dist * 0.8684;
		}
		return (dist);
	}
	
	//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
	//::  This function converts decimal degrees to radians             :::
	//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
	private double deg2rad(double deg) {
		return (deg * Math.PI / 180.0);
	}
	
	//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
	//::  This function converts radians to decimal degrees             :::
	//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
	private double rad2deg(double rad) {
		return (rad / Math.PI * 180.0);
	}


	//pedometer
	private float loLim = 0.005F;
	private float hiLim = 0.3F;
	private int steps = 0;
	//public bool ismoving = false;
	private float fHigh = 8.0F;
	private float curAcc= 0F;
	private float fLow = 0.2F;
	private float avgAcc;
	
	public bool stepDetector(){
		bool ismoving = false;
		curAcc = Mathf.Lerp (curAcc, Input.acceleration.magnitude, Time.deltaTime * fHigh);
		avgAcc = Mathf.Lerp (avgAcc, Input.acceleration.magnitude, Time.deltaTime * fLow);
		float delta = curAcc - avgAcc;
		if (!ismoving) { 
			if (delta > hiLim) {
				beep.GetComponent<AudioSource> ().Play ();
				ismoving = true;
				steps++;
			} else if (delta < loLim) {
				ismoving = false;
			}
			ismoving = false;
		}
		avgAcc = curAcc;
		return ismoving;
		//return steps;
	}
}
