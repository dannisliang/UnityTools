using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ILoggerDevice
{
    void OnLog(string condition, string stackTrace, LogType type);
}

[System.Flags()]
public enum LoggerLogType
{
    Error       = 1 << 0,
    Assert      = 1 << 1,
    Warning     = 1 << 2,
    Log         = 1 << 3,
    Exception   = 1 << 4,
}