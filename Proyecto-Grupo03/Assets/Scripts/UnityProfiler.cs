using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UnityProfiler : MonoBehaviour
{
    //Values for calculating FPS
    private float frameCount = 0;
    private float nextUpdate = 0.0f;
    private float fps = 0.0f;
    private float updateRate = 4.0f;  // 4 updates per sec.

    //Counting objects
    private GameObject[] nObjectsList;
    private int nObjects;


    //Configuration of profiler
    public bool profilerActive = false;

    // Use this for initialization
    Process myProcess;
    void Start()
    {
#if !UNITY_EDITOR
        String[] Data = Environment.GetCommandLineArgs();


        if (Data[1] == "Profiler") //game.exe opened with argument "Profiler"
        {
            TrackerGr03.Tracker.startTracker(Data[2], Application.dataPath + "/ProfilerLogs/Unity/", TrackerGr03.Tracker.TypeFile.CSVSerializer);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter("Profiler.txt", false))
            {
                file.WriteLine(Process.GetCurrentProcess().ProcessName);
                file.Close();
            }

            StartCoroutine(SendTrace());
          



         
        }
#else
        if (profilerActive)
        {
            TrackerGr03.Tracker.startTracker("editor", (Application.dataPath + "/ProfilerLogs/"), TrackerGr03.Tracker.TypeFile.CSVSerializer);

            StartCoroutine(SendTrace());

        }

#endif
    }

    // Update is called once per frame
    void Update()
    {
        frameCount++;
        if (Time.time > nextUpdate)
        {
            nextUpdate += 1.0f / updateRate;
            fps = frameCount * updateRate;
            frameCount = 0;
            UnityEngine.Debug.Log(UnityStats.);
        }
    }



    IEnumerator SendTrace()
    {
        while (true)
        {
            // execute block of code here
#if UNITY_EDITOR
            TrackerGr03.Tracker.sendTrace(new TrackerGr03.Tracker.Trace(UnityStats.screenRes, UnityStats.drawCalls, UnityStats.triangles, UnityStats.vertices, CountObjects(), UnityStats.frameTime));
#else
            TrackerGr03.Tracker.sendTrace(new TrackerGr03.Tracker.Trace(CountObjects(), fps));
#endif
            yield return new WaitForSeconds(1.0f);
        }


    }


    int CountObjects()
    {
        nObjects = 0;
        nObjectsList = FindObjectsOfType<GameObject>();
        foreach (GameObject g in nObjectsList)
        {
            if (g.GetComponent<MeshRenderer>() || g.GetComponent<SpriteRenderer>() || g.GetComponent<Terrain>())
            {
                nObjects++;
            }
        }
        return nObjects;


    }

}
