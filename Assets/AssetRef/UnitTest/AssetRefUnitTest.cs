using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class XXWrap
{
    public RefTexture2D[] texArray;
}

public class AssetRefUnitTest : MonoBehaviour
{
    public GameObject           OrigGo0;
    public GameObject           OrigGo1;

    public RefTexture2D        textureObject;
    public XXWrap                   tex;
    public List<RefTexture2D>  textureArrays;

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 220, 24), "Load textureObject"))
			this.StartCoroutine(this.AsyncLoadAssetRef(-1, this.StartCoroutine));

        if (GUI.Button (new Rect (0, 24, 220, 24), "Unload textureObject"))
			this.UnloadAssetRef(0);
        //if (GUI.Button(new Rect(0, 48, 220, 24), "Load RendererObject"))
        //    Debug.Log(RendererObject.AssetObject);
    }
}
