using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicPoly : MonoBehaviour
{

		[Range(3, 10)]
		public int
				polys = 3;


		private MeshFilter myMeshFilter;
		private MeshCollider myMeshCollider;

		// Use this for initialization
		void Awake ()
		{
				init ();
		}

		public void init ()
		{
				Mesh newMesh = createPolyMesh (polys);
				myMeshFilter = gameObject.GetComponent<MeshFilter> ();
				myMeshCollider = gameObject.GetComponent<MeshCollider> ();

				Debug.Log ("newMesh v: " + newMesh.vertices.Length + " t: " + newMesh.triangles.Length + " u: " + newMesh.uv.Length);
		
				newMesh.RecalculateNormals ();
				//newMesh.RecalculateBounds ();
				newMesh.Optimize ();
		
				myMeshFilter.sharedMesh = newMesh;
		}

		private Mesh createPolyMesh (int polys = 3)
		{
				Mesh mesh = new Mesh ();
				mesh.hideFlags = HideFlags.HideAndDontSave;		

				mesh.vertices = MeshGenerator.getPolyVertices (this.transform.position, 1, polys);
				List<int> tris = MeshGenerator.getPolyTriangles (mesh.vertices);
				List<Vector2> fieldUV = MeshGenerator.getUVPerFace (polys);

				//Debug.Log ("got " + tris.Count + " tris");

				mesh.triangles = tris.ToArray ();
				mesh.uv = fieldUV.ToArray ();

				return mesh;
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}


}
