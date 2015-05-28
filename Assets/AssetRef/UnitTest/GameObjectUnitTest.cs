using UnityEngine;
using System.Collections;

[System.Serializable]
public class GOSerialize
{
	public float			floatValue;
	public RefGameObject	goValue;
}

public class GameObjectUnitTest : MonoBehaviour
{
	public GameObject 		go;
	public GOSerialize		v1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
