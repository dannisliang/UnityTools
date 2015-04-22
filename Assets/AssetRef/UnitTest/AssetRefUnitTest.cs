using UnityEngine;
using System.Collections;

public class AssetRefUnitTest : AssetRefBehaviour
{
    public GameObject           OrigGo0;
    public GameObject           OrigGo1;

    public TAssetRef<GameObject> GameObjectObject;
    public TAssetRef<Transform> TransformObject;
    public TAssetRef<Renderer> RendererObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 220, 24), "Load GameObjectObject"))
            Debug.Log(GameObjectObject.AssetObject);
        if (GUI.Button(new Rect(0, 24, 220, 24), "Load TransformObject"))
            Debug.Log(TransformObject.AssetObject);
        if (GUI.Button(new Rect(0, 48, 220, 24), "Load RendererObject"))
            Debug.Log(RendererObject.AssetObject);
    }
}
