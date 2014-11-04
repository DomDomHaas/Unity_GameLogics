using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

/// <summary>
/// Game session data holds all the Data for every Level in a world.
/// And a <code>LevelData</code> for every try of a level.
/// </summary>
public class GameSessionData
{

		public static readonly string lvlPlayTime = "lvlPlayTime";
		public static readonly string worldPlayTime = "worldPlayTime";
		public static readonly string lvlTry = "lvlTry";
		public static readonly string worldTries = "worldTries";
		public static readonly string lvlQuits = "lvlQuits";

		public bool sessionOpen;
		public DateTime sessionStartTime;
		public DateTime sessionEndTime;

		public LevelData currentLevelData;

		public IDictionary<DateTime, LevelData> levelSessionData;

		public GameSessionData (bool initDirectly = false)
		{
				if (initDirectly) {
						initSession ();
				}
		}


		public class LevelData
		{
				private const string whitePrefix = "whiteMovement";
				private const string blackPrefix = "blackMovement";

				public DateTime	startTime;
				public DateTime endTime;

				public int level;
				public int world;
				public float endWhiteHealth;
				public float endBlackHealth;

				public IDictionary<DateTime, Vector3> whiteMovements;
				public IDictionary<DateTime, Vector3> blackMovements;
				public IDictionary<DateTime, float> healthWhiteToBlack;


				public LevelData (int level = 0, int world = 0)
				{
						initLevelData (level, world);
				}

				public LevelData (JSONClass jClass)
				{
						string strValue = jClass ["startTime"].Value;
						startTime = UnityShortcuts.GetTimestampFromString (strValue);
						strValue = jClass ["endTime"].Value;
						endTime = UnityShortcuts.GetTimestampFromString (strValue);
				
						level = jClass ["level"].AsInt;
						world = jClass ["world"].AsInt;
						endWhiteHealth = jClass ["endWhiteHealth"].AsFloat;
						endBlackHealth = jClass ["endBlackHealth"].AsFloat;
				
						whiteMovements = setJSONMovementData (whitePrefix, jClass);
						blackMovements = setJSONMovementData (blackPrefix, jClass);
						healthWhiteToBlack = setJSONHealthChangeData (jClass ["healthWhiteToBlack"].AsObject);
				}
		

				public void initLevelData (int level, int world)
				{
						this.startTime = DateTime.Now;
						this.level = level;
						this.world = world;
						this.endWhiteHealth = 0f;
						this.endBlackHealth = 0f;

						whiteMovements = new SortedDictionary<DateTime, Vector3> ();
						blackMovements = new SortedDictionary<DateTime, Vector3> ();
						healthWhiteToBlack = new Dictionary<DateTime, float> ();
				}
		
				public LevelData getLevelDataFromJSON (JSONClass jClass)
				{
						return new LevelData (jClass.AsObject);
				}
		
				// borsi greus "über das neue"
				public JSONClass getJSONFromLevelData ()
				{
						JSONClass jClass = new JSONClass ();
			
						jClass ["startTime"] = UnityShortcuts.GetTimestamp (this.startTime);
						jClass ["endTime"] = UnityShortcuts.GetTimestamp (this.endTime);
			
						jClass ["level"].AsInt = this.level;
						jClass ["world"].AsInt = this.world;
			
						jClass ["endWhiteHealth"].AsFloat = this.endWhiteHealth;
						jClass ["endBlackHealth"].AsFloat = this.endBlackHealth;

						jClass [whitePrefix] = getJSONMovementData (this.whiteMovements);
						jClass [blackPrefix] = getJSONMovementData (this.blackMovements);
						jClass ["healthWhiteToBlack"] = getJSONHealthChangeData (this.healthWhiteToBlack);

						return jClass;
				}
		
				public JSONClass getJSONMovementData (IDictionary<DateTime, Vector3> moveDic)
				{
						JSONClass jClass = new JSONClass ();
						//Debug.Log ("getJSONMovementData " + jClass.AsObject.ToString ());

						foreach (KeyValuePair<DateTime, Vector3> kvp in moveDic) {
								string dateAsString = UnityShortcuts.GetTimestamp (kvp.Key).Trim ();
								jClass [dateAsString] = getJSONFromVector3 (kvp.Value);
						}

						return jClass;
				}
		
