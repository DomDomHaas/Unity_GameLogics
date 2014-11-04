using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;


public static class UnityShortcuts
{

	#region ColliderShortCuts

		public static Collider getFirstCollider (GameObject objWithChildCollider)
		{		 
				Collider[] colls = objWithChildCollider.GetComponents<Collider> ();

				for (int i = 0; i < colls.Length; i++) {
						Collider coll = colls [i];
						if (!coll.isTrigger) {
								return coll;
						}
				}

				return null;
		}

		/** 
	 * For now, works only for CircleCollider2D
	 * */
		public static ArrayList getAllCircleCollider2D (GameObject objWithTriggers, bool getOnlyTriggers)
		{
				ArrayList colls = new ArrayList ();

				CircleCollider2D[] colls2D = objWithTriggers.GetComponents<CircleCollider2D> ();

				for (int i = 0; i < colls2D.Length; i++) {
						CircleCollider2D circleCol = colls2D [i];
						if (getOnlyTriggers) {
								if (circleCol.isTrigger) {
										colls.Add (circleCol);
								}
						} else {
								if (!circleCol.isTrigger) {
										colls.Add (circleCol);
								}
						}
				}

				return colls;
		}

		/** 
	 * For now, works only for SphereCollider
	 * */
		public static ArrayList getAllSphereColliders (GameObject objWithTriggers, bool getOnlyTriggers)
		{
				ArrayList colls = new ArrayList ();

				SphereCollider[] colls3D = objWithTriggers.GetComponents<SphereCollider> ();

				for (int i = 0; i < colls3D.Length; i++) {
						SphereCollider sphereColl = colls3D [i];
						if (getOnlyTriggers) {
								if (sphereColl.isTrigger) {
										colls.Add (sphereColl);
								}
						} else {
								if (!sphereColl.isTrigger) {
										colls.Add (sphereColl);
								}
						}
				}


				return colls;
		}

	#endregion

	#region GetChildren

	
		public static ArrayList getChildrenWithTag (GameObject parent, string tag)
		{
				ArrayList children = new ArrayList ();
		
				for (int i = 0; i < parent.transform.childCount; i++) {
						Transform child = parent.transform.GetChild (i);
						if (child.gameObject.tag.Equals (tag)) {
								children.Add (child);	
						}
				}
		
				return children;		
		}
	
		public static GameObject getFirstChildWithTag (this GameObject parent, string tag)
		{
		
				for (int i = 0; i < parent.transform.childCount; i++) {
						Transform child = parent.transform.GetChild (i);
			
						if (child.tag.Equals (tag)) {
								return child.gameObject;
						}
				}
		
				return null;
		}
	
		public static Transform[] GetOnlyChildren (this Transform trans)
		{
				Transform[] children = trans.GetComponentsInChildren<Transform> ();
				//Debug.Log ("length " + children.Length);
		
				if (children.Length > 1) {
						Transform[] exceptMySelf = new Transform [children.Length - 1];
						int newIndex = 0;
			
						for (int i = 0; i < children.Length; i++) {
								Transform child = children [i];
								if (child != trans) {
										exceptMySelf [newIndex] = child;
										newIndex++;
								}
						}
			
						//Debug.Log ("return length " + exceptMySelf.Length);
						return exceptMySelf;
				}
		
				return null;
		}
	
	
		public static void ChangeLayersRecursively (this Transform trans, string name)
		{
				trans.gameObject.layer = LayerMask.NameToLayer (name);
				foreach (Transform child in trans) {
						child.ChangeLayersRecursively (name);
				}
		}
	
		public static void SetStaticRecursively (this Transform trans, bool isStatic)
		{
				trans.gameObject.isStatic = isStatic;
				foreach (Transform child in trans) {
						child.SetStaticRecursively (isStatic);
				}
		}


	#endregion


		public static Component getScriptOnCam (System.Type componentType)
		{
				return Camera.main.GetComponent (componentType);
		}


		public static float getRoundFloat (float number, int digits)
		{
				float multipler = Mathf.Pow (10, (float)digits);
				float roundedNumber = ((int)(number * multipler)) / multipler;
				//Debug.Log ("number: " + number + " multipler: " + multipler + " roundedNumber: " + roundedNumber);
				return roundedNumber;
		}


