using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class WMG_X_puntosChart : MonoBehaviour {
	
	public Text Puntos;
	public Text Kilometros;

	public Text ciudad;
	public Text ciudadKm;

	//public GameObject graphPrefab;
	public WMG_Axis_Graph graph;
	public GameObject graphGO;

	private MainController GMS;
	
	public List<Vector2> chartData;
	public List<string> chartLabels;

	private int posX;
	private int i;
	
	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		ciudadKm.text = "0 KM";
		ciudad.text = "Buenos Aires";

		GMS.db.OpenDB("millasperrunas.db");

		ArrayList result = GMS.db.BasicQueryArray ("select puntos, kilometros, fecha_entrada from paseos where perros_id = '"+GMS.perro.id.ToString()+"' and usuarios_id = '"+GMS.userData.id.ToString()+"' order by fecha_entrada DESC limit 10 ");

		GMS.db.CloseDB();

		Debug.Log("en start");

		float maxPuntos = 0;

		float sumPuntos = 0f;
		float sumKms = 0f;

		i = 0;
		if (result.Count > 0) {

			i = 1;
			if(result.Count == 1){
				chartData.Add(new Vector2 (5, 0 ));
				chartLabels.Add(" ");
			}

			//obtengo el maximo
			foreach(string[] row_ in result){
				sumPuntos += float.Parse(row_[0]);
				sumKms += float.Parse(row_[1]);
				if( float.Parse(row_[0])  > maxPuntos){
					maxPuntos = float.Parse(row_[0]);
				}
			}

			//2500 ---- 90
			//1250 ----- x
			bool hasCKM = false;

			Debug.Log("hay datos");
			Debug.Log("max puntos: " + maxPuntos);
			foreach(string[] row_ in result){
				/*string kmsA = "0";
				if(row_[1] != "" && row_[1] != null){
					float kmsF = float.Parse(row_[1]) / 1000;
					kmsA = kmsF.ToString("n2");
				}*/
				string kmsA = "0";
				if(row_[1] != "" && row_[1] != null){
					kmsA = float.Parse( row_[1] ).ToString("n2");;
				}

				Debug.Log("puntos: " + row_[0] + " | " + row_[1] );
				i ++;
				posX = 5*i;

				float calcPuntos = float.Parse( row_[0] ) * 90 / maxPuntos;
				Debug.Log("calc puntos: " + calcPuntos);

				chartData.Add(new Vector2 (posX, calcPuntos ));

				chartLabels.Add(row_[0]+ " Pts" + System.Environment.NewLine + kmsA + " Km" + System.Environment.NewLine + GMS.userData.formatDate2( row_[2] ) );

				if(!hasCKM){
					ciudadKm.text = kmsA + " KMS";
					hasCKM = true;
				}

				//Debug.Log(row_[0]+ " Pts" + System.Environment.NewLine + kmsA + " Km | %: " + calcPuntos);
			}
		}


		Puntos.text = sumPuntos.ToString () + " PTS";
		Kilometros.text = sumKms.ToString ("n2") + " KMS";

		chartData.Reverse();
		chartLabels.Reverse ();

		startGraph ();
	}
	
	private void startGraph(){

		//cargar los datos faltantes a 10 si no hay
		if (i < 10) {
			for(int i2 = i; i2 <= 10; i2 ++){
				posX = 5*i2;
				chartData.Add(new Vector2 (posX, 0));
			}
		}

		graph = graphGO.GetComponent<WMG_Axis_Graph>();
		graph.lineSeries[0].GetComponent<WMG_Series>().pointValues = chartData;
		graph.xAxisNumTicks = 10; 


		List<string> xLabels = new List<string>();

		foreach (string label_ in chartLabels) {
			xLabels.Add (label_);
		}

		//cargar los labels q faltan segun i
		if (i < 10) {
			for(int i2 = i; i2 <= 10; i2 ++){
				xLabels.Add (" ");
			}
		}

		graph.xAxisLabels = xLabels;

		//cambio el color del actual
		if(chartLabels.Count > 0){
			StartCoroutine(changeActColor());
		}

		/*xLabels.Add ("uno"+ System.Environment.NewLine + "dada");
		xLabels.Add ("dos");
		xLabels.Add ("tres");
		xLabels.Add ("cuatro");
		xLabels.Add ("cinco");
		xLabels.Add ("seis");
		xLabels.Add ("siete");
		xLabels.Add ("ocho");
		xLabels.Add ("nueve");
		xLabels.Add ("diez");*/
		


		//precharge data
		
		/*int prevVal = 0;
		int nextVal = 0;
		
		for(int i = 1; i < 11; i ++){ 
			
			int sumAnt = Random.Range (0, 70);
			
			int posX = 5*i;
			chartData.Add(new Vector2 (posX, sumAnt));
		}*/
		
		//GameObject graphGO = GameObject.Instantiate(graphPrefab) as GameObject;
		/*graph = graphGO.GetComponent<WMG_Axis_Graph>();
		
		graph.lineSeries[0].GetComponent<WMG_Series>().pointValues = chartData;
		
		graph.xAxisNumTicks = 10; 
		Debug.Log("xAxisNumTicks: " + graph.xAxisNumTicks) ;
		
		List<string> xLabels = new List<string>();
		xLabels.Add ("uno"+ System.Environment.NewLine + "dada");
		xLabels.Add ("dos");
		xLabels.Add ("tres");
		xLabels.Add ("cuatro");
		xLabels.Add ("cinco");
		xLabels.Add ("seis");
		xLabels.Add ("siete");
		xLabels.Add ("ocho");
		xLabels.Add ("nueve");
		xLabels.Add ("diez");
		
		graph.xAxisLabels = xLabels;*/

	}

	private IEnumerator changeActColor(){
		int nodeNum = chartLabels.Count - 1;
		yield return new WaitForSeconds (0.3f);
		GameObject.Find ("XAxisLabels/WMG_Node_"+nodeNum+"/Text").GetComponent<Text>().color = new Color(0f/255.0f, 166.0f/255.0f, 75.0f/255.0f);
	}
	
	/*IEnumerator changeData(){
		yield return new WaitForSeconds(1);
		
		int sumAnt = Random.Range (0, 70);
		
		//borrar el primero
		chartData.RemoveRange(0, System.Math.Min(chartData.Count, 1));
		//agregar valor nuevo
		chartData.Add(new Vector2 (100, sumAnt));
		
		StartCoroutine (changeData ());
	}*/

}
