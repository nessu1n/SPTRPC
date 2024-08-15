using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using DiscordRPC;
using RichPresenceAPI;
using RichPresenceAPI.Logging;
using BepInEx.Logging;
using RichPresenceAPI.Native;
using System.Runtime.InteropServices;

namespace SPTRPC
{
    // first string below is your plugin's GUID, it MUST be unique to any other mod. Read more about it in BepInEx docs. Be sure to update it if you copy this project.
    [BepInPlugin("nessu1n.SPTRPC", "SPTRPC", "1.0.0")]
    [BepInDependency("io.github.xhayper.RichPresenceAPI")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;
        public static DiscordRpcClient client;
        private bool isInitialized = false;
        public static bool firstTimeInMenu = true; // Bool variable to control RPC updates in the menu screen



        // BaseUnityPlugin inherits MonoBehaviour, so you can use base unity functions like Awake() and Update()
        private void Awake()
        {
            // save the Logger to variable so we can use it elsewhere in the project
            LogSource = Logger;
            LogSource.LogInfo("SPTRPC Loaded, Initializing RPC...");

            if (!isInitialized) // If condition to ensure the RPC connection is only initialized once upon plugin load.
            {
                LoadDiscordRPC();
                isInitialized = true;
            }

            // uncomment line(s) below to enable desired example patch, then press F6 to build the project:
            new RaidMapInfo().Enable();
            new MenuPatch().Enable();
        }

        private void LoadDiscordRPC()
        {
            DateTime startTime = DateTime.UtcNow;
            Logger.LogInfo(startTime);

            client = RichPresenceAPI.Utility.CreateDiscordRpcClient("1273226966950084688");

            client.Logger = new BepInExLogger(Logger)
            {
                Level = DiscordRPC.Logging.LogLevel.Info
            };

            client.Initialize();

            LogSource.LogInfo("Setting presence...");
            client.SetPresence(new RichPresence()
                {
                    State = "Loading into the Menu",
                    Timestamps = new Timestamps()
                    {
                        Start = startTime,
                        End = null
                    },
                    Assets = new Assets()
                    {
                        LargeImageKey = "mainmenuimage"
                    }
                });
            LogSource.LogInfo("RPC initialized successfully!");
        }

        private void OnDestroy()
        {
            if (client != null)
            {
                client.Dispose();
                LogSource.LogInfo("RPC Client disposed.");
            }
        }
    }
}