
using UnityEngine;
using System.Collections;

public class CamCharFollow : MonoBehaviour
{
		
		public bool autoStartAtPlayer = false;
		public bool fadeInAtStart = true;

		public GameObject deviceObserver;
		public GameObject rotatePrefab;
		public Transform rotateBackPos;

		public GameObject target;

		private Vector3 posTo = Vector3.zero;

		[Range(0, 3)]
		public float
				directionChangeDelay = 1f;
		private float lastXFacing = 1;
		private bool camChangeAfterDeath = false;


		[Range(0, 5)]
		public float
				yMargin = 2f;
		[Range(0, 5)]
		public float
				xMargin = 0f;

		[Range(0, 5)]
		public float
				yMergeWatchMargin = 1f;
		[Range(-5, 5)]
		public float
				xMergeWatchMargin = 0f;


		public bool isStoryTweening = false;
		public bool isZooming = false;
		public float stayMergeTime = 1f;
		public float mergeMoveTime = 0.5f;

		private GameObject mergeCamPoint;
		private Vector3 toMergePos;
		public Vector3 moveBackPos;
		public Vector3 pointToWatch;

		// Show this float in the Inspector as a slider between 0 and 10
		[Range(1f, 179f)]
		public float
				onBigFOV = 80f;
		private float originalFOV;

		[Range(1f, 179f)]
		public float
				duringMergeFOV = 50f;


		public float zoomFOVTime = 1f;
		[Range(1, 5)]
		public float
				defaultZoomTime = 1f;

		[Range(1, 10)]
		public float
				introZoomTime = 3f;

		[Range(1, 20)]
		public float
				introStayTime = 15f;

		[Range(1f, 179f)]
		public float
				introFOV = 25f;

		private SmashButtonEvent buttonSmash;

		/// <summary>
		/// For Warps inside the lvl and for the outro
		/// </summary>
		private Transform lvl_warps;
		//private GameObject rightLimit;
		public bool followTargetAtLimits = true;
		public bool reachedRightLimit = false;
		//private GameObject leftLimit;
		public bool reachedLeftLimit = false;

		private GameObject smallDude;

		private GameObject fadePlane;
		public bool isFading = false;
		private float defaultFadeTime = 1f;
		public float currentFadeTime = 0f;
		private float waitDuringDeath = 0.5f;

		[Range(1, 5)]
		public float
				deathFadeOutTime = 2f;

		[Range(1, 5)]
		public float
				deathFadeInTime = 2f;

		private PlaylistController playlistCont;
		private string currentAudioGroupName;

		void Awake ()
		{
				if (!GameObject.FindWithTag ("DeviceObserver")) {
						Instantiate (deviceObserver);
				}

				//startPosz = this.transform.position.z;
				mergeCamPoint = GameObject.FindWithTag ("MergeCamPoint");
				buttonSmash = UnityShortcuts.getScriptOnCam (typeof(SmashButtonEvent)) as SmashButtonEvent;

				GameObject rotateBack = Instantiate (this.rotatePrefab, new Vector3 (this.transform.position.x, this.transform.position.y, 0), Quaternion.identity) as GameObject;
				rotateBackPos = rotateBack.transform;

				//Debug.Log ("start " + Camera.main.fieldOfView);
				this.originalFOV = Camera.main.fieldOfView;

				GameObject warps = GameObject.FindWithTag ("lvl_Warps");
				if (warps != null) {
						lvl_warps = warps.transform;
				}

				this.fadePlane = GameObject.FindWithTag ("UI_Fader");
				this.fadePlane.renderer.enabled = true;
				currentFadeTime = defaultFadeTime;

				playlistCont = PlaylistController.Instances [0];
		}

		void Start ()
		{
				smallDude = GameObject.FindWithTag ("SmallDudeParent");

				if (autoStartAtPlayer) {
						positionAtPlayer ();
				}


				if (fadeInAtStart) {
						StartCoroutine (FadeIn (false));
				}

				if (autoStartAtPlayer && smallDude.GetComponent<SmallDudeMainControl> ().isIntroActive) {
						MasterAudio.MasterVolumeLevel = 0;			
						StartCoroutine (StoryIntro ());
				} else {
						StartCoroutine ("FollowingTarget");
				}
		}

