
using UnityEngine;
using System.Collections;

public class CamFader : MonoBehaviour
{
	
		public GameObject deviceObserver;

		private GameObject fadePlane;
		private Color originalFadeColor;
		public bool isFading = false;

		[Range(0, 3)]
		public float
				defaultFadeTime = 0.5f;

		public float currentFadeTime = 0f;


		private PlaylistController playlistCont;
		private string currentAudioGroupName;

		void Awake ()
		{

				this.fadePlane = GameObject.FindWithTag ("UI_Fader");
				originalFadeColor = this.fadePlane.renderer.material.color;
				this.fadePlane.renderer.enabled = true;

				currentFadeTime = defaultFadeTime;
		}

		void Start ()
		{

		}

		// Update is called once per frame
		void Update ()
		{

		}

	#region FadingCoroutines

		public IEnumerator FadeIn (Color? color = null)
		{
				this.isFading = true;
				this.fadePlane.renderer.material.color = color.HasValue ? color.Value : originalFadeColor;

				//Debug.Log ("fadeIn color: " + this.fadePlane.renderer.material.color);
				iTween.FadeTo (this.fadePlane,
		               					iTween.Hash (iT.FadeTo.alpha, 0,
                                        iT.FadeTo.time, currentFadeTime));

				yield return new WaitForSeconds (this.currentFadeTime);


				currentFadeTime = defaultFadeTime;
				this.isFading = false;
				yield return null;
		}

		public IEnumerator FadeOut (Color? color = null)
		{
				this.isFading = true;

				Color colorToChange = color.HasValue ? color.Value : originalFadeColor;
				colorToChange.a = 0;
				this.fadePlane.renderer.material.color = colorToChange;
			

				iTween.FadeTo (this.fadePlane,
		               iTween.Hash (iT.FadeFrom.alpha, 1,
		             								iT.FadeFrom.time, currentFadeTime));		

				yield return new WaitForSeconds (this.currentFadeTime);


		
				currentFadeTime = defaultFadeTime;
				this.isFading = false;
				yield return null;
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

		private void setGroupVolume (float newVolume)
		{
				MasterAudio.SetGroupVolume (currentAudioGroupName, newVolume);
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


		private void setMasterVolume (float newVolume)
		{
				MasterAudio.MasterVolumeLevel = newVolume;
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



}
