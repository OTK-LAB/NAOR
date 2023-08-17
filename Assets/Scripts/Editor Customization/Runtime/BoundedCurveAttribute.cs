using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoundedCurveAttribute : PropertyAttribute
{
    public Rect bounds;
    public int height;

    public BoundedCurveAttribute(float x, float y, float xMax, float yMax, int height = 1)
    {
        this.bounds = new Rect(x, y, xMax, yMax);
        this.height = height;
    }

    public BoundedCurveAttribute(int height = 1)
    {
        this.bounds = new Rect(0, 0, 1, 1);
        this.height = height;
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(BoundedCurveAttribute))]
public class BoundedCurveDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        BoundedCurveAttribute boundedCurve = (BoundedCurveAttribute)attribute;
        return EditorGUIUtility.singleLineHeight * boundedCurve.height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        BoundedCurveAttribute boundedCurve = (BoundedCurveAttribute)attribute;

        EditorGUI.BeginProperty(position, label, property);
        property.animationCurveValue = EditorGUI.CurveField(
          position,
          label,
          property.animationCurveValue,
          Color.white,
          boundedCurve.bounds
         );
        EditorGUI.EndProperty();
    }
}
#endif
