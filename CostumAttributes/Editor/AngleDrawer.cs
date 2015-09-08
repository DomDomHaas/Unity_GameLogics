using UnityEditor;
using UnityEngine;

namespace MrWhaleGames.ColorPalette.Attributes {

  [CustomPropertyDrawer(typeof(AngleAttribute))]
  public class AngleDrawer : PropertyDrawer {

    private static Vector2 mousePosition;
    private static Texture2D KnobBack = null;
    private static Texture2D Knob = null; 
    private static GUIStyle s_TempStyle = new GUIStyle();
    private static float height = 75;

    public AngleDrawer (){
      Knob = ColorPaletteStatics.getTextureFromWWW(ColorPaletteStatics.ColorPaletteEditorFullPath + "/Knob.png");
      KnobBack = ColorPaletteStatics.getTextureFromWWW(ColorPaletteStatics.ColorPaletteEditorFullPath + "/KnobBack.png");
      //Debug.Log("constructor " + this + " " + KnobBack + " " + ColorPaletteStatics.ColorPaletteEditorFullPath + "/Knob.png");
    }

    private AngleAttribute angleAttribute {
      get { return (AngleAttribute)attribute; }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      property.floatValue = FloatAngle(position, property.floatValue, angleAttribute.snap, angleAttribute.min, angleAttribute.max);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      return height;
    }

    public static float FloatAngle(Rect rect, float value) {
      return FloatAngle(rect, value, -1, -1, -1);
    }

    public static float FloatAngle(Rect rect, float value, float snap) {
      return FloatAngle(rect, value, snap, -1, -1);
    }

    public static float FloatAngle(Rect rect, float value, float snap, float min, float max) {


      int id = GUIUtility.GetControlID(FocusType.Native, rect);

      Rect knobRect = new Rect(rect.x, rect.y, rect.height, rect.height);

      float delta;
      if (min != max)
        delta = ((max - min) / 360);
      else
        delta = 1;

      if (Event.current != null) {
        if (Event.current.type == EventType.MouseDown && knobRect.Contains(Event.current.mousePosition)) {
          GUIUtility.hotControl = id;
          mousePosition = Event.current.mousePosition;
        } else if (Event.current.type == EventType.MouseUp && GUIUtility.hotControl == id)
          GUIUtility.hotControl = 0;
        else if (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl == id) {
          Vector2 move = mousePosition - Event.current.mousePosition;
          value += delta * (-move.x - move.y);

          if (snap > 0) {
            float mod = value % snap;

            if (mod < (delta * 3) || Mathf.Abs(mod - snap) < (delta * 3))
              value = Mathf.Round(value / snap) * snap;
          }

          mousePosition = Event.current.mousePosition;
          GUI.changed = true;
        }
      }

      //EditorGUI.DrawTextureAlpha(knobRect, KnobBack);
      GUI.DrawTexture(knobRect, KnobBack);
      Matrix4x4 matrix = GUI.matrix;

      if (min != max)
        GUIUtility.RotateAroundPivot(value * (360 / (max - min)), knobRect.center);
      else
        GUIUtility.RotateAroundPivot(value, knobRect.center);

      //EditorGUI.DrawTextureAlpha(knobRect, Knob);
      GUI.DrawTexture(knobRect, Knob);
      GUI.matrix = matrix;

      Rect label = new Rect(rect.x + rect.height, rect.y + (rect.height / 2) - 9, rect.height, 18);
      value = EditorGUI.FloatField(label, value);

      if (min != max)
        value = Mathf.Clamp(value, min, max);

      return value;
    }

    private static void DrawTexture(Rect position, Texture2D texture) {
      if (Event.current.type != EventType.Repaint)
        return;
      s_TempStyle.normal.background = texture;
      s_TempStyle.Draw(position, GUIContent.none, false, false, false, false);
    }
  }


}