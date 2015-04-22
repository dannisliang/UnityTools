using System.Collections;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReflectAssists
{
    public static System.Object Constrct(System.Type rType, params object[] param)
    {
        var rConstructor = rType.GetConstructor(new System.Type[0]);
        if (null != rConstructor)
            return rConstructor.Invoke(param);
        else
            return null;
    }
    public static T Constrct<T>(params object[] param) where T : class
    {
        return Constrct(typeof(T), param) as T;
    }
}

public abstract class AssetRef
{
    public enum AssetState
    {
        None,
        Unload,
        Loaded,
    }

    public Type                         AssetType { get { return mAssetType; } }


    public abstract AssetState          State       { get; }
    public abstract string              AssetName   { get; }
    public abstract UnityEngine.Object  AssetObject { get; set;}
    

    public virtual void Serialize(BinaryWriter rWriter)     {}
    public virtual void Deserialize(BinaryReader rReader)   {}

    protected Type  mAssetType;
}

[System.Serializable]
public class TAssetRef<T> : AssetRef
    where T : UnityEngine.Object
{
    protected string    mAssetPath      = string.Empty;
    protected T         mAssetObject;

    public T Asset
    {
        get { return this.AssetObject as T; }
        set { this.AssetObject = value;  }
    }

    public TAssetRef()
    {
        mAssetType = typeof(T);
    }

    #region AssetRef
    public override string AssetName
    {
        get { return mAssetPath; }
    }
    public override AssetState State
    {
        get
        {
            if (string.IsNullOrEmpty(mAssetPath))
                return AssetState.None;
            else if (!mAssetObject)
                return AssetState.Unload;
            else
                return AssetState.Loaded;
        }
    }
    public override UnityEngine.Object AssetObject
    {
        get
        {
            if (null == mAssetObject)
                mAssetObject = LoadAsset();
            return mAssetObject;
        }
#if UNITY_EDITOR
        set
        {
            if (null == value)
            {
                mAssetPath = string.Empty;
                mAssetObject = null;
                return;
            }

            if ( PrefabUtility.GetPrefabType(value) != PrefabType.Prefab ||
                typeof(T).GetType().IsAssignableFrom(value.GetType()) )
                return;

            var rAssetFolderName = "Resources";
            var rAssetPath = AssetDatabase.GetAssetPath(value);
            var nFindIndex = rAssetPath.LastIndexOf("Resources");
            if (nFindIndex == -1)
                return;

            mAssetPath   = Path.GetFileNameWithoutExtension(
                rAssetPath.Remove(0, nFindIndex + rAssetFolderName.Length + 1));
            mAssetObject = value as T;
        }
#endif
    }
    public override void Serialize(BinaryWriter rWriter)
    {
        base.Serialize(rWriter);

        rWriter.Write(mAssetPath);
    }
    public override void Deserialize(BinaryReader rReader)
    {
        base.Deserialize(rReader);

        mAssetPath = rReader.ReadString();
    }
    #endregion

    protected virtual T LoadAsset()
    {
        return UnityEngine.Resources.Load<T>(mAssetPath);
    }
}
