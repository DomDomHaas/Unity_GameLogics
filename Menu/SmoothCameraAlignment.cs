using System.Collections;
using UnityEngine;


  public class SmoothCameraAlignment : MonoBehaviour
  {
    public bool AlignOnX = true;
    public bool AlignOnY = true;

    [Range(-1, 1)]
    public float DirectionMultiplierX = 1;

    [Range(-1, 1)]
    public float DirectionMultiplierY = 1;

    private float currentVeloX;
    private float currentVeloY;

    private float tempX;
    private float tempY;

    public float smoothness = 0.5f;

    private void FixedUpdate ()
    {
      if (AlignOnX) {
        tempX = Mathf.SmoothDampAngle(transform.eulerAngles.x, Camera.main.transform.eulerAngles.x * DirectionMultiplierX, ref currentVeloX, smoothness);
      } else {
        tempX = transform.eulerAngles.x;
      }

      if (AlignOnY) {
        tempY = Mathf.SmoothDampAngle(transform.eulerAngles.y, Camera.main.transform.eulerAngles.y * DirectionMultiplierY, ref currentVeloY, smoothness);
      } else {
        tempY = transform.eulerAngles.y;
      }

      transform.eulerAngles = new Vector3(tempX, tempY, 0);
    }
  }