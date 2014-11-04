using UnityEngine;
using System.Collections;
using InControl;
using System;


public class SmashButtonEvent : MonoBehaviour
{
		public delegate void EventHandler (object smashTrigger,EventArgs e);

		public event EventHandler start;
		public event EventHandler success;
		public event EventHandler fail;
		public event EventHandler stop;

	
		// how long the whole event takes
		[Range(0, 5)]
		public float
				eventTime = 2f;
		private float eventCount = 0;
		private int clickCheck = 0;

		// on what time the player is able to Smash. Has to be a fracture of the eventTime!
		[Range(0, 5)]
		public float
				startOfSmashTime = 1f;

		// how long the player has time to hit the button
		[Range(0, 5)]
		public float
				smashTime = 0.5f;
		private float smashCount = 0;

		public float playerRefocusTime = 0.5f;

		public int actionButtonNr = 1;
			
		private InputControlType[] buttonPool = new InputControlType[]{
			InputControlType.Action1, InputControlType.Action2, InputControlType.Action3, InputControlType.Action4
		};


		public InputControlType currentButton;
		private Texture2D currentButtonTex;

		public bool playerPushed = false;
		public bool successfulHit = false;
		public bool isMultiSmash = false;
		public int howManySmashesLeft = 0;
		public float multiSmashDelay = 0.25f;
		private float multiSmashCount = 0;

		public Texture2D smashTimeTexture;
		public Texture2D onSmashTexture;
		public Texture2D onSuccessTexture;

		public GameObject planePrefabe;
		[Range(0.001f, 1f)]
		public float
				reScalePercent = 0.5f;

		public bool drawTimingUI = false;

		private GameObject outSidePlane;
		private GameObject smashPlane;
		private GameObject buttonPlane;
		private GameObject succesPlane;

		private Texture2D action1Texture;
		private Texture2D action1ClickedTexture;
		private Texture2D action2Texture;
		private Texture2D action2ClickedTexture;
		private Texture2D action3Texture;
		private Texture2D action3ClickedTexture;
		private Texture2D action4Texture;
		private Texture2D action4ClickedTexture;

		private GameObject target;
		[Range(-2, 2)]
		public float
				xMargin = 0;
		[Range(-2, 2)]
		public float
				yMargin = 0;
		private float zPos;

		// 0 = inactiv; 1 = activ and waiting for Input
		private int state = 0;

		private CutSceneLogic cutSceneLogic;
		private bool playerIsFrozen = false;

		private bool isSingleSmash = false;

		void Awake ()
		{			

				target = Camera.main.gameObject;// GameObject.FindWithTag ("SmallDudeParent");

				cutSceneLogic = GameObject.FindWithTag ("CutSceneLogic").GetComponent<CutSceneLogic> ();

				reset ();

				setupPlanes ();
		}

