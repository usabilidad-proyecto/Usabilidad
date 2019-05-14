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
        private static string proceso;
        static void Main(string[] args)
        {
            //Floats con puntos en vez de comas
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            proceso = args[0];
            //Mirar si existe el proceso
            initialize(proceso);

            do
            {
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(proceso+"-profiler.txt", true))
                {
                    file.WriteLine(getCurrentCpuUsage()+  ";" + getAvailableRAM());
                    file.Close();
                }
                System.Threading.Thread.Sleep(500);
                
            } while (getProcess());
        }
        private static bool getProcess() {
            Process[] a = Process.GetProcessesByName(proceso);
            return a.GetLength(0) != 0;
        }

        public static void initialize(string Proceso)
        {
            try            
            {
                
                cpuCounter = new PerformanceCounter("Process", "% Processor Time", Proceso);
                ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            }
            catch {
                using (System.IO.StreamWriter file =
               new System.IO.StreamWriter(proceso + "DEBUG.txt", true))
                {
                    Console.WriteLine("PerformanceCounter not initialized.");
                    file.Close();
                }
            }
        }

        public static string getCurrentCpuUsage()
        {
            try
            {
                return cpuCounter.NextValue().ToString();
            }
            catch {
                return "";
            }
        }

        public static string getAvailableRAM()
        {
            try
            {
                return ramCounter.NextValue().ToString();
            }
            catch {
                return "";
            }
        }
    }
}
