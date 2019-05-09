using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class Program : MonoBehaviour
{

    // Use this for initialization
    Process myProcess;
    void Start()
    {

        String[] Data = Environment.GetCommandLineArgs();
        if (Data[1] == "Profiler")
        {
            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter("Profiler.txt", false))
            {
                file.WriteLine(Application.productName);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
