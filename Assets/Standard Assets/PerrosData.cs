using System.Collections;
using System;
using UnityEngine;

[Serializable]
public class PerroData {
	public int id;
	
	public string nombre,
	edad,
	peso,
	foto, razas_id,
	parentescos_id, puntos, kilometros, puntos_totales, chat_group;

	public byte[] ImgBytes;

	public string temp_img;
	//public Dictionary<string, int> ExercisesMetricas;
	
	public PerroData(){
		puntos = kilometros = puntos_totales = "0";
		foto = temp_img = chat_group = "";
		//ExercisesMetricas = new Dictionary<string, int> ();
	}

	public string getParentesco(int parentesco_id_){
		string parentesco = "";
		switch(parentesco_id_){
		case 1: parentesco = "Dueño"; break;
		}
		return parentesco;
	}

	public void populatePerro(string[] row_){
		id = int.Parse( row_ [0] );
		nombre = row_ [1];
		edad = row_ [2];
		peso = row_ [3];
		foto = row_ [4];
		razas_id = row_ [5];
		parentescos_id = row_ [6];
		puntos = row_ [7];
		kilometros = row_ [8];
		puntos_totales = row_ [9];
		chat_group = row_ [10];
	}

	public string queryPerrosUsuario(int usuarios_id){
		string query = 
			"select perros.id, perros.nombre, perros.edad, perros.peso, perros.foto, perros.razas_id, " +
				"perros_usuarios.parentescos_id, perros_usuarios.puntos, perros_usuarios.kilometros, perros_usuarios.puntos_totales, perros_usuarios.chat_group " +
				"from perros inner join perros_usuarios on perros_usuarios.perros_id = perros.id where perros_usuarios.aceptado = '1' and perros_usuarios.usuarios_id = " + usuarios_id.ToString();

		return query;
	}

	public string queryPerro(string perros_id, int usuarios_id){
		string query = 
			"select perros.id, perros.nombre, perros.edad, perros.peso, perros.foto, perros.razas_id, " +
				"perros_usuarios.parentescos_id, perros_usuarios.puntos, perros_usuarios.kilometros, perros_usuarios.puntos_totales, perros_usuarios.chat_group " +
				"from perros inner join perros_usuarios on perros_usuarios.perros_id = perros.id where perros_usuarios.perros_id = " + perros_id + " " +
				"and perros_usuarios.usuarios_id = " + usuarios_id.ToString() + " and perros_usuarios.aceptado = '1' ";
		Debug.Log (query);
		return query;
	}

	public string queryPerrosFamilia(string perros_id){
		string query = 
			"select SUM(perros_usuarios.kilometros) as sum_km, SUM(perros_usuarios.puntos) as sum_puntos " +
				"from perros inner join perros_usuarios on perros_usuarios.perros_id = perros.id " +
				"where perros.id = "+ perros_id +" group by perros.id ";

		return query;
	}
}