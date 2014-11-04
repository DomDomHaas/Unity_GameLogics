using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class AnalyzerDataPersitent : JSONPersistent
{

		public Dictionary<string, DateTime> lvlPlayTime = null;
		public Dictionary<string, int> lvlTrys = null;
		public Dictionary<string, int> lvlQuits = null;

		new void Awake ()
		{
				init ();
		}

		new public void init ()
		{
				lvlPlayTime = new Dictionary<string, DateTime> ();		
				lvlTrys = new Dictionary<string, int> ();
				lvlQuits = new Dictionary<string, int> ();
		}


		public override JSONClass getDataClass ()
		{
				JSONClass jClass = new JSONClass ();
		
				foreach (KeyValuePair<string, DateTime> kvp in lvlPlayTime) {
						string dateAsString = UnityShortcuts.GetTimestamp (kvp.Value);
						jClass ["lvlPlayTime"] [kvp.Key] = dateAsString;
				}
		
				foreach (KeyValuePair<string, int> kvp in lvlTrys) {
						jClass ["lvlTrys"] [kvp.Key].AsInt = kvp.Value;
				}
		
				foreach (KeyValuePair<string, int> kvp in lvlQuits) {
						jClass ["lvlQuits"] [kvp.Key].AsInt = kvp.Value;
				}
		
		
				return jClass;
		}
	
		public override void setClassData (JSONClass jClass)
		{
				foreach (string key in jClass ["lvlPlayTime"].Keys) {
						DateTime date = UnityShortcuts.GetTimestampFromString (jClass ["lvlPlayTime"] [key].Value);
						lvlPlayTime.Add (key, date);
				}
		
				foreach (string key in jClass ["lvlTrys"].Keys) {
						lvlTrys.Add (key, jClass ["lvlTrys"] [key].AsInt);
				}
		
				foreach (string key in jClass ["lvlQuits"].Keys) {
						lvlQuits.Add (key, jClass ["lvlQuits"] [key].AsInt);
				}

		}
	
}