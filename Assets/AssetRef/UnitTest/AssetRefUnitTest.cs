using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class XXWrap
{
    public AssetRefTexture2D[] texArray;
}

public class AssetRefUnitTest : MonoBehaviour
{
    public GameObject           OrigGo0;
    public GameObject           OrigGo1;

    public AssetRefTexture2D        textureObject;
    public XXWrap                   tex;
    public List<AssetRefTexture2D>  textureArrays;

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 220, 24), "Load textureObject"))
            Debug.Log(textureObject.AssetObject);
        if (GUI.Button(new Rect(0, 24, 220, 24), "Unload textureObject"))
            textureObject.UnloadAsset();
        //if (GUI.Button(new Rect(0, 48, 220, 24), "Load RendererObject"))
        //    Debug.Log(RendererObject.AssetObject);
    }
}
