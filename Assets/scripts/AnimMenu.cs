using UnityEngine;
using System.Collections;

public class AnimMenu : MonoBehaviour {
	public int menuShow = 0;
	private float resetTime = 0.00F;
	private string AnimName = "MenuRun";

	public Animation panelPrincipal;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void AnimMenuRun(){

		if(menuShow == 0){
			panelPrincipal[AnimName].speed = 1; 
			panelPrincipal[AnimName].time = resetTime;
			panelPrincipal.Play(AnimName);
			menuShow = 1;
		}else if(menuShow == 1){
			panelPrincipal[AnimName].speed = -1; 
			resetTime =panelPrincipal[AnimName].time;
			panelPrincipal[AnimName].time = panelPrincipal[AnimName].length;
			panelPrincipal.Play(AnimName);
			menuShow = 0;
		}
	}

	IEnumerator cargarEscenaMenu(string pageName){
		AnimMenuRun();
		yield return new WaitForSeconds(0.35F);
		Application.LoadLevel(pageName);
		yield break;
	}

	public void cargarEscena(string pageName){
		StartCoroutine(cargarEscenaMenu(pageName));
	}
	
}
