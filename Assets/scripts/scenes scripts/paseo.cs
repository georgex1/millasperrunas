using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using VoxelBusters.NativePlugins;
using VoxelBusters.AssetStoreProductUtility.Demo;

public class paseo : MonoBehaviour {

	private MainController GMS;

	public GameObject textTime;
	public GameObject textPuntos;
	public GameObject textKm;
	public GameObject perro_bck;
	public GameObject GPSSignal;

	private Text textTime_;
	private Text textPuntos_;
	private Text textKm_;

	private float sumDistanceAct;
	WMG_X_velocityChart velocityChart;

	private int actSecs;

	public GameObject btnDebugSumLat;

	private bool showPopupSP = false;

	private float timeToPhoto = 600f; //10 minutos

	//private float charUpdate = 0f;

	//private int actSec;
	//float calcPuntos;

	// Use this for initialization
	void Start () {

		velocityChart = GameObject.Find ("Graph").GetComponent<WMG_X_velocityChart>();


		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		if (!GMS.isDebug) {
			btnDebugSumLat.SetActive(false);
		}

		//checkeo badges
		GMS.checkBadget(3);
		if (GMS.clima_temperatura != "" && GMS.clima_temperatura != null) {
			if (int.Parse (GMS.clima_temperatura) < 0) {
				GMS.checkBadget (4);
			}
			if( int.Parse( GMS.clima_temperatura ) > 40 ){
				GMS.checkBadget(7);
			}
		}
		if( int.Parse( GMS.getHour() ) < 6 && int.Parse( GMS.getHour() ) > 3 ){
			GMS.checkBadget(5);
		}
		if (GMS.clima_viento != "" && GMS.clima_viento != null) {
			if (int.Parse (GMS.clima_viento) > 10) {
				GMS.checkBadget (6);
			}
		}


		if ( GMS.getDayOfTheWeek () == 0) {
			GMS.checkBadget(8);
		}

		if( int.Parse( GMS.getHour() ) > 1 && int.Parse( GMS.getHour() ) < 5 ){
			GMS.checkBadget(9);
		}

		if (GMS.clima_condicion != "" && GMS.clima_condicion != null) {
			if (GMS.clima_condicion == "showers" || GMS.clima_condicion == "freezing rain" || GMS.clima_condicion == "mixed rain and sleet" || 
				GMS.clima_condicion == "thunderstorms" || GMS.clima_condicion == "tropical storm" || GMS.clima_condicion == "mixed rain and hail") {
				GMS.checkBadget (10);
			}
		}

		//inicializo id paseo
		if (GMS.idPaseo == "") {
			GMS.idPaseo = GMS.generateId().ToString();

			//borro el recorrido anterior
			GMS.db.OpenDB("millasperrunas.db");
			GMS.db.BasicQueryInsert("delete from ultimo_recorrido where perros_id = '"+GMS.perro.id+"' ");
			GMS.db.CloseDB();

			string[] fields = {"usuarios_id", "perros_id"};
			string[] values = {GMS.userData.id.ToString(), GMS.perro.id.ToString() };
			GMS.insert_sync(fields, values, "ultimo_recorrido_delete");

			GMS.paseoPhotoTaked = false;
		}

		if (!GMS.paseando) {
			GMS.paseoTime = 0f;
			//GMS.paseoKm = 0f;
			//GMS.showLoading (true);
		}

		GMS.paseando = true;
		textTime_ = textTime.GetComponent<Text>();
		textPuntos_ = textPuntos.GetComponent<Text>();
		textKm_ = textKm.GetComponent<Text>();
		//actSec = 0;
		//calcPuntos = 0f;

		Sprite sprite_ = GMS.spriteFromFile (GMS.perro.foto);
		perro_bck.GetComponent<Image> ().sprite = sprite_;

		//verificar si hay puntos especiales
		GMS.db.OpenDB("millasperrunas.db");

		GMS.puntosCalc = GMS.puntosCalcDefault;
		GMS.puntosEspecialesMotivoId = "0";

		ArrayList result = GMS.db.BasicQueryArray ("select puntos, motivos_id from puntos_especiales where fecha_desde < '"+GMS.getActualDate()+"' and fecha_hasta > '"+GMS.getActualDate()+"' and habilitado = '1' ");
		if (result.Count > 0) {
			foreach (string[] row_ in result) {

				Debug.Log ("hay puntos especiales");

				//comprobar por tipo de puntos especiales la hora, clima, etc
				if(((string[])row_) [1] == "1"){//noche
					if( int.Parse( GMS.getHour() ) > 17 && int.Parse( GMS.getHour() ) < 6 ){
						GMS.puntosCalc = float.Parse (((string[])row_) [0]);
						GMS.puntosEspecialesMotivoId = ((string[])row_) [1];
					}
				}else if(((string[])row_) [1] == "3"){//frio
					if( int.Parse( GMS.clima_temperatura ) < 17 ){
						GMS.puntosCalc = float.Parse (((string[])row_) [0]);
						GMS.puntosEspecialesMotivoId = ((string[])row_) [1];
					}
				}else if(((string[])row_) [1] == "4"){//temprano
					if( int.Parse( GMS.getHour() ) > 5 && int.Parse( GMS.getHour() ) < 12 ){
						GMS.puntosCalc = float.Parse (((string[])row_) [0]);
						GMS.puntosEspecialesMotivoId = ((string[])row_) [1];
					}
				}else if(((string[])row_) [1] == "5"){//lluvia
					if( GMS.clima_condicion == "showers" || GMS.clima_condicion == "freezing rain" || GMS.clima_condicion == "mixed rain and sleet" || 
					   GMS.clima_condicion == "thunderstorms" || GMS.clima_condicion ==  "tropical storm" || GMS.clima_condicion == "mixed rain and hail" ){
						GMS.puntosCalc = float.Parse (((string[])row_) [0]);
						GMS.puntosEspecialesMotivoId = ((string[])row_) [1];
					}
				}else{
					GMS.puntosCalc = float.Parse (((string[])row_) [0]);
					GMS.puntosEspecialesMotivoId = ((string[])row_) [1];
				}

				if( ((string[])row_) [1] == "2" || ((string[])row_) [1] == "6" ){//show o futbol
					GMS.checkBadget(1);
				}
			}
		} /*else {

		}*/
		GMS.db.CloseDB();

		Debug.Log ("puntos especiales: " + GMS.puntosEspecialesMotivoId);

		if (GMS.gps_active) {
			Debug.Log("en paseo... GPS esta activo");
		}
	}

