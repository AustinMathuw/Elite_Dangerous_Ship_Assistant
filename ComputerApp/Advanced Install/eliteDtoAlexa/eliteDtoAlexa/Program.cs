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
using System.Threading.Tasks;
using System.Text;
using WindowsInput;
using System.Windows.Input;
using InputManager;

public class Program
{
    static public Pubnub pubnubToAlexa = new Pubnub("pub-c-06cf8ccb-5bf9-4a97-aca2-3a5eb322dd92", "sub-c-2955e8f4-c6c7-11e6-b8a7-0619f8945a4f", "", "", true); //Initialize PubNub
    
    static public Pubnub pubnubFromAlexa = new Pubnub("pub-c-18a081e9-f557-4146-b7d3-7847a67dfbaa", "sub-c-367dd992-c6c7-11e6-8164-0619f8945a4f", "", "", true); //Initialize PubNub
    

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
        public LandingGear LandingGear = new LandingGear();
        public Chaff Chaff = new Chaff();
        public Scoop Scoop = new Scoop();
    } //Difines the ship info

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
    public class LandingGear
    {
        public string gearUp { get; set; }
        public void gear(string gearState)
        {
            gearUp = gearState;
        }
    }
    public class Chaff
    {
        public string chaffUp { get; set; }
        public void chaff(string chaffState)
        {
            chaffUp = chaffState;
        }
    }
    public class Scoop
    {
        public string scoopUp { get; set; }
        public void scoop(string scoopState)
        {
            scoopUp = scoopState;
        }
    }

    private static AddedContentReader _continuousFileReaderShip = null;

    public string channelShipCommands;

    public static string channelName;
    public static string publishChannel;
    public static string subscribeChannel;


    public static void Main()
    {
        shipInfo shipInfoMaster = new shipInfo();
        Console.WriteLine(" ______ _____     _____ _     _                          _     _              _    ");
        Console.WriteLine("|  ____|  __ \\   / ____| |   (_)           /\\           (_)   | |            | |   ");
        Console.WriteLine("| |__  | |  | | | (___ | |__  _ _ __      /  \\   ___ ___ _ ___| |_ __ _ _ __ | |_  ");
        Console.WriteLine("|  __| | |  | |  \\___ \\| '_ \\| | '_ \\    / /\\ \\ / __/ __| / __| __/ _` | '_ \\| __| ");
        Console.WriteLine("| |____| |__| |  ____) | | | | | |_) |  / ____ \\\\__ \\__ \\ \\__ \\ || (_| | | | | |_  ");
        Console.WriteLine("|______|_____/  |_____/|_| |_|_| .__/  /_/    \\_\\___/___/_|___/\\__\\__,_|_| |_|\\__|");
        Console.WriteLine("                               | |                                                 ");
        Console.WriteLine("                               |_|                                                ");

        Console.WriteLine();

        Console.Write("Please enter your session ID: ");

        channelName = Console.ReadLine();

        Console.WriteLine();



        pubnubToAlexa.EnableResumeOnReconnect = true;
        pubnubFromAlexa.EnableResumeOnReconnect = true;

        Run(shipInfoMaster);

    } //Main loop

    

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static void Run(shipInfo shipInfoMaster)
    {
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
        shipInfoMaster.LandingGear.gear("");
        shipInfoMaster.Chaff.chaff("");
        shipInfoMaster.Scoop.scoop("");


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

        File.WriteAllText(pathDoc + "\\commandsTo.json", "");

        publishChannel = channelName + "A";
        subscribeChannel = channelName + "B";

        pubnubFromAlexa.Subscribe<string>(
           subscribeChannel,
            HandleRecieveCommands,
            DisplaySubscribeConnectStatusMessage,
            DisplayErrorMessage
        );

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

                    HandleChangedLinesShip(shipInfoMaster);
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

    private static void HandleRecieveCommands(string commandRawJSON)
    {
        if (!string.IsNullOrEmpty(commandRawJSON) && !string.IsNullOrEmpty(commandRawJSON.Trim()))
        {
            List<object> deserializedMessage = pubnubFromAlexa.JsonPluggableLibrary.DeserializeToListOfObject(commandRawJSON);
            if (deserializedMessage != null && deserializedMessage.Count > 0)
            {
                object subscribedObject = (object)deserializedMessage[0];

                if (subscribedObject != null)
                {
                    string resultActualMessage = pubnubFromAlexa.JsonPluggableLibrary.SerializeToJsonString(subscribedObject);
                    string command = resultActualMessage; //Format is "{\"command\":\"deployLandingGear\"}"
                    if (command == "{\"command\":\"boost\"}")
                    {

                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("boost");
                        Console.ResetColor();
                        Task.Run(() => boost());
                    }
                    else if (command == "{\"command\":\"balencePower\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("balencePower");
                        Console.ResetColor();
                        Task.Run(() => balencePower());

                    }
                    else if (command == "{\"command\":\"cancelDocking\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("cancelDocking");
                        Console.ResetColor();
                        Task.Run(() => cancelDocking());

                    }
                    else if (command == "{\"command\":\"deployChaff\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("deployChaff");
                        Console.ResetColor();
                        Task.Run(() => deployChaff());
                    }
                    else if (command == "{\"command\":\"deployHardpoints\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("deployHardpoints");
                        Console.ResetColor();
                        Task.Run(() => deployHardpoints());
                    }
                    else if (command == "{\"command\":\"deployLandingGear\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("deployLandingGear");
                        Console.ResetColor();
                        Task.Run(() => deployLandingGear());
                    }
                    else if (command == "{\"command\":\"deployCargoScoop\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("deployCargoScoop");
                        Console.ResetColor();
                        Task.Run(() => deployCargoScoop());
                    }
                    else if (command == "{\"command\":\"deploySRV\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("deploySRV");
                        Console.ResetColor();
                        Task.Run(() => deploySRV());
                    }
                    else if (command == "{\"command\":\"exitFramshift\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("exitFramshift");
                        Console.ResetColor();
                        Task.Run(() => exitFramshift());
                    }
                    else if (command == "{\"command\":\"exitCruise\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("exitCruise");
                        Console.ResetColor();
                        Task.Run(() => exitCruise());
                    }
                    else if (command == "{\"command\":\"powerToEngines\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("powerToEngines");
                        Console.ResetColor();
                        Task.Run(() => powerToEngines());
                    }
                    else if (command == "{\"command\":\"powerToSystems\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("powerToSystems");
                        Console.ResetColor();
                        Task.Run(() => powerToSystems());
                    }
                    else if (command == "{\"command\":\"powerToWeapons\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("powerToWeapons");
                        Console.ResetColor();
                        Task.Run(() => powerToWeapons());
                    }
                    else if (command == "{\"command\":\"emergencyStop\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("emergencyStop");
                        Console.ResetColor();
                        Task.Run(() => emergencyStop());
                    }
                    else if (command == "{\"command\":\"engageFrameshift\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("engageFrameshift");
                        Console.ResetColor();
                        Task.Run(() => engageFrameshift());
                    }
                    else if (command == "{\"command\":\"engageCruise\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("engageCruise");
                        Console.ResetColor();
                        Task.Run(() => engageCruise());
                    }
                    else if (command == "{\"command\":\"fightAssistOff\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("fightAssistOff");
                        Console.ResetColor();
                        Task.Run(() => fightAssistOff());
                    }
                    else if (command == "{\"command\":\"fightAssistOn\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("fightAssistOn");
                        Console.ResetColor();
                        Task.Run(() => fightAssistOn());
                    }
                    else if (command == "{\"command\":\"targetEnemy\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("targetEnemy");
                        Console.ResetColor();
                        Task.Run(() => targetEnemy());
                    }
                    else if (command == "{\"command\":\"screenshot\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("screenshot");
                        Console.ResetColor();
                        Task.Run(() => screenshot());
                    }
                    else if (command == "{\"command\":\"launch\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("launch");
                        Console.ResetColor();
                        Task.Run(() => launch());
                    }
                    else if (command == "{\"command\":\"lightsOff\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("lightsOff");
                        Console.ResetColor();
                        Task.Run(() => lightsOff());
                    }
                    else if (command == "{\"command\":\"lightsOn\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("lightsOn");
                        Console.ResetColor();
                        Task.Run(() => lightsOn());
                    }
                    else if (command == "{\"command\":\"enginesForward100\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesForward100");
                        Console.ResetColor();
                        Task.Run(() => enginesForward100());
                    }
                    else if (command == "{\"command\":\"enginesForward90\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesForward90");
                        Console.ResetColor();
                        Task.Run(() => enginesForward90());
                    }
                    else if (command == "{\"command\":\"enginesForward80\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesForward80\"}");
                        Console.ResetColor();
                        Task.Run(() => enginesForward80());
                    }
                    else if (command == "{\"command\":\"enginesForward75\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesForward75");
                        Console.ResetColor();
                        Task.Run(() => enginesForward75());
                    }
                    else if (command == "{\"command\":\"enginesForward70\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesForward70");
                        Console.ResetColor();
                        Task.Run(() => enginesForward70());
                    }
                    else if (command == "{\"command\":\"enginesForward60\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesForward60");
                        Console.ResetColor();
                        Task.Run(() => enginesForward60());
                    }
                    else if (command == "{\"command\":\"enginesForward50\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesForward50");
                        Console.ResetColor();
                        Task.Run(() => enginesForward50());
                    }
                    else if (command == "{\"command\":\"enginesForward40\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesForward40");
                        Console.ResetColor();
                        Task.Run(() => enginesForward40());
                    }
                    else if (command == "{\"command\":\"enginesForward30\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesForward30");
                        Console.ResetColor();
                        Task.Run(() => enginesForward30());
                    }
                    else if (command == "{\"command\":\"enginesForward25\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesForward25");
                        Console.ResetColor();
                        Task.Run(() => enginesForward25());
                    }
                    else if (command == "{\"command\":\"enginesForward20\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesForward20");
                        Console.ResetColor();
                        Task.Run(() => enginesForward20());
                    }
                    else if (command == "{\"command\":\"enginesForward10\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesForward10");
                        Console.ResetColor();
                        Task.Run(() => enginesForward10());
                    }
                    else if (command == "{\"command\":\"nextFireGroup\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("nextFireGroup");
                        Console.ResetColor();
                        Task.Run(() => nextFireGroup());
                    }
                    else if (command == "{\"command\":\"nextFireGroup\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("nextFireGroup");
                        Console.ResetColor();
                        Task.Run(() => nextFireGroup());
                    }
                    else if (command == "{\"command\":\"nextHostile\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("nextHostile");
                        Console.ResetColor();
                        Task.Run(() => nextHostile());
                    }
                    else if (command == "{\"command\":\"nextSystem\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("nextSystem");
                        Console.ResetColor();
                        Task.Run(() => nextSystem());
                    }
                    else if (command == "{\"command\":\"nextShip\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("nextShip");
                        Console.ResetColor();
                        Task.Run(() => nextShip());
                    }
                    else if (command == "{\"command\":\"prevFireGroup\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("prevFireGroup");
                        Console.ResetColor();
                        Task.Run(() => prevFireGroup());
                    }
                    else if (command == "{\"command\":\"prevHostile\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("prevHostile");
                        Console.ResetColor();
                        Task.Run(() => prevHostile());
                    }
                    else if (command == "{\"command\":\"prevShip\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("prevShip");
                        Console.ResetColor();
                        Task.Run(() => prevShip());
                    }
                    else if (command == "{\"command\":\"requestDocking\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("requestDocking");
                        Console.ResetColor();
                        Task.Run(() => requestDocking());
                    }
                    else if (command == "{\"command\":\"centerHeadset\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("centerHeadset");
                        Console.ResetColor();
                        Task.Run(() => centerHeadset());
                    }
                    else if (command == "{\"command\":\"retractHardpoints\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("retractHardpoints");
                        Console.ResetColor();
                        Task.Run(() => retractHardpoints());
                    }
                    else if (command == "{\"command\":\"retractLandingGear\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("retractLandingGear");
                        Console.ResetColor();
                        Task.Run(() => retractLandingGear());
                    }
                    else if (command == "{\"command\":\"retractCargoScoop\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("retractCargoScoop\"}");
                        Console.ResetColor();
                        Task.Run(() => retractCargoScoop());
                    }
                    else if (command == "{\"command\":\"enginesBack100\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesBack100");
                        Console.ResetColor();
                        Task.Run(() => enginesBack100());
                    }
                    else if (command == "{\"command\":\"enginesBack75\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesBack75");
                        Console.ResetColor();
                        Task.Run(() => enginesBack75());
                    }
                    else if (command == "{\"command\":\"enginesBack50\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesBack50");
                        Console.ResetColor();
                        Task.Run(() => enginesBack50());
                    }
                    else if (command == "{\"command\":\"enginesBack25\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("enginesBack25");
                        Console.ResetColor();
                        Task.Run(() => enginesBack25());
                    }
                    else if (command == "{\"command\":\"SRVRecovery\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("SRVRecovery");
                        Console.ResetColor();
                        Task.Run(() => SRVRecovery());
                    }
                    else if (command == "{\"command\":\"cutEngines\"}")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("cutEngines");
                        Console.ResetColor();
                        Task.Run(() => cutEngines());
                    }
                }
            }
        }
        
                            
    } // When new command is found, assosiate it to a function
    public static void boost()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Tab);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Tab);
        }
        return;
    } //Handles key presses for boost function

    public static void balencePower()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Down);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Down);
        }
    } //Handles key presses for balance power function

    public static void cancelDocking()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.D1);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.D1);
            Thread.Sleep(500);
            Keyboard.KeyDown(Keys.E);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.E);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.E);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.E);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Q);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Q);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Q);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Q);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.D1);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.D1);
        }
    } //Handles key presses for cancel docking function

    public static void deployChaff()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.C);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.C);
        }
    } //Handles key presses for deploy chaff function

    public static void deployHardpoints()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.U);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.U);
        }
    } //Handles key presses for deploy hardpoints function

    public static void deployLandingGear()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            InputSimulator test = new InputSimulator();
            Keyboard.KeyDown(Keys.L);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.L);
        }
    } //Handles key presses for deploy landing gear function

    public static void deployCargoScoop()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Home);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Home);
        }
    } //Handles key presses for deploy cargo scoop function

    public static void deploySRV()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.D3);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.D3);
            Thread.Sleep(500);
            Keyboard.KeyDown(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.W);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.W);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.D3);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.D3);
        }
    } //Handles key presses for deploy SRV function

    public static void exitFramshift()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.J);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.J);
        }
    } //Handles key presses for exit frameshift function

    public static void exitCruise()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Divide);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Divide);
        }
    } //Handles key presses for exit cruise function

    public static void powerToEngines()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Up);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Up);
        }
    } //Handles key presses for power to engines function

    public static void powerToSystems()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Left);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Left);
        }
    } //Handles key presses for power to systems function

    public static void powerToWeapons()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Right);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Right);
        }
    } //Handles key presses for power to weapons function

    public static void emergencyStop()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.J);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.J);
        }
    } //Handles key presses for emegency stop function

    public static void engageFrameshift()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.J);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.J);
        }
    } //Handles key presses for engage frameshift function

    public static void engageCruise()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Divide);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Divide);
        }
    } //Handles key presses for engage crusie function

    public static void fightAssistOff()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Z);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Z);
        }
    } //Handles key presses for fight assist off function

    public static void fightAssistOn()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Z);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Z);
        }
    } //Handles key presses for flight assist on function

    public static void targetEnemy()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Y);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Y);
        }
    } //Handles key presses for target enemy function

    public static void screenshot()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.F10);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.F10);
        }
    } //Handles key presses for screenshot function

    public static void launch()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Back);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Back);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Back);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Back);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Back);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Back);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Space);
        }
    } //Handles key presses for launch function

    public static void lightsOff()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Insert);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Insert);
        }
    } //Handles key presses for lights off function

    public static void lightsOn()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Insert);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Insert);
        }
    } //Handles key presses for lights on function

    public static void enginesForward100()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Add);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Add);
        }
    } //Handles key presses for engines forward 100% function

    public static void enginesForward90()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Add);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Add);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.S);
        }
    } //Handles key presses for engines forward 90% function

    public static void enginesForward80()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Add);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Add);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.S);
        }
    } //Handles key presses for engines forward 80% function

    public static void enginesForward75()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.NumPad3);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.NumPad3);
        }
    } //Handles key presses for engines forward 75% function

    public static void enginesForward70()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.NumPad2);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.NumPad2);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.W);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.W);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.W);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.W);
        }
    } //Handles key presses for engines forward 70% function

    public static void enginesForward60()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.NumPad2);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.NumPad2);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.W);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.W);
        }
    } //Handles key presses for engines forward 60% function

    public static void enginesForward50()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.NumPad2);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.NumPad2);
        }
    } //Handles key presses for engines forward 50% function

    public static void enginesForward40()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.NumPad2);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.NumPad2);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.S);
        }
    } //Handles key presses for engines forward 40% function

    public static void enginesForward30()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.NumPad2);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.NumPad2);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.S);
        }
    } //Handles key presses for engines forward 30% function

    public static void enginesForward25()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.NumPad1);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.NumPad1);
        }
    } //Handles key presses for engines forward 25% function

    public static void enginesForward20()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.X);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.X);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.W);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.W);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.W);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.W);
        }
    } //Handles key presses for engines forward 20% function

    public static void enginesForward10()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.X);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.X);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.W);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.W);
        }
    } //Handles key presses for engines forward 10% function

    public static void nextFireGroup()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.RShiftKey);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.OemPeriod);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.OemPeriod);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.RShiftKey);
        }
    } //Handles key presses for next fire group select function

    public static void nextHostile()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.H);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.H);
        }
    } //Handles key presses for next hostile select function

    public static void nextSystem()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.M);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.M);
        }
    } //Handles key presses for next system select function

    public static void nextShip()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.G);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.G);
        }
    } //Handles key presses for next ship select function

    public static void prevFireGroup()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.RShiftKey);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Oemcomma);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Oemcomma);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.RShiftKey);
        }
    } //Handles key presses for previous fire group select function

    public static void prevHostile()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.N);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.N);
        }
    } //Handles key presses for previous hostile select function

    public static void prevShip()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.B);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.B);
        }
    } //Handles key presses for previous ship select function

    public static void requestDocking()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.D1);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.D1);
            Thread.Sleep(500);
            Keyboard.KeyDown(Keys.E);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.E);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.E);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.E);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Q);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Q);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Q);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Q);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.D1);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.D1);
        }
    } //Handles key presses for request docking function

    public static void centerHeadset()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.F12);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.F12);
        }
    } //Handles key presses for center headset function

    public static void retractHardpoints()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.U);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.U);
        }
    } //Handles key presses for retract hardpoints function

    public static void retractLandingGear()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.L);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.L);
        }
    } //Handles key presses for retract landing gear function

    public static void retractCargoScoop()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Home);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Home);
        }
    } //Handles key presses for retract cargo scoop function

    public static void enginesBack100()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.Subtract);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Subtract);
        }
    } //Handles key presses for engines reverse 100% function

    public static void enginesBack75()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.NumPad9);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.NumPad9);
        }
    } //Handles key presses for engines reverse 75% function

    public static void enginesBack50()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.NumPad8);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.NumPad8);
        }
    } //Handles key presses for engines reverse 50% function

    public static void enginesBack25()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.NumPad7);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.NumPad7);
        }
    } //Handles key presses for engines reverse 25% function

    public static void SRVRecovery()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.D3);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.D3);
            Thread.Sleep(500);
            Keyboard.KeyDown(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.S);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.Space);
            Thread.Sleep(30);
            Keyboard.KeyDown(Keys.D3);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.D3);
        }
    } //Handles key presses for SRV recovery function

    public static void cutEngines()
    {
        Process p = Process.GetProcessesByName("EliteDangerous64").FirstOrDefault();
        if (p != null)
        {
            IntPtr h = p.MainWindowHandle;
            SetForegroundWindow(h);
            Keyboard.KeyDown(Keys.X);
            Thread.Sleep(30);
            Keyboard.KeyUp(Keys.X);
        }
    } //Handles key presses for cut engines function

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
                            //Console.WriteLine(eventName);
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
                
                
                //Console.WriteLine("");
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Rank.rank(universalContent[0], universalContent[1], universalContent[2], universalContent[3], universalContent[4], universalContent[5]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Progress.progress(universalContent[0], universalContent[1], universalContent[2], universalContent[3], universalContent[4], universalContent[5]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Docked.docked(true, universalContent[0]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Docked.docked(false, "");

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                            if(eventContentAttributes[i] == "Docked" && universalContent[i] == "True")
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
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Location.location(universalContent[0], "");

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Location.location(universalContent[0], universalContent[1]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Touchdown.touchdown(true);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.Touchdown.touchdown(false);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.HullDamage.hulldamage(Convert.ToDouble(universalContent[1]));

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.ShieldState.shieldstate(Convert.ToBoolean(universalContent[0]));

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.PVPKill.pvpkill(universalContent[0]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.DataLinkScan.datalinkscan(universalContent[0]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
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
                            //Console.WriteLine("{0}: {1}", eventContentAttributes[i], universalContent[i]);
                        }
                        //This is where event attributes are built using the predefined object
                        shipInfoMaster.RecieveText.recievetext(universalContent[3], universalContent[1]);

                        var json = new JavaScriptSerializer().Serialize(shipInfoMaster);
                        pubnubToAlexa.Publish<string>(
                            publishChannel,
                            json,
                            DisplayReturnMessage,
                            DisplayErrorMessage
                        );
                    }
                }
                
                //Console.WriteLine("");
                //Console.WriteLine("EVENT DONE");
                //Console.WriteLine("");
            }
        }
    } //Handles the ship info update when new event is logged

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
    } //Checks for added content 

    public static void DisplaySubscribeConnectStatusMessage(string result)
    {
        Console.WriteLine();
        Console.WriteLine("CONNECTED");
        Console.WriteLine("Ready for Commands. Please make sure Elite: Dangerous is running. Enjoy!");
        Console.WriteLine();
        //Console.WriteLine(result);
    }

    public static void DisplayErrorMessage(PubnubClientError pubnubError)
    {

    }

    public static void DisplayReturnMessage(string result)
    {
        //Console.WriteLine("PUBLISH STATUS CALLBACK");
        //Console.WriteLine(result);
    }

}