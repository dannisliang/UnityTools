using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Logger : MonoBehaviour
{
    protected   static Logger GInstance = null;
    public      static Logger Instance
    {
        get { return GInstance; }
    }

    public void Attach(ILoggerDevice rDevice)
    {
        if (!LoggerDevices.Contains(rDevice))
            LoggerDevices.Add(rDevice);
    }
    public void Detach(ILoggerDevice rDevice)
    {
        LoggerDevices.Remove(rDevice);
    }

    protected List<ILoggerDevice> LoggerDevices = new List<ILoggerDevice>();



    #region MonoBehaviour
    void Start()
    {
        GInstance = this;

#if UNITY_5_0
        Application.logMessageReceived += this.OnLogNotify;
#else
        Application.RegisterLogCallback(this.OnLogNotify);
#endif
    }
    void OnDestroy()
    {
#if UNITY_5_0
        Application.logMessageReceived -= this.OnLogNotify;
#else
        Application.RegisterLogCallback(null);
#endif
    }
    void OnLogNotify(string condition, string stackTrace, LogType type)
    {
        for (int nIndex = 0; nIndex < LoggerDevices.Count; ++ nIndex)
            LoggerDevices[nIndex].OnLog(condition, stackTrace, type);
    }
    #endregion
}
