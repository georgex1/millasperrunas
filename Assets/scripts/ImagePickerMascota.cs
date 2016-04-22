using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using VoxelBusters.NativePlugins;
using VoxelBusters.AssetStoreProductUtility.Demo;

public class ImagePickerMascota : MonoBehaviour {
	public Texture2D image;
	public GameObject imageTexture;
	public GameObject iconoCamara;

	private MainController GMS;
	
	internal void Start() {

		GameObject GM = GameObject.Find ("MainController");
		GMS = GM.GetComponent<MainController>();

	}
	
	public void getImage(){

#if UNITY_EDITOR
		test_guardar();
#else
		NPBinding.MediaLibrary.PickImage(eImageSource.BOTH, 1.0f, PickImageFinished);
#endif
	}

	private void test_guardar(){

		byte[] fileData = File.ReadAllBytes("Assets/Resources/Huskey-Siberiano.jpg");
		Texture2D  tex = new Texture2D(2, 2);
		tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
		imageTexture.renderer.material.mainTexture = tex;
		
		GMS.perro.temp_img = GMS.generateId ().ToString () + ".png";
		
		StartCoroutine (GMS.saveTextureToFile (imageTexture.renderer.material.mainTexture as Texture2D, GMS.perro.temp_img, 'p', true));
		StartCoroutine (loadImage ());

	}

	private void PickImageFinished (ePickImageFinishReason _reason, Texture2D _image)
	{
		string reasonString = _reason + "";
		if (reasonString == "SELECTED") {
			GMS.showLoading (true);
			image = _image;
			imageTexture.renderer.material.mainTexture = image;
		
			GMS.perro.temp_img = GMS.generateId ().ToString () + ".png";

			bool rotateLeft = false;
			
			if (Input.deviceOrientation == DeviceOrientation.Portrait){
				rotateLeft = true;
			}
		
			StartCoroutine (GMS.saveTextureToFile (imageTexture.renderer.material.mainTexture as Texture2D, GMS.perro.temp_img, 'p', rotateLeft));
			StartCoroutine (loadImage ());
		}
	}

	IEnumerator loadImage(){
		yield return new WaitForSeconds(2);
		Sprite sprite = GMS.spriteFromFile (GMS.perro.temp_img);
		GameObject.Find ("backImage").GetComponent<Image>().sprite = sprite;
		GMS.showLoading (false);
		iconoCamara.SetActive (false);
	}


}