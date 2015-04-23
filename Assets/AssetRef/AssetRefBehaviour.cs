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
	protected static BindingFlags SerializeFieldFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
	protected static System.Type  SerializeFieldType  = typeof(AssetRef);

    [SerializeField, HideInInspector]
    private byte[]      mAssetRefData;  
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
		if (null == mAssetRefData || mAssetRefData.Length == 0)
			return;
		
		using (var rReader = new BinaryReader(new MemoryStream(mAssetRefData)))
		{
			CompatibleDeserialize(rReader);
		}
    }
    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
		var rMemoryStream = new MemoryStream();
		using (var rWriter = new BinaryWriter(rMemoryStream))
		{
			CompatibleSerialize(rWriter);
		}
        mAssetRefData = rMemoryStream.ToArray();
    }
	void CompatibleSerialize(BinaryWriter rWriter)
	{
		foreach (var rField in GetType().GetFields(SerializeFieldFlags))
		{
			if (!rField.IsPublic && !rField.IsDefined(typeof(SerializeField), false))
				continue;
			
			if (!SerializeFieldType.IsAssignableFrom(rField.FieldType))
				continue;
			
			var rAssetRef = rField.GetValue(this) as AssetRef;
			long nAssetLength = 0;
			rWriter.Write(rField.Name);
			rWriter.Flush();
			var nAssetLengthPosition = rWriter.BaseStream.Position;
			rWriter.Write((int)nAssetLength);
			rWriter.Flush();
			var nAssetPosition = rWriter.BaseStream.Position;
			if (null != rAssetRef)
			{
				rAssetRef.Serialize(rWriter);
				rWriter.Flush();
				var nWriterPosition = rWriter.BaseStream.Position;
				nAssetLength = nWriterPosition - nAssetPosition;
				rWriter.Seek((int)nAssetLengthPosition, SeekOrigin.Begin);
				rWriter.Write((int)nAssetLength);
				rWriter.Seek((int)nWriterPosition, SeekOrigin.Begin);
			}
		}
	}
	void CompatibleDeserialize(BinaryReader rReader)
	{
		while (rReader.BaseStream.Position < rReader.BaseStream.Length)
		{
		 	var rFieldName 		= rReader.ReadString();
		 	var nFieldLength 	= (long)rReader.ReadInt32();
		 	var rFieldInfo 		= GetType().GetField(rFieldName, SerializeFieldFlags);
		 	if (null == rFieldInfo)
		 	{
		 		rReader.BaseStream.Seek(nFieldLength, SeekOrigin.Current);
		 		continue;
		 	}

			AssetRef rAssetRef = null;
			if (nFieldLength != 0)
			{
				var nWritePosition = rReader.BaseStream.Position;

				rAssetRef = ReflectAssists.Constrct(rFieldInfo.FieldType) as AssetRef;

				try {
					rAssetRef.Deserialize(rReader);
				}
				catch (System.Exception rException)
				{
					Debug.Log(rException.Message);
				}
				rReader.BaseStream.Seek(nWritePosition + nFieldLength, SeekOrigin.Begin);

				rFieldInfo.SetValue(this, rAssetRef);
			}
		}
	}
	void FastSerialize(BinaryWriter rWriter)
	{
		foreach (var rField in GetType().GetFields(SerializeFieldFlags))
		{
			if (!rField.IsPublic && !rField.IsDefined(typeof(SerializeField), false))
				continue;
			
			if (!SerializeFieldType.IsAssignableFrom(rField.FieldType))
				continue;
			
			var rAssetRef = rField.GetValue(this) as AssetRef;
			rWriter.Write(null != rAssetRef ? true : false);
			if (null != rAssetRef)
				rAssetRef.Serialize(rWriter);
		}
	}
	void FastDeserialize(BinaryReader rReader)
	{
		foreach (var rField in GetType().GetFields(SerializeFieldFlags))
		{
			if (!rField.IsPublic && !rField.IsDefined(typeof(SerializeField), false))
				continue;
			
			if (!SerializeFieldType.IsAssignableFrom(rField.FieldType))
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