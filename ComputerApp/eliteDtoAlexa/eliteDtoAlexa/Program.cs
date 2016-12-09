using System;

using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using PubNubMessaging.Core;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

public class Program
{
    [DllImport("User32.dll")]
    static extern int SetForegroundWindow(IntPtr point);
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
        public DatalinkScan DataLinkScan  = new DatalinkScan();
        public RecieveText RecieveText = new RecieveText();
    }

    public class shipCommands
    {
        public string command { get; set; }
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
        public void docked(bool dock, string stationname)
        {
            Dock = dock;
            StationName = stationname;
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
        public double Health { get; set; }
        public void hulldamage(double health)
        {
            Health = health;
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
        public string From { get; set; }
        public void recievetext(string message, string fromWho)
        {
            Message = message;
            From = fromWho;
        }
    }

    private static AddedContentReader _continuousFileReaderShip = null;
    private static AddedContentReader _continuousFileReaderCommands = null;

    public string channelShipCommands;

    public static void Main()
    {
        shipInfo shipInfoMaster = new shipInfo();
        shipCommands shipCommandsMaster = new shipCommands();
        
        Run(shipInfoMaster, shipCommandsMaster);

    }

    

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static void Run(shipInfo shipInfoMaster, shipCommands shipCommandsMaster)
    {
        shipCommandsMaster.command = "";
        shipInfoMaster.Rank.rank(0, 0, 0, 0, 0, 0);
        shipInfoMaster.Progress.progress(0, 0, 0, 0, 0, 0);
        shipInfoMaster.Docked.docked(false, "");
        shipInfoMaster.Location.location("", "");
        shipInfoMaster.Touchdown.touchdown(false);
        shipInfoMaster.HullDamage.hulldamage(1.0);
        shipInfoMaster.ShieldState.shieldstate(true);
        shipInfoMaster.PVPKill.pvpkill("");
        shipInfoMaster.DataLinkScan.datalinkscan("");
        shipInfoMaster.RecieveText.recievetext("", "");

        
        string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
        if (Environment.OSVersion.Version.Major >= 6)
        {
            path = Directory.GetParent(path).ToString();
        }
        string pathTest = path + "\\Saved Games\\Frontier Developments\\Elite Dangerous";
        string backupDir = pathTest + "\\temp";
        
        string pathDoc = path + "\\Documents\\Elite Dangerous Ship Assistant";
        if (!Directory.Exists(pathDoc))
        {
            Directory.CreateDirectory(pathDoc);
        }

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
                    
                    if (_continuousFileReaderShip == null)
                    {
                        _continuousFileReaderShip = new AddedContentReader(Path.Combine(backupDir, "working.log"));
                    }

                    try
                    {
                        File.Copy(Path.Combine(pathDoc, "commandsTo.json"), Path.Combine(pathDoc, "commandsRead.json"), true);
                    } catch
                    {

                    }

                    try
                    {
                        if (_continuousFileReaderCommands == null)
                        {
                            _continuousFileReaderCommands = new AddedContentReader(Path.Combine(pathDoc, "commandsRead.json"));
                        }
                    }
                    catch
                    {

                    }
                    
                    HandleChangedLinesShip(shipInfoMaster);
                    HandleRecieveCommands(shipCommandsMaster);
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

    private static void HandleRecieveCommands(shipCommands shipCommandsMaster)
    {
        if (_continuousFileReaderCommands != null && _continuousFileReaderCommands.NewDataReady())
        {
            // Specify what is done when a file is changed, created, or deleted.
            string newLines = _continuousFileReaderCommands.GetAddedLine();
            if (newLines != null && newLines.Length > 0)
            {
                //Process p = Process.GetProcessesByName("notepad").FirstOrDefault();
                //if (p != null)
                //{
                //    IntPtr h = p.MainWindowHandle;
                //    SetForegroundWindow(h);
                //    //SendKeys.SendWait("k");
                //}

                string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    path = Directory.GetParent(path).ToString();
                }
                string pathDoc = path + "\\Documents\\Elite Dangerous Ship Assistant";
                if (!Directory.Exists(pathDoc))
                {
                    Directory.CreateDirectory(pathDoc);

                }
                ///See http://www.newtonsoft.com/json/help/html/ReadingWritingJSON.htm
                ///

                

                //string fileinfo = File.ReadAllText(Path.Combine(pathDoc, "commandsRead.json"));
                //Console.WriteLine(fileinfo);

                JsonTextReader reader = new JsonTextReader(new StringReader(newLines));

                string commandName = "";
                string oldTime = "";
                string token = "";
                int test = 0;

                while (reader.Read())
                {

                    if (reader.Value != null)
                    {
                        token = Convert.ToString(reader.TokenType);
                        commandName = Convert.ToString(reader.Value);

                        if (test == 2)
                        {
                            if(commandName != oldTime)
                            {
                                if (shipCommandsMaster.command == "boost")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("boost");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "balencePower")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("balencePower");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "cancelDocking")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("cancelDocking");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "cargoScan")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("cargoScan");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "deployChaff")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("deployChaff");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "deployHardpoints")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("deployHardpoints");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "deployLandingGear")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("deployLandingGear");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "deployCargoScoop")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("deployCargoScoop");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "deploySRV")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("deploySRV");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "exitFramshift")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("exitFramshift");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "exitCruise")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("exitCruise");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "powerToEngines")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("powerToEngines");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "powerToSystems")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("powerToSystems");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "powerToWeapons")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("powerToWeapons");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "emergencyStop")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("emergencyStop");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "engageFrameshift")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("engageFrameshift");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "engageCruise")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("engageCruise");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "fightAssistOff")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("fightAssistOff");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "fightAssistOn")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("fightAssistOn");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "targetEnemy")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("targetEnemy");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "screenshot")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("screenshot");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "launch")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("launch");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "lightsOff")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("lightsOff");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "lightsOn")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("lightsOn");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesForward100")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesForward100");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesForward90")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesForward90");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesForward80")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesForward80");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesForward75")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesForward75");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesForward70")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesForward70");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesForward60")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesForward60");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesForward50")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesForward50");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesForward40")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesForward40");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesForward30")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesForward30");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesForward25")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesForward25");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesForward20")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesForward20");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesForward10")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesForward10");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "nextFireGroup")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("nextFireGroup");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "nextFireGroup")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("nextFireGroup");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "nextSystem")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("nextSystem");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "nextShip")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("nextShip");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "prevFireGroup")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("prevFireGroup");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "prevHostile")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("prevHostile");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "prevShip")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("prevShip");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "requestDocking")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("requestDocking");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "centerHeadset")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("centerHeadset");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "retractHardpoints")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("retractHardpoints");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "retractLandingGear")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("retractLandingGear");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "retractCargoScoop")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("retractCargoScoop");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesBack100")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesBack100");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesBack75")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesBack75");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesBack50")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesBack50");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "enginesBack25")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("enginesBack25");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "SRVRecovery")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("SRVRecovery");
                                    Console.ResetColor();
                                }
                                else if (shipCommandsMaster.command == "cutEngines")
                                {
                                    Console.BackgroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine("cutEngines");
                                    Console.ResetColor();
                                }
                            }
                            test = 0;
                        }

                        if (commandName == "timetoken")
                        {
                            test = 2;
                        }

                        if (test == 1)
                        {
                            shipCommandsMaster.command = commandName;
                            
                        }

                        if (commandName == "command")
                        {

                            test = 1;
                        }

                        if (reader.Value != null)
                        {

                        }
                    }
                }
            }
        }
    }

    private static void HandleChangedLinesShip(shipInfo shipInfoMaster)
    {
        if (_continuousFileReaderShip != null && _continuousFileReaderShip.NewDataReady())
        {
            // Specify what is done when a file is changed, created, or deleted.
            string newLines = _continuousFileReaderShip.GetAddedLine();
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
                    dynamic[] universalContent = new dynamic[0];

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = eventContent[i];
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Docked.docked(true, universalContent[0]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "Undocked")
                {
                    dynamic[] universalContent = new dynamic[0];

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = eventContent[i];
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Docked.docked(false, "");

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "Location")
                {
                    dynamic[] universalContent = new dynamic[0];
                    bool dockTrue = false;

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = eventContent[i];
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                            if(eventContentAttributes[i] == "Docked" && universalContent[i] == "true")
                            {
                                dockTrue = true;
                            }
                        }
                        //This is where event attributes are built using the predefined object
                        if (dockTrue)
                        {
                            shipInfoMaster.Location.location(universalContent[3], universalContent[14]);
                            shipInfoMaster.Docked.docked(true, universalContent[1]);
                        }
                        else
                        {
                            shipInfoMaster.Location.location(universalContent[1], universalContent[12]);
                        }
                        

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "FSDJump" || eventName == "SupercruiseEntry")
                {
                    dynamic[] universalContent = new dynamic[0];

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = eventContent[i];
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Location.location(universalContent[0], "");

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "SupercruiseExit")
                {
                    dynamic[] universalContent = new dynamic[0];

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = eventContent[i];
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Location.location(universalContent[0], universalContent[1]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "Touchdown")
                {
                    dynamic[] universalContent = new dynamic[0];

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = eventContent[i];
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Touchdown.touchdown(true);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "Liftoff")
                {
                    dynamic[] universalContent = new dynamic[0];

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = eventContent[i];
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Touchdown.touchdown(false);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "HullDamage")
                {
                    dynamic[] universalContent = new dynamic[0];

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = eventContent[i];
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.HullDamage.hulldamage(Convert.ToDouble(universalContent[1]));

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "ShieldState")
                {
                    dynamic[] universalContent = new dynamic[0];

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = eventContent[i];
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.ShieldState.shieldstate(Convert.ToBoolean(universalContent[0]));

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "PVPKill")
                {
                    dynamic[] universalContent = new dynamic[0];

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = eventContent[i];
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.PVPKill.pvpkill(universalContent[0]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "DatalinkScan")
                {
                    dynamic[] universalContent = new dynamic[0];

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = eventContent[i];
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.DataLinkScan.datalinkscan(universalContent[0]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                else if (eventName == "ReceiveText")
                {
                    dynamic[] universalContent = new dynamic[0];

                    if (eventContent != null || eventContent.Length != 0)
                    {
                        //For loop to get all attribtes for event
                        for (int i = 0; i < eventContent.Length; i++)
                        {
                            Array.Resize(ref universalContent, i + 1);
                            universalContent[i] = eventContent[i];
                            Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.RecieveText.recievetext(universalContent[3], universalContent[1]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        File.WriteAllText(Path.Combine(pathTest, "shipData.json"), json);
                    }
                }
                
                Console.WriteLine("");
                Console.WriteLine("EVENT DONE");
                Console.WriteLine("");
            }
        }
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