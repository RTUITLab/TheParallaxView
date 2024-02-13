using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Astra;
using System;
using AstraSDK;

public class PerformanceView : MonoBehaviour
{
    public Text fps;
    // public Text cpu;
    // public Text memory;
    // public Text skeleton;
    public Text depthFrameFps;
    public Text colorFrameFps;
    public Text bodyFrameFps;

    private int depthFrameCount;
    private int colorFrameCount;
    private int bodyFrameCount;
    private bool useChinese;

    void Start()
    {
        // if (Application.platform != RuntimePlatform.Android)
        // {
            // cpu.gameObject.SetActive(false);
            // memory.gameObject.SetActive(false);
        // }

        // AstraSDKManager.Instance.OnNewDepthFrame.AddListener(OnNewDepthFrame);
        // AstraSDKManager.Instance.OnNewColorFrame.AddListener(OnNewColorFrame);
        // AstraSDKManager.Instance.OnNewBodyFrame.AddListener(OnNewBodyFrame);
        StartCoroutine(CountBodyFrameFps());

        if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified)
        {
            useChinese = true;
        }
        else
        {
            useChinese = false;
        }
    }

    private void OnNewDepthFrame(DepthFrame arg0)
    {
        depthFrameCount++;
    }

    private void OnNewColorFrame(ColorFrame arg0)
    {
        colorFrameCount++;
    }

    private void OnNewBodyFrame(Astra.BodyFrame arg0)
    {
        bodyFrameCount++;
    }

    // Update is called once per frame
    void Update()
    {
        if(useChinese)
        {
            fps.text = string.Format("应用帧率 : {0}", PerformanceViewModel.Instance.GetFPS());
            // if (Application.platform == RuntimePlatform.Android)
            // {
            //     cpu.text = string.Format("CPU使用率 : {0}%", PerformanceViewModel.Instance.GetCpuUsage());
            //     memory.text = string.Format("内存占用(MB) : {0}", PerformanceViewModel.Instance.GetMemoryAverage());
            // }
            // skeleton.text = string.Format("骨架帧率 : {0}", PerformanceViewModel.Instance.GetSkeletonFPS());
        }
        else
        {
            fps.text = string.Format("App FPS : {0}", PerformanceViewModel.Instance.GetFPS());
            // if (Application.platform == RuntimePlatform.Android)
            // {
            //     cpu.text = string.Format("CPU usage : {0}%", PerformanceViewModel.Instance.GetCpuUsage());
            //     memory.text = string.Format("Memory(MB) average : {0}", PerformanceViewModel.Instance.GetMemoryAverage());
            // }
            // skeleton.text = string.Format("Skeleton FPS : {0}", PerformanceViewModel.Instance.GetSkeletonFPS());
        }
    }

    private IEnumerator CountBodyFrameFps()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            if (useChinese)
            {
                depthFrameFps.text = string.Format("深度帧率 : {0}", depthFrameCount);
                colorFrameFps.text = string.Format("彩色帧率 : {0}", colorFrameCount);
                bodyFrameFps.text = string.Format("骨架帧率 : {0}", bodyFrameCount);
            }
            else
            {
                depthFrameFps.text = string.Format("Depth FPS : {0}", depthFrameCount);
                colorFrameFps.text = string.Format("Color FPS : {0}", colorFrameCount);
                bodyFrameFps.text = string.Format("Skeleton FPS : {0}", bodyFrameCount);
            }
            depthFrameCount = 0;
            colorFrameCount = 0;
            bodyFrameCount = 0;
        }
    }
}
