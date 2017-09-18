#if UNITY_EDITOR

using UnityEngine;
using System;
using System.Collections;
using System.IO;


public class ScreenshotSequencer : MonoBehaviour
{

  #region Fields

  [Header("Which Key Trigger the capturing?")]
  public KeyCode CaptureKey = KeyCode.P;

  [Header("See Tooltip for details!")]
  [Tooltip("Check to render a specific size of the 'Game Viewport' which isn't bigger than your screen. TRUE: Quality is ignored and the GameViewport is used, FALSE: Screenshot size is defined via 'Quality'")]
  public bool CaptureFixGameViewport = false;
  protected bool DelayCaptureInPostRender = false;

  //Quality = 20 seems to be the Maximum, might be different depending on the grafic card
  [Header("5 is roughly ~5000px width, so it's ~ Quality * 1000px")]
  [Range(1, 20)]
  public int
    Quality = 5;

  [Header("How many Screenshot in sequence?")]
  [Range(0, 25)]
  public int
    HowManyScreenShots = 1;

  [Header("How long to wait between screenshots")]
  [Range(0, 5)]
  public float
    DelaysBetweenShots = 0.1f;

  private string _TempTimeStamp = "";

  private string _TempScreenshotName = "";
  private Texture2D _TempTex;

  private string ScreenshotDirectory = "Screenshots";

  private string _TempFilePath = "";

  #endregion Fields

  #region Methods

  public string GetTimestamp(DateTime value, string dateFormat = "yyyyMMddHHmmssffff")
  {
    return value.ToString(dateFormat);
  }

  public void captureScreen()
  {
    captureScreen(string.Empty, this.Quality);
  }

  public void captureScreen(int superSize = 5)
  {
    captureScreen(string.Empty, superSize);
  }

  public void captureScreen(string subDirectory)
  {
    captureScreen(subDirectory, this.Quality);
  }

  public void captureScreen(string subDirectory, int superSize)
  {
    if (CaptureFixGameViewport && !DelayCaptureInPostRender) {
      DelayCaptureInPostRender = true;
      return;
    }
    // in case the Screenshots should be stored under the "Assets" folder
    //string screenDir = Application.dataPath + Path.DirectorySeparatorChar + ScreenshotDirectory;

    _TempTimeStamp = GetTimestamp(System.DateTime.Now);
    _TempScreenshotName = UnityEditor.PlayerSettings.productName + "_" + _TempTimeStamp + ".png";


    if (!System.IO.Directory.Exists(ScreenshotDirectory)) {
      System.IO.Directory.CreateDirectory(ScreenshotDirectory);
    }

    if (!string.IsNullOrEmpty(subDirectory)) {

      if (!System.IO.Directory.Exists(ScreenshotDirectory + Path.DirectorySeparatorChar + subDirectory)) {
        System.IO.Directory.CreateDirectory(ScreenshotDirectory + Path.DirectorySeparatorChar + subDirectory);
        ScreenshotDirectory = ScreenshotDirectory + Path.DirectorySeparatorChar + subDirectory;
      }
    }

    _TempFilePath = ScreenshotDirectory + Path.DirectorySeparatorChar + _TempScreenshotName;

    if (DelayCaptureInPostRender) {

      _TempTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
      _TempTex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
      _TempTex.Apply();

      byte[] bytes = _TempTex.EncodeToPNG();
      File.WriteAllBytes(_TempFilePath, bytes);

      Debug.Log("Captured Screenshot @ " + _TempFilePath + " with + " + bytes.Length + " bytes");


    } else {
      Debug.Log("Captured Screenshot @ " + _TempFilePath);
      Application.CaptureScreenshot(_TempFilePath, superSize);
    }

    if (CaptureFixGameViewport) {
      DelayCaptureInPostRender = false;
    }
  }

  void Start()
  {
    if (this.CaptureFixGameViewport) {
      Debug.Assert(this.GetComponent<Camera>() == null,
        "CaptureInPostRender == TRUE only works when the script is on a Camera!");
    }
  }

  void LateUpdate()
  {
    if (Input.GetKeyDown(CaptureKey)) {
      if (HowManyScreenShots == 1) {
        captureScreen(this.Quality);
      } else {
        StartCoroutine(ScreenshotSequence());
      }
    }
  }

  public void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (this.CaptureFixGameViewport && this.DelayCaptureInPostRender) {
      captureScreen(this.Quality);
    }
  }


  private IEnumerator ScreenshotSequence()
  {
    YieldInstruction w8 = new WaitForSeconds(this.DelaysBetweenShots);
    for (int i = 0; i < this.HowManyScreenShots; i++) {

      captureScreen(this.Quality);
      yield return w8;
    }

  }




  #endregion Methods
}

#endif