using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WMG_X_velocityChart : MonoBehaviour {

	public GameObject graphPrefab;
	public WMG_Axis_Graph graph;
	
	public List<Vector2> chartData;
	public int partialVeloc;
	
	// Use this for initialization
	void Start () {
		partialVeloc = 0;
		startGraph ();
	}
	
	private void startGraph(){
		//precharge data
		for(int i = 1; i < 21; i ++){
			int posX = 5*i;
			chartData.Add(new Vector2 (posX, 0));
		}
		
		GameObject graphGO = GameObject.Instantiate(graphPrefab) as GameObject;
		graph = graphGO.GetComponent<WMG_Axis_Graph>();
		graph.changeSpriteParent(graphGO, this.gameObject);
		graph.changeSpritePositionTo(graphGO, Vector3.zero);
		
		graph.lineSeries[0].GetComponent<WMG_Series>().pointValues = chartData;
		
		StartCoroutine (changeData ());
	}
	
	IEnumerator changeData(){
		yield return new WaitForSeconds(2);
		
		//int sumAnt = Random.Range (0, 70);
		
		//borrar el primero
		chartData.RemoveRange(0, System.Math.Min(chartData.Count, 1));
		//agregar valor nuevo
		chartData.Add(new Vector2 (100, partialVeloc));
		
		StartCoroutine (changeData ());
	}
}
