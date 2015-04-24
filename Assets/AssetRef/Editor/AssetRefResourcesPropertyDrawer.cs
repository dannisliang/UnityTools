using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEngine;
using UnityEditor;

public class AssetRefResourcesDrawer<T> : PropertyDrawer
    where T : UnityEngine.Object
{
    float LoadBtnWidth = 18;
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
		EditorGUI.BeginProperty (pos, label, prop);

        var rAssetRef = GetAssetRef(prop.serializedObject.targetObject, prop.propertyPath);
        if (null != rAssetRef)
        {
            T       rOldAssetObject = null;
            Rect    rObjectFieldRect = pos;
            if (rAssetRef.State == AssetRefState.Loaded)
                rOldAssetObject = rAssetRef.AssetObject;
            else if (rAssetRef.State == AssetRefState.Unload)
                rObjectFieldRect.width = rObjectFieldRect.width - LoadBtnWidth;

			var rAssetPath = prop.FindPropertyRelative("AssetPath");
			var rAssetGUID = prop.FindPropertyRelative("AssetGUID");

			EditorGUI.BeginChangeCheck ();
            var rNewAssetObject = EditorGUI.ObjectField(rObjectFieldRect, label, rOldAssetObject, typeof(T)
#if UNITY_5_0
                , false
#endif
            );
            if (rAssetRef.State == AssetRefState.Unload)
            {
#if UNITY_5_0
                var rButtonPos = new Rect(rObjectFieldRect.xMax, rObjectFieldRect.yMin, LoadBtnWidth, 14);
#else
                var rButtonPos = new Rect(rObjectFieldRect.right, rObjectFieldRect.top, LoadBtnWidth, 14);
#endif
                if (GUI.Button(rButtonPos, new GUIContent("L", "Load Resource")))
                    rNewAssetObject = rAssetRef.AssetObject;
            }
			if (EditorGUI.EndChangeCheck())
			{
                rAssetRef.AssetObject = rNewAssetObject as T;

				rAssetPath.stringValue = rAssetRef.AssetPath;
				rAssetGUID.stringValue = rAssetRef.AssetGUID;
			}
        }

		EditorGUI.EndProperty ();
    }

    public AssetRefResources<T> GetAssetRef(UnityEngine.Object targetObject, string propertyPath)
    {
        var             rType = targetObject.GetType();
        System.Object   rObject = targetObject;
        var             rPropPath = propertyPath.Split('.');
        for (int nIndex = 0; nIndex < rPropPath.Length; ++ nIndex)
        {
            var rFieldName = rPropPath[nIndex];
            if (rFieldName == "Array" && rPropPath.Length > (nIndex + 1) && rPropPath[nIndex + 1].Contains("data["))
            {
                int nArrayIndex = ParseDataIndex(rPropPath[nIndex + 1]);
                if (nArrayIndex == -1)
                    return default(AssetRefResources<T>);

                rObject = ((System.Collections.IList)rObject)[nArrayIndex];

                nIndex++;
            }
            else
            {
                var rFieldInfo = rType.GetField(rFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (null == rFieldInfo)
                    return default(AssetRefResources<T>);

                rObject = rFieldInfo.GetValue(rObject);
                rType = rFieldInfo.FieldType;
            }
        }

        return rObject as AssetRefResources<T>;
    }
    public int ParseDataIndex(string rPropName)
    {
        return int.Parse(rPropName.Substring(5, rPropName.Length - 6));
    }
}
[CustomPropertyDrawer(typeof(AssetRefTexture2D))]
public class AssetRefTexture2DDrawer : AssetRefResourcesDrawer<Texture2D> {}

[CustomPropertyDrawer(typeof(AssetRefGameObject))]
public class AssetRefGameObjectDrawer : AssetRefResourcesDrawer<GameObject> {}