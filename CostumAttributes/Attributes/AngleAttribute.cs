using UnityEngine;

namespace MrWhaleGames.ColorPalette.Attributes {

  public class AngleAttribute : PropertyAttribute {
    public readonly float snap;
    public readonly float min;
    public readonly float max;

    public AngleAttribute() {
      snap = 1;
      min = -360;
      max = 360;
    }

    public AngleAttribute(float snap) {
      this.snap = snap;
      min = -360;
      max = 360;
    }

    public AngleAttribute(float snap, float min, float max) {
      this.snap = snap;
      this.min = min;
      this.max = max;
    }
  }

}
