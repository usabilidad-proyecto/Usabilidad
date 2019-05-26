
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Xml.Linq;

using System.IO;
using System.Text;

namespace TrackerGr03
{
    //Interfaz que utilizaremos para crear los diferentes tipos de serializadores
    interface Serializer
    {
        string ToString(Tracker.Trace myTrace);
    }


    class CSVSerializer : Serializer
    {

        public string ToString(Tracker.Trace myTrace)
        {
            if (Tracker.editor)
                return myTrace.id + ";" + (DateTime.Now.Subtract(Tracker.StartTime)).TotalSeconds + ";" + myTrace.screenRes_ + ";" + myTrace.drawCalls_ + ";" + myTrace.triangles_ + ";" + myTrace.vertices_ + ";" + myTrace.nObjects_ + ";" + myTrace.renderTime_ + "\n";
            else
                return myTrace.id + ";" + (DateTime.Now.Subtract(Tracker.StartTime)).TotalSeconds + ";" + myTrace.fps_ + ";" + myTrace.nObjects_ + "\n";
        }
    }


    class JSONSerializer : Serializer
    {
        public string ToString(Tracker.Trace myTrace)
        {
            if (Tracker.editor)
                return "User: { \n" + " ID: " + myTrace.id + "\n" + "TimeStamp: " + (DateTime.Now.Subtract(Tracker.StartTime)).TotalSeconds + "\n" + " Screen Resolution: " + myTrace.screenRes_ + "\n" + " Draw Calls: " + myTrace.drawCalls_ + "\n" + " Triangles: " + myTrace.triangles_ + "\n"
                 + " Vertices: " + myTrace.vertices_ + "\n" + " Render Time: " + myTrace.renderTime_ + "\n" + "}" + "\n";
            else
                return "User: { \n" + " ID: " + myTrace.id + "\n" + "TimeStamp: " + (DateTime.Now.Subtract(Tracker.StartTime)).TotalSeconds + "\n" + "FPS: " + myTrace.fps_ + "\n" + "Number of Objects: " + myTrace.nObjects_ + "\n" + "}" + "\n";
        }
    }

    interface Persistence
    {
        void WriteData(Serializer mySerializer = null, Queue<Tracker.Trace> _buffer = null, System.IO.StreamWriter objWriter = null);
    }

    class FilePersistence: Persistence
    {
        public void WriteData(Serializer mySerializer, Queue<Tracker.Trace> _buffer, System.IO.StreamWriter objWriter)
        {
            while (_buffer.Count() > 0)
            {
                // objWriter.Write(_buffer.Dequeue().ToString());
                objWriter.Write(mySerializer.ToString(_buffer.Dequeue()));
            }
            objWriter.Close();
        }
    }

    class Tracker
    {

        //Este enum es el que nos permitirá buscar por tipo en un diccionario de Serializadores
        public enum TypeFile { CSVSerializer, JSONSerializer};

        public enum TypePersistence { FilePersistence /*ServerPersistence*/};

        public struct Trace
        {
            public string id;
            public string screenRes_;
            public int drawCalls_;
            public int triangles_;
            public int vertices_;
            public float renderTime_;

            public int nObjects_;
            public float fps_;

            public Trace(string screenRes, int drawCalls, int triangles, int vertices, int nObjects,float renderTime) {
                fps_ = 0;

                id = getId();
                screenRes_ = screenRes;
                drawCalls_ = drawCalls;
                triangles_ = triangles;
                vertices_ = vertices;
                nObjects_ = nObjects;
                renderTime_ = renderTime;
            }
            public Trace(int nObjects, float fps)
            {
                screenRes_ = "";
                drawCalls_ = 0;
                triangles_ = 0;
                vertices_ = 0;
                renderTime_ = 0;

                id = getId();
                nObjects_ = nObjects;
                fps_ = fps;

            }

    }
        public  static bool editor;
        public static DateTime StartTime;
        private static string _path;
        private static string uniqueId;
        public static Queue<Trace> _buffer;
        private static int tamBuffer = 1;
        private static System.IO.StreamWriter objWriter;
        private static Serializer myS;
        private static string format;
        private static Persistence myP;
        private static string persistenceFormat;
        private static Dictionary<string, Serializer> formats = new Dictionary<string, Serializer>();
        private static Dictionary<string, Persistence> persistenceFormats = new Dictionary<string, Persistence>();

        public static void setBufferSize(int tam) {
            tamBuffer = tam;
        }

