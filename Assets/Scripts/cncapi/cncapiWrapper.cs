/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.cncapi
{
    [InitializeOnLoad]
    class cncapiWrapper
    {
        static cncapiWrapper()
        {
            var currentPath = Environment.GetEnvironmentVariable("PATH",
           EnvironmentVariableTarget.Process);
#if UNITY_EDITOR_32
    var dllPath = Application.dataPath

        + Path.DirectorySeparatorChar + "Plugins"
        + Path.DirectorySeparatorChar + "x86";
#elif UNITY_EDITOR_64
            var dllPath = Application.dataPath
                
                + Path.DirectorySeparatorChar + "Plugins"
                + Path.DirectorySeparatorChar + "x86_64";
#else // Player
    var dllPath = Application.dataPath
        + Path.DirectorySeparatorChar + "Plugins";

#endif
            if (currentPath != null && currentPath.Contains(dllPath) == false)
                Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator
                    + dllPath, EnvironmentVariableTarget.Process);
        }
        [DllImport("CncapiNet64.dll", EntryPoint = "CncGetServerVersion")]
        public static extern char[] CncGetServerVersion();
    }
        
    }

    public enum Cnc_Rc
    {
        CNC_RC_ERR_NOT_CONNECTED = -17,
        CNC_RC_ERR_VERSION_MISMATCH = -16,
        CNC_RC_ERR_SERVER_NOT_RUNNING = -15,
        CNC_RC_ERR_COLLISION = -14,
        CNC_RC_ERR_FILEOPEN = -13,
        CNC_RC_EXE_CE = -12,
        CNC_RC_ERR_TIMEOUT = -11,
        CNC_RC_ERR_SYS = -10,
        CNC_RC_ERR_MOT = -9,
        CNC_RC_ERR_CPU = -8,
        CNC_RC_ERR_EXE = -7,
        CNC_RC_ERR_CE = -6,
        CNC_RC_ERR_INT = -5,
        CNC_RC_ERR_CONFIG = -4,
        CNC_RC_ERR_STATE = -3,
        CNC_RC_ERR_PAR = -2,
        CNC_RC_ERR = -1,
        CNC_RC_OK = 0,
        CNC_RC_BUF_EMPTY = 1,
        CNC_RC_TRACE = 2,
        CNC_RC_USER_INFO = 3,
        CNC_RC_SHUTDOWN = 4,
        CNC_RC_EXISTING = 5,
        CNC_RC_ALREADY_RUNS = 6,
        CNC_RC_ALREADY_CONNECTED = 7
    }


*/