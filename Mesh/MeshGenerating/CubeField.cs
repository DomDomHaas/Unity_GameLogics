using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ColorPalette;

/**
* Simple example of creating a procedural 6 sided cube
*/
[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
public class CubeField : MonoBehaviour
{
		[Range(1, 6)]
		public int
				sides = 3;

		public bool ensureTop = true;

		[Range(0.1f, 1f)]
		public float
				cubeWidth = 1;

/*		[Range(0.1f, 1f)]
		public int
				cubeHeight;*/

		[Range(0f, 50f)]
		public float
				heightToWidthRatio = 1.5f;

		public bool usePalette = false;
		public string paletteName = "";
		private PaletteData palette;
		private PaletteCollection collection;

		private MeshFilter myMeshFilter;
		private MeshCollider myMeshCollider;

		public bool createMeshField = false;
		private int cubeCount = 0;

		[Range(2, 50)]
		public int
				fieldWidth = 5;

		[Range(2, 50)]
		public int
				fieldHeight = 5;

		public Texture2D imageAsBase;


		void Awake ()
		{
				init ();
		}

		public void init ()
		{
				if (usePalette) {
						this.palette = loadPalette (this.paletteName);
						//Debug.Log ("found " + this.paletteName + " palette: " + this.palette);
				}

				myMeshFilter = gameObject.GetComponent<MeshFilter> ();
				myMeshCollider = gameObject.GetComponent<MeshCollider> ();
				//renderer.material = new Material (Shader.Find ("Diffuse"));


				if (imageAsBase && createMeshField) {
						createMeshFromImage (imageAsBase);
				} else {
						RecalMesh ();
				}
		}

		public PaletteData loadPalette (string paletteName)
		{
				collection = GameObject.FindWithTag ("ColorPaletteCollection").GetComponent<PaletteCollection> ();
				PaletteCollectionData collData = collection.GetCollectionData ();



				if (!string.IsNullOrEmpty (paletteName)) {
						//Debug.Log ("how many palettes: " + collData.palettes.Count);
						if (collData.palettes.ContainsKey (paletteName)) {
								return collData.palettes [paletteName];
						}
				}

				return null;
		}

		public void RecalMesh ()
		{
				Mesh mesh = new Mesh ();
				mesh.hideFlags = HideFlags.HideAndDontSave;

				int faces = sides;

				if (this.createMeshField) {

						List<Vector3> fieldVertices = new List<Vector3> ();
						List<int> fieldTriangles = new List<int> ();
						List<Vector2> fieldUV = new List<Vector2> ();
						int nextCubeIndex = 0;
						cubeCount = 0;

						for (int i = 0; i < this.fieldWidth; i++) {

								for (int j = 0; j < this.fieldHeight; j++) {

										// TriCube returns 12 vertices (12 x Vector3)
										Vector3[] vertices = getRandomTriCube (this.cubeWidth * i, this.cubeWidth * j, ensureTop);
					
										fieldVertices.AddRange (vertices);
										fieldTriangles.AddRange (MeshGenerator.getTrianglesPerFace (faces, nextCubeIndex));
										fieldUV.AddRange (getUVs (faces));

										nextCubeIndex += vertices.Length;
										cubeCount++;
								}
						}

						mesh.vertices = fieldVertices.ToArray ();
						mesh.triangles = fieldTriangles.ToArray ();
						mesh.uv = fieldUV.ToArray ();

						//Debug.Log ("fieldVertices " + fieldVertices.Count + " fieldTriangles " + fieldTriangles.Count + " fieldUV " + fieldUV.Count);

				} else {

						if (faces == 3) {
								mesh.vertices = getTriCube (this.heightToWidthRatio, 0, 0, ensureTop);
						} else {
								mesh.vertices = MeshGenerator.getFullCube (cubeWidth, this.heightToWidthRatio);
						}

						mesh.triangles = MeshGenerator.getTrianglesPerFace (faces).ToArray ();
						mesh.uv = getUVs (faces).ToArray ();
				}


		
				changeMesh (mesh);
		}

		private void createMeshFromImage (Texture2D tex)
		{
				Mesh mesh = new Mesh ();
				mesh.hideFlags = HideFlags.HideAndDontSave;

				int faces = sides;
				//cubeWidth = 1f / tex.width;
		
				List<Vector3> fieldVertices = new List<Vector3> ();
				List<int> fieldTriangles = new List<int> ();

				List<Vector2> fieldUV = new List<Vector2> ();
				int nextCubeIndex = 0;
				cubeCount = 0;
			
				for (int x = 0; x < tex.width; x++) {
				
						for (int y = 0; y < tex.height; y++) {

								Color col = tex.GetPixel (x, y);

								Vector3[] vertices = getTriCubeFromPixel (col, x, y);
					
								fieldVertices.AddRange (vertices);
								fieldTriangles.AddRange (MeshGenerator.getTrianglesPerFace (faces, nextCubeIndex));
								fieldUV.AddRange (getUVs (faces));
					
								nextCubeIndex += vertices.Length;
								cubeCount++;
						}
				}

			
				mesh.vertices = fieldVertices.ToArray ();
				mesh.triangles = fieldTriangles.ToArray ();
				mesh.uv = fieldUV.ToArray ();



				changeMesh (mesh);
		}


		private Vector3[] getTriCubeFromPixel (Color col, float xOffset = 0, float zOffset = 0)
		{
/*				float h;
				float s;
				float v;

				UnityEditor.EditorGUIUtility.RGBToHSV (col, out h, out s, out v);
*/
				//Debug.Log ("px " + xOffset + "," + zOffset + " " + col);
				return getTriCube (col.b * 10, xOffset, zOffset);
		}

		private void changeMesh (Mesh newMesh)
		{
				newMesh.RecalculateNormals ();
				newMesh.RecalculateBounds ();
				newMesh.Optimize ();

				if (createMeshField) {
						newMesh.name = "generated w: " + (this.cubeWidth * this.fieldWidth) + " h: " + (this.cubeWidth * this.heightToWidthRatio * this.fieldWidth);
				} else {
						newMesh.name = "generated w: " + this.cubeWidth + " h: " + (this.cubeWidth * this.heightToWidthRatio);
				}

				//Debug.Log ("palette " + this.palette.ToString ());

				if (usePalette && this.palette != null) {

						Color[] colors = new Color[newMesh.vertices.Length];

						for (int i = 0; i < newMesh.vertices.Length; i++) {
								if (i % 12 >= 8) {
										colors [i] = this.palette.colors [0];
								} else {
										colors [i] = Color.black;
								}
						}
			
						newMesh.colors = colors;
				}

				if (myMeshCollider != null) {
						myMeshCollider.sharedMesh = getMeshOfTopsOnly (newMesh);
				}
		
				myMeshFilter.sharedMesh = newMesh;
		}

/*		private List<int> getTriangles (int faces, int startIndex = 0)
		{
				List<int> triangles = new List<int> ();
		
				for (int i = 0; i < faces; i++) {
						int triangleOffset = i * 4;
						triangles.Add (startIndex + 0 + triangleOffset);
						triangles.Add (startIndex + 2 + triangleOffset);
						triangles.Add (startIndex + 1 + triangleOffset);
						triangles.Add (startIndex + 0 + triangleOffset);
						triangles.Add (startIndex + 3 + triangleOffset);
						triangles.Add (startIndex + 2 + triangleOffset);
				}

				return triangles;
		}

		private List<int> getTrianglesCollider (int faces, int startIndex = 0)
		{
				List<int> triangles = new List<int> ();
		
				for (int i = 0; i < faces; i++) {
						int triangleOffset = i * 4;
						triangles.Add (startIndex + 0 + triangleOffset);
						triangles.Add (startIndex + 2 + triangleOffset);
						triangles.Add (startIndex + 1 + triangleOffset);
						triangles.Add (startIndex + 0 + triangleOffset);
						triangles.Add (startIndex + 3 + triangleOffset);
						triangles.Add (startIndex + 2 + triangleOffset);
				}
		
				return triangles;
		}
*/
		private Mesh getMeshOfTopsOnly (Mesh visualMesh)
		{
				Mesh colliderMesh = new Mesh ();

				if (visualMesh.vertices.Length > 0) {

						List<Vector3> collVertices = new List<Vector3> ();
						List<int> triangles = new List<int> ();
			
						//Debug.Log ("vertices: " + visualMesh.vertices.Length + " colors " + visualMesh.colors.Length);

						for (int i = 0; i < visualMesh.vertices.Length; i++) {
								// expecting the 8 - 11 to be the top!
								if (i % 12 >= 8) {
										//Debug.Log (" added vertex " + i + " " + (i % 12));
										collVertices.Add (visualMesh.vertices [i]);
								}
						}

						for (int i = 0; i < cubeCount; i++) {
								triangles.AddRange (MeshGenerator.getTrianglesPerFace (1, i * 4));
						}
			
						colliderMesh.vertices = collVertices.ToArray ();
						//Debug.Log (" triangles " + triangles.Count);
						colliderMesh.triangles = triangles.ToArray ();
				}


				return colliderMesh;
		}
	
		private List<Vector2> getUVs (int faces)
		{
				List<Vector2> uvs = new List<Vector2> ();
		
				for (int i = 0; i < faces; i++) {
			
						// same uvs for all faces
						uvs.Add (new Vector2 (0, 0));
						uvs.Add (new Vector2 (1, 0));
						uvs.Add (new Vector2 (1, 1));
						uvs.Add (new Vector2 (0, 1));
				}

				return uvs;
		}

		private Vector3[] getRandomTriCube (float xOffset = 0, float zOffset = 0, bool withTop = true)
		{
				float randRatio = Random.Range (0, this.heightToWidthRatio);
				return getTriCube (randRatio, xOffset, zOffset, withTop);
		}


		private Vector3[] getTriCube (float heightToWidthRatio, float xOffset = 0, float zOffset = 0, bool withTop = true)
		{
				float cubeHeight = cubeWidth * heightToWidthRatio;
				//Debug.Log ("xOffset " + xOffset + " zOffset " + zOffset);

				if (withTop) {
						return new Vector3[]{
						// face 1 (xy plane, z=0)
							new Vector3 (xOffset, 0, zOffset),
							new Vector3 (cubeWidth + xOffset, 0, zOffset),
							new Vector3 (cubeWidth + xOffset, cubeHeight, zOffset),
							new Vector3 (xOffset, cubeHeight, zOffset),
						// face 2 (zy plane, x=1)
							new Vector3 (cubeWidth + xOffset, 0, zOffset),
							new Vector3 (cubeWidth + xOffset, 0, cubeWidth + zOffset),
							new Vector3 (cubeWidth + xOffset, cubeHeight, cubeWidth + zOffset),
							new Vector3 (cubeWidth + xOffset, cubeHeight, zOffset),

						//top  face 5 (zx plane, y=1)
							new Vector3 (xOffset, cubeHeight, zOffset),
							new Vector3 (cubeWidth + xOffset, cubeHeight, zOffset),
							new Vector3 (cubeWidth + xOffset, cubeHeight, cubeWidth + zOffset),
							new Vector3 (xOffset, cubeHeight, cubeWidth + zOffset),
						};
				} else {
						return new Vector3[]{
						// face 1 (xy plane, z=0)
							new Vector3 (xOffset, 0, 0),
							new Vector3 (cubeWidth, 0, 0),
							new Vector3 (cubeWidth, cubeHeight, 0),
							new Vector3 (xOffset, cubeHeight, 0),
						// face 2 (zy plane, x=1)
							new Vector3 (cubeWidth + xOffset, 0, 0),
							new Vector3 (cubeWidth + xOffset, 0, cubeWidth + zOffset),
							new Vector3 (cubeWidth + xOffset, cubeHeight, cubeWidth + zOffset),
							new Vector3 (cubeWidth + xOffset, cubeHeight, 0),

						// bottom face 6 (zx plane, y=0)
							new Vector3 (xOffset, 0, 0),
							new Vector3 (xOffset, 0, cubeWidth + zOffset),
							new Vector3 (cubeWidth + xOffset, 0, cubeWidth + zOffset),
							new Vector3 (cubeWidth + xOffset, 0, 0),
						};
				}
		}
	

		public void HitMesh (RaycastHit hit)
		{
				Debug.Log ("got a hit on triangle: " + hit.triangleIndex);
		}

}

