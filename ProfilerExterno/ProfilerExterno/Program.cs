using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ProfilerExterno
{
    class Program
    {
        private static PerformanceCounter cpuCounter;
        private static PerformanceCounter ramCounter;
        static void Main(string[] args)
        {
            
            initialize("hola");
            do
            {
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter("WriteLines2.txt", true))
                {
                    file.WriteLine(getCurrentCpuUsage() + ";" + getAvailableRAM());
                    file.Close();
                }
                System.Threading.Thread.Sleep(500);
            } while (true);
        }
     

        public static void initialize(string Proceso)
        {

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", Proceso);
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public static string getCurrentCpuUsage()
        {
            return cpuCounter.NextValue().ToString();
        }

        public static string getAvailableRAM()
        {
            return ramCounter.NextValue().ToString();
        }
    }
}
