using System;

using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;

using PubNubMessaging.Core;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

public class Program
{
    public class shipInfo
    {
        public string Event { get; set; }
        public Rank RankBuilder = new Rank();
        public Progress ProgressBuilder = new Progress();
        public Docked DockedBuilder = new Docked();
        public Location LocationBuilder = new Location();
        public Touchdown TouchdownBuilder = new Touchdown();
        public HullDamage HullDamageBuilder = new HullDamage();
        public ShieldState ShieldStateBuilder = new ShieldState();
        public PVPKill PVPKillBuilder = new PVPKill();
        public DatalinkScan DataLinkscanBuilder = new DatalinkScan();
        public RecieveText RecieveTextBuilder = new RecieveText();
    }
    public class Rank
    {
        public string Combat { get; set; }
        public string Trade { get; set; }
        public string Explore { get; set; }
        public string Empire { get; set; }
        public string Federation { get; set; }
        public string CQC { get; set; }
        public void rank(string combat, string trade, string explore, string empire, string federation, string cqc)
        {
            Combat = combat;
            Trade = trade;
            Explore = explore;
            Empire = empire;
            Federation = federation;
            CQC  = cqc;
        }
        
    }
    public class Progress
    {
        public string Combat { get; set; }
        public string Trade { get; set; }
        public string Explore { get; set; }
        public string Empire { get; set; }
        public string Federation { get; set; }
        public string CQC { get; set; }
        public void progress(string combat, string trade, string explore, string empire, string federation, string cqc)
        {
            
            Combat = combat;
            Trade = trade;
            Explore = explore;
            Empire = empire;
            Federation = federation;
            CQC = cqc;
        }
    }
    public class Docked
    {
        public bool Dock { get; set; }
        public string StationName { get; set; }
        public string StationType { get; set; }
        public void docked(bool dock, string stationname, string stationtype)
        {
            Dock = dock;
            StationName = stationname;
            StationType = stationname;
        }
    }
    public class Location
    {
        public string StarSystem { get; set; }
        public string Body { get; set; }
        public void location(string starsystem, string body)
        {
            StarSystem = starsystem;
            Body = body;
        }
    }
    public class Touchdown
    {
        public bool Landed { get; set; }
        public void touchdown(bool landed)
        {
            Landed = landed;
        }
    }
    public class HullDamage
    {
        public double Damage { get; set; }
        public void hulldamage(double damage)
        {
            Damage = damage;
        }
    }
    public class ShieldState
    {
        public bool ShieldUp { get; set; }
        public void shieldstate(bool shieldup)
        {
            ShieldUp = shieldup;
        }
    }
    public class PVPKill
    {
        public string Victim { get; set; }
        public void pvpkill(string victim)
        {
            Victim = victim;
        }
    }
    public class DatalinkScan
    {
        public string Message { get; set; }
        public void datalinkscan(string message)
        {
            Message = message;
        }
    }
    public class RecieveText
    {
        public string Message { get; set; }
        public void recievetext(string message)
        {
            Message = message;
        }
    }

    private static AddedContentReader _continuousFileReader = null;

    public string channelShipCommands;

    public static void Main()
    {
        Run();

    }


    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static void Run()
    {
        shipInfo shipInfoMaster = new shipInfo();
        shipInfoMaster.Event = "";
        shipInfoMaster.RankBuilder.rank("", "", "", "", "", "");
        shipInfoMaster.ProgressBuilder.progress("", "", "", "", "", "");
        shipInfoMaster.DockedBuilder.docked(false, "", "");
        shipInfoMaster.LocationBuilder.location("", "");
        shipInfoMaster.TouchdownBuilder.touchdown(false);
        shipInfoMaster.HullDamageBuilder.hulldamage(0.0);
        shipInfoMaster.ShieldStateBuilder.shieldstate(true);
        shipInfoMaster.PVPKillBuilder.pvpkill("");
        shipInfoMaster.DataLinkscanBuilder.datalinkscan("");
        shipInfoMaster.RecieveTextBuilder.recievetext("");

        var serializer = new JavaScriptSerializer();
        var json = serializer.Serialize(shipInfoMaster);

        Console.WriteLine(json);

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
                    if (Directory.Exists(backupDir))
                    {
                        File.Copy(fileName, Path.Combine(backupDir, "working.log"), true);
                    } else
                    {
                        Directory.CreateDirectory(backupDir);
                        File.Copy(fileName, Path.Combine(backupDir, "working.log"), true);
                    }
                    
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