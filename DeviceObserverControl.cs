using UnityEngine;
using System.Collections;
using InControl;

/**
 * The copyright law of switzerland is applied and owned by the authors, the music files which are made by Chris Zabriskie.
 * 
 *  Authors: Dominik Haas (shaodiese@gmail.com)
 * 					Christian Schmidhalter (chs191991@gmail.com)
 * 					Don Schmocker (don.schmocker@bluewin.ch)
 * 
 **/

public class DeviceObserverControl : MonoBehaviour
{
		[Range(0, 4)]
		public int
				screenShotQuality = 4;
		public string prefixControllers;
		public string prefixActivePlayers;

		public int controllersConnected = 0;
		public bool useKeyboard = false;

		public bool useDeadZone = false;
		[Range(0.01f, 0.3f)]
		public float
				deadzone = 0.25f;

		private InputDevice player1Device;

		private float menuCD = 0.25f;
		private float menuCount = 0;

		// exhibition counter
		public bool useExhibitionRestart = false;
		private float countTime = 0;
		private float exhibitionRestartTime = 90f;

		void Awake ()
		{

				GameObject[] otherDeviceObs = GameObject.FindGameObjectsWithTag ("DeviceObserver");
				if (otherDeviceObs.Length > 1) {
						Debug.Log ("killing " + this.name);
						// prevent having a double when coming back to the startMenu
						Destroy (this.gameObject);
				}

				InputManager.Setup ();

				InputManager.OnDeviceAttached += inputDevice => this.controllersConnected++;
				InputManager.OnDeviceDetached += inputDevice => this.controllersConnected--;

				this.controllersConnected = InputManager.Devices.Count;		


				foreach (InputDevice dev in InputManager.Devices) {
						Debug.Log ("found: " + dev + " meta: " + dev.Meta);
				}

				if (InputManager.Devices.Count < 2) {
						InputManager.AttachDevice (new UnityInputDevice (new BasicKeyboardProfile ()));
						Debug.Log ("not enough Pads! USE KEYBOARD: arrow keys and wasd + shiftLeft " + InputManager.Devices [0]);
						this.useKeyboard = true;
				}


				this.player1Device = getPlayerOneDevice ();
				Debug.Log ("set playerOne: " + this.player1Device + " meta: " + this.player1Device.Meta);

				// exhibition fix
				if (useExhibitionRestart) {
						StartCoroutine (exhibitionCountDown ());
				}
		
				DontDestroyOnLoad (this);
		}
	
		// Use this for initialization
		void Start ()
		{
		}
	
		// Update is called once per frame
		void Update ()
		{
				InputManager.Update ();
	

				if (this.menuCount >= this.menuCD) {
						// cooldown finished
						this.menuCount = 0;
				} else  if (this.menuCount > 0) {
						// input is on cooldown!
						this.menuCount += Time.deltaTime;
				}

				checkCaptureScreen ();
		}
	
		private void checkCaptureScreen ()
		{
				if (Input.GetKeyUp (KeyCode.P)) {
						UnityShortcuts.captureScreen (screenShotQuality);
				}
		}

		public float getMenuP1YAxis ()
		{
				float yAxis = 0;
				if (this.useKeyboard) {
						yAxis = getMenuKeybPlayersYAxis (1);
				} else {			
						yAxis = getMenuPlayersYAxis (this.player1Device);
				}

				return yAxis;
		}

		public float getMenuP1XAxis ()
		{
				float xAxis = 0;
				if (this.useKeyboard) {
						xAxis = getMenuKeybPlayersXAxis (1);
				} else {
						xAxis = getMenuPlayersXAxis (this.player1Device);
				}
				return xAxis;
		}

		public float getIngameXAxis (int playerNr, InputDevice device = null)
		{
				float xAxis = 0;

				if (device != null) {
						if (useDeadZone) {
								Vector2 deadzoneVector = applyDeadZone (device.LeftStick.Vector);
								xAxis = deadzoneVector.x;
						} else {
								xAxis = device.LeftStickX.Value;
						}
				}

				if (this.useKeyboard) {

						if (playerNr == 1) {

								if (Input.GetKey (KeyCode.LeftArrow)) {
										xAxis = -1;
								} else if (Input.GetKey (KeyCode.RightArrow)) {
										xAxis = 1;
								}
				
						} else if (playerNr == 2) {
				
								if (Input.GetKey (KeyCode.A)) {
										xAxis = -1;
								} else if (Input.GetKey (KeyCode.D)) {
										xAxis = 1;
								}
						}
				}

				if (xAxis != 0 && useExhibitionRestart) {
						this.countTime = 0;
				}

				return xAxis;
		}

