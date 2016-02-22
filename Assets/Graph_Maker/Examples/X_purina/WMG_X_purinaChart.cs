using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WMG_X_purinaChart : MonoBehaviour {

	public GameObject graphPrefab;
	public WMG_Axis_Graph graph;

	public List<Vector2> chartData;

	// Use this for initialization
	void Start () {

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
		yield return new WaitForSeconds(1);

		int sumAnt = Random.Range (0, 70);

		//borrar el primero
		chartData.RemoveRange(0, System.Math.Min(chartData.Count, 1));
		//agregar valor nuevo
		chartData.Add(new Vector2 (100, sumAnt));

		StartCoroutine (changeData ());
	}
	/*
	IEnumerator startTests() {
		yield return new WaitForSeconds(testGroupInterval);
		
		// graph type and orientation tests
		graph.graphTitleString = "Graph Type and Orientation Tests";
		StartCoroutine(graphTypeAndOrientationTests());
		if (!noTestDelay) yield return new WaitForSeconds(testInterval * 13);
		yield return new WaitForSeconds(testGroupInterval);
		
		// axes tests
		graph.graphTitleString = "Axes Tests";
		StartCoroutine(axesTests());
		if (!noTestDelay) yield return new WaitForSeconds(testInterval * 12);
		yield return new WaitForSeconds(testGroupInterval);
		
		// axes tests with bar chart
		graph.graphTitleString = "Axes Tests - Bar";
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		if (!noTestDelay) yield return new WaitForSeconds(testInterval);
		StartCoroutine(axesTests());
		if (!noTestDelay) yield return new WaitForSeconds(testInterval * 12);
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		yield return new WaitForSeconds(testGroupInterval);
		
		// add delete tests
		graph.graphTitleString = "Add / Delete Series Tests";
		StartCoroutine(addDeleteTests());
		if (!noTestDelay) yield return new WaitForSeconds(testInterval * 11);
		yield return new WaitForSeconds(testGroupInterval);
		
		// add delete tests with bar chart
		graph.graphTitleString = "Add / Delete Series Tests - Bar";
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		if (!noTestDelay) yield return new WaitForSeconds(testInterval);
		StartCoroutine(addDeleteTests());
		if (!noTestDelay) yield return new WaitForSeconds(testInterval * 11);
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		yield return new WaitForSeconds(testGroupInterval);
		
		// legend tests
		graph.graphTitleString = "Legend Tests";
		StartCoroutine(legendTests());
		if (!noTestDelay) yield return new WaitForSeconds(testInterval * 9);
		yield return new WaitForSeconds(testGroupInterval);
		
		// hide show tests
		graph.graphTitleString = "Hide / Show Tests";
		StartCoroutine(hideShowTests());
		if (!noTestDelay) yield return new WaitForSeconds(testInterval * 9);
		yield return new WaitForSeconds(testGroupInterval);
		
		// grids ticks tests
		graph.graphTitleString = "Grids / Ticks Tests";
		StartCoroutine(gridsTicksTests());
		if (!noTestDelay) yield return new WaitForSeconds(testInterval * 4);
		yield return new WaitForSeconds(testGroupInterval);
		
		// size tests
		graph.graphTitleString = "Resize Tests";
		StartCoroutine(sizeTests());
		if (!noTestDelay) yield return new WaitForSeconds(testInterval * 3);
		yield return new WaitForSeconds(testGroupInterval);
		
		graph.graphTitleString = "";
	}
	
	IEnumerator hideShowTests() {
		graph.legend.hideLegend = true;
		
		if (!noTestDelay) yield return waitTime;
		graph.hideXLabels = true;
		
		if (!noTestDelay) yield return waitTime;
		graph.hideYLabels = true;
		
		if (!noTestDelay) yield return waitTime;
		graph.hideXTicks = true;
		
		if (!noTestDelay) yield return waitTime;
		graph.hideYTicks = true;
		
		if (!noTestDelay) yield return waitTime;
		graph.hideXGrid = true;
		
		if (!noTestDelay) yield return waitTime;
		graph.hideYGrid = true;
		
		if (!noTestDelay) yield return waitTime;
		graph.SetActive(graph.xAxis, false);
		
		if (!noTestDelay) yield return waitTime;
		graph.SetActive(graph.yAxis, false);
		
		if (!noTestDelay) yield return waitTime;
		graph.legend.hideLegend = false;
		graph.hideXLabels = false;
		graph.hideYLabels = false;
		graph.hideXTicks = false;
		graph.hideYTicks = false;
		graph.hideXGrid = false;
		graph.hideYGrid = false;
		graph.SetActive(graph.xAxis, true);
		graph.SetActive(graph.yAxis, true);
	}
	
	IEnumerator gridsTicksTests() {
		List<string> xLabels = new List<string>(graph.xAxisLabels);
		
		WMG_Anim.animInt(()=> graph.yAxisNumTicks, x=> graph.yAxisNumTicks = x, animDuration, 11);
		
		if (!noTestDelay) yield return waitTime;
		graph.xLabelType = WMG_Axis_Graph.labelTypes.ticks;
		graph.SetXLabelsUsingMaxMin = true;
		WMG_Anim.animInt(()=> graph.xAxisNumTicks, x=> graph.xAxisNumTicks = x, animDuration, 11);
		
		if (!noTestDelay) yield return waitTime;
		WMG_Anim.animInt(()=> graph.yAxisNumTicks, x=> graph.yAxisNumTicks = x, animDuration, 3);
		
		if (!noTestDelay) yield return waitTime;
		WMG_Anim.animInt(()=> graph.xAxisNumTicks, x=> graph.xAxisNumTicks = x, animDuration, 5);
		
		if (!noTestDelay) yield return waitTime;
		graph.xLabelType = WMG_Axis_Graph.labelTypes.ticks_center;
		graph.SetXLabelsUsingMaxMin = false;
		graph.xAxisLabels = xLabels;
	}
	
	IEnumerator sizeTests() {
		Vector2 origSize = graph.getSpriteSize(graph.gameObject);
		
		WMG_Anim.animSize(graph.gameObject, animDuration, Ease.Linear, new Vector2(origSize.x * 2, origSize.y * 2));
		
		if (!noTestDelay) yield return waitTime;
		WMG_Anim.animSize(graph.gameObject, animDuration, Ease.Linear, new Vector2(origSize.x * 2, origSize.y));
		
		if (!noTestDelay) yield return waitTime;
		WMG_Anim.animSize(graph.gameObject, animDuration, Ease.Linear, new Vector2(origSize.x, origSize.y * 2));
		
		if (!noTestDelay) yield return waitTime;
		WMG_Anim.animSize(graph.gameObject, animDuration, Ease.Linear, new Vector2(origSize.x, origSize.y));
		
		//		if (!noTestDelay) yield return waitTime;
		//		graph.resizeType = WMG_Axis_Graph.resizeTypes.percentage_padding;
		//		graph.resizeProperties = WMG_Axis_Graph.ResizeProperties.PointSizeBarWidth
		//				| WMG_Axis_Graph.ResizeProperties.SeriesLineWidth
		//				| WMG_Axis_Graph.ResizeProperties.AxesWidth
		//				| WMG_Axis_Graph.ResizeProperties.PointSizeBarWidth
		//				| WMG_Axis_Graph.ResizeProperties.FontSizeAxesLabels
		//				| WMG_Axis_Graph.ResizeProperties.LegendEntryLine
		//				| WMG_Axis_Graph.ResizeProperties.LegendEntrySpacing
		//				| WMG_Axis_Graph.ResizeProperties.FontSizeLegends;
		//		WMG_Anim.animSize(graph.gameObject, animDuration, Holoville.HOTween.EaseType.Linear, new Vector2(origSize.x / 2, origSize.y / 2));
		//
		//		if (!noTestDelay) yield return waitTime;
		//		WMG_Anim.animSize(graph.gameObject, animDuration, Holoville.HOTween.EaseType.Linear, new Vector2(origSize.x, origSize.y));
		//
		//		if (!noTestDelay) yield return waitTime;
		//		graph.resizeType = WMG_Axis_Graph.resizeTypes.none;
		//		graph.resizeProperties = 0;
	}
	
	IEnumerator legendTests() {
		
		graph.legend.legendType = WMG_Legend.legendTypes.Right;
		
		if (!noTestDelay) yield return waitTime;
		graph.legend.legendType = WMG_Legend.legendTypes.Bottom;
		graph.legend.oppositeSideLegend = true;
		
		if (!noTestDelay) yield return waitTime;
		graph.legend.legendType = WMG_Legend.legendTypes.Right;
		
		if (!noTestDelay) yield return waitTime;
		graph.legend.legendType = WMG_Legend.legendTypes.Bottom;
		graph.legend.oppositeSideLegend = false;
		
		if (!noTestDelay) yield return waitTime;
		addSeriesWithRandomData();
		addSeriesWithRandomData();
		addSeriesWithRandomData();
		
		if (!noTestDelay) yield return waitTime;
		graph.legend.numRowsOrColumns = 2;
		
		if (!noTestDelay) yield return waitTime;
		graph.legend.legendType = WMG_Legend.legendTypes.Right;
		
		if (!noTestDelay) yield return waitTime;
		graph.legend.numRowsOrColumns = 1;
		
		if (!noTestDelay) yield return waitTime;
		graph.deleteSeries();
		graph.deleteSeries();
		graph.deleteSeries();
		
		if (!noTestDelay) yield return waitTime;
		graph.legend.legendType = WMG_Legend.legendTypes.Bottom;
		
	}
	
	IEnumerator graphTypeAndOrientationTests() {
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		
		if (!noTestDelay) yield return waitTime;
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_stacked;
		
		if (!noTestDelay) yield return waitTime;
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_stacked_percent;
		
		if (!noTestDelay) yield return waitTime;
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		
		if (!noTestDelay) yield return waitTime;
		graph.orientationType = WMG_Axis_Graph.orientationTypes.horizontal;
		
		if (!noTestDelay) yield return waitTime;
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		
		if (!noTestDelay) yield return waitTime;
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_stacked;
		
		if (!noTestDelay) yield return waitTime;
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_stacked_percent;
		
		if (!noTestDelay) yield return waitTime;
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		
		if (!noTestDelay) yield return waitTime;
		graph.orientationType = WMG_Axis_Graph.orientationTypes.vertical;
		
		if (!noTestDelay) yield return waitTime;
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		
		if (!noTestDelay) yield return waitTime;
		graph.orientationType = WMG_Axis_Graph.orientationTypes.horizontal;
		
		if (!noTestDelay) yield return waitTime;
		graph.orientationType = WMG_Axis_Graph.orientationTypes.vertical;
		
		if (!noTestDelay) yield return waitTime;
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
	}
	
	IEnumerator axesTests() {
		graph.axesType = WMG_Axis_Graph.axesTypes.I_II;
		
		if (!noTestDelay) yield return waitTime;
		graph.axesType = WMG_Axis_Graph.axesTypes.II;
		
		if (!noTestDelay) yield return waitTime;
		graph.axesType = WMG_Axis_Graph.axesTypes.II_III;
		
		if (!noTestDelay) yield return waitTime;
		graph.axesType = WMG_Axis_Graph.axesTypes.III;
		
		if (!noTestDelay) yield return waitTime;
		graph.axesType = WMG_Axis_Graph.axesTypes.III_IV;
		
		if (!noTestDelay) yield return waitTime;
		graph.axesType = WMG_Axis_Graph.axesTypes.IV;
		
		if (!noTestDelay) yield return waitTime;
		graph.axesType = WMG_Axis_Graph.axesTypes.I_IV;
		
		if (!noTestDelay) yield return waitTime;
		graph.axesType = WMG_Axis_Graph.axesTypes.CENTER;
		
		if (!noTestDelay) yield return waitTime;
		graph.axesType = WMG_Axis_Graph.axesTypes.I;
		
		if (!noTestDelay) yield return waitTime;
		graph.axesType = WMG_Axis_Graph.axesTypes.AUTO_ORIGIN_X;
		graph.xAxisUseNonTickPercent = true;
		WMG_Anim.animVec2(()=> graph.theOrigin, x=> graph.theOrigin = x, animDuration, new Vector2(graph.theOrigin.x, graph.yAxisMaxValue));
		
		if (!noTestDelay) yield return waitTime;
		graph.axesType = WMG_Axis_Graph.axesTypes.AUTO_ORIGIN_Y;
		graph.yAxisUseNonTickPercent = true;
		WMG_Anim.animVec2(()=> graph.theOrigin, x=> graph.theOrigin = x, animDuration, new Vector2(graph.xAxisMaxValue, graph.theOrigin.y));
		
		if (!noTestDelay) yield return waitTime;
		graph.axesType = WMG_Axis_Graph.axesTypes.AUTO_ORIGIN;
		WMG_Anim.animVec2(()=> graph.theOrigin, x=> graph.theOrigin = x, animDuration, new Vector2(graph.xAxisMaxValue / 4, graph.yAxisMaxValue / 2));
		
		if (!noTestDelay) yield return waitTime;
		graph.axesType = WMG_Axis_Graph.axesTypes.I;
	}
	
	IEnumerator addDeleteTests() {
		WMG_Series s1 = graph.lineSeries[0].GetComponent<WMG_Series>();
		WMG_Series s2 = graph.lineSeries[1].GetComponent<WMG_Series>();
		List<Vector2> s1Data = s1.pointValues;
		List<Vector2> s2Data = s2.pointValues;
		Color s1PointColor = s1.pointColor;
		Color s2PointColor = s2.pointColor;
		float barWidth = graph.barWidth;
		
		addSeriesWithRandomData();
		
		if (!noTestDelay) yield return waitTime;
		graph.deleteSeries();
		
		if (!noTestDelay) yield return waitTime;
		addSeriesWithRandomData();
		
		if (!noTestDelay) yield return waitTime;
		addSeriesWithRandomData();
		
		if (!noTestDelay) yield return waitTime;
		graph.deleteSeries();
		
		if (!noTestDelay) yield return waitTime;
		graph.deleteSeries();
		
		if (!noTestDelay) yield return waitTime;
		graph.deleteSeries();
		
		if (!noTestDelay) yield return waitTime;
		graph.deleteSeries();
		
		if (!noTestDelay) yield return waitTime;
		addSeriesWithRandomData();
		
		if (!noTestDelay) yield return waitTime;
		graph.deleteSeries();
		
		if (!noTestDelay) yield return waitTime;
		addSeriesWithRandomData();
		graph.lineSeries[0].GetComponent<WMG_Series>().pointValues = s1Data;
		graph.lineSeries[0].GetComponent<WMG_Series>().pointColor = s1PointColor;
		
		if (!noTestDelay) yield return waitTime;
		addSeriesWithRandomData();
		graph.lineSeries[1].GetComponent<WMG_Series>().pointValues = s2Data;
		graph.lineSeries[1].GetComponent<WMG_Series>().pointColor = s2PointColor;
		graph.lineSeries[1].GetComponent<WMG_Series>().pointPrefab = 1;
		graph.barWidth = barWidth;
	}
	
	void addSeriesWithRandomData() {
		WMG_Series series = graph.addSeries();
		series.UseXDistBetweenToSpace = true;
		series.AutoUpdateXDistBetween = true;
		series.lineScale = 0.5f;
		series.pointColor = new Color(Random.Range(0, 1f),Random.Range(0, 1f),Random.Range(0, 1f),1);
		series.seriesName = "Series " + graph.lineSeries.Count;
		series.pointValues = graph.GenRandomY(graph.groups.Count, 1, graph.groups.Count, graph.yAxisMinValue, graph.yAxisMaxValue);
	}*/
	
}