        //En path, desde unity, pondríamos application.datapath, para poder guardar las trazas en la carpeta donde 
        //se encuentra el juego.
        public static void startTracker(string id, string path = "", TypeFile tySerializer = TypeFile.JSONSerializer, TypePersistence tyPersistence = TypePersistence.FilePersistence)
        {
            if (id == "editor")
            {
                editor = true;

                uniqueId = Guid.NewGuid().ToString("N");
            }
            else
            {
                editor = false;
                uniqueId = id;
            }
            _buffer = new Queue<Trace>(tamBuffer);
            StartTime = DateTime.Now;
            //Serializadores
            addFormat("csv", new CSVSerializer());
            addFormat("json", new JSONSerializer());
            setSerializeFormat(tySerializer);

            //Persistencias
            addPersistenceFormat("File", new FilePersistence());
            setPersistenceFormat(tyPersistence);

            //Path
            _path = path;
           

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }

        public static string getId()
        {
            return uniqueId;
        }

        public static void sendTrace(Trace myTrace)
        {
            //Enviar traza
            pushTrace(myTrace);
        }

        private static void pushTrace(Trace trace) {

            Debug.Assert(_buffer.Count() < tamBuffer);
             _buffer.Enqueue(trace);

            if (_buffer.Count() >= tamBuffer)
            {
                //Escribes el csv
                writeFile();
            }

        }
        public static int getCountBuffer() {
            return _buffer.Count();
        }

        private static void setFileType(TypeFile tipo) {

        }

        private static void writeFile(){
            if(formats.ContainsKey(format))
            {
                if(editor)
                    objWriter = new System.IO.StreamWriter(_path  + uniqueId +"-UnityEditor" + format, true);
                else
                    objWriter = new System.IO.StreamWriter(_path  + uniqueId + "-UnityProfiler" + format, true);

            }
            myP.WriteData(myS, _buffer, objWriter);
        }
        
        public static void addFormat(string key, Serializer formatS)
        {
            formats.Add("." + key, formatS);
        }

        public static void addPersistenceFormat(string key, Persistence formatP)
        {
            persistenceFormats.Add(key, formatP);
        }

        //Idem que el setPersistenceFormat pero con los serializadores.
        private static void setSerializeFormat(TypeFile formatS)
        {
            if (formats.Count > 0)
            {
                foreach (KeyValuePair<string, Serializer> pair in formats)
                {
                    //(Quitar comentario), seguro que hay una forma mas limpia de hacer esto, pero en el momento no se me ha ocurrido
                    if(pair.Value.ToString() == typeof(Tracker).Namespace + "." + formatS.ToString())
                    {
                        myS = pair.Value;
                        format = pair.Key;
                        break;
                    }
                }
            }
            else
            {
                throw new Exception("Dictionary of Serializers empty");
            }
        }

        //En estos metodos set, lo que se hace es, establecer el format que queremos utilizar, y a partir de ahí, según este formato, buscamos
        //en el diccionario (Los nombres del enum son los mismos que los de los diferentes tipos de persistencia), y si lo encontramos establecemos el formato de persistencia
        //a utilizar, si no, lanzamos excepción.
        private static void setPersistenceFormat(TypePersistence formatP)
        {
            if (persistenceFormats.Count > 0)
            {
                foreach (KeyValuePair<string, Persistence> pair in persistenceFormats)
                {
                    //(Quitar comentario), seguro que hay una forma mas limpia de hacer esto, pero en el momento no se me ha ocurrido
                    if (pair.Value.ToString() == typeof(Tracker).Namespace + "." + formatP.ToString())
                    {
                        myP = pair.Value;
                        persistenceFormat = pair.Key;
                        break;
                    }
                }
            }
            else
            {
                throw new Exception("Dictionary of Persistences empty");
            }
        }
    }
    /*class MainClass
    {
        //static Tracker myTr;
        static void Main(string[] args)
        {
            Tracker.startTracker("U:/");
            Random rnd = new Random();
            for (int i = 0; i < 202; i++)
            {
                float n = (float)rnd.Next(1, 9);
                Tracker.sendTrace(new Tracker.Trace(Tracker.Events.BRIDGE, Tracker.SUBTYPE.APPLES, n, n, n, n, n));
            }
            Console.WriteLine(Tracker._buffer.Dequeue().ToString());
            Console.ReadKey();

            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }
    }*/
}