		private void positionAtPlayer ()
		{
				Vector3 smallTransPos = smallDude.transform.position;
				this.transform.position = new Vector3 (smallTransPos.x, smallTransPos.y, this.transform.position.z);
		}

		// Update is called once per frame
		void Update ()
		{
				if (!this.isStoryTweening && !this.isFading) {
						checkDeath ();
				}
		}

	#region TargetFollowing

		public IEnumerator FollowingTarget ()
		{
				while (true) {

/*						float xFacing = getTargetxFacing ();

						posTo = new Vector3 (this.target.transform.position.x + xMargin * xFacing,
			                     this.target.transform.position.y + yMargin,
			                     this.transform.position.z);
*/						//Debug.Log ("target " + this.target + " tweening " + isStoryTweening + " fading " + isFading);
						if (this.target != null && !isStoryTweening && !isFading) {
								yield return StartCoroutine (followTarget ());
						} else {
								yield return new WaitForEndOfFrame ();
						}
				}
		}

		private IEnumerator followTarget ()
		{
				float xFacing = getTargetxFacing ();

				if (lastXFacing != xFacing || camChangeAfterDeath) {
						float moveTime = 3f;
						lastXFacing = xFacing;
						if (camChangeAfterDeath) {
								camChangeAfterDeath = false;
								moveTime = 5f;
						}
						//Debug.Log ("change dir!");
						yield return StartCoroutine ("WaitForPosition", moveTime);

				} else {			
						lastXFacing = xFacing;
						yield return StartCoroutine ("WaitForPosition", 0);
				}
		}

		private IEnumerator WaitForPosition (float moveTime)
		{
				while (true) {
						float xFacing = getTargetxFacing ();

						float newXPos = this.target.transform.position.x + xMargin * xFacing;

						if ((reachedRightLimit && newXPos > this.transform.position.x)
								|| (reachedLeftLimit && newXPos < this.transform.position.x)) {

								newXPos = this.transform.position.x;
						}

						posTo = new Vector3 (newXPos,
			                     this.target.transform.position.y + yMargin,
			                     this.transform.position.z);

						if (followTargetAtLimits && (reachedRightLimit || reachedLeftLimit)) {
								float limitXMargin = xMargin / 2;
				
								if (reachedLeftLimit) {
										limitXMargin *= -1;
								}


								Vector3 lookAtTarget = this.target.transform.position + new Vector3 (limitXMargin, yMargin, 0);

								iTween.MoveUpdate (this.gameObject,
			                   iTween.Hash (iT.MoveUpdate.position, posTo,
												             iT.MoveUpdate.time, moveTime,
				             								 iT.MoveUpdate.looktime, 2f,
				             								 iT.MoveUpdate.looktarget, lookAtTarget));
						} else {
								iTween.MoveUpdate (this.gameObject,
				                   iTween.Hash (iT.MoveUpdate.position, posTo,
														             iT.MoveUpdate.time, moveTime));

						}
			
						float myX = UnityShortcuts.getRoundFloat (this.transform.position.x, 2);
						float posX = UnityShortcuts.getRoundFloat (this.posTo.x, 2);

						//Debug.Log ("myX " + myX + " posX " + posX);

						if (myX == posX) {
								//Debug.Log ("waited for position finished");
								break;
						}
						yield return new WaitForEndOfFrame ();
				}

		}

	#endregion

		private float getTargetxFacing ()
		{
				float xFacing = 1;

				if (this.target.GetComponent<SmallDudeMainControl> () != null) {
						return this.target.GetComponent<SmallDudeMainControl> ().xFacing;
				}

				if (this.target.GetComponent<BigControl> () != null) {
						return this.target.GetComponent<BigControl> ().xFacing;
				}

				return xFacing;
		}