				public SortedDictionary<DateTime, Vector3> setJSONMovementData (string prefix, JSONClass jClass)
				{
						SortedDictionary<DateTime, Vector3> moveDic = new SortedDictionary<DateTime, Vector3> ();
						//Debug.Log ("setJSONMovementData " + jClass [prefix].AsObject.ToString ());

						foreach (string key in jClass[prefix].Keys) {
								//Debug.Log ("setJSONMovementData key " + key.Trim ());
								DateTime keyDate = UnityShortcuts.GetTimestampFromString (key.Trim ());
								moveDic.Add (keyDate, getVector3FromJSON (jClass [prefix] [key.Trim ()].AsObject));
						}


						return moveDic;
				}

				public JSONClass getJSONHealthChangeData (IDictionary<DateTime, float> healthDic)
				{
						JSONClass jClass = new JSONClass ();

						foreach (KeyValuePair<DateTime, float> kvp in healthDic) {
								string dateAsString = UnityShortcuts.GetTimestamp (kvp.Key).Trim ();
								jClass [dateAsString].AsFloat = kvp.Value;
						}
			
						return jClass;
				}

				public IDictionary<DateTime, float> setJSONHealthChangeData (JSONClass jClass)
				{
						IDictionary<DateTime, float> healthDic = new Dictionary<DateTime, float> ();
						//Debug.Log ("setJSONHealthChangeData " + jClass [prefix].AsObject.ToString ());
			
						foreach (string key in jClass.Keys) {
								//Debug.Log ("setJSONHealthChangeData key " + key.Trim ());
								DateTime keyDate = UnityShortcuts.GetTimestampFromString (key.Trim ());
								healthDic.Add (keyDate, jClass [key.Trim ()].AsFloat);
						}
						
						return healthDic;
				}

				private JSONClass getJSONFromVector3 (Vector3 vec)
				{
						JSONClass jClass = new JSONClass ();
			
						jClass ["x"].AsFloat = vec.x;
						jClass ["y"].AsFloat = vec.y;
						jClass ["z"].AsFloat = vec.z;

						//Debug.Log ("jVec " + jClass.AsObject.ToString ());
						return jClass;
				}
		
				private Vector3 getVector3FromJSON (JSONClass jClass)
				{
						Vector3 vec = new Vector3 ();
			
						vec.x = jClass ["x"].AsFloat;
						vec.y = jClass ["y"].AsFloat;
						vec.z = jClass ["z"].AsFloat;

						//Debug.Log ("vec " + vec);			
						return vec;
				}

		}


		public void setData (JSONClass jClass)
		{
				if (levelSessionData.Count > 0) {
						levelSessionData.Clear ();
				}

				sessionStartTime = UnityShortcuts.GetTimestampFromString (jClass ["sessionStartTime"].Value);
				sessionEndTime = UnityShortcuts.GetTimestampFromString (jClass ["sessionEndTime"].Value);
				sessionOpen = jClass ["sessionOpen"].AsBool;

				//Debug.Log ("jClass ['levelData']: " + jClass ["levelData"].AsObject);

				foreach (string key in jClass ["levelData"].Keys) {
						JSONClass lvlClass = jClass ["levelData"] [key.Trim ()].AsObject;
						//Debug.Log ("key: " + key.Trim ());
						DateTime timeStamp = UnityShortcuts.GetTimestampFromString (key.Trim ());
						//Debug.Log ("timestamp from file: " + timeStamp);
						LevelData lvlData = new LevelData (lvlClass);
						this.levelSessionData.Add (timeStamp, lvlData);
				}

		}

		public JSONClass getJSONData ()
		{
				JSONClass jClass = new JSONClass ();

				jClass ["sessionStartTime"] = UnityShortcuts.GetTimestamp (sessionStartTime);
				jClass ["sessionEndTime"] = UnityShortcuts.GetTimestamp (sessionEndTime);
				jClass ["sessionOpen"].AsBool = sessionOpen;
				//jClass ["sessionCount"].AsInt = sessionCount;

				foreach (KeyValuePair<DateTime, LevelData> kvp in levelSessionData) {
						string dateAsString = UnityShortcuts.GetTimestamp (kvp.Key);
						jClass ["levelData"].Add (dateAsString, kvp.Value.getJSONFromLevelData ());
				}

				return jClass;
		}

