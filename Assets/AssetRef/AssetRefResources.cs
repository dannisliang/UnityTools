using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum AssetRefState
{
    None,
    Unload,
    Loaded,
}

[Serializable]
public class AssetRefResources<T>
    where T : UnityEngine.Object
{
    public string AssetPath;
    public string AssetGUID;

    public AssetRefState State
    {
        get
        {
            if (string.IsNullOrEmpty(AssetPath))
                return AssetRefState.None;
            else if (!mAssetObject)
                return AssetRefState.Unload;
            else
                return AssetRefState.Loaded;
        }
    }
    public T AssetObject
    {
        get
        {
            if (null == mAssetObject)
            {
#if UNITY_EDITOR
                ResourcePathRepair();
#endif
                mAssetObject = Resources.Load<T>(AssetPath);
            }
            return mAssetObject;
        }
#if UNITY_EDITOR
        set
        {
            if (null == value)
            {
                AssetPath = string.Empty;
                mAssetObject = null;
                return;
            }

            AssetPath       = ConverPath(AssetDatabase.GetAssetPath(value));
            AssetGUID       = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
            mAssetObject    = value as T;
        }
#endif
    }
    public string ConverPath(string rAssetPath)
    {
        const string rAssetFolderName = "Resources";
        var nFindIndex = rAssetPath.LastIndexOf(rAssetFolderName);
        if (nFindIndex == -1)
            return string.Empty;

        rAssetPath = rAssetPath.Remove(0, nFindIndex + rAssetFolderName.Length + 1);
        nFindIndex = rAssetPath.LastIndexOf('.');
        if (nFindIndex != -1)
            rAssetPath = rAssetPath.Substring(0, nFindIndex);

        return rAssetPath;
    }

    public static implicit operator T (AssetRefResources<T> exists)
    {
        return exists.AssetObject;
    }
    public static implicit operator UnityEngine.Object (AssetRefResources<T> exists)
    {
        return exists.AssetObject;
    }
#if UNITY_EDITOR
    public void ResourcePathRepair()
    {
        if (!string.IsNullOrEmpty(AssetGUID))
        {
            // 资源路劲变更检测
            var rRealAssetPath = ConverPath(AssetDatabase.GUIDToAssetPath(AssetGUID));
            if (!string.IsNullOrEmpty(rRealAssetPath) && rRealAssetPath != AssetPath)
            {
                AssetPath = rRealAssetPath;

                Debug.LogError("资源路径发生了变化～重新保存一下该资源.");
            }
        }
    }
#endif

    public void UnloadAsset()
    {
        Resources.UnloadAsset(mAssetObject);
        mAssetObject = null;
    }

    private T mAssetObject;
}

[Serializable] public class AssetRefTexture2D   : AssetRefResources<Texture2D>  {}
[Serializable] public class AssetRefGameObject  : AssetRefResources<GameObject> {}