		// Use this for initialization
		void Start ()
		{
				if (!drawTimingUI) {
						deactivateAllPlanes ();
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (state == 1) {
						if (isSingleSmash) {
								showNormalSmash ();
						} else if (this.isMultiSmash && this.howManySmashesLeft > 0) {
								if (this.multiSmashCount <= 0) {
										showButtonSmash ();
								} else {
										this.multiSmashCount -= Time.deltaTime;
								}
						} else {
								showButtonSmash ();
						}
				}

		}

		private void showNormalSmash ()
		{
				// only used for the Button switching animation
				eventCount -= Time.deltaTime;

				// easy-version!
				drawButtonPlane (true);
				//updatePlanesPositions ();

				if (!playerPushed) {
						checkSingleSmash ();
				}

		}

		private void showButtonSmash ()
		{
				if (eventCount >= 0) { 
						eventCount -= Time.deltaTime;

						bool inTime = false;
						if (eventCount <= startOfSmashTime) {
								inTime = true;
						}

						showOutSidePlane ();
			
						// easy-version!
						drawButtonPlane (inTime);
						scalePlanes ((eventCount / eventTime));
						//updatePlanesPositions ();
			
						//Debug.Log ("buttonSmash " + playerPushed);
						if (!playerPushed) {
								checkButtonSmash (inTime);
						}
				} else {
						stopCountDown ();
				}

		}

		private void checkSingleSmash ()
		{
				if (isButtonPressed (currentButton)) {
						successfulSmash ();
						stopCountDown ();
				} else if (anyButtonOfPoolPressed (currentButton)) {
						MasterAudio.PlaySound ("ButtonSmashFail");
				}
		}
	
		private void checkButtonSmash (bool inSmashTime)
		{
				if (!inSmashTime) {
						checkButtonToEarly ();
				} else {

						//Debug.Log ("checkbuttonSmash " + smashCount);

						if (smashCount >= 0) {
								smashCount -= Time.deltaTime;

								checkRightButton ();
					
						} else {
								// no input in time! faaaail!
								failedSmash ();
						}
				}

		}

		private void checkRightButton ()
		{
				if (isButtonPressed (currentButton)) {
						// yeees you did it!
						successfulSmash ();
				} else {
						if (anyButtonOfPoolPressed (currentButton)) {
								// wrong button! faaaail!
								failedSmash ();
						}
				}

		}
	
		private void checkButtonToEarly ()
		{
				if (anyButtonPressed () != null) {
						// any input to early! faaaail!
						failedSmash ();
				}
		}

		private bool isButtonPressed (InputControlType thatButton)
		{
				return InputManager.ActiveDevice.GetControl (thatButton).IsPressed;
		}

		private bool anyButtonOfPoolPressed (InputControlType exceptThatButton)
		{

				InputControl button = anyButtonPressed ();

				if (button != null && !button.GetType ().Equals (exceptThatButton)) {
						return true;
				}

				return false;
		}

		private InputControl anyButtonPressed ()
		{
				for (int i = 0; i < buttonPool.Length; i++) {
			
						InputControlType buttonInQuestion = buttonPool [i];
						InputControl button = InputManager.ActiveDevice.GetControl (buttonInQuestion);

						if (button.IsPressed) {
								return button;
						}
				}
		
				return null;
		}

		private void reset ()
		{
				eventCount = eventTime;
				smashCount = smashTime;
				clickCheck = 0;
				playerPushed = false;
				successfulHit = false;
				isMultiSmash = false;
				howManySmashesLeft = 0;
				multiSmashCount = multiSmashDelay;
				//isSingleSmash = false;
		}

		private void resetForMultiSmash ()
		{
				//isSingleSmash = false;
				this.howManySmashesLeft--;
				this.currentButton = getRandomButton ();

				if (successfulHit && howManySmashesLeft > 0) {
						eventCount = eventTime;
						smashCount = smashTime;
						clickCheck = 0;
						multiSmashCount = this.multiSmashDelay;
						playerPushed = false;
						//Debug.Log ("multiSmashCount: " + multiSmashCount + " howManySmashes: " + howManySmashesLeft);						
				}
		}

	
		private void successfulSmash ()
		{
				if (drawTimingUI) {
						// show green one!
						succesPlane.SetActive (true);
		
						// hide red one!
						smashPlane.SetActive (false);
				}

				playerPushed = true;
				successfulHit = true;

				MasterAudio.PlaySound ("ButtonSmashSuccess");
		}

	#region Events

		private void OnStart (EventArgs e)
		{
				if (this.start == null) {
						//Debug.Log ("OnStart but no one to notify");
				} else {
						this.start (this, e);
				}
		}

		private void OnSuccess (EventArgs e)
		{
				if (this.success == null) {
						//Debug.Log ("OnSuccess but no one to notify");
				} else {
						this.success (this, e);
				}
		}

		private void OnFail (EventArgs e)
		{
				if (this.fail == null) {
						//Debug.Log ("OnFail but no one to notify");
				} else {
						this.fail (this, e);
				}
		}

		private void OnStop (EventArgs e)
		{
				if (this.stop == null) {
						//Debug.Log ("OnStop but no one to notify");
				} else {
						this.stop (this, e);
				}
		}

	#endregion

		private void failedSmash ()
		{
				if (drawTimingUI) {
						// show red one!
						smashPlane.SetActive (true);
				}

				playerPushed = true;
				successfulHit = false;

				if (this.isMultiSmash) {
						this.howManySmashesLeft = 0;
				}

				MasterAudio.PlaySound ("ButtonSmashFail");
				//Debug.Log ("failed ButtonSmash!");
		}

		private InputControlType getRandomButton ()
		{
				int buttonNr = UnityEngine.Random.Range (0, buttonPool.Length - 1);

				return buttonPool [buttonNr];
		}

		/*
		private InputControlType getPredefinedButton ()
		{
				return buttonPool [actionButtonNr];
		}
*/
		public void startButtonSmashEvent (bool freezePlayer, int howManySmashes = 0)
		{
				state = 1;

				reset ();

				if (howManySmashes > 0) {
						this.isMultiSmash = true;
						this.isSingleSmash = false;
						this.howManySmashesLeft = howManySmashes;
				}

				currentButton = getRandomButton ();
				currentButtonTex = getCurrentButtonTexture ();

				if (freezePlayer) {
						cutSceneLogic.freezePlayer (0);
						playerIsFrozen = true;
				}

				//Debug.Log ("buttonSmash " + this.smashCount);

				OnStart (EventArgs.Empty);
		}

		public void startNormalButtonSmashEvent (bool freezePlayer, InputControlType whichButton)
		{
				state = 1;
				this.isSingleSmash = true;
				this.isMultiSmash = false;

				reset ();

				currentButton = whichButton;
				currentButtonTex = getCurrentButtonTexture ();

				if (freezePlayer) {
						cutSceneLogic.freezePlayer (0);
						playerIsFrozen = true;
				}

				//Debug.Log ("normal smash subcribers: " + this.success.GetInvocationList ().Length + " " + this.fail.GetInvocationList ().Length);

				OnStart (EventArgs.Empty);
		}

		public void disableSmashEvent ()
		{
				if (this.isMultiSmash && this.howManySmashesLeft > 0) {
						// stopCountDown resets the state = 0
						resetForMultiSmash ();
				} else {

						state = 0;
						reset ();
			
						if (playerIsFrozen) {
								cutSceneLogic.unfreezePlayer (playerRefocusTime);
						}			
			
						OnStop (EventArgs.Empty);
				}
		
				deactivateAllPlanes ();			
		}
	
	
	#region intialLoading

		private Texture2D getCurrentButtonTexture ()
		{
				if (currentButton.Equals (InputControlType.Action1)) {
						return action1Texture;
				} else if (currentButton.Equals (InputControlType.Action2)) {
						return action2Texture;
				} else if (currentButton.Equals (InputControlType.Action3)) {
						return action3Texture;
				} else if (currentButton.Equals (InputControlType.Action4)) {
						return action4Texture;
				}

				return null;
		}

		private Texture2D getCurrentClickedTexture ()
		{
				if (currentButton.Equals (InputControlType.Action1)) {
						return action1ClickedTexture;
				} else if (currentButton.Equals (InputControlType.Action2)) {
						return action2ClickedTexture;
				} else if (currentButton.Equals (InputControlType.Action3)) {
						return action3ClickedTexture;
				} else if (currentButton.Equals (InputControlType.Action4)) {
						return action4ClickedTexture;
				}

				return null;
		}
	
		private void setupPlanes ()
		{
				Shader sprDef = Shader.Find ("Sprites/Default");
				zPos = Camera.main.transform.position.z + 0.5f;			
		
				outSidePlane = Instantiate (planePrefabe) as GameObject;
				outSidePlane.name = "_outSidePlane";
				outSidePlane.renderer.material.mainTexture = this.smashTimeTexture;
				outSidePlane.renderer.material.shader = sprDef;
				outSidePlane.transform.position = new Vector3 (this.target.transform.position.x + xMargin,
		                                               this.target.transform.position.y + yMargin, zPos);
		
				smashPlane = Instantiate (planePrefabe) as GameObject;
				smashPlane.name = "_smashPlane";
				smashPlane.renderer.material.mainTexture = this.onSmashTexture;
				smashPlane.renderer.material.shader = sprDef;
				smashPlane.transform.position = new Vector3 (this.target.transform.position.x + xMargin,
		                                             this.target.transform.position.y + yMargin, zPos + 0.1f);

				buttonPlane = Instantiate (planePrefabe) as GameObject;
				buttonPlane.name = "_buttonPlane";
				buttonPlane.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
				buttonPlane.renderer.material.shader = sprDef;
				buttonPlane.transform.position = new Vector3 (this.target.transform.position.x + xMargin,
		                                              this.target.transform.position.y + yMargin, zPos - 0.1f);

				succesPlane = Instantiate (planePrefabe) as GameObject;
				succesPlane.name = "_succesPlane";
				succesPlane.renderer.material.mainTexture = this.onSuccessTexture;
				succesPlane.renderer.material.shader = sprDef;
				succesPlane.transform.position = new Vector3 (this.target.transform.position.x + xMargin,
		                                              this.target.transform.position.y + yMargin, zPos + 0.2f);


				if (reScalePercent != 0) {
						succesPlane.transform.localScale *= reScalePercent;
						buttonPlane.transform.localScale *= reScalePercent;
						smashPlane.transform.localScale *= reScalePercent;
						outSidePlane.transform.localScale *= reScalePercent;
				}

				childPlanesToCam ();

				deactivateAllPlanes ();

				action1Texture = Resources.Load ("visuals/Button_A") as Texture2D;
				action1ClickedTexture = Resources.Load ("visuals/Button_A_clicked") as Texture2D;
				action2Texture = Resources.Load ("visuals/Button_B") as Texture2D;
				action2ClickedTexture = Resources.Load ("visuals/Button_B_clicked") as Texture2D;
				action3Texture = Resources.Load ("visuals/Button_X") as Texture2D;
				action3ClickedTexture = Resources.Load ("visuals/Button_X_clicked") as Texture2D;
				action4Texture = Resources.Load ("visuals/Button_Y") as Texture2D;
				action4ClickedTexture = Resources.Load ("visuals/Button_Y_clicked") as Texture2D;
		}

		private void childPlanesToCam ()
		{
				outSidePlane.transform.parent = Camera.main.transform;
				smashPlane.transform.parent = Camera.main.transform;
				buttonPlane.transform.parent = Camera.main.transform;
				succesPlane.transform.parent = Camera.main.transform;
		}

	#endregion

		public void stopCountDown ()
		{
				if (this.isMultiSmash && this.howManySmashesLeft > 0) {
						// stopCountDown resets the state = 0
						resetForMultiSmash ();
				} else {
						// trigger successEvent
						if (this.successfulHit) {
								OnSuccess (EventArgs.Empty);
						} else {
								OnFail (EventArgs.Empty);
						}
						state = 0;

						
						if (playerIsFrozen) {
								cutSceneLogic.unfreezePlayer (playerRefocusTime);
						}
						
			
						OnStop (EventArgs.Empty);
				}

				deactivateAllPlanes ();
		}

		private void deactivateAllPlanes ()
		{
				outSidePlane.SetActive (false);
				smashPlane.SetActive (false);
				buttonPlane.SetActive (false);
				// reset button specific texture
				buttonPlane.renderer.material.mainTexture = null;
				succesPlane.SetActive (false);
		}

		private void scalePlanes (float percent)
		{
				if (drawTimingUI) {
						float roundedPercent = UnityShortcuts.getRoundFloat (percent, 2);
						//Debug.Log ("roundedPercent: " + roundedPercent + " percent" + percent);
		
						float newWidth = this.planePrefabe.transform.localScale.x * roundedPercent;
						//Debug.Log ("outside percent: " + roundedPercent + " newWidth: " + newWidth);

						newWidth = Mathf.Max (newWidth, 0.001f);

						outSidePlane.transform.localScale = new Vector3 (newWidth,
		                                                 this.planePrefabe.transform.localScale.y,
		                                                 newWidth);			
						smashPlane.transform.localScale = new Vector3 (newWidth,
		                                               this.planePrefabe.transform.localScale.y,
		                                               newWidth);
						succesPlane.transform.localScale = new Vector3 (newWidth,
		                                               this.planePrefabe.transform.localScale.y,
		                                               newWidth);
				}
		}

		private void drawButtonPlane (bool inTime)
		{
				if (buttonPlane.activeSelf == playerPushed) {
						buttonPlane.SetActive (!playerPushed);
				}

				if (buttonPlane.activeSelf) {
						if (buttonPlane.renderer.material.mainTexture == null) {
								buttonPlane.renderer.material.mainTexture = getCurrentButtonTexture ();
						}

						if (inTime) {

								clickCheck++;

								//Debug.Log ("clicked swap: " + clickCheck + " clickCheck % 20 " + (clickCheck % 20));
								if (clickCheck % 10 == 0) {
										//Debug.Log ("buttonPlane.renderer.material.mainTexture: " + buttonPlane.renderer.material.mainTexture.Equals (getCurrentButtonTexture ()));
										if (buttonPlane.renderer.material.mainTexture.Equals (getCurrentButtonTexture ())) {
												buttonPlane.renderer.material.mainTexture = getCurrentClickedTexture ();
										} else {
												buttonPlane.renderer.material.mainTexture = getCurrentButtonTexture ();
										}
								}
						}
				}

		}

		private void showOutSidePlane ()
		{
				if (drawTimingUI) {
						/*
				if (!outSidePlane.activeSelf && !playerPushed) {
						outSidePlane.SetActive (true);
				}
				*/
						outSidePlane.SetActive (!playerPushed);
				}
		}
	
		private void updatePlanesPositions ()
		{
				outSidePlane.transform.position = new Vector3 (this.target.transform.position.x,
		                                               this.target.transform.position.y, zPos);
				buttonPlane.transform.position = new Vector3 (this.target.transform.position.x,
		                                               this.target.transform.position.y, zPos - 0.1f);

				smashPlane.transform.position = new Vector3 (this.target.transform.position.x,
		                                             this.target.transform.position.y, zPos + 0.1f);

				succesPlane.transform.position = new Vector3 (this.target.transform.position.x,
		                                              this.target.transform.position.y, zPos + 0.2f);

		}

		private void planesLookAtCam ()
		{
				succesPlane.transform.LookAt (Camera.main.transform);
				outSidePlane.transform.LookAt (Camera.main.transform);
				buttonPlane.transform.LookAt (Camera.main.transform);
				smashPlane.transform.LookAt (Camera.main.transform);
		}

}
