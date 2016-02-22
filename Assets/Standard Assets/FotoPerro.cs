using System.Collections;
using System;
using UnityEngine;

[Serializable]
public class FotoPerro{
	public int id;

	public string fecha_entrada, 
	usuarios_id,
	perros_id;
	
	public byte[] ImgBytes;
	
	public string temp_img;

	// Use this for initialization
	public FotoPerro () {
		usuarios_id = perros_id = "0";
		fecha_entrada = temp_img = "";
	}

	public void pupulateFotoPerro(string[] row_){
		id = int.Parse( row_ [0] );
		fecha_entrada = row_ [1];
		usuarios_id = row_ [2];
		perros_id = row_ [3];
		temp_img = row_ [4];
	}

}
