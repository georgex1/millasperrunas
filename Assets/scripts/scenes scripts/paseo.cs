using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class paseo : MonoBehaviour {

	private MainController GMS;

	public GameObject textTime;
	public GameObject textPuntos;
	public GameObject textKm;
	public GameObject perro_bck;

	private Text textTime_;
	private Text textPuntos_;
	private Text textKm_;

	private float sumDistanceAct;
	WMG_X_velocityChart velocityChart;

	private int actSecs;

	//private int actSec;
	//float calcPuntos;

	// Use this for initialization
	void Start () {

		velocityChart = GameObject.Find ("Graph").GetComponent<WMG_X_velocityChart>();


		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		//checkeo badges
		GMS.checkBadget(3);
		if( int.Parse( GMS.clima_temperatura ) < 0 ){
			GMS.checkBadget(4);
		}
		if( int.Parse( GMS.getHour() ) < 6 && int.Parse( GMS.getHour() ) > 3 ){
			GMS.checkBadget(5);
		}
		if( int.Parse( GMS.clima_viento ) > 10 ){
			GMS.checkBadget(6);
		}
		if( int.Parse( GMS.clima_temperatura ) > 40 ){
			GMS.checkBadget(7);
		}

		if ( GMS.getDayOfTheWeek () == 0) {
			GMS.checkBadget(8);
		}

		if( int.Parse( GMS.getHour() ) > 1 && int.Parse( GMS.getHour() ) < 5 ){
			GMS.checkBadget(9);
		}

		if( GMS.clima_condicion == "showers" || GMS.clima_condicion == "freezing rain" || GMS.clima_condicion == "mixed rain and sleet" || 
		   GMS.clima_condicion == "thunderstorms" || GMS.clima_condicion ==  "tropical storm" || GMS.clima_condicion == "mixed rain and hail" ){
			GMS.checkBadget(10);
		}


		//inicializo id paseo
		if (GMS.idPaseo == "") {
			GMS.idPaseo = GMS.generateId().ToString();
		}

		if (!GMS.paseando) {
			GMS.paseoTime = 0f;
			GMS.paseoKm = 0f;
			GMS.showLoading (true);
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

		ArrayList result = GMS.db.BasicQueryArray ("select puntos, motivos_id from puntos_especiales where fecha_desde > '"+GMS.getActualDate()+"' and fecha_hasta < '"+GMS.getActualDate()+"' and habilitado = '1' ");
		if (result.Count > 0) {

			//comprobar por tipo de puntos especiales la hora, clima, etc
			if(((string[])result [0]) [1] == "1"){//noche
				if( int.Parse( GMS.getHour() ) > 17 && int.Parse( GMS.getHour() ) < 6 ){
					GMS.puntosCalc = float.Parse (((string[])result [0]) [0]);
					GMS.puntosEspecialesMotivoId = ((string[])result [0]) [1];
				}
			}else if(((string[])result [0]) [1] == "3"){//frio
				if( int.Parse( GMS.clima_temperatura ) < 17 ){
					GMS.puntosCalc = float.Parse (((string[])result [0]) [0]);
					GMS.puntosEspecialesMotivoId = ((string[])result [0]) [1];
				}
			}else if(((string[])result [0]) [1] == "4"){//temprano
				if( int.Parse( GMS.getHour() ) > 5 && int.Parse( GMS.getHour() ) < 12 ){
					GMS.puntosCalc = float.Parse (((string[])result [0]) [0]);
					GMS.puntosEspecialesMotivoId = ((string[])result [0]) [1];
				}
			}else if(((string[])result [0]) [1] == "5"){//lluvia
				if( GMS.clima_condicion == "showers" || GMS.clima_condicion == "freezing rain" || GMS.clima_condicion == "mixed rain and sleet" || 
				   GMS.clima_condicion == "thunderstorms" || GMS.clima_condicion ==  "tropical storm" || GMS.clima_condicion == "mixed rain and hail" ){
					GMS.puntosCalc = float.Parse (((string[])result [0]) [0]);
					GMS.puntosEspecialesMotivoId = ((string[])result [0]) [1];
				}
			}else{
				GMS.puntosCalc = float.Parse (((string[])result [0]) [0]);
				GMS.puntosEspecialesMotivoId = ((string[])result [0]) [1];
			}

			if( ((string[])result [0]) [1] == "2" || ((string[])result [0]) [1] == "6" ){//show o futbol
				GMS.checkBadget(1);
			}


			//GMS.puntosCalc = float.Parse (((string[])result [0]) [0]);
			//GMS.puntosEspecialesMotivoId = ((string[])result [0]) [1];
		} /*else {

		}*/
		GMS.db.CloseDB();

		if (GMS.gps_active) {
			Debug.Log("en pase... GPS esta activo");
		}

		//StartCoroutine (updateGraph());
		//Debug.Log ("desde paseo act_lat de GPSC: " + GMS.GPSC.act_lat);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (GMS.gps_active && GMS.loading.activeSelf) {
			GMS.showLoading (false);
		}

		textTime_.text = GMS.gps_displayHours.ToString () + "H - " + GMS.gps_displayMinutes.ToString () + "M - " +GMS.gps_displaySeconds.ToString () + "S";

		if (actSecs != GMS.gps_displaySeconds) {
			actSecs = GMS.gps_displaySeconds;
			textPuntos_.text = GMS.gps_calcPuntos.ToString ("n2") + " PUNTOS";
			textKm_.text = GMS.gps_partialVeloc.ToString ("n2") + "KM/H";
		Debug.Log ("velocidad para chart: " + GMS.gps_partialVeloc);
			velocityChart.partialVeloc = int.Parse( GMS.gps_partialVeloc.ToString() ) * 2;
		}

		//if (GPSC.gps_active) {

		/*int roundedRestSeconds = Mathf.CeilToInt (GMS.paseoTime);
		int displaySeconds = roundedRestSeconds % 60;
		int displayMinutes = roundedRestSeconds / 60;
		int displayHours = displayMinutes / 60;
		
		textTime_.text = displayHours.ToString () + "H - " + displayMinutes.ToString () + "M - " + displaySeconds.ToString () + "S";
		
		//guardo en la db
		if (actSec != displaySeconds) {
			actSec = displaySeconds;
			
			if(sumDistanceAct != float.Parse(GPSC.sumDistance.ToString())){
				
				sumDistanceAct = float.Parse(GPSC.sumDistance.ToString());
				GMS.db.OpenDB ("millasperrunas.db");
				
				calcPuntos = calcPuntos + ( sumDistanceAct * GMS.puntosCalc );
				
				textPuntos_.text = calcPuntos.ToString ("n2") + " PUNTOS";
				textKm_.text = GPSC.partialVeloc.ToString ("n2") + "KM/H";
				
				float sumPuntos = float.Parse (GMS.perro.puntos) + calcPuntos;
				float sumKms = float.Parse (GMS.perro.kilometros) + float.Parse(GPSC.partialVeloc.ToString());
				
				GMS.db.BasicQueryInsert ("update perros_usuarios set puntos = " + sumPuntos.ToString () + ", kilometros = " + sumKms.ToString () + " " +
				                         "where perros_id = " + GMS.perro.id + " and usuarios_id = " + GMS.userData.id + " ");
				GMS.db.CloseDB ();
			}
		}*/
		//}
	}

	public void cargarEscena(string escena){
		Application.LoadLevel (escena);
	}

	public void finalizarPaseo(){
		GMS.paseoTime = 0f;
		GMS.paseoKm = 0f;
		GMS.paseando = false;
		GMS.idPaseo = "";
		//GMS.GPSC.resetVars ();

		gpsController GPSC = GameObject.Find ("GPSController").GetComponent<gpsController>();
		GPSC.SendMessage ("resetVars");

		if (GMS.puntosEspecialesMotivoId != "0") {
			Application.LoadLevel ("notificacion_puntos_especiales");
		} else {
			Application.LoadLevel ("home");
		}
	}
}
