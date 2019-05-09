using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class Program : MonoBehaviour {

    // Use this for initialization
    Process myProcess;
        void Start () {
        try
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
            myProcess = new Process();
            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = Application.streamingAssetsPath + "/ProfilerExterno.exe";
            myProcess.EnableRaisingEvents = true;
            myProcess.Start();
            
            
            //myProcess.WaitForExit();
            //int ExitCode = myProcess.ExitCode;
            //print(ExitCode);
        }
        catch (Exception e)
        {
            print(e);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnApplicationQuit()
    {
        UnityEngine.Debug.Log("HOLA");
        myProcess.Kill();
        //myProcess.Close();
    }
}
