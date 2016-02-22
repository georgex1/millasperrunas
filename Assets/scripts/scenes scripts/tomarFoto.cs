using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class tomarFoto : MonoBehaviour {

	private MainController GMS;

	public Texture2D image;
	public GameObject imageTexture;
	public WebCamTexture mCamera = null;
	WebCamTexture webCamTexture;

	// Use this for initialization
	void Start () {
		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

		mCamera = new WebCamTexture (768,1200,40);
		//mCamera = new WebCamTexture (1920,1080,40);
		imageTexture.renderer.material.mainTexture = mCamera;
		mCamera.Play ();
	}

	public void loadImage(){
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

	/*IEnumerator loadImage(){
		yield return new WaitForSeconds(1);
		Sprite sprite = GMS.spriteFromFile (GMS.perro.temp_img);
		GameObject.Find ("backImage").GetComponent<Image>().sprite = sprite;
	}*/
	
	// Update is called once per frame
	void Update () {
	
	}
}
