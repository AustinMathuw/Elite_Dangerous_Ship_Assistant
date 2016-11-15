using System;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;

using PubNubMessaging.Core;
using System.Collections.Generic;

public class Program
{
    private static AddedContentReader _continuousFileReader = null;

    public string channelShipCommands;

    public static void Main()
    {
        Run();

    }


    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static void Run()
    {
        

        System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
        string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
        if (Environment.OSVersion.Version.Major >= 6)
        {
            path = Directory.GetParent(path).ToString();
        }
        string pathTest = path + "\\Saved Games\\Frontier Developments\\Elite Dangerous";
        string backupDir = pathTest + "\\temp";
        // This will exit automatically if main window closes 
        // due to thread being in the background
        long lastUpdate = 0;
        int loop = 0;
        while (loop == 0)
        {
            Thread.Yield();
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if (milliseconds - lastUpdate > 5)
            {
                lastUpdate = milliseconds;
                try
                {
                    string[] fileEntries = Directory.GetFiles(pathTest);
                    string fileName = fileEntries[fileEntries.Length - 1];
                    File.Copy(fileName, Path.Combine(backupDir, "working.log"), true);
                    if (_continuousFileReader == null)
                    {
                        _continuousFileReader = new AddedContentReader(Path.Combine(backupDir, "working.log"));
                    }
                    HandleChangedLines();
                    HandleRecieveCommands();
                }
                catch
                {
                    Console.WriteLine("I cannot find your Elite Dangerous journal path!");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadLine();
                    loop = 1;
                }
            }
        };
    }

    private static void HandleChangedLines()
    {
        if (_continuousFileReader != null && _continuousFileReader.NewDataReady())
        {
            // Specify what is done when a file is changed, created, or deleted.
            string newLines = _continuousFileReader.GetAddedLine();
            if (newLines != null && newLines.Length > 0)
            {
                Console.WriteLine("New data: " + newLines);
            }
        }
    }

    private static void HandleRecieveCommands()
    {
        
    }

    public class AddedContentReader
    {

        private readonly FileStream _fileStream;
        private readonly StreamReader _reader;

        //Start position is from where to start reading first time. consequent read are managed by the Stream reader
        public AddedContentReader(string fileName, long startPosition = 0)
        {
            //Open the file as FileStream
            _fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _reader = new StreamReader(_fileStream);
            //Set the starting position
            _fileStream.Position = startPosition;
        }


        //Get the current offset. You can save this when the application exits and on next reload
        //set startPosition to value returned by this method to start reading from that location
        public long CurrentOffset
        {
            get { return _fileStream.Position; }
        }

        public bool NewDataReady()
        {
            return (_fileStream.Length >= _fileStream.Position);
        }

        //Returns the lines added after this function was last called
        public string GetAddedLine()
        {
            return _reader.ReadLine();
        }
    }
}