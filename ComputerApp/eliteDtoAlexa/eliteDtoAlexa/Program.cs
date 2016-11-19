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
        public Rank Rank = new Rank();
        public Progress Progress = new Progress();
        public Docked Docked = new Docked();
        public Location Location = new Location();
        public Touchdown Touchdown  = new Touchdown();
        public HullDamage HullDamage  = new HullDamage();
        public ShieldState ShieldState  = new ShieldState();
        public PVPKill PVPKill  = new PVPKill();
        public DatalinkScan DataLinkscan  = new DatalinkScan();
        public RecieveText RecieveText = new RecieveText();
    }
    public class Rank
    {
        public int Combat { get; set; }
        public int Trade { get; set; }
        public int Explore { get; set; }
        public int Empire { get; set; }
        public int Federation { get; set; }
        public int CQC { get; set; }
        public void rank(int combat, int trade, int explore, int empire, int federation, int cqc)
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
        public int Combat { get; set; }
        public int Trade { get; set; }
        public int Explore { get; set; }
        public int Empire { get; set; }
        public int Federation { get; set; }
        public int CQC { get; set; }
        public void progress(int combat, int trade, int explore, int empire, int federation, int cqc)
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
        shipInfo shipInfoMaster = new shipInfo();
        Run(shipInfoMaster);

    }

    

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static void Run(shipInfo shipInfoMaster)
    {
        shipInfoMaster.Rank.rank(0, 0, 0, 0, 0, 0);
        shipInfoMaster.Progress.progress(0, 0, 0, 0, 0, 0);
        shipInfoMaster.Docked.docked(false, "", "");
        shipInfoMaster.Location.location("", "");
        shipInfoMaster.Touchdown.touchdown(false);
        shipInfoMaster.HullDamage.hulldamage(0.0);
        shipInfoMaster.ShieldState.shieldstate(true);
        shipInfoMaster.PVPKill.pvpkill("");
        shipInfoMaster.DataLinkscan.datalinkscan("");
        shipInfoMaster.RecieveText.recievetext("");

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
        while (loop < 4)
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
                    HandleChangedLines(shipInfoMaster);
                    HandleRecieveCommands();
                    loop = 0;
                }
                catch
                {
                    loop++;
                }
                if(loop == 3)
                {
                    Console.WriteLine("I cannot find your Elite Dangerous journal path!");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadLine();
                }
            }
        };
    }

    private static void HandleChangedLines(shipInfo shipInfoMaster)
    {
        if (_continuousFileReader != null && _continuousFileReader.NewDataReady())
        {
            // Specify what is done when a file is changed, created, or deleted.
            string newLines = _continuousFileReader.GetAddedLine();
            if (newLines != null && newLines.Length > 0)
            {
                ///See http://www.newtonsoft.com/json/help/html/ReadingWritingJSON.htm
                JsonTextReader reader = new JsonTextReader(new StringReader(newLines));
                string eventName = "";
                string[] eventContent = new string[0];
                string[] eventContentAttributes = new string[0];
                string attributeName = "";

                int arraySize = 0;

                int type = 0;
                while (reader.Read())
                {
                    
                    if (reader.Value != null)
                    {
                        string check1 = Convert.ToString(reader.TokenType);
                        string check2 = Convert.ToString(reader.Value);
                        if (type == 1)
                        {
                            eventName = check2;
                            Console.WriteLine(eventName);
                            type = 0;
                        }

                        if (type == 2)
                        {
                            type = 0;
                        }
                        
                        if (check1 == "PropertyName")
                        {
                            if (check2 == "event")
                            {
                                type = 1;
                            }
                            else
                            {
                                attributeName = check2;
                                type = 2;
                            }
                        } else if (check2 != eventName && attributeName != "timestamp")
                        {
                            arraySize++;
                            Array.Resize(ref eventContentAttributes, arraySize);
                            eventContentAttributes[arraySize - 1] = attributeName;
                            Array.Resize(ref eventContent, arraySize);
                            eventContent[arraySize - 1] = check2;
                            attributeName = "";
                        }

                        
                        
                        
                        if(reader.Value != null)
                        {

                        }
                        
                    }
                }
                
                
                Console.WriteLine("");
                System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
                string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    path = Directory.GetParent(path).ToString();
                }
                string pathTest = path + "\\Documents\\Elite Dangerous Ship Assistant";
                if (!Directory.Exists(pathTest))
                {
                    Directory.CreateDirectory(pathTest);
                    
                }

                if (eventName == "Rank")
                {
                    dynamic[] universalContent = new dynamic[0];
                    
                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = Convert.ToInt32(eventContent[i]);
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Rank.rank(universalContent[0], universalContent[1], universalContent[2], universalContent[3], universalContent[4], universalContent[5]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "Progress")
                {
                    dynamic[] universalContent = new dynamic[0];

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = Convert.ToInt32(eventContent[i]);
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Progress.progress(universalContent[0], universalContent[1], universalContent[2], universalContent[3], universalContent[4], universalContent[5]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "Docked")
                {

                }
                else if (eventName == "Location")
                {

                }
                else if (eventName == "Touchdown")
                {

                }
                else if (eventName == "HullDamage")
                {

                }
                else if (eventName == "ShieldState")
                {

                }
                else if (eventName == "PVPKill")
                {

                }
                else if (eventName == "DatalinkScan")
                {

                }
                else if (eventName == "RecieveText")
                {

                }
                
                Console.WriteLine("");
                Console.WriteLine("EVENT DONE");
                Console.WriteLine("");
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