		private void resetFollow ()
		{
				posTo = Vector3.zero;
		}
	

		public void setTarget (GameObject target)
		{
				this.target = target;
				//Debug.Log ("set CamTarget: " + target);

				if (this.target.tag.Equals ("SmallDudeParent") && Camera.main.fieldOfView != originalFOV) {
						StartCoroutine (CheckZoom (Camera.main.fieldOfView, this.originalFOV));
				}
		}

		public void triggerMergeTween (Vector3 pointToWatch, float bigDudeXFacing)
		{
				StopCoroutine ("FollowingTarget");
				StopCoroutine ("WaitForPosition");

				this.isStoryTweening = true;
				this.pointToWatch = pointToWatch;
				//Debug.Log ("bigDudeXFacing: " + bigDudeXFacing);

				this.pointToWatch += new Vector3 (xMergeWatchMargin * bigDudeXFacing, yMergeWatchMargin, 0);	

				register ();

				this.toMergePos = this.mergeCamPoint.transform.position;
				this.toMergePos = new Vector3 (this.toMergePos.x, this.toMergePos.y, this.transform.position.z);

				setMoveBackPos (this.transform.position);
				storeCurrentRotateBackPos ();

				StartCoroutine ("MoveToMergePoint", true);
		}

		private void setMoveBackPos (Vector3 backPos)
		{
				this.moveBackPos = backPos;
		}
		private void storeCurrentRotateBackPos ()
		{
				this.rotateBackPos.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
		}

		public void OnSuccess (object smashTrigger, System.EventArgs e)
		{		
				unregister ();
		}

		public void OnFail (object smashTrigger, System.EventArgs e)
		{
				//stop the normal merge zoom
				StopCoroutine ("MoveToMergePoint");

				StartCoroutine ("MoveBack", true);

				unregister ();
		}
		
		private void unregister ()
		{
				buttonSmash.success -= OnSuccess;
				buttonSmash.fail -= OnFail;
		}
	
		private void register ()
		{
				buttonSmash.success += OnSuccess;
				buttonSmash.fail += OnFail;
		}


	#region iTweens


		private IEnumerator StoryIntro ()
		{
				StartCoroutine (FadeAudioMaster (1, 4, iTween.EaseType.linear));
				// wait for cam to focus on SmallDude!
				yield return new WaitForSeconds (1.5f);

				this.isStoryTweening = true;

				// zoom close to SmallDude!
				zoomFOVTime = this.introZoomTime;
		
				StartCoroutine (CheckZoom (Camera.main.fieldOfView, this.introFOV));
		
				yield return new WaitForSeconds (this.introStayTime);

				this.isStoryTweening = false;

				// zoom back out!
				zoomFOVTime = 2f;	
				StartCoroutine (CheckZoom (Camera.main.fieldOfView, this.originalFOV));

				yield return StartCoroutine (WaitForZoomToFinish ());

				yield return StartCoroutine ("WaitForPosition", 5f);

				StartCoroutine ("FollowingTarget");
		}

		private void checkDeath ()
		{
				if (this.smallDude.GetComponent<SmallDudeMainControl> ().isDead) {						
						StartCoroutine (FadeOutAndIn ());
						this.camChangeAfterDeath = true;
				}
		}

		private IEnumerator FadeOutAndIn ()
		{
				this.isStoryTweening = true;
				// 2.5f (deathAnimationTime) + deathCooldown - fadeouttime!
				float waitForDeathAnimation = this.smallDude.GetComponent<SmallDudeMainControl> ().deathCoolDown + 2.5f - this.deathFadeOutTime;
				yield return new WaitForSeconds (waitForDeathAnimation);

				StopCoroutine ("WaitForPosition");		
				StopCoroutine ("FollowingTarget");

				this.isFading = true;

				currentFadeTime = this.deathFadeOutTime;
				yield return StartCoroutine (FadeOut (true));

				yield return new WaitForSeconds (waitDuringDeath / 2);

				positionAtPlayer ();

				yield return new WaitForSeconds (waitDuringDeath / 2);

				// start fresh at new position
				//resetFollow ();


				currentFadeTime = this.deathFadeInTime;
				yield return StartCoroutine (FadeIn (true));

				this.isFading = false;
				this.isStoryTweening = false;
				StartCoroutine ("FollowingTarget");
		}


