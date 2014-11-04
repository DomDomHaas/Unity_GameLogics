using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class AnalyzerData
{

		public int world = 0;
		public double worldPlaytime = 0;
		public int worldTries = 0;
		public Dictionary<string, double> lvlPlayTime = null;
		public Dictionary<string, int> lvlTrys = null;
		public Dictionary<string, int> lvlQuits = null;

		public AnalyzerData ()
		{
				init ();
		}

		private void init ()
		{
				world = 0;
				worldPlaytime = 0;
				worldTries = 0;
				lvlPlayTime = new Dictionary<string, double> ();		
				lvlTrys = new Dictionary<string, int> ();
				lvlQuits = new Dictionary<string, int> ();
		}


		public JSONClass getJSONData ()
		{
				JSONClass jClass = new JSONClass ();
				jClass ["world"].AsInt = world;

				foreach (KeyValuePair<string, double> kvp in lvlPlayTime) {
						jClass [GameSessionData.lvlPlayTime] [kvp.Key].AsDouble = kvp.Value;
				}
		
				foreach (KeyValuePair<string, int> kvp in lvlTrys) {
						jClass [GameSessionData.lvlTry] [kvp.Key].AsInt = kvp.Value;
				}
		
				foreach (KeyValuePair<string, int> kvp in lvlQuits) {
						jClass [GameSessionData.lvlQuits] [kvp.Key].AsInt = kvp.Value;
				}

				jClass [GameSessionData.worldPlayTime] = UnityShortcuts.GetTimeOnly (worldPlaytime);
				jClass [GameSessionData.worldTries].AsInt = worldTries;

				return jClass;
		}
	
		public void setData (JSONClass jClass)
		{

				world = jClass ["world"].AsInt;

				foreach (string key in jClass [GameSessionData.lvlPlayTime].Keys) {						
						lvlPlayTime.Add (key, jClass ["lvlPlayTime"] [key].AsDouble);
				}
		
				foreach (string key in jClass [GameSessionData.lvlTry].Keys) {
						lvlTrys.Add (key, jClass [GameSessionData.lvlTry] [key].AsInt);
				}
		
				foreach (string key in jClass [GameSessionData.lvlQuits].Keys) {
						lvlQuits.Add (key, jClass [GameSessionData.lvlQuits] [key].AsInt);
				}

				worldPlaytime = jClass [GameSessionData.worldPlayTime].AsDouble;
				worldTries = jClass [GameSessionData.worldTries].AsInt;
		}
	
}