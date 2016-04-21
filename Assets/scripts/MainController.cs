using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using UnityEngine.UI;
/*using System.Data;*/

public class MainController : MonoBehaviour {

	public string appHash = "M11774Sp3RRun4A2!";
/*#if UNITY_EDITOR
	private string responseURL = "http://local.betterpixel.com/millasperrunas/response/response.php";
	private string responseAssets = "http://local.betterpixel.com/millasperrunas/response/assets/images/perros/";
#else*/
	public string responseURL = "http://thepastoapps.com/proyectos/millasperrunas/response/response.php";
	private string responseAssets = "http://thepastoapps.com/proyectos/millasperrunas/response/assets/images/perros/";
//#endif
	private string Uid;

	private float loadTime;
	private bool closeApp;
	private bool checkUpdate;

	public string toRedirectLogin = "";

	//variables para paseo
	public float paseoTime;
	public bool paseando;
	public float puntosCalc;
	public float puntosCalcDefault;
	//public float paseoKm;
	public float partialTime = 1f;
	public string idPaseo;
	public string puntosEspecialesMotivoId;
	public Dictionary<int,string> puntosEspecialesMotivoIds = new Dictionary<int,string>();
	public int paseoPerroId = 0;
	public bool paseoPhotoTaked = false;
	public bool gpsRunning = true;

	//variables para popup en paseo de seguir paseando
	/*public float ultLat = 0f;
	public float ultLng = 0f;*/
	public int iddleTime = 0;

	//GPS vars
	public bool gps_active = false;
	public double gps_calcPuntos;
	public double gps_partialVeloc;
	public int gps_displaySeconds;
	public int gps_displayMinutes;
	public int gps_displayHours;
	public int gps_refreshGPS = 2;
	public float debug_lng = 0f;

	public dbAccess db ;

	public UserData userData;
	public PerroData perro;
	public InvitadoData invitado;
	public FotoPerro foto_perro;

	public bool haveInet;
	public bool checkingCon = false;

	//para debug
	public bool isDebug = false;
	public string sendDataDebug;
	public string yahooDebug;

	//menu
	public string antScene = "home";

	//notificaciones
	public notifications notificationsScript;
	public string appName;

	//popup
	public GameObject popup;
	public GameObject popupText;
	public GameObject popupButton;
	private string errorChrs = "";

	//QstPopup
	public GameObject qstPopup;
	public GameObject qstPopupText;

	//loading
	public GameObject loading;

	//notificacion
	public string notificacionId;
	public string tipId;

	//clima
	public string clima_temperatura;
	public string clima_condicion;
	public string clima_viento = "0";
	public string init_lat;
	public string init_lng;


	//badgets


	void OnGUI(){
		if (isDebug) {
			GUI.skin.label.fontSize = 20;
			GUI.Label (new Rect (0, Screen.height * 0.775f, Screen.width, Screen.height * 0.05f), "DEBUG : " + sendDataDebug);
			GUI.Label (new Rect (0, Screen.height * 0.800f, Screen.width, Screen.height * 0.05f), "Yahoo DEBUG : " + yahooDebug);
		}
	}