		public float getIngameYAxis (int playerNr, InputDevice device = null)
		{
				float yAxis = 0;
				if (device != null) {		
						if (useDeadZone) {
								Vector2 deadzoneVector = applyDeadZone (device.LeftStick.Vector);
								yAxis = deadzoneVector.y;
						} else {
								yAxis = device.LeftStickY.Value;
						}
				}

				if (this.useKeyboard) {
						if (playerNr == 1) {
				
								if (Input.GetKey (KeyCode.UpArrow)) {
										yAxis = 1;
								} else if (Input.GetKey (KeyCode.DownArrow)) {
										yAxis = -1;
								}
				
						} else if (playerNr == 2) {
				
								if (Input.GetKey (KeyCode.W)) {
										yAxis = 1;
								} else if (Input.GetKey (KeyCode.S)) {
										yAxis = -1;
								}
						}
				}

				if (yAxis != 0 && useExhibitionRestart) {
						this.countTime = 0;
				}

				return yAxis;
		}

		public bool getMenuButtonPressedAny ()
		{
				bool menuPressed = false;

				foreach (InputDevice device in InputManager.Devices) {
						if (getMenuButtonPressed (device)) {
								menuPressed = true;
						}
				}

				if (!menuPressed) {
						// check for keyboard
						menuPressed = getMenuButtonPressed ();
				}

				return menuPressed;
		}

		public bool getMenuButtonPressed (InputDevice device = null)
		{

				if (device != null) {
						return device.MenuWasPressed;
				}

				if (this.useKeyboard) {
						if (Input.GetKey (KeyCode.Escape)) {
								return true;
						}
				}
			
				return false;
		}
		


		public bool getIngameAction1 (int playerNr, InputDevice device = null)
		{

				if (device != null) {	
						return checkDeviceAction1 (device);
				}

				if (this.useKeyboard) {
						if (playerNr == 1) {
								if (Input.GetKey (KeyCode.RightControl)) {
										return true;
								}			

						}
						if (playerNr == 2) {
								if (Input.GetKey (KeyCode.LeftControl)) {
										return true;
								}			

						}
				}

				return false;
		}

		public bool getIngameAction2 (int playerNr, InputDevice device = null)
		{
				if (device != null) {	
						return checkDeviceAction2 (device);
				}

				if (this.useKeyboard) {
						if (playerNr == 1) {
								if (Input.GetKey (KeyCode.RightShift)) {
										return true;
								}			
						}
						if (playerNr == 2) {
								if (Input.GetKey (KeyCode.LeftShift)) {
										return true;
								}			
						}

				}

				return false;
		}


