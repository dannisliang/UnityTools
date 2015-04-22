using UnityEngine;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

/// <summary>
/// 
/// TODO : 如果成员变量名称或者成员顺序发生变化，都可能产生错误
///     编辑器模式下兼容格式，手机运行使用更加快速的顺序格式。
/// </summary>
public class AssetRefBehaviour : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector]
    private byte[]      mAssetRefData;  
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (null == mAssetRefData || mAssetRefData.Length == 0)
            return;

        using (var rReader = new BinaryReader(new MemoryStream(mAssetRefData)))
        {
            // 顺序格式
            var rBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var rAssetRefType = typeof(AssetRef);
            foreach (var rField in GetType().GetFields(rBindingFlags))
            {
                if (!rField.IsPublic && !rField.IsDefined(typeof(SerializeField), false))
                    continue;

                if (!rAssetRefType.IsAssignableFrom(rField.FieldType))
                    continue;

                AssetRef rAssetRef = null;
                bool bValid = rReader.ReadBoolean();
                if (bValid)
                {
                    rAssetRef = ReflectAssists.Constrct(rField.FieldType) as AssetRef;
                    rAssetRef.Deserialize(rReader);
                }

                rField.SetValue(this, rAssetRef);
            }
        }
    }
    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        var rMemoryStream = new MemoryStream();
        using (var rWriter = new BinaryWriter(rMemoryStream))
        {
            // 顺序格式
            var rBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var rAssetRefType = typeof(AssetRef);
            foreach (var rField in GetType().GetFields(rBindingFlags))
            {
                if (!rField.IsPublic && !rField.IsDefined(typeof(SerializeField), false))
                    continue;

                if (!rAssetRefType.IsAssignableFrom(rField.FieldType))
                    continue;

                var rAssetRef = rField.GetValue(this) as AssetRef;
                rWriter.Write(null != rAssetRef ? true : false);
                if (null != rAssetRef)
                    rAssetRef.Serialize(rWriter);
            }
        }
        mAssetRefData = rMemoryStream.ToArray();
    }
}