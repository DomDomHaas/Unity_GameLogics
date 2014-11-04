using UnityEngine;
using UnityEditor;
using System.Collections; 
using System.Xml.Serialization; 
using ColorPalette;

[CustomEditor(typeof(CubeField))]
public class CubeFieldInspector : Editor
{ 
		/*
		protected Texture2D plusTex;
		protected Texture2D minusTex;
		protected float height = 100;

		protected bool showPalette = true;
		protected bool changeColors = false;

		protected bool adjustPCTBefore = false;
		protected float minPct = 0.01f;

		protected float paletteHeight = 100;
		protected float paletteTopMargin = 40;
		protected float paletteBotMargin = 20;

		protected float hexFieldWidth = 55;

		protected float colorChangerRowHeight = 20;
		protected float colorChangeLeftMargin = 5;
		protected float colorChangeRightMargin = 20;
		protected float colorChangeMarginBetween = 25;

		protected float buttonMarginBetween = 10;*/

		private CubeField myCube;


		[ExecuteInEditMode]
		public void OnEnable ()
		{
				myCube = target as CubeField;
				//Debug.Log ("onenable");
				myCube.init ();
		}

		[ExecuteInEditMode]
		public void OnDisable ()
		{
				DestroyImmediate (myCube.GetComponent<MeshFilter> ().sharedMesh);
		}
	
		public override void OnInspectorGUI ()
		{    
				// uncomment for debugging
				base.DrawDefaultInspector ();

				if (GUI.changed) {
						myCube.RecalMesh ();
				}

				//EditorUtility.SetDirty (myCube);
		}




}