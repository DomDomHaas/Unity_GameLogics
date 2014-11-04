using UnityEngine;
using System.Collections;

/**
 * copy paste from http://www.mikedoesweb.com/2012/camera-shake-in-unity/
 **/
public class CameraLocShaker : MonoBehaviour
{
		// Transform of the camera to shake. Grabs the gameObject's transform
		// if null.
		public Transform camTransform;

		// How long the object should shake for.
		//public float shake = 0f;

		//private bool doShake = false;

		// Amplitude of the shake. A larger value shakes the camera harder.
		[Range(0.1f, 3f)]
		public float
				shakeAmount = 1f;
		//public float decreaseFactor = 1.0f;

		Vector3 originalPos;

		void Awake ()
		{
				if (camTransform == null) {
						camTransform = GetComponent (typeof(Transform)) as Transform;
				}
		}

		void Update ()
		{
				/*
				if (!doShake)
						return;

				if (shake > 0) {
//						camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
						Vector2 shakeVector = Random.insideUnitCircle * shakeAmount;
						//Debug.Log ("random vector2: " + shakeVector);
						Vector3 only2DShake = originalPos + new Vector3 (shakeVector.x, shakeVector.y, 0);
						//Debug.Log ("only2DShake: " + only2DShake);
						camTransform.localPosition = only2DShake;
						shake -= Time.deltaTime * decreaseFactor;
				} else {
						shake = 0f;
						doShake = false;
						camTransform.localPosition = originalPos;
				}
				*/
		}

		public void doCamShake (float shakeDelay, float shakeLength)
		{
				doCamShake (shakeDelay, shakeLength, shakeAmount);
		}

		public void doCamShake (float shakeDelay, float shakeLength, float shakeStrength)
		{
				StartCoroutine (ShakeCam (shakeDelay, shakeLength, shakeStrength));
		}

		private IEnumerator ShakeCam (float delay, float shakeTime, float shakeStrength)
		{
				Vector2 shakeVector = Random.insideUnitCircle * shakeStrength;
				Vector3 only2DShake = new Vector3 (shakeVector.x, shakeVector.y, 0);

				//Vector3 only2DShake = new Vector3 (2, 2, 0);
				//Debug.Log ("cam shake: " + only2DShake.magnitude);

				yield return new WaitForSeconds (delay);

				iTween.ShakePosition (this.gameObject,
		                      iTween.Hash (iT.ShakePosition.amount, only2DShake,
                                        iT.ShakePosition.time, shakeTime));

				yield return new WaitForSeconds (shakeTime);
		}

		
}