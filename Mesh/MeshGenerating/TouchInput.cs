using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour
{

		public float rayLength;
		public LayerMask fieldMask;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
		}

		void OnGUI ()
		{
				ProcessClick ();
		}

		private void ProcessClick ()
		{
				Event e = Event.current;
		
				if (e != null && e.type == EventType.MouseDrag) {

						// Event mouse data is switched...
						//Vector3 clickPos = new Vector3 (e.mousePosition.x, e.mousePosition.y, Camera.main.transform.position.z);

						Vector3 clickPos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);

						Ray camRay = Camera.main.ScreenPointToRay (clickPos);
						//Vector3 worldPos = Camera.main.ScreenToWorldPoint (clickPos);
						//Vector3 startRay = new Vector3 (camPos.x, camPos.y, Camera.main.transform.position.z);


						//Debug.DrawRay (camRay.origin, camRay.direction, Color.red, 1);
						//Debug.DrawLine (camRay.origin, camRay.direction * rayLength, Color.yellow, 1);
						//Debug.DrawLine (camRay.origin, camRay.direction, Color.yellow, 1);

						//Debug.Log ("line from: " + camRay.origin + " to: " + camRay.direction * rayLength);
			
						RaycastHit hit;
			
						if (Physics.Raycast (camRay.origin, camRay.direction,
			                     out hit, rayLength, fieldMask)) {
				
								hit.collider.GetComponent<CubeField> ().HitMesh (hit);
						}

				}

		}
}

