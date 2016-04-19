using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class tomarFoto : MonoBehaviour {

	private MainController GMS;

	public Texture2D image;
	public WebCamTexture mCamera = null;

	public int width = 640;
	public int height = 480;

	private int actCamera = 1;
	private float actX = 0f;

	// Use this for initialization
	void Start () {
		//#if !UNITY_EDITOR
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();
//#endif
		changeCamera ();

	}

	public void changeCamera(){
		if (mCamera != null) {
			mCamera.Stop ();
		}

		actCamera = (actCamera == 1) ? 0 : 1;

		mCamera = new WebCamTexture (WebCamTexture.devices [actCamera].name, width, height);

		if (actX == 0f) {
			actX = gameObject.transform.localScale.x;
		}
		float newX = actX;
		if (actCamera == 1) {
			newX = newX*-1f;
		}
		gameObject.transform.localScale = new Vector3 (newX, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
		
		renderer.material.mainTexture = mCamera;
		mCamera.Play ();
	}

	void OnDestroy(){
		mCamera.Stop ();
		//Application.LoadLevel ("paseo");
	}

	public void loadImage(){
		GMS.showLoading (true);
		Debug.Log ("entro a loadImage");
		GMS.foto_perro.temp_img = GMS.generateId ().ToString() + ".png";
		Debug.Log ("pongo id a la foto del paseo");

		int oldW = mCamera.width;
		int oldH = mCamera.height;
		
		int newW = oldH;
		int newH = oldW;
		
		Texture2D photo = new Texture2D (newW, newH);
		for (var y = 0; y < oldH; y++){
			for (var x = 0; x < oldW; x++){
				int newX = y;
				int newY = newH - x - 1;
				photo.SetPixel (newX, newY , mCamera.GetPixel(x, y));
			}
		}
		photo.Apply ();
		
		mCamera.Stop ();

		StartCoroutine ( GMS.saveTextureToFile (photo, GMS.foto_perro.temp_img, 'f') );
	}

	void Update(){

	}

	/*IEnumerator loadImage(){
		yield return new WaitForSeconds(1);
		Sprite sprite = GMS.spriteFromFile (GMS.perro.temp_img);
		GameObject.Find ("backImage").GetComponent<Image>().sprite = sprite;
	}*/

}
