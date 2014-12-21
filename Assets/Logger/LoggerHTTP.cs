using UnityEngine;
using System.Collections;
using System.Collections.Generic;

internal class LoggerHTTP : MonoBehaviour, ILoggerDevice
{
    public string           ServerHost;
    public LoggerLogType    LogType;

    void Start()
    {
        Logger.Instance.Attach(this);
    }
    void OnDestroy()
    {
        Logger.Instance.Detach(this);
    }

    void ILoggerDevice.OnLog(string condition, string stackTrace, LogType type)
    {
        if (type == UnityEngine.LogType.Log)
            this.StartCoroutine(SendToServer(condition, stackTrace));
    }

    IEnumerator SendToServer(string condition, string stackTrace)
    {
        WWWForm Form = new WWWForm();

        Form.AddField("Condition", condition);
        Form.AddField("StackTrace", stackTrace);

        WWW www = new WWW(ServerHost, Form);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
            Debug.LogError(www.text);
        else
            Debug.LogError(www.error);
    }
}