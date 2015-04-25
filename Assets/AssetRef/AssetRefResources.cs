using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Object = UnityEngine.Object;

public enum AssetRefState
{
    None,
    Unload,
    Loaded,
}

[Serializable]
public class AssetRefResources<T>
    where T : Object
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
				LoadAsset();
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

	public T LoadAsset()
	{
#if UNITY_EDITOR
		ResourcePathRepair();
#endif
		mAssetObject = Resources.Load<T>(AssetPath);
		return mAssetObject;
	}
	public IEnumerator AsyncLoadAsset()
	{
#if UNITY_EDITOR
		ResourcePathRepair();
#endif
		var rAsyncLoadRequest = Resources.LoadAsync<T> (AssetPath);
		yield return rAsyncLoadRequest;

		mAssetObject = rAsyncLoadRequest.asset as T;
	}

    public void UnloadAsset()
    {
        Resources.UnloadAsset(mAssetObject);
        mAssetObject = null;
    }

    private T mAssetObject;
}

[AttributeUsage(AttributeTargets.Field)]
public class AssetRefInvokeAttribute : Attribute 
{
	public int InvokeID {
		get { return mInvokeID; }
	}

	private int mInvokeID;

	public AssetRefInvokeAttribute(int nInvokeID)
	{
		mInvokeID = nInvokeID;
	}
}

public static class AssetRefExpand
{
	public const string AssetRefResourceTypeNmae = "AssetRefResources`1";
	public delegate UnityEngine.Coroutine StartCoroutineDelegate(IEnumerator rEnumerator);
	public static IEnumerator AsyncLoadAssetRef(this System.Object rObject, int nInvokeID, StartCoroutineDelegate rStartCoroutine)
	{
		if (null == rObject)
			yield break;
		
		var rType 	= rObject.GetType();
		if (rType.BaseType.Name == AssetRefResourceTypeNmae)
		{
			var rMethod = rType.GetMethod("AsyncLoadAsset");
			if (null != rMethod)
				yield return rStartCoroutine((IEnumerator)rMethod.Invoke(rObject, new object[0]));
		}
		foreach(var rField in rType.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))
		{
			if (!rField.IsPublic && !rField.IsDefined(typeof(SerializeField), false))
				continue;
			
			if (-1 != nInvokeID && rField.IsDefined(typeof(AssetRefInvokeAttribute), false) && 
			    (rField.GetCustomAttributes(typeof(AssetRefInvokeAttribute), false)[0] as AssetRefInvokeAttribute).InvokeID != nInvokeID)
			{
				continue;
			}
			
			if (rField.FieldType.BaseType.Name == AssetRefResourceTypeNmae)
			{
				var rMethod = rField.FieldType.GetMethod("AsyncLoadAsset");
				if (null != rMethod)
					yield return rStartCoroutine((IEnumerator)rMethod.Invoke(rField.GetValue(rObject), new object[0]));
			}
			else if (rField.FieldType.GetInterface("System.Collections.IList") != null)
			{
				IList rList = (IList)rField.GetValue(rObject);
				if (null != rList)
				{
					foreach(var rListElement in rList)
					{
						if (null != rListElement && rListElement.GetType ().IsClass)
							yield return rStartCoroutine(AsyncLoadAssetRef(rListElement, nInvokeID, rStartCoroutine));
					}
				}
			}
			else if (rField.FieldType.IsClass)
			{
				yield return rStartCoroutine(AsyncLoadAssetRef(rField.GetValue(rObject), nInvokeID, rStartCoroutine));
			}
		}
	}
	public static void LoadAssetRef(this System.Object rObject, int nInvokeID)
	{
		EnumFieldAndInvoke (rObject, nInvokeID, "LoadAsset");
	}
	public static void UnloadAssetRef(this System.Object rObject, int nInvokeID)
	{
		EnumFieldAndInvoke (rObject, nInvokeID, "UnloadAsset");
	}

	public static void EnumFieldAndInvoke(System.Object rObject, int nInvokeID, string rInvokeName)
	{
		if (null == rObject)
			return;

		var rType 	= rObject.GetType();
		if (rType.BaseType.Name == AssetRefResourceTypeNmae)
		{
			var rMethod = rType.GetMethod(rInvokeName);
			if (null != rMethod)
				rMethod.Invoke(rObject, new object[0]);
			return;
		}
		foreach(var rField in rType.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))
		{
			if (!rField.IsPublic && !rField.IsDefined(typeof(SerializeField), false))
				continue;
			
			if (-1 != nInvokeID && rField.IsDefined(typeof(AssetRefInvokeAttribute), false) && 
			    (rField.GetCustomAttributes(typeof(AssetRefInvokeAttribute), false)[0] as AssetRefInvokeAttribute).InvokeID != nInvokeID)
			{
				continue;
			}
			
			if (rField.FieldType.BaseType.Name == AssetRefResourceTypeNmae)
			{
				var rMethod = rField.FieldType.GetMethod(rInvokeName);
				if (null != rMethod)
					rMethod.Invoke(rField.GetValue(rObject), new object[0]);
			}
			else if (rField.FieldType.GetInterface("System.Collections.IList") != null)
			{
				IList rList = (IList)rField.GetValue(rObject);
				if (null != rList)
				{
					foreach(var rListElement in rList)
					{
						if (null != rListElement && rListElement.GetType ().IsClass)
							EnumFieldAndInvoke(rListElement, nInvokeID, rInvokeName);
					}
				}
			}
			else if (rField.FieldType.IsClass)
			{
				EnumFieldAndInvoke(rField.GetValue(rObject), nInvokeID, rInvokeName);
			}
		}
	}
}

[Serializable] public class AssetRefTexture2D   : AssetRefResources<Texture2D>  {}
[Serializable] public class AssetRefGameObject  : AssetRefResources<GameObject> {}
