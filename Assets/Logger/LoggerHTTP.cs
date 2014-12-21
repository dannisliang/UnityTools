using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
        var rStringBuild = new StringBuilder();
        rStringBuild.AppendFormat("Device         : {0}\r\n", SystemInfo.deviceName);
        rStringBuild.AppendFormat("Device Module  : {0}\r\n", SystemInfo.deviceModel);
        rStringBuild.AppendFormat("System         : {0}\r\n", SystemInfo.operatingSystem);
        rStringBuild.AppendFormat("CPU Count      : {0}\r\n", SystemInfo.processorCount);
        rStringBuild.AppendFormat("CPU            : {0}\r\n", SystemInfo.processorType);
        rStringBuild.AppendLine("================================");
        rStringBuild.AppendFormat("Condition      : \r\n{0}\r\n", condition);
        rStringBuild.AppendLine("================================");
        rStringBuild.AppendFormat("StackTrace     : \r\n{0}\r\n", stackTrace);

        WWW www = new WWW(ServerHost, Encoding.ASCII.GetBytes(rStringBuild.ToString()));
        yield return www;

        if (string.IsNullOrEmpty(www.error))
            Debug.LogError(www.text);
        else
            Debug.LogError(www.error);
    }
}