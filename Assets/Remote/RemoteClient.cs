using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.Remoting.Messaging;
using System;

using RemoteClass;

public class RemoteInvoke
{
    public IAsyncResult AsyncResult;

    public static RemoteInvoke2<R, P1, P2> AsyncInvoke<R, P1, P2>(
        RemoteInvoke2<R, P1, P2>.RemoteAsyncDelegate rDelegate, P1 v1, P2 v2)
    {
        return new RemoteInvoke2<R, P1, P2>(rDelegate, v1, v2);
    }
    public static RemoteInvoke0<R> AsyncInvoke<R>(RemoteInvoke0<R>.RemoteAsyncDelegate rDelegate)
    {
        return new RemoteInvoke0<R>(rDelegate);
    }
}

public class RemoteInvoke2<R, P1, P2> : RemoteInvoke
{
    public R                RetunValue;

    public RemoteInvoke2(RemoteAsyncDelegate rDelegate, P1 a, P2 b)
    {
        AsyncCallback RemoteCallback = new AsyncCallback(this.OurRemoteAsyncCallBack);

        this.AsyncResult = rDelegate.BeginInvoke(a, b, RemoteCallback, null);
    }

    public delegate R RemoteAsyncDelegate(P1 a, P2 b);
    [OneWayAttribute]
    protected void OurRemoteAsyncCallBack(IAsyncResult ar)
    {
        RemoteAsyncDelegate del = (RemoteAsyncDelegate)((AsyncResult)ar).AsyncDelegate;

        this.RetunValue = del.EndInvoke(ar);
    }
}

public class RemoteInvoke0<R> : RemoteInvoke
{
    public R RetunValue;

    public RemoteInvoke0(RemoteAsyncDelegate rDelegate)
    {
        AsyncCallback RemoteCallback = new AsyncCallback(this.OurRemoteAsyncCallBack);

        this.AsyncResult = rDelegate.BeginInvoke(RemoteCallback, null);
    }

    public delegate R RemoteAsyncDelegate();
    [OneWayAttribute]
    protected void OurRemoteAsyncCallBack(IAsyncResult ar)
    {
        RemoteAsyncDelegate del = (RemoteAsyncDelegate)((AsyncResult)ar).AsyncDelegate;

        this.RetunValue = del.EndInvoke(ar);
    }
}

public class RemoteClient : MonoBehaviour {
    ServiceClass remoteObject = null;
	// Use this for initialization
	void Start () {
        remoteObject = (ServiceClass)System.Activator.GetObject(typeof(ServiceClass),
                "tcp://localhost:6666/testUri");

        RemoteRunable.Instance.Invoke(DoRemoteInvoke());
	}

    IEnumerator DoRemoteInvoke()
    {
        yield return 0;

        while (true)
        {
            // TODO : Unity 无法直接转换函数到Delegate
            var rAsyncResult = RemoteInvoke.AsyncInvoke<int, int, int>(
                remoteObject.Sum, UnityEngine.Random.Range(0, 9999), UnityEngine.Random.Range(0, 9999));
            yield return rAsyncResult;

            Debug.Log(rAsyncResult.RetunValue);

            var rAsyncResult1 = RemoteInvoke.AsyncInvoke<int>(remoteObject.Increment);
            yield return rAsyncResult1;

            Debug.Log(rAsyncResult1.RetunValue);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
