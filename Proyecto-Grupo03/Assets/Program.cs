using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using UnityEngine.Profiling;


public class Program : MonoBehaviour
{
    private string ProfilerUnityPath = "Editor-UnityProfiler.txt";
    public bool profilerActive = false;
    // Use this for initialization
    Process myProcess;
    void Start()
    {

        String[] Data = Environment.GetCommandLineArgs();
        if (Data[1] == "Profiler" || profilerActive)
        {
            Profiler.logFile = "/profilerLog.txt";
            // write Profiler Data to "profilerLog.txt.data"                                                                                        
            Profiler.enableBinaryLog = true;
            Profiler.enabled = true;


            using (System.IO.StreamWriter file = new System.IO.StreamWriter("Profiler.txt", false))
            {
                file.WriteLine(Process.GetCurrentProcess().ProcessName);
                file.Close();
            }

            if (Data[2] != null)
                ProfilerUnityPath = Data[2] + "-UnityProfiler.txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(ProfilerUnityPath, false))
            {
                file.Close();
            }



            //var firstFrameIndex = ProfilerDriver.firstFrameIndex;
            //var lastFrameIndex = ProfilerDriver.lastFrameIndex;
            //var profilerSortColumn = ProfilerColumn.TotalTime;
            //var viewType = ProfilerViewType.Hierarchy;

            ////var profilerData = new ProfilerData();
            //for (int frameIndex = firstFrameIndex; frameIndex <= lastFrameIndex; ++frameIndex)
            //{
            //    var property = new ProfilerProperty();
            //    property.SetRoot(frameIndex, profilerSortColumn, viewType);
            //    property.onlyShowGPUSamples = false;
            //    bool enterChildren = true;

            //    while (property.Next(enterChildren))
            //    {
            //        // get all the desired ProfilerColumn
            //        var name = property.GetColumn(ProfilerColumn.FunctionName);
            //        var totalTime = property.GetColumn(ProfilerColumn.TotalTime);
            //        // store values somewhere
            //    }

            //    property.Cleanup();
            //}
        }


    }

    // Update is called once per frame
    void Update()
    {
      

    }
}
