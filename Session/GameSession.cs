using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameSession : JSONPersistent
{
		public static readonly string filePrefix = "gamesession";
		public static GameSession instance = null;

		public bool recordingReplay = false;
		private int ticks = 0;

		public bool trackPlayers = false;
		public bool loaded = false;
		public GameSessionData myData;
		public int worldNr = 0;

		public GameObject player1 = null;
		public GameObject player2 = null;

		new void Awake ()
		{
				instance = this;
				DontDestroyOnLoad (this.gameObject.transform.root);
		}

		private void loadPlayers ()
		{
				if (player1 == null) {
						player1 = GameObject.FindWithTag ("WhiteSwarm");
				}

				if (player2 == null) {
						player2 = GameObject.FindWithTag ("BlackSwarm");
				}
		}

		public void StartGameSession (int worldNr, bool initFromFile = false)
		{
				this.worldNr = worldNr;
				this.fileName = getFileName ();

				myData = new GameSessionData (initFromFile);


				if (initFromFile) {
						load ();
				} else {
						this.myData.initSession ();
				}

		}

		void Start ()
		{
				if (player1 == null || player2 == null) {
						loadPlayers ();
				}

		}
		void Update ()
		{
	
		}

		void FixedUpdate ()
		{

				if (trackPlayers && this.myData.currentLevelData != null) {
			
						if (!recordingReplay
								&& player1 != null && player2 != null) {
								StartCoroutine (recordMovement ());
						}
			
				}

		}

		private IEnumerator recordMovement ()
		{
				recordingReplay = true;
				float lastHealth = player1.GetComponent<Swarm> ().health;

				while (!LevelControl.instance.showSuccessOutro) {
						DateTime timestamp = DateTime.Now;
						//Debug.Log ("save tick: " + timestamp);
		
						if (!this.myData.currentLevelData.whiteMovements.ContainsKey (timestamp)) {
								this.myData.currentLevelData.whiteMovements.Add (timestamp, player1.transform.position);
						}

						if (lastHealth != player1.GetComponent<Swarm> ().health) {
								float diff = player1.GetComponent<Swarm> ().health - lastHealth;
								this.myData.currentLevelData.healthWhiteToBlack.Add (timestamp, diff);
						}
		
						if (!this.myData.currentLevelData.blackMovements.ContainsKey (timestamp)) {
								this.myData.currentLevelData.blackMovements.Add (timestamp, player2.transform.position);
						}

						lastHealth = player1.GetComponent<Swarm> ().health;
						yield return new WaitForSeconds (LevelControl.replayTick);
				}

				recordingReplay = false;
		}
		

		public void CloseGameSession ()
		{
				this.myData.endSession ();
				save ();
		}


/*		public void OpenGameSession (int sessionCounter)
		{
				if (!this.myData.sessionOpen) {
						this.myData.initSession ();
				}
		}
*/


		public void loadSpecificSession (int worldNr)
		{
				this.worldNr = worldNr;
				fileName = getFileName ();
				load ();
		}


		void OnLevelWasLoaded (int level)
		{
				loadPlayers ();
		}


	#region persistency

		protected override string getFileName ()
		{				
				DateTime time = DateTime.Now;
				return filePrefix + "_" + this.worldNr + "_" + UnityShortcuts.GetTimestamp (time);
		}

		public override SimpleJSON.JSONClass getDataClass ()
		{
				return this.myData.getJSONData ();
		}

		public override void setClassData (SimpleJSON.JSONClass jClass)
		{
				this.myData.setData (jClass);
		}

		public override void save ()
		{
				base.save ();
		}

		public override void load ()
		{
				base.load ();
				loaded = true;
		}

	#endregion
}

