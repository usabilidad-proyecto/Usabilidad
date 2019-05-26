using System;
using System.Diagnostics;
using System.IO;

namespace HW_Profiler
{
    class Program
    {
        private static PerformanceCounter cpuCounter;
        private static PerformanceCounter cpuProcessCounter;
        private static PerformanceCounter ramProcessCounter;
        private static PerformanceCounter ramCounter;
        private static float TOTAL_RAM;
        private static string proceso;
        private static DateTime StartTime;
        ///Recibe el nombre del proceso a medir y como segundo parametro el tiempo en segundos(UNIX TIMESTAMP)
        ///el tiempo nos permitirá crear un archivo único para cada ejecución ligado al profiler interno.
        static void Main(string[] args)
        {
            //Floats con puntos en vez de comas
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            proceso = args[0];
            //Mirar si existe el proceso
            initialize(proceso);
            string _path = "./" + args[0] + "_Data/" + "ProfilerLogs/HW/";
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            using (System.IO.StreamWriter file =
               new System.IO.StreamWriter(_path + args[1] + "-HWProfiler.csv", true))
            {
                do
                {
                    file.WriteLine((DateTime.Now.Subtract(StartTime)).TotalSeconds + ";" + getRamProcess() + ";" + getCPUProcess() + ";" + getAvailableRAM() + ";" + getTotalCPUUsage());
                    System.Threading.Thread.Sleep(1000);
                } while (getProcess());
                file.Close();
            }
            close();

        }
        private static bool getProcess()
        {
            Process[] a = Process.GetProcessesByName(proceso);
            return a.GetLength(0) != 0;
        }

        public static void initialize(string Proceso)
        {
            try
            {
                StartTime = DateTime.Now;
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                cpuProcessCounter = new PerformanceCounter("Process", "% Processor Time", Proceso);
                ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
                ramProcessCounter = new PerformanceCounter("Process", "Working Set - Private", Proceso);

                TOTAL_RAM = GetTotalMemoryInBytes();
            }
            catch
            {
                using (System.IO.StreamWriter file =
               new System.IO.StreamWriter(proceso + "DEBUG.txt", true))
                {
                    Console.WriteLine("PerformanceCounter not initialized.");
                    file.Close();
                }
            }
        }
        static ulong GetTotalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }

        public static void close()
        {
            cpuCounter.Close();
            cpuCounter.Dispose();
            cpuProcessCounter.Close();
            cpuProcessCounter.Dispose();
            ramCounter.Close();
            ramCounter.Dispose();
            ramProcessCounter.Close();
            ramProcessCounter.Dispose();
        }
        //USO DE LA CPU EN PORCENTAJE TOTAL
        public static string getTotalCPUUsage()
        {
            try
            {
                return  cpuCounter.NextValue().ToString();
            }
            catch
            {
                return "";
            }
        }

        //USO DE LA CPU EN PORCENTAJE DEL PROCESO
        public static string getCPUProcess()
        {
            try
            {
                return cpuProcessCounter.NextValue().ToString();
            }
            catch
            {
                return "";
            }
        }

        //RAM DISPONIBLE (GENERAL)
        public static string getAvailableRAM()
        {
            try
            {

                return ramCounter.NextValue().ToString();
            }
            catch
            {
                return "";
            }
        }

        ///Uso en kb de memoria ram del proceso
        public static string getRamProcess()
        {
            try
            {
                return ((ramProcessCounter.NextValue() / TOTAL_RAM) * 100).ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}
