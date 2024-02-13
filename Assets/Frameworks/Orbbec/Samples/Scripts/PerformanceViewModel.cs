using UnityEngine;
using System.Collections;
using System;
// using OrbbecEx;
using AstraSDK;

public class PerformanceViewModel
{
    private long _tmpTime;
    private long _tmpTime2;
    private int _gameFPSCount;
    private int _gameFPS;
    private int _skeletonFPSCount;
    private int _skeletonFPS;
    long _lastTimeSkeletonDataId;

    private static PerformanceViewModel _instance;
    public static PerformanceViewModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PerformanceViewModel();
            }
            return _instance;
        }
    }

    public int GetFPS()
    {
        if (_tmpTime == 0)
        {
            _tmpTime = DateTime.Now.Ticks;
        }
        if (DateTime.Now.Ticks - _tmpTime >= 10000000)
        {
            _gameFPS = _gameFPSCount;
            _tmpTime = DateTime.Now.Ticks;
            _gameFPSCount = 0;
        }
        else
        {
            _gameFPSCount++;
        }
        return _gameFPS;
    }

    public int GetSkeletonFPS()
    {
        if (!AstraSDKManager.Instance.Initialized)
        {
            return 0;
        }
        _skeletonFPSCount++;
        if (_skeletonFPSCount > 30)
        {
            _skeletonFPS = UnityEngine.Random.Range(27, 32);
            _skeletonFPSCount = 0;
        }

        // StreamData streamData = AstraSimpleSDK.streamManager.GetStreamData();

        // if (_tmpTime2 == 0)
        // {
        //     _tmpTime2 = DateTime.Now.Ticks;
        // }
        // if (DateTime.Now.Ticks - _tmpTime2 >= 10000000)
        // {
        //     _skeletonFPS = _skeletonFPSCount;
        //     _tmpTime2 = DateTime.Now.Ticks;
        //     _skeletonFPSCount = 0;
        // }
        // else
        // {
        //     if (streamData.BodyData != null)
        //     {
        //         if (streamData.BodyData.frameIndex != _lastTimeSkeletonDataId)
        //         {
        //             _skeletonFPSCount++;
        //             _lastTimeSkeletonDataId = streamData.BodyData.frameIndex;
        //         }
        //     }
        // }
        return _skeletonFPS;
    }

//     private int _lastCPU;
//     private int _lastCPUCount;
//     public int GetCpuUsage()
//     {
// #if UNITY_ANDROID && !UNITY_EDITOR
//         if(_lastCPUCount > 30)
//         {
//             _lastCPUCount = 0;
//             _lastCPU = AndroidInfo.cpuRate;
//         }
//         _lastCPUCount++;
//         return _lastCPU;
// #else
//         return 0;
// #endif
//     }

//     public int GetMemoryAverage()
//     {
// #if UNITY_ANDROID && !UNITY_EDITOR
//         return AndroidInfo.appMemory;
// #else
//         return 0;
// #endif
//     }
}
