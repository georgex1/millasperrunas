using System.Collections;
using System;
using UnityEngine;

[Serializable]
public class UserData {

	public int id;
	
	public string nombre,
	email,
	password,
	fecha_nacimiento,
	reg_id,
	plataforma,
	ciudad,
	fbid,
	sexo,
	foto;

	public string date_month;
	public string date_day;
	public string date_year;

	public byte[] ImgBytes;
	public string temp_img;

	public string token;
	
	//public Dictionary<string, int> ExercisesMetricas;
	
	public UserData(){
		id = 0;
		password = "";
		fecha_nacimiento = "";
		reg_id = "";
		plataforma = "";
		ciudad = "";
		fbid = "";
		sexo = "";
		foto = temp_img = "";
		token = "";
		//ExercisesMetricas = new Dictionary<string, int> ();
	}

	public void save(){

	}

	public void populateUser(string[] row_){
		id = int.Parse( row_ [0] );
		email = row_ [1];
		nombre = row_ [2];
		fbid = row_ [3];
		fecha_nacimiento = row_ [4];
		sexo = row_ [5];
	}

	public void populateUser2(string[] row_){
		id = int.Parse( row_ [0] );
		email = row_ [1];
		nombre = row_ [2];
		fbid = row_ [3];
		fecha_nacimiento = row_ [4];
		sexo = row_ [5];
		foto = row_ [6];
		token = row_ [7];
	}

	public void format_month(string month_){
		int monthInt_ = 0;
		switch (month_) {
		case "ENERO": monthInt_ = 01; break;
		case "FEBRERO": monthInt_ = 02; break;
		case "MARZO": monthInt_ = 03; break;
		case "ABRIL": monthInt_ = 04; break;
		case "MAYO": monthInt_ = 05; break;
		case "JUNIO": monthInt_ = 06; break;
		case "JULIO": monthInt_ = 07; break;
		case "AGOSTO": monthInt_ = 08; break;
		case "SEPTIEMBRE": monthInt_ = 09; break;
		case "OCTUBRE": monthInt_ = 10; break;
		case "NOVIEMBRE": monthInt_ = 11; break;
		case "DICIEMBRE": monthInt_ = 12; break;
		}
		date_month = monthInt_.ToString();
	}

	public string unformat_month(string month_){
		string monthString_ = "ENERO";
		switch (month_) {
		case "1": monthString_ = "ENERO"; break;
		case "2": monthString_ = "FEBRERO"; break;
		case "3": monthString_ = "MARZO"; break;
		case "4": monthString_ = "ABRIL"; break;
		case "5": monthString_ = "MAYO"; break;
		case "6": monthString_ = "JUNIO"; break;
		case "7": monthString_ = "JULIO"; break;
		case "8": monthString_ = "AGOSTO"; break;
		case "9": monthString_ = "SEPTIEMBRE"; break;
		case "10": monthString_ = "OCTUBRE"; break;
		case "11": monthString_ = "NOVIEMBRE"; break;
		case "12": monthString_ = "DICIEMBRE"; break;
		}
		return monthString_;
	}

	public string queryBadgesUsuario(int usuarios_id, int perros_id){
		string query = 
			"select badges.id, badges.nombre, badges.foto, badges.descripcion " +
				"from badges inner join badges_usuarios on badges_usuarios.badges_id = badges.id where badges_usuarios.usuarios_id = '" + usuarios_id.ToString() + "' " +
				"and badges_usuarios.perros_id = '" + perros_id.ToString() + "'";
		
		return query;
	}

	public string queryFamiliaRanking(string perroId){
		string query = 
			"select familia.id, familia.email, familia.nombre, familia.foto, perros_usuarios.puntos from familia inner join perros_usuarios " +
				"on perros_usuarios.usuarios_id = familia.id where perros_usuarios.perros_id = '"+perroId+"' and perros_usuarios.aceptado = '1' order by puntos DESC";
		//Debug.Log (query);
		return query;
	}

	public string formatDate(string sqlDate){
		//yyyy-MM-dd HH:mm:ss
		if (sqlDate != "") {
			string[] fdate = sqlDate.Split (' ');
			string[] fdate2 = fdate [0].Split ('-');

			return fdate2 [2] + "/" + fdate2 [1] + "/" + fdate2 [0] + " " + fdate [1];
		} else {
			return " ";
		}
	}

	public string formatDate2(string sqlDate){
		//yyyy-MM-dd HH:mm:ss
		if (sqlDate != "") {
			string[] fdate = sqlDate.Split (' ');
			string[] fdate2 = fdate [0].Split ('-');
			
			return fdate2 [2] + "/" + fdate2 [1] + "/" + fdate2 [0] ;
		} else {
			return " ";
		}
	}

}
