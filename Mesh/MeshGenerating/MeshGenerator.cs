using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public static class MeshGenerator
{


		public static List<int> getTrianglesPerFace (int faces, int startIndex = 0)
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

		public static List<int> getPolyTriangles (int faces, int startIndex = 0)
		{
				List<int> triangles = new List<int> ();
		
				for (int i = 0; i < faces; i++) {
						int triangleOffset = i * 1;
						triangles.Add (startIndex + 0 + triangleOffset);
						triangles.Add (startIndex + 2 + triangleOffset);
						triangles.Add (startIndex + 1 + triangleOffset);
				}
		
				return triangles;
		}

	
		public static List<int> getPolyTriangles (Vector3[] vertices)
		{
				List<int> triangles = new List<int> ();

				//for (int i = 0; i < vertices.Length; i++) {
				//triangles.AddRange (getPolyTriangles (vertices.Length - 2, i * 4));
				triangles.AddRange (getPolyTriangles (vertices.Length - 2));
				//}

				Debug.Log ("expected faces " + (vertices.Length - 2) + " tris: " + triangles.Count);

				return triangles;
		}

		public static Vector3[] getFullCube (float cubeWidth, float heightToWidthRatio)
		{
				float cubeHeight = cubeWidth * heightToWidthRatio;
		
				return new Vector3[]{
				// face 1 (xy plane, z=0)
						new Vector3 (0, 0, 0),
						new Vector3 (cubeWidth, 0, 0),
						new Vector3 (cubeWidth, cubeHeight, 0),
						new Vector3 (0, cubeHeight, 0),
				// face 2 (zy plane, x=1)
						new Vector3 (cubeWidth, 0, 0),
						new Vector3 (cubeWidth, 0, cubeWidth),
						new Vector3 (cubeWidth, cubeHeight, cubeWidth),
						new Vector3 (cubeWidth, cubeHeight, 0),
				// face 3 (xy plane, z=1)
						new Vector3 (cubeWidth, 0, cubeWidth),
						new Vector3 (0, 0, cubeWidth),
						new Vector3 (0, cubeHeight, cubeWidth),
						new Vector3 (cubeWidth, cubeHeight, cubeWidth),
				// face 4 (zy plane, x=0)
						new Vector3 (0, 0, cubeWidth),
						new Vector3 (0, 0, 0),
						new Vector3 (0, cubeHeight, 0),
						new Vector3 (0, cubeHeight, cubeWidth),
				// face 5 (zx plane, y=1)
						new Vector3 (0, cubeHeight, 0),
						new Vector3 (cubeWidth, cubeHeight, 0),
						new Vector3 (cubeWidth, cubeHeight, cubeWidth),
						new Vector3 (0, cubeHeight, cubeWidth),				
				// bottom face 6 (zx plane, y=0)
						new Vector3 (0, 0, 0),
						new Vector3 (0, 0, cubeWidth),
						new Vector3 (cubeWidth, 0, cubeWidth),
						new Vector3 (cubeWidth, 0, 0),
					};
		
		}


		public static Vector3[] getPolyVertices (Vector3 origin, float distance, int polys)
		{
				if (polys <= 3) {
						return new Vector3[]{
						origin + new Vector3 (0, 0, 0),
						origin + new Vector3 (distance, 0, 0),
						origin + new Vector3 (distance, distance, 0)
					};
				} else {
						List<Vector3> vertices = new List<Vector3> ();
						for (int i = 0; i < polys; i++) {
								vertices.Add (origin + new Vector3 (distance * Mathf.Cos (2 * Mathf.PI * i / polys),
				                                  distance * Mathf.Sin (2 * Mathf.PI * i / polys), 0));
						}

						return vertices.ToArray ();
				}
	
		}

		public static List<Vector2> getUVPerFace (int faces)
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

}