		public IEnumerator FadeIn (bool isDeathFade)
		{
				if (!isDeathFade) {
						this.isFading = true;
				}

				iTween.FadeTo (this.fadePlane,
		               					iTween.Hash (iT.FadeTo.alpha, 0,
                                        iT.FadeTo.time, currentFadeTime));

				yield return new WaitForSeconds (this.currentFadeTime);
				currentFadeTime = defaultFadeTime;
				if (!isDeathFade) {
						this.isFading = false;
				}
				yield return null;
		}

		public IEnumerator FadeOut (bool isDeathFade)
		{
				if (!isDeathFade) {
						this.isFading = true;
				}

				iTween.FadeTo (this.fadePlane,
		               iTween.Hash (iT.FadeFrom.alpha, 1,
		             								iT.FadeFrom.time, currentFadeTime));		

				yield return new WaitForSeconds (this.currentFadeTime);
				currentFadeTime = defaultFadeTime;
				if (!isDeathFade) {
						this.isFading = false;
				}
				yield return null;
		}


		private IEnumerator MoveToMergePoint (bool stopStoryTweening)
		{
				yield return new WaitForEndOfFrame ();
				//this.isStoryTweening = true;
				//Debug.Log ("start moveToMergePoint! from " + transform.position + " move to " + toMergePos + " pointToWatch " + pointToWatch);
		
				iTween.MoveTo (this.gameObject,
		               iTween.Hash (iT.MoveTo.position, toMergePos,
		             iT.MoveTo.looktarget, pointToWatch, 
		             //iT.MoveTo.looktime, this.mergeMoveTime / 2f,
		             iT.MoveTo.time, this.mergeMoveTime,
		             iT.MoveTo.easetype, iTween.EaseType.easeInOutSine));


				// zoom to merge!
				yield return StartCoroutine (CheckZoom (Camera.main.fieldOfView, this.duringMergeFOV));

				yield return new WaitForSeconds (this.mergeMoveTime);

				// just in case the zoom isn't finished yet
				yield return StartCoroutine (WaitForZoomToFinish ());
				//Debug.Log ("stay during merge!");
		
				yield return new WaitForSeconds (this.stayMergeTime);

				StartCoroutine ("MoveBack", false);


				// zoom back out to big!
				zoomFOVTime = 2f;

				StartCoroutine (CheckZoom (Camera.main.fieldOfView, this.onBigFOV));

				yield return new WaitForSeconds (this.mergeMoveTime);

				yield return StartCoroutine (WaitForZoomToFinish ());


				if (stopStoryTweening) {
						this.isStoryTweening = false;
						StartCoroutine ("FollowingTarget");
				}
		}
	
		public IEnumerator MoveBack (bool stopStoryTweening)
		{
				//Debug.Log ("start moveback to " + moveBackPos);
		
				iTween.MoveTo (this.gameObject,
		               iTween.Hash (iT.MoveTo.position, moveBackPos,
										             iT.MoveTo.looktarget, rotateBackPos, 
										             iT.MoveTo.time, this.mergeMoveTime,
										             iT.MoveTo.looktime, this.mergeMoveTime / 2,
										             iT.MoveTo.easetype, iTween.EaseType.easeInOutSine));

				yield return new WaitForSeconds (this.mergeMoveTime + 0.05f);

				if (stopStoryTweening) {
						this.isStoryTweening = false;
						StartCoroutine ("FollowingTarget");
				}		
		}

