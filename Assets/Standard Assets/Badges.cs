using System.Collections;
using System;

public class Badges {

	public int initWeekDay = 1;//lunes

	public Badges () {
		
	}

	public bool checkBadges(int badgeId = 0){

		//badgeId == 1 | verificacion en paseo.cs
		/*if(badgeId == 1){//SUPERSTAR
			return true;
		}

		if(badgeId == 2){//REY DEL PASEO
			return true;
		}

		if(badgeId == 3){//PRIMER CAMINATA
			return true;
		}

		if(badgeId == 4){//PASEO BAJO CERO
			return true;
		}

		if(badgeId == 5){//MADRUGADOR
			return true;
		}

		if(badgeId == 6){//Contra viento y correa
			return true;
		}

		if(badgeId == 7){//EN LLAMAS
			return true;
		}

		DateTime dateValue = DateTime.Now;
		int dayoftheweek = (int) dateValue.DayOfWeek;*/
		
		return true;
	}
}


/*
1	SUPERSTAR	Se entrega a todo aquel que saque a su perro durante un evento televisivo importante tanto del espectáculo como del deporte. Ej: Oscars, Mundial, Superclásico
2	REY DEL PASEO	Se entrega a la persona que al terminar el desafío semanal termine en el PRIMER lugar del ranking.
3	PRIMER CAMINATA	Lo reciben todas las personas después de haber finalizado la primer caminata con la app.
4	PASEO BAJO CERO	Cuando la temperatura es bajo cero y alguien saca a su perro a pasear a pesar de todo se lo premia con este badge.
5	MADRUGADOR	Aprovecha al máximo el día y los paseos con su perro son muy temprano a la mañana
6	Contra viento y correa.	Personas que sacan a pasear a sus perros durante condiciones ventosas
7	EN LLAMAS	Cuando las personas sacan a pasear a sus perros durante temperaturas muy altas.
8	PASEADOR DOMINGUERO	Aquellas personas que aprovechan el domingo no solo para descansar sino también para pasear a su perro.
9	BUHO	Para aquellas personas que aprovechan cuando todos duermen para sacar a pasear a su perro
10	PASEO PASADO POR AGUA	Se le otorga a aquellos que a pesar de la lluvia no resignan el paseo de su perro.
 */