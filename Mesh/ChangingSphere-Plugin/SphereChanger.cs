using UnityEngine;
using System.Collections;

public class SphereChanger : MonoBehaviour
{
		public bool useNoise = false;
		public int octave = 3;
		public float amplitude = 0.2f;
		public float speed = 1.0f;
		public float scale = 4.0f;

		MeshFilter meshFilter;

		private Vector3[] originalVertices;


		void Awake ()
		{
				meshFilter = GetComponent<MeshFilter> ();
		
				if (meshFilter.sharedMesh != null) {
						Destroy (meshFilter.sharedMesh);
				}

				var builder = new Emgen.IcosphereBuilder ();
				builder.Subdivide ();
				
/*				builder.Subdivide ();
				builder.Subdivide ();
				builder.Subdivide ();
*/
				BuildAndReplaceMesh (builder);

				meshFilter.sharedMesh = builder.vertexCache.BuildFlatMesh ();

				// TODO smooth
				originalVertices = meshFilter.sharedMesh.vertices;
		}


		// Update is called once per frame
		void Update ()
		{
				if (renderer.isVisible) {
						displaceMesh ();
				}
				//drawNormals ();
		}


		void drawNormals ()
		{
				var mesh = meshFilter.sharedMesh;
				var normals = mesh.vertices;
				string normalsStr = "";
				for (var i = 0; i < 10 /*normals.Length*/; i++) {
						var n = normals [i];
						normalsStr += " " + n;
						Debug.DrawLine (n, n + Vector3.one, Color.red);
				}
				//Debug.Log ("normals: " + normalsStr);
		}

		void displaceMesh ()
		{
				var mesh = meshFilter.sharedMesh;
				var vertices = mesh.vertices;

				for (var i = 0; i < vertices.Length; i++) {
						var v = vertices [i];
						//var nc = new Vector3 (v.x * scale, v.y * scale, v.z * scale) * Time.time * speed;
						var nc = new Vector3 (v.x * scale, v.y * scale, v.z * scale) + Vector3.one * Time.time * speed;
						float perlinThingy = 0;
						if (useNoise) {
								perlinThingy = Perlin.Noise (nc) * amplitude;
						} else {
								perlinThingy = Perlin.Fbm (nc, octave) * amplitude;
						}

						vertices [i].x = this.originalVertices [i].x + perlinThingy;
						vertices [i].y = this.originalVertices [i].y + perlinThingy;
						vertices [i].z = this.originalVertices [i].z + perlinThingy;
				}

				mesh.vertices = vertices;
				mesh.RecalculateNormals ();
		}


		void BuildAndReplaceMesh (Emgen.IcosphereBuilder builder)
		{
				var meshFilter = GetComponent<MeshFilter> ();
			
				if (meshFilter.sharedMesh != null) {
						Destroy (meshFilter.sharedMesh);
				}

				meshFilter.sharedMesh = builder.vertexCache.BuildFlatMesh ();
		}

}
