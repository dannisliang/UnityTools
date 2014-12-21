using UnityEngine;
using System.Collections;

public class UnitTest_ToHTTP : MonoBehaviour {
    public float    CycleTime = 1.0f;
    public LogType  LogType = LogType.Log;

	// Update is called once per frame
    float nLastTime = 0;
	void Update () {
        if (Time.realtimeSinceStartup - nLastTime > CycleTime)
        {
            nLastTime = Time.realtimeSinceStartup;
            NoitfyLog("!!!!!fuck!!!!");
        }
	}
    void NoitfyLog(string log)
    {
        switch (LogType)
        {
            case UnityEngine.LogType.Log:
                Debug.Log(log);
                break;

            case UnityEngine.LogType.Error:
                Debug.LogError(log);
                break;

            case UnityEngine.LogType.Warning:
                Debug.LogWarning(log);
                break;

            case UnityEngine.LogType.Exception:
                try
                {
                    UnitTest_ToHTTP f = null;
                    f.LogType = UnityEngine.LogType.Exception;
                }
                catch(System.Exception e)
                {
                    Debug.LogException(e);
                }
                break;
        }
    }
}
