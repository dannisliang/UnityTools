using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

[CustomEditor(typeof(AssetRefBehaviour), true)]
public class AssetRefInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("AssetRef");

        var rAssetRef = this.target as AssetRefBehaviour;
        var rType = this.target.GetType();
        foreach (var rField in rType.GetFields())
        {
            if (!rField.FieldType.FullName.Contains("AssetRef"))
                continue;

            if (rField.IsDefined(typeof(HideInInspector), false))
                continue;

            var rAssetPropInfo  = rField.FieldType.GetProperty("AssetObject");
            var rAssetType      = rField.FieldType.GetGenericArguments()[0];
            var rAssetRefObject = rField.GetValue(rAssetRef) as AssetRef;
            if (null == rAssetRefObject)
            {
                rAssetRefObject = ReflectAssists.Constrct(rField.FieldType) as AssetRef;
                rField.SetValue(rAssetRef, rAssetRefObject);
            }
            UnityEngine.Object rNewAssetObject = null;

            if (rAssetRefObject.State == AssetRef.AssetState.None)
            {
                rNewAssetObject = EditorGUILayout.ObjectField(rField.Name, null, rAssetType);
                if (null != rNewAssetObject)
                    rAssetRefObject.AssetObject = rNewAssetObject;
            }
            else if (rAssetRefObject.State == AssetRef.AssetState.Unload)
            {
                GUILayout.BeginHorizontal();
                rNewAssetObject = EditorGUILayout.ObjectField(rField.Name, null, rAssetType);
                if (null != rNewAssetObject)
                    rAssetRefObject.AssetObject = rNewAssetObject;

                var rTooltip = string.Format("Load {0}.{1} forme {2} asset.",
                    rType.Name, rField.Name, rAssetRefObject.AssetName);
                if (GUILayout.Button(new GUIContent("L", rTooltip), GUILayout.Width(18), GUILayout.Height(14)))
                    rNewAssetObject = rAssetRefObject.AssetObject;
                GUILayout.EndHorizontal();
            }
            else
            {
                rNewAssetObject = EditorGUILayout.ObjectField(rField.Name, 
                    rAssetRefObject.AssetObject, rAssetType);
                if (rNewAssetObject != rAssetRefObject.AssetObject)
                    rAssetRefObject.AssetObject = rNewAssetObject;
            }
        }
    }
}
