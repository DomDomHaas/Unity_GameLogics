using UnityEngine;
using System.Collections;

public class iTweenable : MonoBehaviour
{

		public bool isTweening = false;


	#region iTween_coroutines


		public IEnumerator Fade (float from, float to, float fadeTime, iTween.EaseType easeType = iTween.EaseType.linear)
		{
				isTweening = true;

				if (fadeTime <= 0) {
						setAlpha (to);
				} else {

						iTween.ValueTo (this.gameObject, iTween.Hash (iT.ValueTo.from, from,
		                                              iT.ValueTo.to, to,
		                                              iT.ValueTo.time, fadeTime,
		                                              iT.ValueTo.onupdate, "setAlpha",
		                                              iT.ValueTo.easetype, easeType));
		
						yield return new WaitForSeconds (fadeTime);
				}

				isTweening = false;
		}
	
		private void setAlpha (float newAlpha)
		{
				Color currentColor = this.renderer.material.color;

				this.renderer.material.color = new Color (currentColor.r, currentColor.g, currentColor.b, newAlpha);

				/*				if (this.hasMesh) {
						currentColor = this.myMesh.renderer.material.color;
						this.myMesh.renderer.material.color = new Color (currentColor.r, currentColor.g, currentColor.b, newAlpha);
				}
				if (this.hasGUIText) {
						currentColor = this.myText.color;
						this.myText.color = new Color (currentColor.r, currentColor.g, currentColor.b, newAlpha);
				}
*/
		}
	
		public IEnumerator ShakeScaleAnimation (float time, float shakeStrength, bool loop = false, iTween.LoopType loopType = iTween.LoopType.none)
		{
				isTweening = true;
		
				Vector3 shakeAmount = new Vector3 (shakeStrength, 0, shakeStrength);
		
				iTween.ShakeScale (this.gameObject, iTween.Hash (iT.ShakeScale.amount, shakeAmount,
		                                                 iT.ShakeScale.time, time,
		                                                 iT.ShakeScale.looptype, loopType));
		
				yield return new WaitForSeconds (time);
		
				isTweening = false;
		}
	
		public IEnumerator MoveToAnimation (float time, Vector3 toPos, iTween.EaseType easeType = iTween.EaseType.linear)
		{
				isTweening = true;
		
				iTween.MoveTo (this.gameObject, iTween.Hash (iT.MoveTo.position, toPos,
		                                             iT.MoveTo.time, time,
		                                             iT.MoveTo.islocal, true,
		                                             iT.MoveTo.easetype, easeType));
		
				yield return new WaitForSeconds (time);
		
				isTweening = false;
		}
	
		public IEnumerator ScaleToAnimation (float time, Vector3 toScale, iTween.EaseType easeType = iTween.EaseType.linear)
		{
				isTweening = true;

				if (time <= 0) {
						this.transform.localScale = toScale;
						yield return new WaitForEndOfFrame ();
				} else {
		
						//Debug.Log (this.name + " storyAnimating " + this.storyAnimating + " scale: " + this.transform.localScale);
		
						iTween.ScaleTo (this.gameObject, iTween.Hash (iT.ScaleTo.scale, toScale,
		                                              iT.ScaleTo.time, time,
		                                              iT.ScaleTo.easetype, easeType));
						//		iT.ScaleTo.easetype, iTween.EaseType.easeInExpo));
		
						yield return new WaitForSeconds (time);
				}
		
				isTweening = false;
		}

		public IEnumerator ShakeAndScale (float shakeScaleRatio, float time, float strength, Vector3 scaleTo)
		{
				float scaleTime = 1 - shakeScaleRatio;
				float shakeTime = 1 - scaleTime;
		
				yield return  StartCoroutine (ShakeScaleAnimation (shakeTime, strength));
				StartCoroutine (ScaleToAnimation (scaleTime, scaleTo));
		}
	
	#endregion
}