		public IEnumerator MoveToPos (Vector3 toPos, Vector3 pointToWatch, float moveTime, float lookTime = 0f)
		{
				if (pointToWatch == Vector3.zero) {
						//Debug.Log ("cam move without target");

						iTween.MoveTo (this.gameObject,
						               iTween.Hash (iT.MoveTo.position, toPos,
													             iT.MoveTo.time, moveTime,
													             iT.MoveTo.easetype, iTween.EaseType.easeInOutSine));

				} else {
						//Debug.Log ("cam move with a target");

						iTween.MoveTo (this.gameObject,
		               iTween.Hash (iT.MoveTo.position, toPos,
									             iT.MoveTo.looktarget, pointToWatch, 
									             iT.MoveTo.time, moveTime,
			             						 iT.MoveTo.looktime, lookTime,
									             iT.MoveTo.easetype, iTween.EaseType.easeInOutSine));
				}
		
		
				yield return new WaitForSeconds (moveTime);
		}

		/// <summary>
		/// Fake MoveToPos with the last RotateBackPosition and afterwards start FollowingTarget again
		/// </summary>
		/// <returns>The move after follow.</returns>
		public IEnumerator ResetMoveAfterFollow ()
		{
				Vector3 pointToWatch = this.rotateBackPos.position;
				yield return StartCoroutine (MoveToPos (this.transform.position, pointToWatch, 2f, 2f));

				//Debug.Log ("waitforposition again");
				yield return StartCoroutine ("WaitForPosition", 3f);
		
				StartCoroutine ("FollowingTarget");
		}
	
		public IEnumerator CheckZoom (float fromFOV, float toFOV)
		{
				//Debug.Log ("start zoom! from " + fromFOV + " to " + toFOV);

				if (toFOV > 0 && fromFOV != toFOV) {
						isZooming = true;
						iTween.ValueTo (this.gameObject,
		                iTween.Hash (iT.ValueTo.from, fromFOV,
										             iT.ValueTo.to, toFOV, 
										             iT.ValueTo.time, this.zoomFOVTime,
		             								 iT.ValueTo.onupdate, "OnFOVChange",
		             								 iT.ValueTo.easetype, iTween.EaseType.easeOutExpo));

						yield return new WaitForSeconds (this.zoomFOVTime);
						this.zoomFOVTime = this.defaultZoomTime;
						isZooming = false;

				} else {
						yield return null;
				}
		}

		public IEnumerator ZoomBackToOriginal ()
		{
				//Debug.Log ("zoom back to original. from " + Camera.main.fieldOfView + " to " + this.originalFOV);
		
				if (Camera.main.fieldOfView != this.originalFOV) {
						isZooming = true;

						iTween.ValueTo (this.gameObject,
			                iTween.Hash (iT.ValueTo.from, Camera.main.fieldOfView,
			             iT.ValueTo.to, this.originalFOV, 
			             iT.ValueTo.time, this.zoomFOVTime,
			             iT.ValueTo.onupdate, "OnFOVChange",
			             iT.ValueTo.easetype, iTween.EaseType.easeOutExpo));

						yield return new WaitForSeconds (this.zoomFOVTime);
						this.zoomFOVTime = this.defaultZoomTime;
						isZooming = false;

				} else {
						yield return null;
				}

		}

		private void OnFOVChange (float newValue)
		{
				Camera.main.fieldOfView = newValue;
		}

		/// <summary>
		/// Fades the play list. set from = -1 to use the current volume of the playlist
		/// <returns>The play list.</returns>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="fadeTime">Fade time.</param>
		/// <param name="easeType">Ease type.</param>
		/// </summary>
		public IEnumerator FadePlayList (float from, float to, float fadeTime, iTween.EaseType easeType)
		{
				if (from == -1) {
						from = playlistCont.CurrentPlaylistSource.volume;
				}

				iTween.ValueTo (this.gameObject, iTween.Hash (iT.ValueTo.from, from,
		                                              iT.ValueTo.to, to,
		                                              iT.ValueTo.time, fadeTime,
		                                              iT.ValueTo.onupdate, "setVolume",
		                                              iT.ValueTo.easetype, easeType));
		
				yield return new WaitForSeconds (fadeTime + 0.1f);
		}
	