		public bool getMenuP1Action1 ()
		{

				if (this.useKeyboard) {
						if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.RightShift) || Input.GetKey (KeyCode.Space) || Input.GetKey (KeyCode.Return)) {
								return true;
						}			
				}

				if (this.player1Device != null) {	
						return checkDeviceAction1 (this.player1Device);
				}

				return false;
		}
	
		public bool checkMenuClick ()
		{
				if (this.menuCount == 0) {
						// no keyBoard checking, enable both!

						if (Input.GetKeyUp (KeyCode.Return)) {
								this.menuCount += Time.deltaTime;
								return true;
						}

						if (Input.GetKeyUp (KeyCode.Space)) {
								this.menuCount += Time.deltaTime;
								return true;
						}

						if (checkDeviceAction1 (this.player1Device)) {
								this.menuCount += Time.deltaTime;
								return true;
						}
				}

				return false;
		}


		public float getMenuKeybPlayersXAxis (int playerNr)
		{
				//print ("check player " + playerNr);

				if (this.menuCount == 0) {
						if (playerNr == 1) {

								if (Input.GetKeyDown (KeyCode.LeftArrow)) {
										this.menuCount += Time.deltaTime;
										return -1;
								} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
										this.menuCount += Time.deltaTime;
										return 1;
								}

						} else if (playerNr == 2) {

								if (Input.GetKeyDown (KeyCode.A)) {
										this.menuCount += Time.deltaTime;
										return -1;
								} else if (Input.GetKeyDown (KeyCode.D)) {
										this.menuCount += Time.deltaTime;
										return 1;
								}
						}
				}

				return 0;
		}

		public float getMenuKeybPlayersYAxis (int playerNr)
		{
				if (this.menuCount == 0) {
						if (playerNr == 1) {
			
								if (Input.GetKeyDown (KeyCode.UpArrow)) {
										this.menuCount += Time.deltaTime;
										return -1;
								} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
										this.menuCount += Time.deltaTime;
										return 1;
								}
			
						} else if (playerNr == 2) {
			
								if (Input.GetKeyDown (KeyCode.W)) {
										this.menuCount += Time.deltaTime;
										return -1;
								} else if (Input.GetKeyDown (KeyCode.S)) {
										this.menuCount += Time.deltaTime;
										return 1;
								}
						}
				}
		
				return 0;
		}

		public float getMenuPlayersXAxis (InputDevice playersDevice)
		{
				if (this.menuCount == 0) {
						if (playersDevice.LeftStickX.WasReleased) {
				
								if (playersDevice.LeftStickX.LastValue > 0.0f) {
										this.menuCount += Time.deltaTime;
										return 1;
								} else if (playersDevice.LeftStickX.LastValue < 0.0f) {
										this.menuCount += Time.deltaTime;
										return -1;
								}			
						}
				}
		
				return 0;
		}

		public float getMenuPlayersYAxis (InputDevice playersDevice)
		{
				if (this.menuCount == 0) {
						if (playersDevice.LeftStickY.WasReleased) {

								//print ("playersDevice.LeftStickY.LastValue " + playersDevice.LeftStickY.LastValue);

								// flip axis for buttons!
								if (playersDevice.LeftStickY.LastValue > 0.0f) {
										this.menuCount += Time.deltaTime;
										return -1;
								} else if (playersDevice.LeftStickY.LastValue < 0.0f) {
										this.menuCount += Time.deltaTime;
										return 1;
								}			
						}
				}
		
				return 0;
		}

		private bool checkDeviceAction1 (InputDevice device)
		{

				if (device.Action1.HasChanged && device.Action1.Value == 1) {
						return true;
				}

				return false;
		}

		public float checkDeviceTriggerRight (int playerNr, InputDevice device)
		{
				//Debug.Log (playerNr + " device: " + device);

				if (device == null) {
						if (getIngameAction2 (playerNr, device)) {
								return 1;
						} else {
								return 0;
						}

				} else {
						return device.RightTrigger.Value;
				}
		}

		private bool checkDeviceAction2 (InputDevice device)
		{
				if (device.Action2.HasChanged && device.Action2.Value == 1) {
						return true;
				}
		
				return false;
		}


		public static Material loadPlayerMats (int playerNr, int teamNr, bool trailMat = false)
		{
				if (trailMat) {
						return (Material)Resources.Load ("mats/team" + teamNr + "_player" + playerNr, typeof(Material));
				} else {
						return (Material)Resources.Load ("mats/team" + teamNr + "_player" + playerNr + "Trail", typeof(Material));
				}
		}

		public InputDevice getPlayerOneDevice ()
		{
/*				if (InputManager.Devices.Count > 0 && !useKeyboard) {
						player1Device = InputManager.Devices [0];
				}
*/
				// always use the device it's either the first gamepade, or if none is connected the keyboard!
				return InputManager.Devices [0];
		}


		private Vector2 applyDeadZone (Vector2 stickVector)
		{
				if (stickVector.magnitude < deadzone)
						stickVector = Vector2.zero;
				else
						stickVector = stickVector.normalized * ((stickVector.magnitude - deadzone) / (1 - deadzone));

				return stickVector;
		}

		private IEnumerator exhibitionCountDown ()
		{
		
				while (countTime < this.exhibitionRestartTime) {
			
						if (Application.loadedLevel > 0) {
								countTime += Time.deltaTime;
						} else {
								countTime = 0;
						}

						yield return new WaitForEndOfFrame ();
				}
		
				//restart to menu
				Application.LoadLevel (0);
		
				yield return null;
		}

}
