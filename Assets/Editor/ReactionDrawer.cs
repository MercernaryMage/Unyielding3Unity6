using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Reaction))]
public class ReactionDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		Reaction r = (Reaction)property.boxedValue;
		if (r.isAggravatged)
		{
			label.text = r.aggravatedReaction.GetCardName(); ;
		}
		else
		{
			label.text = r.normalReaction.GetCardName(); ;
		}

		EditorGUI.PropertyField(position, property, label, true);
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}
}