		private void setVolume (float newVolume)
		{
				playlistCont.CurrentPlaylistSource.volume = newVolume;
		}

		public IEnumerator FadeAudioGroup (string audioGroupName, float to, float fadeTime, iTween.EaseType easeType, float from = -1f)
		{
				currentAudioGroupName = audioGroupName;
				if (from == -1) {
						from = MasterAudio.GetGroupVolume (currentAudioGroupName);
				}
		
				iTween.ValueTo (this.gameObject, iTween.Hash (iT.ValueTo.from, from,
		                                              iT.ValueTo.to, to,
		                                              iT.ValueTo.time, fadeTime,
		                                              iT.ValueTo.onupdate, "setGroupVolume",
		                                              iT.ValueTo.easetype, easeType));
		
				yield return new WaitForSeconds (fadeTime + 0.1f);

		}

		public IEnumerator FadeAudioMaster (float to, float fadeTime, iTween.EaseType easeType, float from = -1f)
		{
				if (from == -1) {
						from = MasterAudio.MasterVolumeLevel;
				}
				iTween.ValueTo (this.gameObject, iTween.Hash (iT.ValueTo.from, from,
			                                              iT.ValueTo.to, to,
			                                              iT.ValueTo.time, fadeTime,
			                                              iT.ValueTo.onupdate, "setMasterVolume",
			                                              iT.ValueTo.easetype, easeType));
			
				yield return new WaitForSeconds (fadeTime + 0.1f);
			
		}

		private void setGroupVolume (float newVolume)
		{
				//		 = newVolume;
				MasterAudio.SetGroupVolume (currentAudioGroupName, newVolume);
		}

		private void setMasterVolume (float newVolume)
		{
				MasterAudio.MasterVolumeLevel = newVolume;
		}

		public IEnumerator WaitForZoomToFinish ()
		{
				while (true) {
						if (!isZooming) {
								//Debug.Log ("waited for zoom, finsihed: " + Camera.main.fieldOfView);
								break;
						}
						yield return new WaitForSeconds (0.05f);
				}
		}

		public IEnumerator WaitForFadeToFinish ()
		{
				while (true) {
						if (!isFading) {
								break;
						}
						yield return new WaitForSeconds (0.05f);
				}
		}
	
	#endregion

	#region CameraLimits

		void OnTriggerEnter (Collider coll)
		{
				if (coll.gameObject.tag.Equals ("RightLimit")) {
						reachedRightLimit = true;
				} else if (coll.gameObject.tag.Equals ("LeftLimit")) {
						reachedLeftLimit = true;
						//Debug.Log (Time.time + " reached left " + this.transform.position.x);
				}

		}

		void OnTriggerStay (Collider coll)
		{
				if (!reachedRightLimit && coll.gameObject.tag.Equals ("RightLimit")) {
						reachedRightLimit = true;
				} else if (!reachedLeftLimit && coll.gameObject.tag.Equals ("LeftLimit")) {
						reachedLeftLimit = true;
						//Debug.Log ("reached left later " + this.transform.position.x);
				}		

				if (followTargetAtLimits
						&& (coll.gameObject.tag.Equals ("RightLimit") || coll.gameObject.tag.Equals ("LeftLimit"))) {
						storeCurrentRotateBackPos ();
				}
		}

	
		void OnTriggerExit (Collider coll)
		{
				if (coll.gameObject.tag.Equals ("RightLimit")) {
						reachedRightLimit = false;
				} else if (coll.gameObject.tag.Equals ("LeftLimit")) {
						reachedLeftLimit = false;
						//Debug.Log (Time.time + " out of left " + this.transform.position.x);
				}

				if (followTargetAtLimits
						&& (coll.gameObject.tag.Equals ("RightLimit") || coll.gameObject.tag.Equals ("LeftLimit"))) {
						StopCoroutine ("FollowingTarget");
						StartCoroutine (ResetMoveAfterFollow ());
				}
		}
	
	#endregion

}
