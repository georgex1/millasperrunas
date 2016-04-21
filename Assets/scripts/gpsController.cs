using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class gpsController : MonoBehaviour
{

	//public bool gps_active;
	public float ant_lat;
	public float ant_lng;
	private float timing;
	public double sumDistance;
	private int sumTime;
	private int partialTime;
	private double partialDistance;
	private double partialDistanceSum;

	//public double partialVeloc;

	private int displaySeconds2;
	private float loadTime;
	private float refreshTime;
	private float refreshTimeVeloc;
	private bool isCalcDist = false;
	private bool isCalcVeloc;
	private string debugGps;
	private string debugVeloc;
	private MainController GMS;
	public GameObject beep;
	private bool superDebug = false;

	//private int refreshGPS;
	private int trychangeVel;
	public float act_lat;
	public float act_lng;
	private double gps_partialVelocAnt = 0f;


	//variables cuando se esta corriendo...
	public double sumDistanceAct;
	public double sumPuntosPaseo;
	public int actSec;
	//public float calcPuntos;
	/*public int displaySeconds;
	public int displayMinutes;
	public int displayHours;*/

	private bool isStart = false;
	private int segsToRecorrido = 0;
	public bool IsSetAntVars = false;

	private double disAnt = 0;


	//variables para popup de seguir paseando?


	// Use this for initialization
	void Start ()
	{

		Application.runInBackground = true;

#if UNITY_EDITOR
		superDebug = true;
#endif

		Debug.Log ("en GPS controller");

		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController> ();

		StartCoroutine (waitForStart ());
		// Stop service if there is no need to query location updates continuously
		//Input.location.Stop();
	}

	private IEnumerator waitForStart ()
	{
		yield return new WaitForSeconds (2);

		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController> ();

		if (GMS.userData.id != 0 && GMS.userData.id != null) {

			GMS.gps_active = false;
		
			isStart = true;
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
		
			StartCoroutine (init_gps ());
			StartCoroutine (redirect ());
		} else {
			StartCoroutine (waitForStart ());
		}
	}

	private IEnumerator redirect ()
	{
		yield return new WaitForSeconds (1);
		Application.LoadLevel (GMS.toRedirectLogin);
	}

	private IEnumerator init_gps ()
	{
		yield return new WaitForSeconds (2);

		// First, check if user has location service enabled
		if (!Input.location.isEnabledByUser) {
			Debug.Log ("GPS deshabilitado");
			debugGps = "GPS deshabilitado";

			if (!GMS.popup.activeSelf) {
				GMS.errorPopup ("GPS Deshabilitado... por favor habilitelo para usar la app");
			}

			StartCoroutine (init_gps ());
			// remind user to enable GPS
			// As far as I know, there is no way to forward user to GPS setting menu in Unity
		} else {
			// Start service before querying location
			Input.location.Start (5f, 10f);

			// Wait until service initializes
			int maxWait = 20;
			while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
				yield return new WaitForSeconds (1);
				maxWait--;
				Debug.Log ("veces reintentando: " + maxWait);
				debugGps = "veces reintentando: " + maxWait;
			}
			
			// Service didn't initialize in 20 seconds
			if (maxWait < 1) {
				Debug.Log ("error");
				GMS.gps_active = false;

				StartCoroutine (init_gps ());
			}
			
			// Connection has failed
			if (Input.location.status == LocationServiceStatus.Failed) {
				Debug.Log ("conexion fallida");
				debugGps = "conexion fallida";
				GMS.gps_active = false;

				StartCoroutine (init_gps ());
			}
			
			// Access granted and location value could be retrieved
			else {
				Debug.Log ("ubicacion: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

				setAntStats ();
				
				if (ant_lat != 0f) {
					//Debug.Log("cargar lat y lng de GPS para clima...");
					GMS.init_lat = ant_lat.ToString ();
					GMS.init_lng = ant_lng.ToString ();
					
					PlayerPrefs.SetString ("init_lat", ant_lat.ToString ());
					PlayerPrefs.SetString ("init_lng", ant_lng.ToString ());
				}
				debugGps = "esperar 3 segs para inicializar lectura GPS...";
				StartCoroutine (checkToStart ());
				//StartCoroutine (UpdateGPS ());
			}
		}
	}

	private void setAntStats ()
	{
		#if !UNITY_EDITOR
		act_lat = ant_lat = Input.location.lastData.latitude;
		act_lng = ant_lng = Input.location.lastData.longitude;
		GMS.debug_lng = act_lng;
		#else
		act_lat = ant_lat = -27.36836f;
		act_lng = ant_lng = GMS.debug_lng = -55.90231f;
		#endif
	}

	public void resetVars ()
	{
		sumTime = 0;
		sumDistance = 0;
		partialTime = 0;
		partialDistance = 0;
		partialDistanceSum = 0;
		GMS.gps_partialVeloc = 0;
		isCalcDist = false;
		isCalcVeloc = false;
		refreshTime = 0f;
		refreshTimeVeloc = 0f;
		sumPuntosPaseo = 0f;
		IsSetAntVars = false;
		
		Debug.Log ("iniciando GPS...");
	}

	/*void OnApplicationPause(bool pauseStatus) {
		if (pauseStatus) {
			GMS.gps_active = false;
		}
	}*/

	void Awake ()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;
		
		DontDestroyOnLoad (transform.gameObject);

		StartCoroutine (init_gps ());
	}
	
	IEnumerator checkToStart ()
	{
		yield return new WaitForSeconds (3);
		debugGps = " gps_active = true ";

		//inicializo de nuevo las variables de ant lng y lat
		setAntStats ();
		GMS.gps_active = true;
		GMS.showLoading (false);
	}

	void checkGps ()
	{
		if (Input.location.status != LocationServiceStatus.Running && GMS.gps_active) {
			GMS.gps_active = false;
			StartCoroutine (init_gps ());
		}
	}

	private void setRecorrido ()
	{
		Debug.Log ("setRecorrido");


		if (act_lng != 0f && act_lng != null) {
			int id_ = GMS.generateId ();

			string[] cols = new string[] {
				"id",
				"usuarios_id",
				"lat",
				"lng",
				"fecha",
				"perros_id"
			};
			string[] colsVals = new string[] {
				id_.ToString (),
				GMS.userData.id.ToString () + "",
				act_lat.ToString () + "",
				act_lng.ToString () + "",
				GMS.getActualDate () + "",
				GMS.perro.id.ToString () + ""
			};

			GMS.db.OpenDB ("millasperrunas.db");
			GMS.db.InsertIntoSpecific ("ultimo_recorrido", cols, colsVals);
			GMS.db.CloseDB ();

			GMS.insert_sync (cols, colsVals, "ultimo_recorrido");
		}



		segsToRecorrido = 0;
	}

	private void UpdateGPS ()
	{

		//yield return new WaitForSeconds (GMS.gps_refreshGPS);
		Debug.Log ("UpdateGPS");

		if (isStart) {
			
			//pruebo de nuevo para el puto loading q no se va
			if (GMS.gps_active && GMS.loading.activeSelf) {
				GMS.showLoading (false);
			}
			
			if (GMS.paseando) {

				if (GMS.paseoTime > 1.5f) {
					
					//Input.location.Start();
					#if !UNITY_EDITOR
					if (GMS.gps_active) {
						#endif
						
						//checkear GPS activo
						//checkGps ();
						
						if (superDebug) {
							act_lat = ant_lat;
							act_lng = GMS.debug_lng;
						}
						
						//if (refreshTimeS == GMS.gps_refreshGPS && !isCalcDist) { //calculo cada 2 segs
							
						segsToRecorrido += GMS.gps_refreshGPS;
							
						if (segsToRecorrido == 12) { //guardo poscicion de ult recorrido cada 20 segs
							setRecorrido ();
						}
							
						//isCalcDist = true;
							
						double getDistance = Math.Round ((double)distance (ant_lat, ant_lng, act_lat, act_lng), 5);
					
						GMS.iddleTime += 1;

						if (!Double.IsNaN (getDistance) && (ant_lat != act_lat || ant_lng != act_lng)) {

							//verificar si no fue un salto del GPS
							if( getDistance > 30 && GMS.partialTime < 3 && disAnt == 0){
								Debug.Log("SALTO...");
							}else{
								disAnt = getDistance;
								partialDistanceSum += getDistance;

								if (partialDistanceSum > 5) {
									sumDistance += partialDistanceSum;
									partialDistance = partialDistanceSum;
									partialDistanceSum = 0;
								
									transform.GetComponent<AudioSource> ().Play ();
								
									GMS.iddleTime = 0;
								}
							}
						
						}
						ant_lat = act_lat;
						ant_lng = act_lng;
							
							
						//Debug.Log ("distancia sumada: " + sumDistance + " KM");
							
						if (partialDistance == 0) {
							trychangeVel++;
						}
							
						Debug.Log("trychangeVel: " + trychangeVel);

						if (trychangeVel > 3 || partialDistance != 0) {
							Debug.Log("trychangeVel > 3 || partialDistance != 0" );
							
							//calculo para metros sobre segundos
							GMS.gps_partialVeloc = (partialDistance) / GMS.partialTime;
							GMS.gps_partialVeloc = GMS.gps_partialVeloc * 3.6; //pasaje de m/s a km/h

							gps_partialVelocAnt = GMS.gps_partialVeloc;

							//Debug.Log ("Velocidad: " + GMS.gps_partialVeloc + " KM/H");

							partialDistance = 0;
							trychangeVel = 0;
						}else{
							if(gps_partialVelocAnt != 0){
								Debug.Log("gps_partialVelocAnt");
								GMS.gps_partialVeloc =  gps_partialVelocAnt / 2;
							}
						}
							
						//debugVeloc = partialDistance.ToString () + " * 1000 / " + GMS.gps_refreshGPS.ToString ();
							
						if (sumDistanceAct != sumDistance) {
							GMS.partialTime = 0f;
								
							Debug.Log (" sumDistanceAct != sumDistance ");
								
							sumDistanceAct = sumDistance;
							Debug.Log (" sumDistanceAct : " + sumDistanceAct);
								
							GMS.db.OpenDB ("millasperrunas.db");
								
							sumPuntosPaseo = Math.Round (sumPuntosPaseo + (getDistance * GMS.puntosCalc));
								
							GMS.gps_calcPuntos = Math.Round (GMS.gps_calcPuntos + (getDistance * GMS.puntosCalc));
								
							Debug.Log (" calcPuntos : " + GMS.gps_calcPuntos);
								
							//textPuntos_.text = calcPuntos.ToString ("n2") + " PUNTOS";
							//textKm_.text = GMS.gps_partialVeloc.ToString ("n2") + "KM/H";
								
							double sumDistanceKms = sumDistance / 1000;

							float sumPuntos = float.Parse (GMS.perro.puntos) + float.Parse (GMS.gps_calcPuntos.ToString ());
							//float sumKms = float.Parse (GMS.perro.kilometros) + float.Parse(GMS.gps_partialVeloc.ToString());
							float sumKms = float.Parse (GMS.perro.kilometros) + float.Parse (sumDistanceKms.ToString ());
								
							GMS.db.BasicQueryInsert ("update perros_usuarios set puntos = " + sumPuntos.ToString () + ", kilometros = " + sumKms.ToString () + " " +
								"where perros_id = " + GMS.perro.id + " and usuarios_id = " + GMS.userData.id + " ");
								
								
							//cargar paseo
							string[] cols = new string[] {
							"id",
							"perros_id",
							"usuarios_id",
							"puntos",
							"kilometros",
							"fecha_entrada"
						};
							string[] colsVals = new string[] {
									GMS.idPaseo,
									GMS.perro.id.ToString (),
									GMS.userData.id.ToString (),
									sumPuntosPaseo.ToString (),
									sumDistanceKms.ToString (),
									GMS.getActualDate ()
								};
								
							GMS.db.InsertIgnoreInto ("paseos", cols, colsVals, GMS.idPaseo);
								
							GMS.db.CloseDB ();
								
							string[] fields = {
									"puntos",
									"kilometros",
									"perros_id",
									"usuarios_id",
									"paseos_id",
									"kilometros_paseo",
									"puntos_paseo"
								};
							string[] values = {
									sumPuntos.ToString (),
									sumKms.ToString (),
									GMS.perro.id.ToString () ,
									GMS.userData.id.ToString (),
									GMS.idPaseo,
									sumDistanceKms.ToString (),
									sumPuntosPaseo.ToString ()
								};

							GMS.insert_sync (fields, values, "perros_puntos");
								
						}
						
						//}
						#if !UNITY_EDITOR
					}
					#endif


					isCalcDist = false;
					refreshTime = 0f;

				} else {
					ant_lat = act_lat;
					ant_lng = act_lng;
				}
			}
		}



		//StartCoroutine (UpdateGPS ());
	}

	//int refreshTimeS = 0;
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		callInUpdate ();
	}

	void callInUpdate ()
	{
		if (GMS.gps_active) {
			#if !UNITY_EDITOR
			act_lat = Input.location.lastData.latitude;
			act_lng = Input.location.lastData.longitude;
			#endif
		}
		
		if (!IsSetAntVars) {
			setAntStats ();
			IsSetAntVars = true;
		}
		
		if (isStart && GMS.paseando && IsSetAntVars) {
			#if !UNITY_EDITOR
			if (Input.location.status == LocationServiceStatus.Running) {
				GMS.gpsRunning = true;
				#endif
				
				GMS.paseoTime += Time.deltaTime;
				GMS.partialTime += Time.deltaTime;
				//GMS.paseoKm += Time.deltaTime;
				
				//calculos de paseo
				int roundedRestSeconds = Mathf.CeilToInt (GMS.paseoTime);
				GMS.gps_displaySeconds = roundedRestSeconds % 60;
				GMS.gps_displayMinutes = roundedRestSeconds / 60;
				GMS.gps_displayHours = GMS.gps_displayMinutes / 60;
				
				refreshTime += Time.deltaTime;
				int refreshTimeS = Mathf.CeilToInt (refreshTime) % 60;
				//refreshTimeVeloc += Time.deltaTime;
				
				loadTime += Time.deltaTime;
				
				if (refreshTimeS == GMS.gps_refreshGPS && !isCalcDist) {
					isCalcDist = true;
					UpdateGPS ();
				}

				#if !UNITY_EDITOR
			}else{
				GMS.gpsRunning = false;
			}
			#endif
		}

	}

	void OnApplicationPause (bool isGamePause)
	{
		if (isGamePause) {
			//callInUpdate();
		}
	}

	//debug
	void OnGUI ()
	{
		if (isStart) {
			if (GMS.isDebug) {
				Double tometers = sumDistance * 1000;
				GUIStyle myStyle = new GUIStyle ();
				myStyle.normal.textColor = Color.red;
				GUI.skin.label.fontSize = 32;
				#if UNITY_EDITOR
				GUI.skin.label.fontSize = 22;
				myStyle.fontSize = 22;
				//GUI.contentColor = Color.red;
				#endif
				GUI.Label (new Rect (0, 0, Screen.width, Screen.height * 0.05f), "distancia sumada : " + sumDistance + " KM" + " | en metros: " + tometers, myStyle);
				GUI.Label (new Rect (0, Screen.height * 0.100f, Screen.width, Screen.height * 0.05f), "Velocidad : " + GMS.gps_partialVeloc + " M/S", myStyle);

				GUI.Label (new Rect (0, Screen.height * 0.125f, Screen.width, Screen.height * 0.05f), "Debug GPS : " + debugGps, myStyle);

				GUI.Label (new Rect (0, Screen.height * 0.150f, Screen.width, Screen.height * 0.05f), "GPS info: " + act_lat + " | " + act_lng, myStyle);
				GUI.Label (new Rect (0, Screen.height * 0.175f, Screen.width, Screen.height * 0.05f), "GPS info Ant: " + ant_lat + " | " + ant_lng, myStyle);

				GUI.Label (new Rect (0, Screen.height * 0.200f, Screen.width, Screen.height * 0.05f), "GPS Status : " + Input.location.status, myStyle);

				GUI.Label (new Rect (0, Screen.height * 0.225f, Screen.width, Screen.height * 0.05f), "Calculo velocidad : " + debugVeloc, myStyle);
				GUI.Label (new Rect (0, Screen.height * 0.250f, Screen.width, Screen.height * 0.05f), "verticalAccuracy : " + Input.location.lastData.verticalAccuracy, myStyle);
				GUI.Label (new Rect (0, Screen.height * 0.275f, Screen.width, Screen.height * 0.05f), "horizontalAccuracy : " + Input.location.lastData.horizontalAccuracy, myStyle);
				GUI.Label (new Rect (0, Screen.height * 0.300f, Screen.width, Screen.height * 0.05f), "GPS timestamp : " + Input.location.lastData.timestamp, myStyle);
				GUI.Label (new Rect (0, Screen.height * 0.325f, Screen.width, Screen.height * 0.05f), "GPS partialTime : " + GMS.partialTime, myStyle);
			}
		}
	}
	
	/*private double distance (double lat1, double lon1, double lat2, double lon2, char unit)
	{
		double theta = lon1 - lon2;
		double dist = Math.Sin (deg2rad (lat1)) * Math.Sin (deg2rad (lat2)) + Math.Cos (deg2rad (lat1)) * Math.Cos (deg2rad (lat2)) * Math.Cos (deg2rad (theta));
		dist = Math.Acos (dist);
		dist = rad2deg (dist);
		dist = dist * 60 * 1.1515;
		if (unit == 'K') {
			dist = dist * 1.609344;
		} else if (unit == 'N') {
			dist = dist * 0.8684;
		}
		return (dist);
	}*/

	private double distance (double lat1, double lon1, double lat2, double lon2)
	{
		
		// Convert degrees to radians
		lat1 = lat1 * Math.PI / 180.0;
		lon1 = lon1 * Math.PI / 180.0;
		
		lat2 = lat2 * Math.PI / 180.0;
		lon2 = lon2 * Math.PI / 180.0;
		
		// radius of earth in metres
		double r = 6378100;
		
		// P
		double rho1 = r * Math.Cos (lat1);
		double z1 = r * Math.Sin (lat1);
		double x1 = rho1 * Math.Cos (lon1);
		double y1 = rho1 * Math.Sin (lon1);
		
		// Q
		double rho2 = r * Math.Cos (lat2);
		double z2 = r * Math.Sin (lat2);
		double x2 = rho2 * Math.Cos (lon2);
		double y2 = rho2 * Math.Sin (lon2);
		
		// Dot product
		double dot = (x1 * x2 + y1 * y2 + z1 * z2);
		double cos_theta = dot / (r * r);
		
		double theta = Math.Acos (cos_theta);
		
		// Distance in Metres
		return r * theta;
	}
	
	//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
	//::  This function converts decimal degrees to radians             :::
	//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
	private double deg2rad (double deg)
	{
		return (deg * Math.PI / 180.0);
	}
	
	//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
	//::  This function converts radians to decimal degrees             :::
	//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
	private double rad2deg (double rad)
	{
		return (rad / Math.PI * 180.0);
	}


	//pedometer
	private float loLim = 0.005F;
	private float hiLim = 0.3F;
	private int steps = 0;
	//public bool ismoving = false;
	private float fHigh = 8.0F;
	private float curAcc = 0F;
	private float fLow = 0.2F;
	private float avgAcc;
	
	public bool stepDetector ()
	{
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