	private IEnumerator checkGPSactive(){
		yield return new WaitForSeconds (2);
		if (GMS.gps_active) {
			GMS.showLoading (false);
		}
	}

	private void takePhotoQst(){
		Application.LoadLevel ("tomar-foto"); 
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!GPSSignal.activeSelf && !GMS.gpsRunning) {
			GPSSignal.SetActive(true);
		}

		if (GPSSignal.activeSelf && GMS.gpsRunning) {
			GPSSignal.SetActive(false);
		}



		if ( GMS.paseoTime >= timeToPhoto && !GMS.paseoPhotoTaked ) {
			GMS.paseoPhotoTaked = true;
			takePhotoQst();
		}

		textTime_.text = GMS.gps_displayHours.ToString () + "H - " + GMS.gps_displayMinutes.ToString () + "M - " +GMS.gps_displaySeconds.ToString () + "S";

		//if(charUpdate >= 0.5f){
		//	charUpdate = 0f;
		if (actSecs != GMS.gps_displaySeconds) {
			actSecs = GMS.gps_displaySeconds;
			textPuntos_.text = GMS.gps_calcPuntos.ToString () + " PUNTOS";
			textKm_.text = GMS.gps_partialVeloc.ToString ("n1") + "KM/H";
			//Debug.Log ("velocidad para chart: " + GMS.gps_partialVeloc);
			string valocPart = (GMS.gps_partialVeloc.ToString() == "") ? "0f" : GMS.gps_partialVeloc.ToString();
			int valocPart_int = (int) Mathf.Round( float.Parse(valocPart) ) ;

			velocityChart.partialVeloc = valocPart_int * 2;
		}
		//charUpdate += Time.deltaTime;

		if(!showPopupSP && GMS.iddleTime > 120){ //2 minutos
			showPopupSP = true;
			showSeguirPopup();
		}

	}

	private void showSeguirPopup(){
		string[] m_buttons = new string[] { "NO", "SI" };
		NPBinding.UI.ShowAlertDialogWithMultipleButtons ("Walk", "Detectamos que no te estas moviendo. ¿Seguís paseando?", m_buttons, (string _buttonPressed)=>{
			if(_buttonPressed == "SI"){
				GMS.iddleTime = 0;
				showPopupSP = false;
			}
			if(_buttonPressed == "NO"){
				finalizarPaseo();
			}
		});
	}

	public void cargarEscena(string escena){
		Application.LoadLevel (escena);
	}

	public void finalizarPaseo(){
		GMS.paseoTime = 0f;
		//GMS.paseoKm = 0f;
		GMS.paseando = false;
		GMS.idPaseo = "";
		GMS.gps_calcPuntos = 0f;
		//GMS.GPSC.resetVars ();

		gpsController GPSC = GameObject.Find ("GPSController").GetComponent<gpsController>();
		GPSC.SendMessage ("resetVars");

		if (GMS.puntosEspecialesMotivoId != "0") {
			Application.LoadLevel ("notificacion_puntos_especiales");
		} else {
			Application.LoadLevel ("home");
		}
	}

	//super debug
	public void sumLat(){
		GMS.debug_lng += 0.00002f;
	}
}
