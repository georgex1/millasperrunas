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

	private string appHash = "M11774Sp3RRun4A!";
	//private string responseURL = "http://thepastoapps.com/proyectos/millasperrunas/response/response.php";
	//private string responseAssets = "http://thepastoapps.com/proyectos/millasperrunas/response/assets/images/perros/";
	private string responseURL = "http://localhost/betterpixel/millasperrunas/response/response.php";
	private string responseAssets = "http://localhost/betterpixel/millasperrunas/response/assets/images/perros/";

	private string Uid;

	private float loadTime;
	private bool closeApp;
	private bool checkUpdate;

	//variables para paseo
	public float paseoTime;
	public bool paseando;
	public float puntosCalc;
	public float puntosCalcDefault;
	public float paseoKm;
	public string idPaseo;
	public string puntosEspecialesMotivoId;
	public int paseoPerroId = 0;

	//GPS vars
	public bool gps_active;
	public float gps_calcPuntos;
	public double gps_partialVeloc;
	public int gps_displaySeconds;
	public int gps_displayMinutes;
	public int gps_displayHours;
	public int gps_refreshGPS = 3;

	public dbAccess db ;

	public UserData userData;
	public PerroData perro;
	public InvitadoData invitado;
	public FotoPerro foto_perro;

	public bool haveInet;
	public bool checkingCon = false;

	//para debug
	public bool isDebug;
	public string sendDataDebug;

	//notificaciones
	public notifications notificationsScript;

	//popup
	public GameObject popup;
	public GameObject popupText;
	public GameObject popupButton;

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

		cols = new string[]{"id", "perros_id", "usuarios_id", "parentescos_id", "puntos", "kilometros", "puntos_totales", "puntos_semana", "aceptado"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT"};
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

		cols = new string[]{"id", "nombre", "descripcion", "orden", "habilitado", "serverupdate"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "INT", "TEXT", "TEXT"};
		db.CreateTable ("tips_paseo", cols, colTypes);

		cols = new string[]{"id", "titulo", "descripcion", "plataforma", "visto", "tipo", "serverupdate", "perros_id"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("notificaciones", cols, colTypes);

		cols = new string[]{"id", "fecha_desde", "fecha_hasta", "puntos", "habilitado", "motivos_id", "serverupdate"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("puntos_especiales", cols, colTypes);

		cols = new string[]{"id", "fecha_entrada", "usuarios_id", "perros_id", "foto", "subida"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("fotos_paseo", cols, colTypes);

		cols = new string[]{"id", "func", "sfields", "svalues"};
		colTypes = new string[]{"INT", "TEXT", "TEXT", "TEXT"};
		db.CreateTable ("sync", cols, colTypes);

		db.CloseDB();
	}

	// Use this for initialization
	void Start () {

		Uid = "";
		isDebug = false;
		checkUpdate = true;
		loadTime = 0;
		db = GetComponent<dbAccess>();
		createDb ();
		puntosCalc = 2;
		puntosCalcDefault = 2;

		paseoTime = 0f;
		paseando = false;
		paseoKm = 0f;
		puntosEspecialesMotivoId = "0";

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
		yield return new WaitForSeconds (4);

		if (haveInet) {
			yahooWeather yahooWeather_ = new yahooWeather (init_lat, init_lng);
			yahooWeather_.GetWeatherXml ();
			Debug.Log ("CLIMA CONDICION: " + yahooWeather_.Condition + " TEMPERATURA: " + yahooWeather_.Temperature);

			clima_condicion = yahooWeather_.Condition;
			clima_temperatura = yahooWeather_.Temperature;
			clima_viento = yahooWeather_.WindSpeed;

			PlayerPrefs.SetString("clima_temperatura", clima_temperatura);
			PlayerPrefs.SetString("clima_condicion", clima_condicion);
			PlayerPrefs.SetString("clima_viento", clima_viento);

		} else {
			StartCoroutine (get_clima());
		}
	}
	
	void Awake () {
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
			Debug.Log("WWW Ok!: " + www.text);

			IDictionary Wresponse = (IDictionary) MiniJSON.Json.Deserialize (www.text);

			string Wcontent_ = MiniJSON.Json.Serialize(Wresponse["content"]);
			string WarrayData_ = MiniJSON.Json.Serialize(Wresponse["arrayData"]);

			//Debug.Log("WWW content: " + Wcontent_);

			IDictionary Wresponse2 = (IDictionary) MiniJSON.Json.Deserialize ( Wcontent_ );
			IDictionary Wresponse3 = (IDictionary) MiniJSON.Json.Deserialize ( WarrayData_ );

			if((string)Wresponse["status"] == "error"){

				errorPopup((string)Wresponse2["mgs"], (string)Wresponse2["toclose"]);
			}else{
				
				if(response == "save_perro"){
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

					saveUserData(true);
					Application.LoadLevel ("subir-foto");

					download_perros();
				}
				if(response == "login_email"){

					userData.id = int.Parse( (string)Wresponse3["id"] );
					
					//saveUserData(true);
					populateUserData(Wresponse3);
					Application.LoadLevel ("subir-foto");

					download_perros();
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
					
					Debug.Log((string)Wresponse2["hasArray"]);
					if( (string)Wresponse2["hasArray"] != "0" ){
						for(int i = 1; i <= int.Parse( (string)Wresponse2["hasArray"] ); i++ ){
							Debug.Log("posicion: " + i);

							IDictionary reponseContent = (IDictionary) MiniJSON.Json.Deserialize ( (string)WresponseContent[i.ToString()]  );

							db.OpenDB("millasperrunas.db");

							//cargar perros
							string[] cols = new string[]{ "id", "nombre", "edad", "peso", "razas_id", "foto"};
							string[] colsVals = new string[]{ (string)reponseContent["id"], (string)reponseContent["nombre"], (string)reponseContent["edad"], (string)reponseContent["peso"], (string)reponseContent["razas_id"], (string)reponseContent["foto"] };
							
							db.InsertIgnoreInto("perros", cols, colsVals, (string)reponseContent["id"]);

							//intentar bajar imagen del perro
							try_download_perro_imagen((string)reponseContent["foto"]);

							//cargar perros_usuarios
							ArrayList result = db.BasicQueryArray ("select perros_id from perros_usuarios where usuarios_id = '"+userData.id.ToString()+"' and perros_id = '"+(string)reponseContent["id"]+"' ");
							if (result.Count == 0) {
								cols = new string[]{"perros_id", "usuarios_id", "parentescos_id", "puntos", "kilometros", "puntos_totales", "aceptado"};
								colsVals = new string[]{ (string)reponseContent["id"], userData.id.ToString(), (string)reponseContent["parentescos_id"], (string)reponseContent["puntos"], (string)reponseContent["kilometros"], (string)reponseContent["puntos_totales"], (string)reponseContent["aceptado"] };
								
								db.InsertIntoSpecific("perros_usuarios", cols, colsVals);
							}

							if((string)reponseContent["invitado_por_usuarios_id"] != ""){
								//ingresar notificacion de perro agregado
								cols = new string[]{"id", "titulo", "descripcion", "plataforma", "visto", "tipo", "serverupdate", "perros_id"};
								colsVals = new string[]{ generateId().ToString(), "Invitación para pasear a " + (string)reponseContent["nombre"], 
									(string)reponseContent["usuario_nombre"] + " te invita a pasear a " + (string)reponseContent["nombre"], "Android", "0", "invitacion", getActualDate(), (string)reponseContent["id"] };
								
								db.InsertIntoSpecific("notificaciones", cols, colsVals);
							}

							db.CloseDB();
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
							
							db.OpenDB("millasperrunas.db");

							//cargar familia
							string[] cols = new string[]{"id", "email", "nombre", "foto" };
							string[] colsVals = new string[]{ (string)reponseContent["id"], (string)reponseContent["email"], 
								(string)reponseContent["nombre"], (string)reponseContent["foto"] };
							
							db.InsertIgnoreInto("familia", cols, colsVals, (string)reponseContent["id"]);

							//insertar el perro al usuario

							cols = new string[]{ "perros_id", "usuarios_id", "parentescos_id", "puntos", "kilometros", "puntos_totales", "aceptado"};
							colsVals = new string[]{ (string)reponseContent["perros_id"], (string)reponseContent["id"], 
								(string)reponseContent["parentescos_id"], "0", "0", "0", "0"};

							db.InsertIntoSpecific("perros_usuarios", cols, colsVals);

							//intentar bajar imagen del perro
							try_download_perro_imagen((string)reponseContent["foto"]);
							
							db.CloseDB();
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
								
								db.InsertIgnoreInto("perros_usuarios", cols, colsVals, ((string[])result [0]) [0]);
							}
							
							db.CloseDB();
						}
					}
				}

				if(response == "get_updates"){

					//if((string)Wresponse2["mgs"] == "puntos_especiales_updated"){

					string WarrayContent_ = MiniJSON.Json.Serialize(Wresponse["arrayContent"]);
					IDictionary WresponseContent = (IDictionary) MiniJSON.Json.Deserialize ( WarrayContent_ );

					Debug.Log((string)Wresponse2["hasArray"]);
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
								string[] colsUsuarios = new string[]{ "id", "nombre", "descripcion", "orden", "habilitado", "serverupdate"};
								string[] colsUsuariosValues = new string[]{ (string)reponseContent["id"], (string)reponseContent["nombre"], (string)reponseContent["descripcion"], (string)reponseContent["orden"], (string)reponseContent["habilitado"], (string)reponseContent["serverupdate"] };
								
								db.InsertIgnoreInto("tips_paseo", colsUsuarios, colsUsuariosValues, (string)reponseContent["id"]);
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

	private void populateUserData(IDictionary values){
		userData.email = (string)values["email"];
		userData.nombre = (string)values["nombre"];
		userData.fecha_nacimiento = (string)values["fecha_nacimiento"];
		userData.sexo = (string)values["sexo"];

		saveUserData (false);
	}

	private void saveUserData(bool isfb){
		sendDataDebug = "entro a saveUserData";
		db.OpenDB("millasperrunas.db");
		
		string[] colsUsuarios = new string[]{ "id", "email", "nombre", "fbid", "fecha_nacimiento", "sexo"};

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
		string[] colsUsuariosValues = new string[]{ userData.id.ToString(), userData.email, userData.nombre, userData.fbid, userData.fecha_nacimiento, userData.sexo };
		
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

	private void download_perros(){
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

	public int generateId(){
		int timestamp = (int)Math.Truncate((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
		return timestamp;
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
		
		string[] colsUsuarios = new string[]{ "email", "nombre", "fbid", "fecha_nacimiento", "sexo", "plataforma", "regid"};
		string[] colsUsuariosValues = new string[]{ userData.email, userData.nombre, userData.fbid, userData.fecha_nacimiento, userData.sexo, userData.plataforma, userData.reg_id };
		
		sendData (colsUsuarios, colsUsuariosValues, "login_facebook");
	}

	public void loginEmail(){
		
		string[] colsUsuarios = new string[]{ "email", "password", "plataforma", "regid"};
		string[] colsUsuariosValues = new string[]{ userData.email, userData.password, userData.plataforma, userData.reg_id };
		
		sendData (colsUsuarios, colsUsuariosValues, "login_email");
	}

	public void registerPerro(){
		perro.id = generateId ();
		string[] cols = new string[]{ "id", "nombre", "edad", "peso", "razas_id", "parentescos_id", "usuarios_id"};
		string[] colsValues = new string[]{ perro.id.ToString(), perro.nombre, perro.edad, perro.peso, perro.razas_id, perro.parentescos_id, userData.id.ToString() };
		sendData (cols, colsValues, "save_perro");
	}

	public void savePerro(){
		db.OpenDB("millasperrunas.db");

		string[] cols = new string[]{ "id", "nombre", "edad", "peso", "razas_id", "foto"};
		string[] colsValues = new string[]{ perro.id.ToString(), perro.nombre, perro.edad, perro.peso, perro.razas_id, perro.foto };

		db.InsertIntoSpecific ("perros", cols, colsValues);


		cols = new string[]{ "perros_id", "usuarios_id", "parentescos_id", "puntos", "kilometros", "puntos_totales", "aceptado"};
		colsValues = new string[]{ perro.id.ToString(), userData.id.ToString(), perro.parentescos_id, "0", "0", "0", "1" };
		
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
		get_familiares_puntos ();
		download_perros();

		StartCoroutine (get_updates ());
	}

	public void get_familiares(){
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
		yield return new WaitForSeconds (4);
		sync ();
	}

	public void sync(){
		Debug.Log ("sync....");
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

		db.OpenDB ("millasperrunas.db");

		string fields_json = MiniJSON.Json.Serialize(fields);
		string values_json = MiniJSON.Json.Serialize(values);

		//Debug.Log ("insertar en sync fields: " + fields_json + "values: " + values_json + " func: " + sync_func);

		string[] colsF = new string[]{ "id", "func", "sfields", "svalues"};
		string[] colsV = new string[]{ generateId().ToString(), sync_func, fields_json, values_json };
		
		db.InsertIntoSpecific("sync", colsF, colsV);

		db.CloseDB ();
	}
	
	public void upload_foto_paseo(){
		if (haveInet) {
			byte[] fileData = File.ReadAllBytes (Application.persistentDataPath + "/" + foto_perro.temp_img);
		
			Debug.Log ("try upload: foto perro");
			string[] cols2 = new string[]{"id", "fecha_entrada", "usuarios_id", "perros_id", "fileUpload", "foto"};
			string[] data2 = new string[] {
				foto_perro.id.ToString (),
				foto_perro.fecha_entrada,
				foto_perro.usuarios_id,
				foto_perro.perros_id,
				"imagen_perro",
				foto_perro.temp_img
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

	public void errorPopup(string error = "Error", string toclose = ""){
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
			Texture2D tex = Resources.Load("default") as Texture2D;
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

	public IEnumerator saveTextureToFile(Texture2D /*savedTexture */loadTexture, string fileName, char tosave){
		yield return new WaitForSeconds(0.5f);

		int newWidth = 800;
		int newHeigth =  (newWidth * loadTexture.height / loadTexture.width) ;

		Texture2D savedTexture = ScaleTexture (loadTexture, newWidth, newHeigth);

		Debug.Log ("guardar textura en imagen: " + fileName + " " + savedTexture.width + "x" + savedTexture.height);

		Texture2D newTexture = new Texture2D(savedTexture.width, savedTexture.height, TextureFormat.ARGB32, false);
		
		newTexture.SetPixels(0,0, savedTexture.width, savedTexture.height, savedTexture.GetPixels());
		newTexture.Apply();
		if (tosave == 'p') {
			perro.ImgBytes = newTexture.EncodeToPNG ();
			perro.temp_img = fileName;
		
			File.WriteAllBytes (Application.persistentDataPath + "/" + perro.temp_img, perro.ImgBytes);
			Debug.Log (Application.persistentDataPath + "/" + perro.temp_img);
		}else if(tosave == 'u'){
			userData.ImgBytes = newTexture.EncodeToPNG ();
			userData.temp_img = fileName;
			
			File.WriteAllBytes (Application.persistentDataPath + "/" + userData.temp_img, userData.ImgBytes);
			Debug.Log (Application.persistentDataPath + "/" + userData.temp_img);
		}else if(tosave == 'f'){

			foto_perro.ImgBytes = newTexture.EncodeToPNG ();
			foto_perro.temp_img = fileName;
			
			File.WriteAllBytes (Application.persistentDataPath + "/" + foto_perro.temp_img, foto_perro.ImgBytes);
			Debug.Log (Application.persistentDataPath + "/" + foto_perro.temp_img);


			string[] cols = new string[]{ "id", "fecha_entrada", "usuarios_id", "perros_id", "foto", "subida"};
			foto_perro.id = generateId ();
			foto_perro.fecha_entrada = getActualDate();
			foto_perro.usuarios_id = userData.id.ToString();
			foto_perro.perros_id = perro.id.ToString();
			string[] colsValues = new string[]{ foto_perro.id.ToString(), foto_perro.fecha_entrada, foto_perro.usuarios_id, foto_perro.perros_id, foto_perro.temp_img, "0" };

			db.OpenDB("millasperrunas.db");
			db.InsertIntoSpecific ("fotos_paseo", cols, colsValues);

			db.UpdateSingle("notificaciones", "visto", "1", "id", notificacionId);
			db.CloseDB();

			//redireccionar a gracias
			Application.LoadLevel ("msg-foto-tomada");
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

		Badges badges = new Badges();

		//verificar si ya tengo el badget
		db.OpenDB ("millasperrunas.db");
		ArrayList result = db.BasicQueryArray ("select badges_id from badges_usuarios where badges_id = '"+badgetId.ToString()+"' and usuarios_id = '"+userData.id.ToString()+"' and perros_id = '"+perroId+"'  ");

		if ( result.Count == 0 && badges.checkBadges(badgetId) ) {

			/*if(badgetId == 2){
				DateTime dateValue = DateTime.Now;
				int dayoftheweek = (int) dateValue.DayOfWeek;

				if(dayoftheweek == 1){//termino la semana

				}
			}else{}*/

			string generatedId = generateId().ToString();
			string[] cols = new string[]{"id", "badges_id", "usuarios_id", "fecha_entrada", "perros_id"};
			string[] colsVals = new string[]{ generatedId, badgetId.ToString(), userData.id.ToString(), getActualDate(), perroId };
			
			db.InsertIntoSpecific("badges_usuarios", cols, colsVals);

			//sync
			insert_sync(cols, colsVals, "badges_usuarios");

		}

		db.CloseDB ();
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

