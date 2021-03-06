﻿using UnityEngine;
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

	
		public static List<GameObject> getChildrenWithTag (GameObject parent, string tag)
		{
				List<GameObject> children = new List<GameObject> ();
		
				for (int i = 0; i < parent.transform.childCount; i++) {
						Transform child = parent.transform.GetChild (i);
						if (child.gameObject.tag.Equals (tag)) {
								children.Add (child.gameObject);	
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
	
		public static Transform[] GetOnlyChildren (this Transform trans, bool getRecursive = true)
		{
				Transform[] children = trans.GetComponentsInChildren<Transform> ();
				//Debug.Log ("GetOnlyChildren initial length " + children.Length);
				//Debug.Log ("GetOnlyChildren recursive: " + children.Length + " parts and direct: " + trans.childCount);

		
				if (children.Length > 1) {
						List<Transform> exceptMySelf = new List<Transform> ();
						int newIndex = 0;

						for (int i = 0; i < children.Length; i++) {
								Transform child = children [i];

								if (child != trans) {

										if (getRecursive) {
												exceptMySelf.Add (child);
												newIndex++;
										} else if (!getRecursive && child.parent == trans) {
												//Debug.Log (i + " child.parent == trans " + (child.parent == trans));
												//only allow direct childrend
												exceptMySelf.Add (child);
												newIndex++;
										}
								}
						}
			
						//Debug.Log ("GetOnlyChildren return length " + exceptMySelf.Length);
						return exceptMySelf.ToArray ();
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

	#region SpriteMethods


		/// <summary>
		/// Gets a sprite from any path
		/// </summary>
		/// <returns>The sprite from WW.</returns>
		/// <param name="fullPath">Full path.</param>
		public static Sprite getSpriteFromWWW (string fullPath, bool packTight = false)
		{
				fullPath = "file:///" + fullPath;
				WWW wwwLoader = new WWW (fullPath);
				//Debug.Log ("loading texture " + fullPath + " via www, loaded: " + wwwLoader.bytes.Length + " bytes");
				Rect rect = new Rect (0, 0, wwwLoader.texture.width, wwwLoader.texture.height);
				SpriteMeshType meshType = SpriteMeshType.FullRect;
				if (packTight) {
						meshType = SpriteMeshType.Tight;
				}
		
				// use 100f to scale down
				Sprite spr = Sprite.Create (wwwLoader.texture, rect, new Vector2 (0.5f, 0.5f), 100f, 0, meshType);
				spr.name = fullPath.Substring (fullPath.LastIndexOf ("/") + 1);
				return spr;
		}

		public static Mesh getMeshFromSprite (Sprite spr)
		{
				Vector2[] spriteVerts = UnityEditor.Sprites.DataUtility.GetSpriteMesh (spr, false);
				ushort[] spriteIndices = UnityEditor.Sprites.DataUtility.GetSpriteIndices (spr, false);
				List<Vector3> meshVerts = new List<Vector3> ();
				List<int> meshIndices = new List<int> ();
				foreach (Vector2 v2 in spriteVerts) {
						meshVerts.Add (new Vector3 (v2.x, v2.y, 0));
				}
				foreach (ushort indice in spriteIndices) {
						meshIndices.Add ((int)indice);
				}
				Mesh spriteMesh = new Mesh ();
				spriteMesh.vertices = meshVerts.ToArray ();
				spriteMesh.triangles = meshIndices.ToArray ();
		
				return spriteMesh;
		}

/*		public static string getSpritePath (Sprite spr, string pattern)
		{

				Directory.GetFiles (Application.dataPath, pattern);
				return "";
		}
*/
		public static float WidthUnits (Sprite spr)
		{
				Debug.Log ("w: " + spr.rect.width + " x: " + spr.bounds.size.x + " = " + (spr.rect.width / spr.bounds.size.x));
				return spr.rect.width / spr.bounds.size.x;
		}

		public static float HeightUnits (Sprite spr)
		{
				Debug.Log ("h: " + spr.rect.height + " y: " + spr.bounds.size.y + " = " + (spr.rect.height / spr.bounds.size.y));
				return spr.rect.height / spr.bounds.size.y;
		}

	#endregion

		public static Vector3 RandomCircleFixY (Vector3 center, float radius)
		{
				float angle = UnityEngine.Random.value * 360;
				Vector3 pos;

				pos.x = center.x + radius * Mathf.Sin (angle * Mathf.Deg2Rad);

				//if (fixY) {
				pos.y = center.y;
/*				} else {
						pos.y = center.y + radius * Mathf.Cos (angle * Mathf.Deg2Rad);
				}*/

				pos.z = center.z + radius * Mathf.Cos (angle * Mathf.Deg2Rad);

				return pos;
		}

		public static Vector3 RandomCircleFixZ (Vector3 center, float radius)
		{
				float angle = UnityEngine.Random.value * 360;
				Vector3 pos;
		
				pos.x = center.x + radius * Mathf.Sin (angle * Mathf.Deg2Rad);		
				pos.y = center.y + radius * Mathf.Cos (angle * Mathf.Deg2Rad);
				pos.z = center.z;

				return pos;
		}


		public static int getRandomWithException (int from, int to, int exception)
		{
				int newRandom = UnityEngine.Random.Range (from, to);
		
				while (exception == newRandom) {
						newRandom = UnityEngine.Random.Range (from, to);
				}
		
				return newRandom;
		}

/*		public static int getRandomWithException (int from, int to, List<int> exceptions)
		{
				int newRandom = UnityEngine.Random.Range (from, to);
		
				while (exception == newRandom) {
						newRandom = UnityEngine.Random.Range (from, to);
				}
		
				return newRandom;
		}*/

}