		public static Vector3 getGroundNormal (Vector3 pos, Vector3 direction, float rayLength, LayerMask moveAlignLayer)
		{
				RaycastHit hit;

				//Debug.DrawRay (pos, direction, Color.red, 1, true);
				if (Physics.Raycast (pos, direction, out hit, rayLength, moveAlignLayer)) {
			
						return hit.normal;
				}
		
				return Vector3.up;
		}


	#region ScreenCapturing

		public static string gameName = "Schlicht";

		public static void captureScreen ()
		{
				captureScreen (string.Empty, 1);
		}
		public static void captureScreen (int superSize)
		{
				captureScreen (string.Empty, superSize);
		}
		public static void captureScreen (string subDirectory)
		{
				captureScreen (subDirectory, 1);
		}

	#endregion


	#region TimeHandling

		public static readonly string dateFormat = "yyyyMMddHHmmssffff";
		//public static readonly string timeOnlyFormat = "hh:mm:ss:ffff";
		public static readonly string timeOnlyFormat = "hh:mm ss :";

		public static string GetTimestamp (DateTime value)
		{
				return value.ToString (dateFormat);
		}

		public static string GetTimeOnly (double millis)
		{
				return GetTimeOnly (getTimeFromMilliseconds (millis));
		}

		public static string GetTimeOnly (DateTime value)
		{
				return value.ToString (timeOnlyFormat);
		}

		public static double GetMillisFromString (string timeInMillis)
		{
				DateTime time = GetTimestampFromString (timeInMillis);
				//Debug.Log ("GetMillisFromString " + timeInMillis + " time: " + time.ToOADate ());
				return time.ToOADate ();
		}

		public static DateTime GetTimestampFromString (string dateAsString)
		{
	
				DateTime parsedDate;
/*				if (DateTime.TryParse (dateAsString, out parsedDate)) {
						return parsedDate;*/

				if (DateTime.TryParseExact (dateAsString, dateFormat,
		                           System.Globalization.CultureInfo.CurrentCulture,
		                           System.Globalization.DateTimeStyles.None, out parsedDate)) {
						return parsedDate;
				} else {
						throw new UnityException ("couldn't parse: " + dateAsString);
				}

				//return DateTime.ParseExact (dateAsString, dateFormat, System.Globalization.CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Subtracts the second from the first one and returns it in Milliseconds
		/// </summary>
		/// <returns>The timestamp diff.</returns>
		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
		public static double getTimestampDiff (DateTime first, DateTime second)
		{
				TimeSpan diff = first.Subtract (second);
				//Debug.Log ("first " + first + " second " + second + " Timestamp diff: " + diff);
				return diff.TotalMilliseconds;
		}

		public static double getTimestampDiff (string first, string second)
		{
				DateTime firstDate = GetTimestampFromString (first);
				DateTime secondDate = GetTimestampFromString (second);

				return getTimestampDiff (firstDate, secondDate);
		}

		public static TimeSpan getTimespanFromMillis (double millis)
		{
				return TimeSpan.FromMilliseconds (millis);
		}

		public static DateTime getTimeFromMilliseconds (double millis)
		{
				return DateTime.FromOADate (millis);
/*				DateTime time = new DateTime ();
				time.AddMilliseconds (millis);
				Debug.Log ("getTimeFromMilliseconds " + time);
				return time;
*/
		}

	#endregion

	#region screenshot

		public static void captureScreen (string subDirectory, int superSize)
		{
				// so far only works during the development mode right out of unity!
				#if UNITY_EDITOR

				// in case the Screenshots should be stored under the "Assets" folder
				//string screenDir = Application.dataPath + Path.DirectorySeparatorChar + "Screenshots";
				string screenDir = "Screenshots";

				string timeStamp = GetTimestamp (System.DateTime.Now);		
				string screenName = gameName + "_" + timeStamp + ".png";


				if (!System.IO.Directory.Exists (screenDir)) {
						System.IO.Directory.CreateDirectory (screenDir);
				}
				
				if (!string.IsNullOrEmpty (subDirectory)) {

						if (!System.IO.Directory.Exists (screenDir + Path.DirectorySeparatorChar + subDirectory)) {
								System.IO.Directory.CreateDirectory (screenDir + Path.DirectorySeparatorChar + subDirectory);
								screenDir = screenDir + Path.DirectorySeparatorChar + subDirectory;
						}
				}

				string filePath = screenDir + Path.DirectorySeparatorChar + screenName;

				Debug.Log ("Captured Screenshot @ " + filePath);
				Application.CaptureScreenshot (filePath, superSize);

				#endif
		}

	#endregion

}
