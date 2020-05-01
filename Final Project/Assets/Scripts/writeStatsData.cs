using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class writeStatsData : MonoBehaviour {

	private bool written;
	private bool read;
	private int[] dataArr;
	private TestScript mainScript;

	// Use this for initialization
	void Start () {
		written = false;
		read = false;
		dataArr = new int[10];
		for(int i = 0; i < dataArr.Length; i++){
			dataArr[i] = 0;
		}
		mainScript = GameObject.Find("GameBoard").GetComponent<TestScript>();
	}
	
	// Update is called once per frame
	void Update () {

		if(!read){
			if(File.Exists(@"stats.csv")){
				using(var reader = new StreamReader(@"stats.csv")){
					var line = reader.ReadLine();
					var dataString = line.Split(',');
					for(int i = 0; i < dataString.Length; i++){
						dataArr[i] = Int32.Parse(dataString[i]);
					}

				}
			}
			read = true;
		}

		if(read && !written && GameObject.Find("GameBoard").GetComponent<TestScript>().gameOver){

			//total games
			dataArr[0] += 1;

			//lightTeamWins
			if(mainScript.lightWon && !mainScript.draw){
				dataArr[1] += 1;
			}

			//DarkTeamWins
			if(!mainScript.lightWon && !mainScript.draw){
				dataArr[2] += 1;
			}

			//LightTeamForfeits
			if(mainScript.forfeit && mainScript.lightsTurn){
				dataArr[3] += 1;
			}

			//DarkTeamForfeits
			if(mainScript.forfeit && !mainScript.lightsTurn){
				dataArr[4] += 1;
			}

			//Draws
			if(mainScript.draw){
				dataArr[5] += 1;
			}

			//SinglePlayerGames
			if(mainScript.singlePlayer){
				dataArr[6] += 1;
			}

			//TwoPlayerGames
			if(!mainScript.singlePlayer){
				dataArr[7] += 1;
			}

			//AIWins
			if(mainScript.singlePlayer && !mainScript.lightWon){
				dataArr[8] += 1;
			}

			//AILosses
			if(mainScript.singlePlayer && mainScript.lightWon){
				dataArr[9] += 1;
			}

			String data = dataArr[0] + "";
			for(int i = 1; i < dataArr.Length; i++){
				data += "," + dataArr[i];
			}

			var csv = new System.Text.StringBuilder();
			csv.AppendLine(data);
			System.IO.File.WriteAllText("stats.csv", csv.ToString()); 
			written = true;
		}
	}
}
