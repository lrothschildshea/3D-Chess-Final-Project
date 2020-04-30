using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class StatsLogic : MonoBehaviour {

	private bool written;
	private bool read;

	public Text totalGamesPlayed;
	public Text lightTeamWins;
	public Text darkTeamWins;
	public Text lightForfeits;
	public Text darkForfeits;
	public Text draws;
	public Text singlePlayerGames;
	public Text twoPlayerGames;
	public Text aiWins;
	public Text aiLosses;


	// Use this for initialization
	void Start () {
		written = false;
		read = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!read){
			if(File.Exists(@"stats.csv")){
				using(var reader = new StreamReader(@"stats.csv")){
					var line = reader.ReadLine();
					var values = line.Split(',');

					totalGamesPlayed.text = values[0];
					lightTeamWins.text = values[1];
					darkTeamWins.text = values[2];
					lightForfeits.text = values[3];
					darkForfeits.text = values[4];
					draws.text = values[5];
					singlePlayerGames.text = values[6];
					twoPlayerGames.text = values[7];
					aiWins.text = values[8];
					aiLosses.text = values[9];
				}
			}
			read = true;
		}

		if(read && !written && GameObject.Find("GameBoard").GetComponent<TestScript>().gameOver){
			//need to make sure the texts have been updated with the correcct values at this point

			String data = "";
			data += totalGamesPlayed.text + ",";
			data += lightTeamWins.text + ",";
			data += darkTeamWins.text + ",";
			data += lightForfeits.text + ",";
			data += darkForfeits.text + ",";
			data += draws.text + ",";
			data += singlePlayerGames.text + ",";
			data += twoPlayerGames.text + ",";
			data += aiWins.text + ",";
			data += aiLosses.text;

			var csv = new System.Text.StringBuilder();
			csv.AppendLine(data);
			System.IO.File.WriteAllText("stats.csv", csv.ToString()); 
			written = true;
		}
	}
}