	void createDb(){
		db.OpenDB("millasperrunas.db");

		string[] cols = new string[]{"id", "email", "nombre", "fbid", "fecha_nacimiento", "sexo", "foto"};
		string[] colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("usuarios", cols, colTypes);

		cols = new string[]{"id", "nombre", "edad", "peso", "razas_id", "foto"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("perros", cols, colTypes);

		cols = new string[]{"id", "email", "nombre", "foto" };
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT" };
		db.CreateTable ("familia", cols, colTypes);

		cols = new string[]{"id", "perros_id", "usuarios_id", "parentescos_id", "puntos", "kilometros", "puntos_totales", "puntos_semana", "aceptado", "chat_group"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("perros_usuarios", cols, colTypes);

		cols = new string[]{"id", "perros_id", "usuarios_id", "puntos", "kilometros", "fecha_entrada"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("paseos", cols, colTypes);

		cols = new string[]{"id", "nombre", "foto", "descripcion", "habilitado", "serverupdate"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "INT", "INT", "TEXT"};
		db.CreateTable ("badges", cols, colTypes);

		cols = new string[]{"id", "badges_id", "usuarios_id", "perros_id", "fecha_entrada"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("badges_usuarios", cols, colTypes);

		cols = new string[]{"id", "nombre"};
		colTypes = new string[]{"INT", "TEXT"};
		db.CreateTable ("razas", cols, colTypes);

		cols = new string[]{"id", "nombre", "descripcion", "orden", "habilitado", "serverupdate", "imagen"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "INT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("tips_paseo", cols, colTypes);

		cols = new string[]{"id", "titulo", "descripcion", "plataforma", "visto", "tipo", "serverupdate", "perros_id"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("notificaciones", cols, colTypes);

		cols = new string[]{"id", "fecha_desde", "fecha_hasta", "puntos", "habilitado", "motivos_id", "serverupdate"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("puntos_especiales", cols, colTypes);

		cols = new string[]{"id", "fecha_entrada", "usuarios_id", "perros_id", "foto", "subida", "paseos_id"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("fotos_paseo", cols, colTypes);

		cols = new string[]{"id", "usuarios_id", "lat", "lng", "fecha", "perros_id"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("ultimo_recorrido", cols, colTypes);

		cols = new string[]{"id", "func", "sfields", "svalues"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("sync", cols, colTypes);

		db.CloseDB();
	}

	public void logout(){
		db.OpenDB("millasperrunas.db");
		db.BasicQueryInsert ( "drop table usuarios" );
		db.BasicQueryInsert ( "drop table perros" );
		db.BasicQueryInsert ( "drop table familia" );
		db.BasicQueryInsert ( "drop table perros_usuarios" );
		db.BasicQueryInsert ( "drop table paseos" );
		db.BasicQueryInsert ( "drop table badges_usuarios" );
		db.BasicQueryInsert ( "drop table tips_paseo" );
		db.BasicQueryInsert ( "drop table notificaciones" );
		db.BasicQueryInsert ( "drop table puntos_especiales" );
		db.BasicQueryInsert ( "drop table fotos_paseo" );
		db.BasicQueryInsert ( "drop table ultimo_recorrido" );
		db.CloseDB();

		//Application.LoadLevel ("login");
		#if !UNITY_EDITOR
		#if UNITY_ANDROID
		Application.Quit();
		#else
		Application.LoadLevel ("login");
		#endif
		#endif



	}

	// Use this for initialization
	void Start () {

		Uid = "";
//#if UNITY_EDITOR
		isDebug = false;
//#endif
		checkUpdate = true;
		loadTime = 0;
		db = GetComponent<dbAccess>();
		createDb ();
		appName = "Millas Perrunas";
		puntosCalc = 1;
		puntosCalcDefault = 1;

		paseoTime = 0f;
		paseando = false;
		//paseoKm = 0f;
		puntosEspecialesMotivoId = "0";

		toRedirectLogin = "home";

		/*db.OpenDB("millasperrunas.db");
		ArrayList result = db.BasicQueryArray ("select * from usuarios where fbid = 10207435825803946 ");
		db.CloseDB();*/

		haveInet = false;
		checkConnection ();

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		DontDestroyOnLoad (transform.gameObject);

		if (Uid == "") {
			if (PlayerPrefs.HasKey ("Uid")) {
				Uid = PlayerPrefs.GetString ("Uid");
			} else {
				Uid = SystemInfo.deviceUniqueIdentifier;
				PlayerPrefs.SetString ("Uid", Uid);
			}
		}

		if (!PlayerPrefs.HasKey ("config_alerts")) {
			PlayerPrefs.SetString ("config_alerts", "true");
		} else {
			string config_alerts = PlayerPrefs.GetString ("config_alerts");
			if(config_alerts == "false"){
				notificationsScript.disableNotifs();
			}
		}

		StartCoroutine (call_sync());
		StartCoroutine (get_updates ());

		/*string[] fields = {"puntos", "kilometros", "perros_id", "usuarios_id"};
		string[] values = {"100", "150", "454545" , "3"};
		insert_sync(fields, values, "perros_puntos");*/

		//PlayerPrefs.DeleteAll ();

		//leer clima
		init_clima ();

	}

	private void init_clima(){
		if(PlayerPrefs.HasKey("clima_temperatura")){
			clima_temperatura = PlayerPrefs.GetString("clima_temperatura");
			clima_condicion = PlayerPrefs.GetString("clima_condicion");
			clima_viento = PlayerPrefs.GetString("clima_viento");
		}
		if (PlayerPrefs.HasKey ("init_lat")) {
			init_lat = PlayerPrefs.GetString("init_lat");
			init_lng = PlayerPrefs.GetString("init_lng");
		}
		
		StartCoroutine (get_clima());
	}

	private IEnumerator get_clima(){
		yield return new WaitForSeconds (3);

		yahooDebug = "Obteniendo clima...";

		if (haveInet) {
			yahooWeather yahooWeather_ = new yahooWeather (init_lat, init_lng);
			yahooWeather_.GetWeatherXml ();
			Debug.Log ("CLIMA CONDICION: " + yahooWeather_.Condition + " TEMPERATURA: " + yahooWeather_.Temperature + " VIENTO: " + yahooWeather_.WindSpeed);

			clima_condicion = yahooWeather_.Condition;
			clima_temperatura = yahooWeather_.Temperature;
			clima_viento = yahooWeather_.WindSpeed;

			PlayerPrefs.SetString("clima_temperatura", clima_temperatura);
			PlayerPrefs.SetString("clima_condicion", clima_condicion);
			PlayerPrefs.SetString("clima_viento", clima_viento);

			yahooDebug = "CLIMA: temp: " + clima_temperatura + " | condicion: " + clima_condicion + " | viento: " + clima_viento ;

		} else {
			StartCoroutine (get_clima());
		}
	}
	
	void Awake () {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		DontDestroyOnLoad (transform.gameObject);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		loadTime += Time.deltaTime;

		int roundedRestSeconds = Mathf.CeilToInt (loadTime);
		int displaySeconds = roundedRestSeconds % 60;
		
		/*if (Input.GetKeyDown(KeyCode.Escape)) {
			closeApp = true;
		}*/

		if (closeApp && loadTime > 1) {
			Application.Quit();
		}
		if (loadTime > 6) {
			loadTime = 0;
			checkUpdate = true;
			checkingCon = false;
		}

		/*if (checkUpdate && displaySeconds == 5) {
			if(haveInet){
				checkUpdate = false;
				get_updates();
			}
		}*/

		if(displaySeconds == 5 && !checkingCon){
			checkingCon = true;
			checkConnection ();
		}



		//paseo contador
		/*if (paseando) {
			paseoTime += Time.deltaTime;
			paseoKm += Time.deltaTime;
		}*/
	}

	void checkConnection(){

		if (!haveInet) {
			WWWForm form = new WWWForm ();
			form.AddField ("appHash", appHash);
			form.AddField ("action", "check_connection");
			WWW www = new WWW (responseURL, form);
			StartCoroutine (WaitForRequest (www, "check_connection"));
		}
	}

	//send data inet
	public void sendData(string[] vars, string[] values, string action_, byte[] uploadImage = null){
		
		WWWForm form = new WWWForm();
		form.AddField("appHash", appHash);
		form.AddField("action", action_);
		
		int index=0;
		sendDataDebug = "preparando variables";

		foreach (string vars_ in vars) {
			if(vars_ != "fileUpload"){
				try{
					form.AddField(vars_, values[index]);
				}catch(Exception e){
					sendDataDebug = "error en variable: "+index;
				}
			}else{
				form.AddBinaryData("fileUpload", uploadImage);
			}
			index++;
		}

		sendDataDebug = "iniciando WWW";
		
		WWW www = new WWW(responseURL, form);
		StartCoroutine(WaitForRequest(www, action_));
		//Debug.Log(www.text);
		
	}

	IEnumerator WaitForRequest(WWW www, string response){
		yield return www;
		
		// check for errors
		if (www.error == null){
			sendDataDebug = "WWW Ok!";
			//Debug.Log("WWW Ok!: " + www.text);
			//Debug.Log("response: " + response);

			IDictionary Wresponse = (IDictionary) MiniJSON.Json.Deserialize (www.text);

			string Wcontent_ = MiniJSON.Json.Serialize(Wresponse["content"]);
			string WarrayData_ = MiniJSON.Json.Serialize(Wresponse["arrayData"]);

			Debug.Log("WWW content: " + Wcontent_);

			IDictionary Wresponse2 = (IDictionary) MiniJSON.Json.Deserialize ( Wcontent_ );
			IDictionary Wresponse3 = (IDictionary) MiniJSON.Json.Deserialize ( WarrayData_ );

			if((string)Wresponse["status"] == "error"){

				errorPopup((string)Wresponse2["mgs"], (string)Wresponse2["toclose"]);
			}else{
				
				if(response == "save_perro"){
					perro.id = int.Parse( (string)Wresponse2["insertId"] );
					savePerro();
					//Application.LoadLevel ("home-mascota");
				}
				if(response == "check_connection"){
					haveInet = true;
				}


				if(response == "login_facebook"){
					sendDataDebug = "entro a login_facebook";
					Debug.Log("login facebook OK! ID: "+ (string)Wresponse3["id"]);

					userData.id = int.Parse( (string)Wresponse3["id"] );

					if( (string)Wresponse2["hasArray"] != "0" ){
						string WarrayContent_ = MiniJSON.Json.Serialize(Wresponse["arrayContent"]);
						IDictionary WresponseContent = (IDictionary) MiniJSON.Json.Deserialize ( WarrayContent_ );
						
						
						for(int i = 1; i <= int.Parse( (string)Wresponse2["hasArray"] ); i++ ){
							IDictionary reponseContent = (IDictionary) MiniJSON.Json.Deserialize ( (string)WresponseContent[i.ToString()]  );
							
							userData.email = (string)reponseContent["email"];
							userData.nombre = (string)reponseContent["nombre"];
							userData.sexo = (string)reponseContent["sexo"];
							
							userData.foto = (string)reponseContent["foto"];
							
							try_download_perro_imagen((string)reponseContent["foto"]);
							
							saveUserData(true);
							
							//upload_user_foto();
							StartCoroutine( redirect("subir-foto", 3f) );
							download_perros();
							get_ultimo_recorrido();
							
						}
					}

					/*saveUserData(true);
					Application.LoadLevel ("subir-foto");

					download_perros();*/
				}

				if(response == "login_email"){

					showLoading(true);

					userData.id = int.Parse( (string)Wresponse3["id"] );
					
					//saveUserData(true);
					populateUserData(Wresponse3);

					Debug.Log("imagen: " + (string)Wresponse3["foto"]);

					if((string)Wresponse3["foto"] != "" && (string)Wresponse3["foto"] != null ){
						try_download_perro_imagen((string)Wresponse3["foto"]);
						toRedirectLogin = "cargar-invitar";
						StartCoroutine( redirect("initGPS", 3f) );
					}else{
						StartCoroutine( redirect("subir-foto", 3f) );
					}

					download_perros();
					get_ultimo_recorrido();
				}

				if(response == "register"){

					userData.id = int.Parse( (string)Wresponse3["id"] );

					saveUserData(false);
					Application.LoadLevel ("subir-foto");

					download_perros();
				}

				if(response == "get_perros"){
					string WarrayContent_ = MiniJSON.Json.Serialize(Wresponse["arrayContent"]);
					IDictionary WresponseContent = (IDictionary) MiniJSON.Json.Deserialize ( WarrayContent_ );
					
					//Debug.Log((string)Wresponse2["hasArray"]);
					if( (string)Wresponse2["hasArray"] != "0" ){
						for(int i = 1; i <= int.Parse( (string)Wresponse2["hasArray"] ); i++ ){
							Debug.Log("posicion: " + i);

							IDictionary reponseContent = (IDictionary) MiniJSON.Json.Deserialize ( (string)WresponseContent[i.ToString()]  );

							db.OpenDB("millasperrunas.db");

							//cargar perros
							string[] cols = new string[]{ "id", "nombre", "edad", "peso", "razas_id", "foto"};
							string[] colsVals = new string[]{ (string)reponseContent["id"], (string)reponseContent["nombre"], (string)reponseContent["edad"], (string)reponseContent["peso"], (string)reponseContent["razas_id"], (string)reponseContent["foto"] };
							
							db.InsertIgnoreInto("perros", cols, colsVals, (string)reponseContent["id"]);
							db.CloseDB();

							//intentar bajar imagen del perro
							try_download_perro_imagen((string)reponseContent["foto"]);

							//cargar perros_usuarios
							db.OpenDB("millasperrunas.db");
							ArrayList result = db.BasicQueryArray ("select id from perros_usuarios where usuarios_id = '"+userData.id.ToString()+"' and perros_id = '"+(string)reponseContent["id"]+"' ");
							db.CloseDB();

							if (result.Count == 0) {
								//Debug.Log("cargar perros_usuarios: " + (string)reponseContent["id"]);

								string perrosUsuariosId = getPerrosUsuariosNewId ();
								//string newId = generateId().ToString();

								cols = new string[]{"id", "perros_id", "usuarios_id", "parentescos_id", "puntos", "kilometros", "puntos_totales", "aceptado", "chat_group"};
								colsVals = new string[]{ perrosUsuariosId, (string)reponseContent["id"], userData.id.ToString(), (string)reponseContent["parentescos_id"], (string)reponseContent["puntos"], (string)reponseContent["kilometros"], (string)reponseContent["puntos_totales"], (string)reponseContent["aceptado"], (string)reponseContent["chat_group"] };

								db.OpenDB("millasperrunas.db");
								db.InsertIntoSpecific("perros_usuarios", cols, colsVals);
								db.CloseDB();
							}

							if((string)reponseContent["invitado_por_usuarios_id"] != ""){
								//ingresar notificacion de perro agregado
								cols = new string[]{"id", "titulo", "descripcion", "plataforma", "visto", "tipo", "serverupdate", "perros_id"};
								string newNotifId = getNotificacionesNewId();
								colsVals = new string[]{ newNotifId, "Invitación para pasear a " + (string)reponseContent["nombre"], 
									(string)reponseContent["usuario_nombre"] + " te invita a pasear a " + (string)reponseContent["nombre"], "Android", "0", "invitacion", getActualDate(), (string)reponseContent["id"] };

								db.OpenDB("millasperrunas.db");
								db.InsertIntoSpecific("notificaciones", cols, colsVals);
								db.CloseDB();
							}


						}
					}
				}

				if(response == "invitar"){
					Application.LoadLevel ("invitar-gracias");
				}

				if(response == "upload_info"){
					sendDataDebug = "imagen subida";
				}
				if(response == "upload_perfil"){
					sendDataDebug = "imagen subida";

					db.OpenDB("millasperrunas.db");

					string[] colsUsuarios = new string[]{ "foto" };
					string[] colsUsuariosValues = new string[]{ userData.foto};

					db.UpdateSingle("usuarios", "foto", userData.foto, "id" , userData.id.ToString());
					db.UpdateSingle("familia", "foto", userData.foto, "id" , userData.id.ToString());

					db.CloseDB();

					//Application.LoadLevel ("cargar-invitar");
				}

				if(response == "upload_paseo"){
					sendDataDebug = "imagen paseo subida";
					
					db.OpenDB("millasperrunas.db");
					
					string[] colsUsuarios = new string[]{ "foto" };
					string[] colsUsuariosValues = new string[]{ userData.foto};
					
					db.UpdateSingle("fotos_paseo", "subida", "1", "id" , (string)Wresponse3["id"] );
					
					db.CloseDB();
				}

				if(response == "get_familiares"){
					string WarrayContent_ = MiniJSON.Json.Serialize(Wresponse["arrayContent"]);
					IDictionary WresponseContent = (IDictionary) MiniJSON.Json.Deserialize ( WarrayContent_ );
					
					//Debug.Log((string)Wresponse2["hasArray"]);
					if( (string)Wresponse2["hasArray"] != "0" ){
						for(int i = 1; i <= int.Parse( (string)Wresponse2["hasArray"] ); i++ ){
							//Debug.Log("posicion: " + i);
							
							IDictionary reponseContent = (IDictionary) MiniJSON.Json.Deserialize ( (string)WresponseContent[i.ToString()]  );

							/*db.OpenDB("millasperrunas.db");
							ArrayList result = db.BasicQueryArray ("select id from perros_usuarios where perros_id = '"+(string)reponseContent["perros_id"]+"' and usuarios_id = '"+(string)reponseContent["usuarios_id"]+"' ");
							db.CloseDB();

							if (result.Count > 0) {}*/

							//cargar familia
							string[] cols = new string[]{"id", "email", "nombre", "foto"  };
							string[] colsVals = new string[]{ (string)reponseContent["id"], (string)reponseContent["email"], 
								(string)reponseContent["nombre"], (string)reponseContent["foto"] };

							db.OpenDB("millasperrunas.db");
							db.InsertIgnoreInto("familia", cols, colsVals, (string)reponseContent["id"]);
							db.CloseDB();

							//insertar el perro al usuario
							if((string)reponseContent["perros_id"] != ""){

								//verificar si ya esta el perro/usuario
								db.OpenDB("millasperrunas.db");
								ArrayList result = db.BasicQueryArray ("select id from perros_usuarios where usuarios_id = '"+(string)reponseContent["id"]+"' and perros_id = '"+(string)reponseContent["perros_id"]+"' ");
								db.CloseDB();
								
								if (result.Count == 0) {
									//generar id nuevo auto_incremental

									string perrosUsuariosId = getPerrosUsuariosNewId ();
									Debug.Log("nuevo id de perros_usuarios: " + perrosUsuariosId);
									//string newId = generateId().ToString();

									cols = new string[]{ "id", "perros_id", "usuarios_id", "parentescos_id", "puntos", "kilometros", "puntos_totales", "aceptado", "chat_group"};
									colsVals = new string[]{ perrosUsuariosId, (string)reponseContent["perros_id"], (string)reponseContent["id"], 
										(string)reponseContent["parentescos_id"], "0", "0", "0", (string)reponseContent["aceptado"], (string)reponseContent["chat_group"]};

									//Debug.Log("perros_usaurios cargar: " + (string)reponseContent["perros_id"] + "|" + (string)reponseContent["id"] + "|" + (string)reponseContent["parentescos_id"]);

									//Debug.Log("cargar perros_usuarios: " + (string)reponseContent["perros_id"]);
									db.OpenDB("millasperrunas.db");
									db.InsertIntoSpecific("perros_usuarios", cols, colsVals);
									db.CloseDB();
								}
							}

							//intentar bajar imagen del perro
							try_download_perro_imagen((string)reponseContent["foto"]);
							

						}
					}
				}

				if(response == "get_familiares_puntos"){
					string WarrayContent_ = MiniJSON.Json.Serialize(Wresponse["arrayContent"]);
					IDictionary WresponseContent = (IDictionary) MiniJSON.Json.Deserialize ( WarrayContent_ );
					
					//Debug.Log((string)Wresponse2["hasArray"]);
					if( (string)Wresponse2["hasArray"] != "0" ){
						for(int i = 1; i <= int.Parse( (string)Wresponse2["hasArray"] ); i++ ){
							//Debug.Log("posicion: " + i);
							
							IDictionary reponseContent = (IDictionary) MiniJSON.Json.Deserialize ( (string)WresponseContent[i.ToString()]  );
							
							db.OpenDB("millasperrunas.db");
							
							//cargar puntos familia
							ArrayList result = db.BasicQueryArray ("select id from perros_usuarios where perros_id = '"+(string)reponseContent["perros_id"]+"' and usuarios_id = '"+(string)reponseContent["usuarios_id"]+"' ");
							
							if (result.Count > 0) {

								string[] cols = new string[]{ "puntos", "kilometros", "puntos_totales", "puntos_semana" };
								string[] colsVals = new string[]{ (string)reponseContent["puntos"], (string)reponseContent["kilometros"], (string)reponseContent["puntos_totales"], (string)reponseContent["puntos_semana"] };

								//Debug.Log("update perros_usuarios puntos en id: " + ((string[])result [0]) [0] );

								db.InsertIgnoreInto("perros_usuarios", cols, colsVals, ((string[])result [0]) [0]);
							}
							
							db.CloseDB();
						}
					}
				}

				if(response == "get_badges"){
					string WarrayContent_ = MiniJSON.Json.Serialize(Wresponse["arrayContent"]);
					IDictionary WresponseContent = (IDictionary) MiniJSON.Json.Deserialize ( WarrayContent_ );
					
					//Debug.Log((string)Wresponse2["hasArray"]);
					if( (string)Wresponse2["hasArray"] != "0" ){
						for(int i = 1; i <= int.Parse( (string)Wresponse2["hasArray"] ); i++ ){
							//Debug.Log("posicion: " + i);
							
							IDictionary reponseContent = (IDictionary) MiniJSON.Json.Deserialize ( (string)WresponseContent[i.ToString()]  );
							
							db.OpenDB("millasperrunas.db");
							ArrayList result = db.BasicQueryArray ("select id from badges_usuarios where perros_id = '"+(string)reponseContent["perros_id"]+"' and badges_id = '"+(string)reponseContent["badges_id"]+"' ");
							db.CloseDB();

							if (result.Count == 0) {

								string generatedId = getBadgesUsuariosNewId();
								
								string[] cols = new string[]{ "id", "badges_id", "usuarios_id", "perros_id", "fecha_entrada" };
								string[] colsVals = new string[]{ generatedId, (string)reponseContent["badges_id"], userData.id.ToString(), (string)reponseContent["perros_id"], (string)reponseContent["fecha_entrada"] };

								//Debug.Log("update perros_usuarios puntos en id: " + ((string[])result [0]) [0] );

								db.OpenDB("millasperrunas.db");
								db.InsertIgnoreInto( "badges_usuarios", cols, colsVals, generatedId );
								db.CloseDB();
							}
							

						}
					}
				}


				if(response == "get_ultimo_recorrido"){
					string WarrayContent_ = MiniJSON.Json.Serialize(Wresponse["arrayContent"]);
					IDictionary WresponseContent = (IDictionary) MiniJSON.Json.Deserialize ( WarrayContent_ );
					
					//Debug.Log((string)Wresponse2["hasArray"]);
					if( (string)Wresponse2["hasArray"] != "0" ){
						for(int i = 1; i <= int.Parse( (string)Wresponse2["hasArray"] ); i++ ){
							//Debug.Log("posicion: " + i);

							IDictionary reponseContent = (IDictionary) MiniJSON.Json.Deserialize ( (string)WresponseContent[i.ToString()]  );

							string[] cols = new string[]{ "id", "usuarios_id", "lat", "lng", "fecha", "perros_id" };
							string[] colsVals = new string[]{ (string)reponseContent["id"], (string)reponseContent["usuarios_id"], (string)reponseContent["lat"], (string)reponseContent["lng"], (string)reponseContent["fecha"], (string)reponseContent["perros_id"] };

							db.OpenDB("millasperrunas.db");
							db.InsertIgnoreInto( "ultimo_recorrido", cols, colsVals, (string)reponseContent["id"] );

							db.CloseDB();

						}
					}
				}

				/*if(response == "check_aceptados"){
					string WarrayContent_ = MiniJSON.Json.Serialize(Wresponse["arrayContent"]);
					IDictionary WresponseContent = (IDictionary) MiniJSON.Json.Deserialize ( WarrayContent_ );
					
					//Debug.Log((string)Wresponse2["hasArray"]);
					if( (string)Wresponse2["hasArray"] != "0" ){
						for(int i = 1; i <= int.Parse( (string)Wresponse2["hasArray"] ); i++ ){

							IDictionary reponseContent = (IDictionary) MiniJSON.Json.Deserialize ( (string)WresponseContent[i.ToString()]  );

							db.OpenDB("millasperrunas.db");
							ArrayList result = db.BasicQueryArray ("select id from perros_usuarios where perros_id = '"+(string)reponseContent["perros_id"]+"' and usuarios_id = '"+(string)reponseContent["usuarios_id"]+"' ");
							db.CloseDB();
							
							if (result.Count > 0) {
								string[] cols = new string[]{ "aceptado" };
								string[] colsVals = new string[]{ (string)reponseContent["aceptado"] };
								
								db.OpenDB("millasperrunas.db");
								db.InsertIgnoreInto( "perros_usuarios", cols, colsVals, ((string[])result [0]) [0] );
								db.CloseDB();
							}
						}
					}
				}*/

				if(response == "get_updates"){

					//if((string)Wresponse2["mgs"] == "puntos_especiales_updated"){

					string WarrayContent_ = MiniJSON.Json.Serialize(Wresponse["arrayContent"]);
					IDictionary WresponseContent = (IDictionary) MiniJSON.Json.Deserialize ( WarrayContent_ );

					//Debug.Log((string)Wresponse2["hasArray"]);
					if( (string)Wresponse2["hasArray"] != "0" ){
						for(int i = 1; i <= int.Parse( (string)Wresponse2["hasArray"] ); i++ ){
							Debug.Log("posicion: " + i);

							//string dada = MiniJSON.Json.Serialize(WresponseContent["1"]);
							IDictionary reponseContent = (IDictionary) MiniJSON.Json.Deserialize ( (string)WresponseContent[i.ToString()]  );

							//actualizar db de puntos especiales
							db.OpenDB("millasperrunas.db");

							if((string)Wresponse2["mgs"] == "puntos_especiales_updated"){
								string[] colsUsuarios = new string[]{ "id", "fecha_desde", "fecha_hasta", "puntos", "habilitado", "motivos_id", "serverupdate"};
								string[] colsUsuariosValues = new string[]{ (string)reponseContent["id"], (string)reponseContent["fecha_desde"], (string)reponseContent["fecha_hasta"], (string)reponseContent["puntos"], (string)reponseContent["habilitado"], (string)reponseContent["motivos_id"], (string)reponseContent["serverupdate"] };
								
								db.InsertIgnoreInto("puntos_especiales", colsUsuarios, colsUsuariosValues, (string)reponseContent["id"]);
							}

							if((string)Wresponse2["mgs"] == "tips_paseo_updated"){
								string[] colsUsuarios = new string[]{ "id", "nombre", "descripcion", "orden", "habilitado", "serverupdate", "imagen"};
								string[] colsUsuariosValues = new string[]{ (string)reponseContent["id"], (string)reponseContent["nombre"], (string)reponseContent["descripcion"], (string)reponseContent["orden"], (string)reponseContent["habilitado"], (string)reponseContent["serverupdate"], (string)reponseContent["imagen"] };
								
								db.InsertIgnoreInto("tips_paseo", colsUsuarios, colsUsuariosValues, (string)reponseContent["id"]);

								try_download_perro_imagen((string)reponseContent["imagen"]);
							}

							if((string)Wresponse2["mgs"] == "badges_updated"){
								string[] colsUsuarios = new string[]{ "id", "nombre", "foto", "descripcion", "habilitado", "serverupdate"};
								string[] colsUsuariosValues = new string[]{ (string)reponseContent["id"], (string)reponseContent["nombre"], (string)reponseContent["foto"], (string)reponseContent["descripcion"], (string)reponseContent["habilitado"], (string)reponseContent["serverupdate"] };
								
								db.InsertIgnoreInto("badges", colsUsuarios, colsUsuariosValues, (string)reponseContent["id"]);

								try_download_perro_imagen((string)reponseContent["foto"]);
							}

							if((string)Wresponse2["mgs"] == "notificaciones_updated"){
								string[] colsUsuarios = new string[]{"id", "titulo", "descripcion", "plataforma", "visto", "tipo", "serverupdate"};
								string[] colsUsuariosValues = new string[]{ (string)reponseContent["id"], (string)reponseContent["titulo"], (string)reponseContent["descripcion"], (string)reponseContent["plataforma"], (string)reponseContent["visto"], (string)reponseContent["tipo"], (string)reponseContent["serverupdate"] };
								
								db.InsertIgnoreInto("notificaciones", colsUsuarios, colsUsuariosValues, (string)reponseContent["id"]);
							}

							db.CloseDB();
							Debug.Log(reponseContent["puntos"]);
						}

						Debug.Log("updated: " + (string)Wresponse2["mgs"]);
					}
					//}
				}

				if(response == "checkFinishWeek"){
					string WarrayContent_ = MiniJSON.Json.Serialize(Wresponse["arrayContent"]);
					IDictionary WresponseContent = (IDictionary) MiniJSON.Json.Deserialize ( WarrayContent_ );

					if( (string)Wresponse2["hasArray"] != "0" ){
						for(int i = 1; i <= int.Parse( (string)Wresponse2["hasArray"] ); i++ ){
							IDictionary reponseContent = (IDictionary) MiniJSON.Json.Deserialize ( (string)WresponseContent[i.ToString()]  );
							checkBadget(2, int.Parse( (string)reponseContent["perros_id"] ) );//REY DEL PASEO
						}
					}
				}

				if(response == "sync"){
					db.OpenDB("millasperrunas.db");
					db.BasicQueryInsert("delete from sync where id = '" +(string)Wresponse3["id"]+ "' ");
					db.CloseDB();
				}
			}


		} else {
			haveInet = false;
			sendDataDebug = "WWW Error: "+www.error;
			Debug.Log("WWW Error: "+ www.error);
		}
	}

	private IEnumerator redirect(string escene_, float seconds){
		yield return new WaitForSeconds (seconds);
		
		Debug.Log ("redirect escene: " + escene_ + " en " + seconds);
		showLoading(false);
		Application.LoadLevel (escene_);
	}

	private void populateUserData(IDictionary values){
		userData.email = (string)values["email"];
		userData.nombre = (string)values["nombre"];
		userData.fecha_nacimiento = (string)values["fecha_nacimiento"];
		userData.sexo = (string)values["sexo"];
		userData.foto = (string)values["foto"];

		saveUserData (false);
	}

	private string getPerrosUsuariosNewId(){
		db.OpenDB("millasperrunas.db");
		ArrayList result = db.BasicQueryArray ("select id from perros_usuarios order by id DESC limit 1");
		db.CloseDB();

		string newId = "1";

		if (result.Count > 0) {
			newId = ((string[])result [0]) [0];
			int newIdInt = int.Parse(newId)+1;
			newId = newIdInt.ToString();
		}

		return newId;
	}

	private string getBadgesUsuariosNewId(){
		db.OpenDB("millasperrunas.db");
		ArrayList result = db.BasicQueryArray ("select id from badges_usuarios order by id DESC limit 1");
		db.CloseDB();
		
		string newId = "1";
		
		if (result.Count > 0) {
			newId = ((string[])result [0]) [0];
			int newIdInt = int.Parse(newId)+1;
			newId = newIdInt.ToString();
		}
		
		return newId;
	}

	private string getNotificacionesNewId(){
		db.OpenDB("millasperrunas.db");
		ArrayList result = db.BasicQueryArray ("select id from notificaciones order by id DESC limit 1");
		db.CloseDB();
		
		string newId = "1";
		
		if (result.Count > 0) {
			newId = ((string[])result [0]) [0];
			int newIdInt = int.Parse(newId)+1;
			newId = newIdInt.ToString();
		}
		
		return newId;
	}

	private void saveUserData(bool isfb){
		sendDataDebug = "entro a saveUserData";
		db.OpenDB("millasperrunas.db");
		
		string[] colsUsuarios = new string[]{ "id", "email", "nombre", "fbid", "fecha_nacimiento", "sexo", "foto"};

		ArrayList result = new ArrayList();
		if (isfb) {
			try{
				result = db.BasicQueryArray ("select fbid from usuarios where fbid = '" + userData.fbid + "' ");
			}catch(Exception e){
				sendDataDebug = "error con db";
			}
		} else {
			result = db.BasicQueryArray ("select email from usuarios where email = '"+userData.email+"' ");
		}


		//ArrayList result = db.SingleSelectWhere ("usuarios", "*", "fbid", "=", userData.fbid  );
		string[] colsUsuariosValues = new string[]{ userData.id.ToString(), userData.email, userData.nombre, userData.fbid, userData.fecha_nacimiento, userData.sexo, userData.foto };
		
		if (result.Count == 0) {
			sendDataDebug = "count = 0 inserto usuario";
			db.InsertIntoSpecific ("usuarios", colsUsuarios, colsUsuariosValues);

			//me agrego como parte de la familia

			colsUsuarios = new string[]{ "id", "email", "nombre", "foto"};
			colsUsuariosValues = new string[]{ userData.id.ToString(), userData.email, userData.nombre, userData.foto };
			db.InsertIntoSpecific("familia", colsUsuarios, colsUsuariosValues);
		}

		db.CloseDB();
	}

	public void updateUserData(){
		string[] colsUsuarios = new string[]{ "id", "email", "nombre", "fbid", "fecha_nacimiento", "sexo"};
		string[] colsUsuariosValues = new string[]{ userData.id.ToString(), userData.email, userData.nombre, userData.fbid, userData.fecha_nacimiento, userData.sexo };

		db.OpenDB("millasperrunas.db");
		db.InsertIgnoreInto("usuarios", colsUsuarios, colsUsuariosValues, userData.id.ToString() );
		db.CloseDB ();

		sendData (colsUsuarios, colsUsuariosValues, "updateUserData");
	}

	public void download_perros(){
		if (userData.id != 0) {
			string perros_list = "0";

			db.OpenDB("millasperrunas.db");
			ArrayList result = db.BasicQueryArray ("select id from perros");
			db.CloseDB();
			
			if (result.Count > 0) {
				foreach (string[] row_ in result) {
					perros_list += "," + row_[0];
				}
			}

			string[] colsUsuarios = new string[]{ "usuarios_id", "lista_descargada" };
			string[] colsUsuariosValues = new string[]{ userData.id.ToString (), perros_list };
		
			sendData (colsUsuarios, colsUsuariosValues, "get_perros");
		}
	}

	private void try_download_perro_imagen(string foto_){
		string filepath = Application.persistentDataPath + "/" + foto_;
		if (!File.Exists (filepath)) {
			StartCoroutine( downloadImg(foto_) );
		}
	}

	IEnumerator downloadImg (string image_name){
		if (image_name != "") {
			Texture2D texture = new Texture2D (1, 1);
			Debug.Log ("try download image: " + responseAssets + image_name);
			WWW www = new WWW (responseAssets + image_name);
			yield return www;
			www.LoadImageIntoTexture (texture);
		
			byte[] ImgBytes = texture.EncodeToPNG ();
		
			File.WriteAllBytes (Application.persistentDataPath + "/" + image_name, ImgBytes);
		}
	}

	public void upload_user_foto(){
		//subir imagen
		byte[] fileData = File.ReadAllBytes (Application.persistentDataPath + "/" + userData.foto);
		
		Debug.Log ("try upload: imagen usuario");
		string[] cols2 = new string[]{"usuarios_id", "fileUpload", "usuario_foto"};
		string[] data2 = new string[]{userData.id.ToString (), "imagen_usuario", userData.foto };
		try {
			sendData (cols2, data2, "upload_perfil", fileData);
		} catch (IOException e) {
			Debug.Log (e);
		}
	}

	public void upload_recorrido(string google_recorrido, string image_name){
		string[] cols = new string[]{ "google_recorrido", "image_name" };
		string[] colsVals = new string[]{ google_recorrido, image_name };
		
		sendData (cols, colsVals, "upload_recorrido");
	}

	public string getShareImage(string image_){
		return responseAssets + "recorridos/" + image_ + ".png";
	}

	/*public void upload_recorrido(string imagen_, byte[] fileData){
		
		Debug.Log ("try upload: imagen recorrido");
		string[] cols2 = new string[]{ "fileUpload", "imagen"};
		string[] data2 = new string[]{ "imagen_usuario", imagen_ };
		try {
			sendData (cols2, data2, "upload_recorrido", fileData);
		} catch (IOException e) {
			Debug.Log (e);
		}
	}*/

	public int generateId(){
		int timestamp = (int)Math.Truncate((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
		return timestamp;
		/*int randR = UnityEngine.Random.Range (0, 9);
		string conctTr = timestamp.ToString () + randR.ToString ();
		return int.Parse(conctTr);*/
	}

	public void register(){

		string[] colsUsuarios = new string[]{ "email", "nombre", "password", "fecha_nacimiento", "sexo", "plataforma", "regid"};
		string[] colsUsuariosValues = new string[]{ userData.email, userData.nombre, userData.password, userData.fecha_nacimiento, userData.sexo, userData.plataforma, userData.reg_id };

		sendData (colsUsuarios, colsUsuariosValues, "register");
	}

	public void invitar(){
		
		string[] colsUsuarios = new string[]{ "email", "nombre", "parentescos_id", "perros_id", "usuarios_id", "usuario_nombre"};
		string[] colsUsuariosValues = new string[]{ invitado.email, invitado.nombre, invitado.parentescos_id, invitado.perros_id, userData.id.ToString(), userData.nombre };
		
		sendData (colsUsuarios, colsUsuariosValues, "invitar");
	}

	public void loginFacebook(){

		userData.foto = userData.temp_img;
		byte[] fileData = File.ReadAllBytes (Application.persistentDataPath + "/" + userData.foto);
		
		string[] colsUsuarios = new string[]{ "email", "nombre", "fbid", "fecha_nacimiento", "sexo", "plataforma", "regid", "usuario_foto", "fileUpload"};
		string[] colsUsuariosValues = new string[]{ userData.email, userData.nombre, userData.fbid, userData.fecha_nacimiento, userData.sexo, userData.plataforma, userData.reg_id, userData.foto, "imagen_usuario" };
		
		sendData (colsUsuarios, colsUsuariosValues, "login_facebook", fileData);
	}

	public void loginEmail(){
		
		string[] colsUsuarios = new string[]{ "email", "password", "plataforma", "regid"};
		string[] colsUsuariosValues = new string[]{ userData.email, userData.password, userData.plataforma, userData.reg_id };
		
		sendData (colsUsuarios, colsUsuariosValues, "login_email");
	}

	public void registerPerro(){
		//perro.id = generateId ();
		perro.chat_group = generateId ().ToString();

		string[] cols = new string[]{  "nombre", "edad", "peso", "razas_id", "parentescos_id", "usuarios_id", "chat_group"};
		string[] colsValues = new string[]{   perro.nombre, perro.edad, perro.peso, perro.razas_id, perro.parentescos_id, userData.id.ToString(), perro.chat_group };
		sendData (cols, colsValues, "save_perro");
	}

	public void savePerro(){
		db.OpenDB("millasperrunas.db");

		string[] cols = new string[]{ "id", "nombre", "edad", "peso", "razas_id", "foto"};
		string[] colsValues = new string[]{ perro.id.ToString(), perro.nombre, perro.edad, perro.peso, perro.razas_id, perro.foto };

		db.InsertIntoSpecific ("perros", cols, colsValues);
		db.CloseDB();

		string perrosUsuariosId = getPerrosUsuariosNewId ();

		cols = new string[]{ "id", "perros_id", "usuarios_id", "parentescos_id", "puntos", "kilometros", "puntos_totales", "aceptado", "chat_group"};
		colsValues = new string[]{ perrosUsuariosId, perro.id.ToString(), userData.id.ToString(), perro.parentescos_id, "0", "0", "0", "1", perro.chat_group };

		db.OpenDB("millasperrunas.db");
		db.InsertIntoSpecific ("perros_usuarios", cols, colsValues);
		db.CloseDB();

		
		//subir imagen
		byte[] fileData = File.ReadAllBytes( Application.persistentDataPath + "/" + perro.foto );
		
		Debug.Log ("try upload: imagen perro");
		string[] cols2 = new string[]{"perros_id", "fileUpload", "perro_foto"};
		string[] data2 = new string[]{perro.id.ToString(), "imagen_perro", perro.foto };
		try{
			sendData(cols2, data2, "upload_info", fileData);
		}catch(IOException e){
			Debug.Log(e);
		}

		Application.LoadLevel ("home-mascota");
	}

	private IEnumerator get_updates(){
		yield return new WaitForSeconds (6);

		call_updates ("puntos_especiales");
		call_updates ("tips_paseo");
		call_updates ("notificaciones");
		call_updates ("badges");
		get_familiares ();
		get_badges ();
		//get_perros_familiares ();
		get_familiares_puntos ();
		download_perros();

		StartCoroutine (get_updates ());
	}

	/*public void get_perros_familiares(){
		if (userData.id != 0) {
			string perros_usuarios_list = "0-0";
			
			db.OpenDB("millasperrunas.db");
			ArrayList result = db.BasicQueryArray ("select perros_id, usuarios_id from perros_usuarios where usuarios_id <> '"+userData.id+"'");
			db.CloseDB();
			
			if (result.Count > 0) {
				foreach (string[] row_ in result) {
					perros_usuarios_list += "," + row_[0] + "-" + row_[1];
				}
			}
			db.CloseDB();
			
			string[] colsUsuarios = new string[]{ "usuarios_id", "perros_usuarios_list"  };
			string[] colsUsuariosValues = new string[]{ userData.id.ToString (), perros_usuarios_list  };
			
			sendData (colsUsuarios, colsUsuariosValues, "get_perros_familiares");
		}
	}*/

	/*public void get_familiares3(){
		if (userData.id != 0) {
			string perros_list = "0";
			
			db.OpenDB("millasperrunas.db");
			ArrayList result = db.BasicQueryArray ("select id from perros");
			db.CloseDB();
			
			if (result.Count > 0) {
				foreach (string[] row_ in result) {
					perros_list += "," + row_[0];
				}
			}

			string familia_list = "0";

			db.OpenDB("millasperrunas.db");
			result = db.BasicQueryArray ("select id from familia");
			db.CloseDB();
			
			if (result.Count > 0) {
				foreach (string[] row_ in result) {
					familia_list += "," + row_[0];
				}
			}

			string[] colsUsuarios = new string[]{ "usuarios_id", "lista_perros", "lista_familia" };
			string[] colsUsuariosValues = new string[]{ userData.id.ToString (), perros_list, familia_list };
			
			sendData (colsUsuarios, colsUsuariosValues, "get_familiares");
		}
	}*/

	public void get_familiares(){

		if (userData.id != 0) {
			string perros_usuarios_list = "'0-0'";
			
			db.OpenDB("millasperrunas.db");
			ArrayList result = db.BasicQueryArray ("select perros_id, usuarios_id from perros_usuarios ");

			
			if (result.Count > 0) {
				foreach (string[] row_ in result) {
					perros_usuarios_list += ",'" + row_[0] + "-" + row_[1] + "'";
				}
			}

			string perros_list = "0";

			result = db.BasicQueryArray ("select id from perros");
			db.CloseDB();
			
			if (result.Count > 0) {
				foreach (string[] row_ in result) {
					perros_list += "," + row_[0];
				}
			}

			string[] colsUsuarios = new string[]{ "usuarios_id", "perros_usuarios_list", "perros_list" };
			string[] colsUsuariosValues = new string[]{ userData.id.ToString (), perros_usuarios_list, perros_list };
			
			sendData (colsUsuarios, colsUsuariosValues, "get_familiares");
		}
	}

	public void get_badges(){
		if (userData.id != 0) {

			db.OpenDB("millasperrunas.db");
			
			string badges_perros_list = "'0-0'";
			
			ArrayList result = db.BasicQueryArray ("select badges_id, perros_id from badges_usuarios");
			db.CloseDB();

			if (result.Count > 0) {
				foreach (string[] row_ in result) {
					badges_perros_list += ",'" + row_[0] + "-" + row_[1] + "'";
				}
			}
			
			string[] colsUsuarios = new string[]{ "usuarios_id", "badges_perros_list" };
			string[] colsUsuariosValues = new string[]{ userData.id.ToString (), badges_perros_list };
			
			sendData (colsUsuarios, colsUsuariosValues, "get_badges");
		}
	}

	private void get_ultimo_recorrido(){
		string[] colsUsuarios = new string[]{ "usuarios_id" };
		string[] colsUsuariosValues = new string[]{ userData.id.ToString () };
		
		sendData (colsUsuarios, colsUsuariosValues, "get_ultimo_recorrido");
	}

	public void get_familiares_puntos(){

		if(userData.id != 0){
			string perros_list = "0";
			
			db.OpenDB("millasperrunas.db");
			ArrayList result = db.BasicQueryArray ("select id from perros");
			db.CloseDB();
			
			if (result.Count > 0) {
				foreach (string[] row_ in result) {
					perros_list += "," + row_[0];
				}
			}

			string[] colsUsuarios = new string[]{ "usuarios_id", "lista_perros" };
			string[] colsUsuariosValues = new string[]{ userData.id.ToString (), perros_list };
			
			sendData (colsUsuarios, colsUsuariosValues, "get_familiares_puntos");
		}
	}

	public void call_updates( string table ){
		if (haveInet) {

			db.OpenDB ("millasperrunas.db");

			ArrayList result = db.BasicQueryArray ("select serverupdate from " + table + " order by serverupdate DESC limit 1");
			string serverUpdate = "2015-01-01";
			if (result.Count > 0) {
				serverUpdate = ((string[])result [0]) [0];
			}
			db.CloseDB ();
			string[] cols = new string[]{ "table", "serverupdate"};
			string[] values = new string[]{ table, serverUpdate};
			sendData (cols, values, "get_updates");
		}
	}

	private IEnumerator call_sync(){
		yield return new WaitForSeconds (2);
		sync ();
	}

	public void sync(){
		//Debug.Log ("sync....");
		//sync foto perro
		db.OpenDB("millasperrunas.db");
		ArrayList result = db.BasicQueryArray ("select * from fotos_paseo where subida = '0' limit 1");
		if (result.Count > 0) {
			foreach (string[] row_ in result) {
				Debug.Log("fotos_paseo sync");
				foto_perro.pupulateFotoPerro (row_);
				upload_foto_paseo();
			}
		}

		result = db.BasicQueryArray ("select id, func, sfields, svalues from sync order by id ASC limit 1");
		if (result.Count > 0) {
			if(haveInet){
				string[] cols = new string[]{ "id", "func", "fields", "values"};
				string[] values = new string[]{ ((string[])result [0])[0] , ((string[])result [0])[1], ((string[])result [0])[2], ((string[])result [0])[3]};
				sendData (cols, values, "sync");
			}
			//((string[])result [0])[0];
		}

		db.CloseDB();
		StartCoroutine (call_sync ());
	}

	public void insert_sync(string[] fields, string[] values, string sync_func){

		string fields_json = MiniJSON.Json.Serialize(fields);
		string values_json = MiniJSON.Json.Serialize(values);

		//Debug.Log ("insertar en sync fields: " + fields_json + "values: " + values_json + " func: " + sync_func);
		string newSyncId = getSyncNewId ();

		string[] colsF = new string[]{ "id", "func", "sfields", "svalues"};
		string[] colsV = new string[]{ newSyncId, sync_func, fields_json, values_json };

		db.OpenDB ("millasperrunas.db");
		db.InsertIntoSpecific("sync", colsF, colsV);
		db.CloseDB ();
	}

	private string getSyncNewId(){
		db.OpenDB("millasperrunas.db");
		ArrayList result = db.BasicQueryArray ("select id from sync order by id DESC limit 1");
		db.CloseDB();
		
		string newId = "1";
		
		if (result.Count > 0) {
			newId = ((string[])result [0]) [0];
			int newIdInt = int.Parse(newId)+1;
			newId = newIdInt.ToString();
		}
		
		return newId;
	}
	
	public void upload_foto_paseo(){
		if (haveInet) {
			byte[] fileData = File.ReadAllBytes (Application.persistentDataPath + "/" + foto_perro.temp_img);
		
			Debug.Log ("try upload: foto perro");
			string[] cols2 = new string[]{"id", "fecha_entrada", "usuarios_id", "perros_id", "fileUpload", "foto", "paseos_id"};
			string[] data2 = new string[] {
				foto_perro.id.ToString (),
				foto_perro.fecha_entrada,
				foto_perro.usuarios_id,
				foto_perro.perros_id,
				"imagen_perro",
				foto_perro.temp_img,
				foto_perro.paseos_id
			};
			try {
				sendData (cols2, data2, "upload_paseo", fileData);
			} catch (IOException e) {
				Debug.Log (e);
			}
		}
	}

	public bool validEmail(string emailaddress){
		return System.Text.RegularExpressions.Regex.IsMatch(emailaddress, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
	}

	public void errorPopup2(string error = "Error", string toclose = ""){
		/*popupScript.btnAction = 'c';
		popupObj.SetActive (true);
		popupScript.SendMessage ("setText", error);*/

		popup.SetActive (true);
		popupText.GetComponent<Text> ().text = error;
		if (toclose == "1") {
			popupButton.SetActive (false);
		} else {
			popupButton.SetActive (true);
		}
	}

	public void showLoading(bool show = true){
		loading.SetActive (show);
	}

	public void closePopup(){
		popup.SetActive (false);
	}

	private string todoQst = "";
	//question popup
	public void QestionPopup(string todo_ = "", string qstText = "Estas seguro?"){
		qstPopup.SetActive (true);
		qstPopupText.GetComponent<Text> ().text = qstText;
		todoQst = todo_;
	}

	public void closeQstPopup(){
		qstPopup.SetActive (false);
	}

	public void acceptQstPopup(){
		if ( todoQst == "deletePerroUsuario" ) {
			db.OpenDB("millasperrunas.db");
			db.BasicQueryInsert ( "update perros_usuarios set aceptado = '0' where perros_id = '"+perro.id.ToString()+"' and usuarios_id = '"+userData.id.ToString()+"' " );
			db.CloseDB();
			
			string[] cols = new string[]{ "usuarios_id", "perros_id" };
			string[] colsVals = new string[]{ userData.id.ToString(), perro.id.ToString() };
			
			//sync
			insert_sync(cols, colsVals, "delete_perro_usuario");
			
			Application.LoadLevel("cargar-invitar");
		}

		qstPopup.SetActive (false);
	}


	public void errorPopup(string error = "Error", string toclose = ""){

		string btnText = "Aceptar";
		/*if (toclose != "" && toclose != null) {
			btnText = "Entiendo";
			errorChrs = error;
		}*/

		NPBinding.UI.ShowAlertDialogWithSingleButton ("Alerta!", error, btnText, (string _buttonPressed)=>{
			if (_buttonPressed == "Aceptar") {
				Debug.Log("aceptado");
			}
			if (_buttonPressed == "Entiendo") {
				errorPopup(errorChrs, "1");
			}
		}); 
	}


	public bool checkImageExists(string image_){
		string filepath = Application.persistentDataPath + "/" + image_;
		if (File.Exists (filepath)) {
			return true;
		} else {
			return false;
		}
	}

	public Sprite spriteFromFile(string image_){
		Debug.Log ("spriteFromFile: " + image_);
		Sprite sprite = new Sprite ();
		if (image_ != "") {

			byte[] fileData = File.ReadAllBytes (Application.persistentDataPath + "/" + image_);
			Texture2D tex = new Texture2D (2, 2);
			tex.LoadImage (fileData); //..this will auto-resize the texture dimensions.

			Debug.Log (tex.width + "x" + tex.height);
			sprite = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0f, 0f));

		} else {
			Texture2D tex = Resources.Load("default (2)") as Texture2D;
			sprite = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0f, 0f));
		}
		return sprite;
	}

	/*public void uploadImageFromFile(string image_){
		byte[] fileData = File.ReadAllBytes( Application.persistentDataPath + "/" + image_ );

		Debug.Log ("try upload: imagen");
		string[] cols = new string[]{"usuarios_id", "fileUpload"};
		string[] data = new string[]{userData.id.ToString(), "imagen_perro" };
		try{
			sendData(cols, data, "upload_info", fileData);
		}catch(IOException e){
			Debug.Log(e);
		}
		
		Debug.Log("Sending to upload_info");
	}

	public void tryUpload() {
		Debug.Log ("try upload: imagen");
		string[] cols = new string[]{"usuarios_id", "fileUpload"};
		string[] data = new string[]{userData.id.ToString(), "imagen_perro" };
		try{
			sendData(cols, data, "upload_info", perro.ImgBytes);
		}catch(IOException e){
			Debug.Log(e);
		}
		
		Debug.Log("Sending to upload_info");
	}*/

	public Sprite saveTextureRotate(string fileName, string rotate = "L"){
		//yield return new WaitForSeconds(0.2f);

		byte[] fileData = File.ReadAllBytes (Application.persistentDataPath + "/" + fileName);
		Texture2D loadTexture = new Texture2D (2, 2);
		loadTexture.LoadImage (fileData);

		Texture2D photoCam = new Texture2D(loadTexture.height, loadTexture.width, TextureFormat.ARGB32, false);;
		
		for (var y = 0; y < loadTexture.height; y++) {

			int newY = y;
			if(rotate == "L"){
				newY = loadTexture.height - y;
			}

			for (var x = 0; x < loadTexture.width; x++) {
				int newX = x;
				if(rotate == "R"){
					newX = loadTexture.width - x;
				}
				photoCam.SetPixel (newY, newX, loadTexture.GetPixel (x, y));
			}
		}
		
		photoCam.Apply ();

		byte[] ImgBytes_ = photoCam.EncodeToPNG ();
		File.WriteAllBytes (Application.persistentDataPath + "/" + fileName, ImgBytes_);

		Sprite sprite = Sprite.Create (photoCam, new Rect (0, 0, photoCam.width, photoCam.height), new Vector2 (0f, 0f));
		return sprite;
	}

	public IEnumerator saveTextureToFile(Texture2D /*savedTexture */loadTexture, string fileName, char tosave, bool rotateLeft = false){
		yield return new WaitForSeconds(0.5f);

		int newWidth = 800;
		int newHeigth =  (newWidth * loadTexture.height / loadTexture.width) ;

		Texture2D savedTexture = ScaleTexture (loadTexture, newWidth, newHeigth);

		Debug.Log ("guardar textura en imagen: " + fileName + " " + savedTexture.width + "x" + savedTexture.height);

		Texture2D newTexture = new Texture2D(savedTexture.width, savedTexture.height, TextureFormat.ARGB32, false);
		
		newTexture.SetPixels(0,0, savedTexture.width, savedTexture.height, savedTexture.GetPixels());
		newTexture.Apply();

		//rotar si es necesario
		Texture2D photoCam = newTexture;
		if (rotateLeft) {

			photoCam = new Texture2D (newTexture.height, newTexture.width);
			
			for (var y = 0; y < newTexture.height; y++) {
				for (var x = 0; x < newTexture.width; x++) {
					//int newX = newTexture.width - x;
					photoCam.SetPixel (y, x, newTexture.GetPixel (x, y));
				}
			}
			
			photoCam.Apply ();
		}

		if (tosave == 'p') {
			if (rotateLeft) {
				perro.ImgBytes = photoCam.EncodeToPNG ();
			}else{
				perro.ImgBytes = newTexture.EncodeToPNG ();
			}
			perro.temp_img = fileName;
		
			File.WriteAllBytes (Application.persistentDataPath + "/" + perro.temp_img, perro.ImgBytes);
			Debug.Log (Application.persistentDataPath + "/" + perro.temp_img);
		}else if(tosave == 'u'){
			if (rotateLeft) {
				userData.ImgBytes = photoCam.EncodeToPNG ();
			}else{
				userData.ImgBytes = newTexture.EncodeToPNG ();
			}

			userData.temp_img = fileName;
			
			File.WriteAllBytes (Application.persistentDataPath + "/" + userData.temp_img, userData.ImgBytes);
			Debug.Log (Application.persistentDataPath + "/" + userData.temp_img);
		}else if(tosave == 'f'){
			if (rotateLeft) {
				foto_perro.ImgBytes = photoCam.EncodeToPNG ();
			}else{
				foto_perro.ImgBytes = newTexture.EncodeToPNG ();
			}

			foto_perro.temp_img = fileName;
			
			File.WriteAllBytes (Application.persistentDataPath + "/" + foto_perro.temp_img, foto_perro.ImgBytes);
			Debug.Log (Application.persistentDataPath + "/" + foto_perro.temp_img);


			string[] cols = new string[]{ "id", "fecha_entrada", "usuarios_id", "perros_id", "foto", "subida", "paseos_id"};
			foto_perro.id = generateId ();
			foto_perro.fecha_entrada = getActualDate();
			foto_perro.usuarios_id = userData.id.ToString();
			foto_perro.perros_id = perro.id.ToString();
			foto_perro.paseos_id = idPaseo;
			string[] colsValues = new string[]{ foto_perro.id.ToString(), foto_perro.fecha_entrada, foto_perro.usuarios_id, foto_perro.perros_id, foto_perro.temp_img, "0", idPaseo };

			db.OpenDB("millasperrunas.db");
			db.InsertIntoSpecific ("fotos_paseo", cols, colsValues);
			db.CloseDB();
			/*db.UpdateSingle("notificaciones", "visto", "1", "id", notificacionId);
			db.CloseDB();*/

			//redireccionar a gracias
			Application.LoadLevel ("msg-foto-tomada");
			showLoading (false);
		}
	}

	public string getActualDate(){
		return DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss");
	}

	public string getHour(){
		return DateTime.Now.ToString ("HH");
	}

	public int getDayOfTheWeek(){
		return (int) DateTime.Now.DayOfWeek;
	}


	private Texture2D ScaleTexture(Texture2D source,int targetWidth,int targetHeight) {
		Texture2D result=new Texture2D(targetWidth,targetHeight,source.format,false);
		float incX=(1.0f / (float)targetWidth);
		float incY=(1.0f / (float)targetHeight);
		for (int i = 0; i < result.height; ++i) {
			for (int j = 0; j < result.width; ++j) {
				Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
				result.SetPixel(j, i, newColor);
			}
		}
		result.Apply();
		return result;
	}

	public IEnumerator updateRegId(){
		yield return new WaitForSeconds (1f);
		if (userData.id != null && userData.id != 0) {
			string[] cols = new string[]{ "usuarios_id", "regid", "plataforma" };
			string[] colsVals = new string[]{ userData.id.ToString (), userData.reg_id, userData.plataforma };
		
			//sync
			insert_sync (cols, colsVals, "update_regid");
		}
	}

	private void checkFinishWeek(){

		/*string perros_list = "0";
		
		db.OpenDB("millasperrunas.db");
		ArrayList result = db.BasicQueryArray ("select id from perros");
		db.CloseDB();
		
		if (result.Count > 0) {
			foreach (string[] row_ in result) {
				perros_list += "," + row_[0];
			}
		}*/

		string[] colsUsuarios = new string[]{ "usuarios_id" };
		string[] colsUsuariosValues = new string[]{ userData.id.ToString () };
		
		sendData (colsUsuarios, colsUsuariosValues, "checkFinishWeek");
	}

	public void checkBadget(int badgetId = 0, int perro_id = 0){

		string perroId = (perro_id != 0) ? perro_id.ToString() : perro.id.ToString ();

		if (perroId != null && perroId != "") {

			Badges badges = new Badges ();

			//verificar si ya tengo el badget
			db.OpenDB ("millasperrunas.db");
			ArrayList result = db.BasicQueryArray ("select badges_id from badges_usuarios where badges_id = '" + badgetId.ToString () + "' and usuarios_id = '" + userData.id.ToString () + "' and perros_id = '" + perroId + "'  ");
			db.CloseDB ();

			if (result.Count == 0 && badges.checkBadges (badgetId)) {

				string generatedId = getBadgesUsuariosNewId();
				string[] cols = new string[]{"id", "badges_id", "usuarios_id", "fecha_entrada", "perros_id"};
				string[] colsVals = new string[] {
					generatedId,
					badgetId.ToString (),
					userData.id.ToString (),
					getActualDate (),
					perroId
				};

				//Debug.Log(generatedId+ "|" + badgetId + "|" + userData.id + "|" + getActualDate () + "|" + perroId );
			
				Debug.Log("guardo badge nro: " + badgetId);
				db.OpenDB ("millasperrunas.db");
				db.InsertIntoSpecific ("badges_usuarios", cols, colsVals);
				db.CloseDB ();

				//sync
				insert_sync (cols, colsVals, "badges_usuarios");

			}


		}
	}


	/*public enum ImageFilterMode : int {
		Nearest = 0,
		Biliner = 1,
		Average = 2
	}
	public static Texture2D ResizeTexture(Texture2D pSource, ImageFilterMode pFilterMode, float pScale){
		
		//*** Variables
		int i;
		
		//*** Get All the source pixels
		Color[] aSourceColor = pSource.GetPixels(0);
		Vector2 vSourceSize = new Vector2(pSource.width, pSource.height);
		
		//*** Calculate New Size
		float xWidth = Mathf.RoundToInt((float)pSource.width * pScale);                     
		float xHeight = Mathf.RoundToInt((float)pSource.height * pScale);
		
		//*** Make New
		Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.RGBA32, false);
		
		//*** Make destination array
		int xLength = (int)xWidth * (int)xHeight;
		Color[] aColor = new Color[xLength];
		
		Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);
		
		//*** Loop through destination pixels and process
		Vector2 vCenter = new Vector2();
		for(i=0; i<xLength; i++){
			
			//*** Figure out x&y
			float xX = (float)i % xWidth;
			float xY = Mathf.Floor((float)i / xWidth);
			
			//*** Calculate Center
			vCenter.x = (xX / xWidth) * vSourceSize.x;
			vCenter.y = (xY / xHeight) * vSourceSize.y;
			
			//*** Do Based on mode
			//*** Nearest neighbour (testing)
			if(pFilterMode == ImageFilterMode.Nearest){
				
				//*** Nearest neighbour (testing)
				vCenter.x = Mathf.Round(vCenter.x);
				vCenter.y = Mathf.Round(vCenter.y);
				
				//*** Calculate source index
				int xSourceIndex = (int)((vCenter.y * vSourceSize.x) + vCenter.x);
				
				//*** Copy Pixel
				aColor[i] = aSourceColor[xSourceIndex];
			}
			
			//*** Bilinear
			else if(pFilterMode == ImageFilterMode.Biliner){
				
				//*** Get Ratios
				float xRatioX = vCenter.x - Mathf.Floor(vCenter.x);
				float xRatioY = vCenter.y - Mathf.Floor(vCenter.y);
				
				//*** Get Pixel index's
				int xIndexTL = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
				int xIndexTR = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
				int xIndexBL = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
				int xIndexBR = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
				
				//*** Calculate Color
				aColor[i] = Color.Lerp(
					Color.Lerp(aSourceColor[xIndexTL], aSourceColor[xIndexTR], xRatioX),
					Color.Lerp(aSourceColor[xIndexBL], aSourceColor[xIndexBR], xRatioX),
					xRatioY
					);
			}
			
			//*** Average
			else if(pFilterMode == ImageFilterMode.Average){
				
				//*** Calculate grid around point
				int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
				int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
				int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
				int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);
				
				//*** Loop and accumulate
				Vector4 oColorTotal = new Vector4();
				Color oColorTemp = new Color();
				float xGridCount = 0;
				for(int iy = xYFrom; iy < xYTo; iy++){
					for(int ix = xXFrom; ix < xXTo; ix++){
						
						//*** Get Color
						oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];
						
						//*** Sum
						xGridCount++;
					}
				}
				
				//*** Average Color
				aColor[i] = oColorTemp / (float)xGridCount;
			}
		}
		
		//*** Set Pixels
		oNewTex.SetPixels(aColor);
		oNewTex.Apply();
		
		//*** Return
		return oNewTex;
	}*/

}
/*
[Serializable]
class perrosData {

	
	public perrosData(){
	}
}*/

