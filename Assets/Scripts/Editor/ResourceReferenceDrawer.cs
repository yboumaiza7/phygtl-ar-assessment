#region Namespaces

using Phygtl.ARAssessment.Core;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

#endregion

namespace Phygtl.ARAssessment.Editor
{
    [CustomPropertyDrawer(typeof(ResourceReference<>))]
    internal class ResourceReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
			EditorGUI.BeginProperty(position, label, property);

			var pathProperty = property.FindPropertyRelative("path");
			var referenceType = property.GetUnderlyingType().GenericTypeArguments[0];
			var resource = Resources.Load(pathProperty.stringValue, referenceType);
			var newResource = EditorGUI.ObjectField(position, label, resource, referenceType, false);

			if (!newResource)
				// Handle missing resource
				pathProperty.stringValue = string.Empty;
			else if (resource != newResource)
			{
				if (ResourceReference<Object>.ValidateAsset(newResource, out var path))
					pathProperty.stringValue = path;
				else
					AppDebugger.LogWarning($"The object {newResource.name} should be in the Resources folder!", property.serializedObject.targetObject, nameof(ResourceReference<Object>));
			}

			EditorGUI.EndProperty();
        }
    }
}
