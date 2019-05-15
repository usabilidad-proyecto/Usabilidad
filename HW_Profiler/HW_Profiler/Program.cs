using System;
using System.Diagnostics;


namespace HW_Profiler
{
    class Program
    {
        private static PerformanceCounter cpuCounter;
        private static PerformanceCounter ramProcessCounter;
        private static PerformanceCounter ramCounter;
        private static string proceso;

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
            using (System.IO.StreamWriter file =
               new System.IO.StreamWriter(args[1] + "-HWProfiler.txt", true))
            {
                do
                {
                    file.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ";" + getRamProcess() + ";" + getCurrentCpuUsage() + ";" + getAvailableRAM());
                    System.Threading.Thread.Sleep(500);
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
                cpuCounter = new PerformanceCounter("Process", "% Processor Time", Proceso);
                ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                ramProcessCounter = new PerformanceCounter("Process", "Working Set - Private", Proceso);
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
        public static void close() {
            cpuCounter.Close();
            cpuCounter.Dispose();
            ramCounter.Close();
            ramCounter.Dispose();
            ramProcessCounter.Close();
            ramProcessCounter.Dispose();
        }

        //USO DE LA CPU EN PORCENTAJE
        public static string getCurrentCpuUsage()
        {
            try
            {
                return cpuCounter.NextValue().ToString();
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
                return ((int)ramProcessCounter.NextValue()>>10).ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}
