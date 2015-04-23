using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RemoteRunable : MonoBehaviour {

    protected static RemoteRunable GInstance;
    public static RemoteRunable Instance
    {
        get { return GInstance; }
    }

    public void Invoke(IEnumerator rIt)
    {
        RemoteInvokeList.Add(rIt);
    }

    protected List<IEnumerator> RemoteInvokeList = new List<IEnumerator>();

	// Use this for initialization
	void Start () {
        GInstance = this;
	}

    List<IEnumerator> RemoveList = new List<IEnumerator>();
	// Update is called once per frame
	void Update () {
        RemoveList.Clear();
        for (int nIndex = 0; nIndex < RemoteInvokeList.Count; ++ nIndex)
        {
            var it = RemoteInvokeList[nIndex];
            var rRemoteInvoke = it.Current as RemoteInvoke;
            bool bRemove = false;
            if (null != rRemoteInvoke)
            {
                if (rRemoteInvoke.AsyncResult.IsCompleted)
                    bRemove = !it.MoveNext();
            }
            else
            {
                bRemove = !it.MoveNext();
            }
            if (bRemove)
                RemoveList.Add(it);
        }

        for (int nIndex = 0; nIndex < RemoveList.Count; ++ nIndex )
            RemoteInvokeList.Remove(RemoveList[nIndex]);
	}
}
