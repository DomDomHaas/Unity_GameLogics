using UnityEngine;
using System.Collections;

public class OnFocusControl : MonoBehaviour, I_focus
{

		[Range(1, 2)]
		public float
				scaleOnFocus = 1.25f;
		private Vector3 originalScale;
		private float originalCharSize;

		public bool changeColor = false;
		public Color onFocusColor;
		public Color originalColor;
		public string mainSoundGroup;
	
		// onFocus
		public bool playFocusSound = false;
		public string focusSoundName;
		private bool focusSoundplayed = false;	

		// onClick
		public bool playClickSound = false;
		public string clickSoundName;
		private bool clickSoundplayed = false;	

		private bool hasMesh = false;
		private bool hasGUIText = false;
		TextMesh myMesh;
		GUIText myText;

		void Awake ()
		{
				originalScale = this.transform.localScale;

				myMesh = this.GetComponent<TextMesh> ();
		
				myText = this.GetComponent<GUIText> ();
		
				if (myMesh != null) {
						this.hasMesh = true;
						this.originalColor = myMesh.color;
						originalCharSize = myMesh.characterSize;
				} else if (myText != null) {
						this.hasGUIText = true;
						this.originalColor = myText.color;
				} else {
						print (this + " button has problem, no thing to set color!");
				}
			
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
	
		public void onFocusEnter ()
		{
				if (changeColor) {
						if (this.hasMesh) {
								this.myMesh.color = this.onFocusColor;
								this.myMesh.characterSize = this.originalCharSize * this.scaleOnFocus;
						}
			
						if (this.hasGUIText) {
								this.myText.color = this.onFocusColor;
								this.transform.localScale = originalScale * this.scaleOnFocus;
						}
				}

				if (mainSoundGroup != "" && this.playFocusSound && !focusSoundplayed) {
						MasterAudio.PlaySound (sType: mainSoundGroup, variationName: focusSoundName);
						focusSoundplayed = true;
				}		

		}
	
		public void onClick ()
		{
				if (mainSoundGroup != "" && this.playClickSound) {
						MasterAudio.PlaySound (sType: mainSoundGroup, variationName: clickSoundName);
				}		
		}
	
		public void onFocusExit ()
		{
				if (changeColor) {
						if (this.hasMesh) {
								this.myMesh.color = this.originalColor;
								this.myMesh.characterSize = this.originalCharSize;
						}
			
						if (this.hasGUIText) {
								this.myText.color = this.originalColor;
								this.transform.localScale = originalScale;
						}
				}
		
				if (this.playFocusSound) {
						focusSoundplayed = false;	
				}

				if (clickSoundplayed) {
						clickSoundplayed = false;
				}
		}
	
		public void setVisible (bool visible)
		{
				if (this.hasMesh) {
						this.myMesh.renderer.enabled = visible;
				}
		
				if (this.hasGUIText) {
						this.myText.enabled = visible;
				}
		}

		public IEnumerator FadeText (float from, float to, float fadeTime, iTween.EaseType easeType)
		{

				if (fadeTime == 0) {

						setAlpha (to);

						yield return null;
				} else {

						iTween.ValueTo (this.gameObject, iTween.Hash (iT.ValueTo.from, from,
		                                      iT.ValueTo.to, to,
		                                      iT.ValueTo.time, fadeTime,
                                          iT.ValueTo.onupdate, "setAlpha",
                                          iT.ValueTo.easetype, easeType));
		
						yield return new WaitForSeconds (fadeTime);
				}
		}
	
		private void setAlpha (float newAlpha)
		{
				Color currentColor;
				if (this.hasMesh) {
						currentColor = this.myMesh.renderer.material.color;
						this.myMesh.renderer.material.color = new Color (currentColor.r, currentColor.g, currentColor.b, newAlpha);
				}
				if (this.hasGUIText) {
						currentColor = this.myText.color;
						this.myText.color = new Color (currentColor.r, currentColor.g, currentColor.b, newAlpha);
				}
		}

		public float getAlpha ()
		{
				if (this.hasMesh) {
						return this.myMesh.renderer.material.color.a;
				}
				if (this.hasGUIText) {
						return this.myText.color.a;
				}

				return -1;
		}
	
}