		public void initSession ()
		{
				sessionStartTime = DateTime.Now;
				sessionOpen = true;
				//sessionCount = 0;
				currentLevelData = null;
				levelSessionData = new Dictionary<DateTime, LevelData> ();
		}

		public void endSession (float endWhiteHealth = 0, float endBlackHealth = 0)
		{
				if (this.currentLevelData != null) {
						closeCurrentLevelData ();
				}

				sessionEndTime = DateTime.Now;
				sessionOpen = false;
		}

		public void openLevelData (int level, int world)
		{
				if (currentLevelData != null) {
						closeCurrentLevelData ();
				}
				currentLevelData = new LevelData (level, world);
				//this.levelSessionData.Add (currentLevelData.startTime, currentLevelData);
		}

		public void closeCurrentLevelData (float endWhiteHealth = 0, float endBlackHealth = 0)
		{
				//Debug.Log ("close currentLevelData " + currentLevelData);
				currentLevelData.endWhiteHealth = endWhiteHealth; 
				currentLevelData.endBlackHealth = endBlackHealth;
				currentLevelData.endTime = DateTime.Now;

				if (this.levelSessionData.ContainsKey (currentLevelData.startTime)) {
						this.levelSessionData.Remove (currentLevelData.startTime);
				}

				this.levelSessionData.Add (currentLevelData.startTime, currentLevelData);
				this.currentLevelData = null;
		}


		/// <summary>
		/// Loads up the LevelData to the currentLevelData
		/// </summary>
		/// <returns><c>true</c>, if up level was loaded, <c>false</c> otherwise.</returns>
		/// <param name="level">Level.</param>
		/// <param name="world">World.</param>
		public bool loadUpLevel (int level, int world)
		{
				this.currentLevelData = null;

				foreach (KeyValuePair<DateTime, LevelData> kvp in this.levelSessionData) {
						if (kvp.Value.level == level && kvp.Value.world == world) {
								this.currentLevelData = kvp.Value;
								break;
						}
				}

				return this.currentLevelData != null;
		}

		public bool removeCurrentLevelData ()
		{
				if (this.currentLevelData != null) {
						return this.levelSessionData.Remove (currentLevelData.startTime);
				}
				return false;
		}

		public JSONClass getTotal ()
		{
				JSONClass jClass = new JSONClass ();

				//Debug.Log ("levelSessionDatas " + levelSessionData.Count);

				foreach (KeyValuePair<DateTime, LevelData> kvp in this.levelSessionData) {
						string lvlKey = kvp.Value.world + "," + kvp.Value.level;

						jClass ["world"].AsInt = kvp.Value.world;

						//Debug.Log ("check " + lvlKey);

						int currentCount = 0;
						if (string.IsNullOrEmpty (jClass [lvlTry] [lvlKey].Value)) {
								// initialize
								jClass [lvlTry] [lvlKey].AsInt = currentCount;
						}
						currentCount = jClass [lvlTry] [lvlKey].AsInt;
						currentCount++;
						jClass [lvlTry] [lvlKey].AsInt = currentCount;
								

						double currentPlayTime = 0f;
						if (string.IsNullOrEmpty (jClass [lvlPlayTime] [lvlKey].Value)) {
								// initialize
								jClass [lvlPlayTime] [lvlKey].AsDouble = currentPlayTime;
						}

						currentPlayTime = jClass [lvlPlayTime] [lvlKey].AsDouble;
						double diff = UnityShortcuts.getTimestampDiff (kvp.Value.endTime, kvp.Value.startTime);
						currentPlayTime += diff;
						jClass [lvlPlayTime] [lvlKey].AsDouble = currentPlayTime;

				}

				//Debug.Log ("jClass[lvlPlayTime] " + jClass [lvlPlayTime].AsObject.ToString ());

				double worldPlaytime = 0;
				foreach (string key in jClass[lvlPlayTime].Keys) {
						worldPlaytime += jClass [lvlPlayTime] [key].AsDouble;
				}

				jClass [worldPlayTime].AsDouble = worldPlaytime;

				//Debug.Log ("jClass[lvlTry] " + jClass [lvlTry].AsObject.ToString ());

				int worldtries = 0;
				foreach (string key in jClass[lvlTry].Keys) {
						worldtries += jClass [lvlTry] [key].AsInt;
				}

				jClass [worldTries].AsInt = worldtries;

				return jClass;
		}

}