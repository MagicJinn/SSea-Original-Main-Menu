using BepInEx;
using Sunless.Game.Scripts.Menus;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OriginalMainMenu;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private const string CONFIG_FILENAME = "OriginalMainMenu.ini";
    public static Dictionary<string, bool> ConfigOptions = new();

    private void Awake()
    {
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        LoadConfig();

        Harmony.CreateAndPatchAll(typeof(TitleScreenInitPatch));
    }

    private static class TitleScreenInitPatch
    {
        [HarmonyPatch(typeof(TitleScreenInit), "Start")]
        [HarmonyPostfix]
        private static void Postfix(TitleScreenInit __instance)
        {
            // Is this code messy? Yes. Is it what was already like this in the original game? Also yes.
            if (ConfigOptions["ReplaceTitle"])
            {
                Transform child = GameObject.Find("TitlePanel").transform.GetChild(0);
                Transform child2 = GameObject.Find("TitlePanel").transform.GetChild(1);
                child.gameObject.SetActive(true);
                child2.gameObject.SetActive(false);
            }
            if (ConfigOptions["ReplaceBackground"])
            {
                __instance.ZubmarinerScene.SetActive(false);
                __instance.VanillaScene.SetActive(true);
            }
        }
    }

    private void LoadConfig(bool loadDefault = false)
    {
        string[] lines;
        if (File.Exists(CONFIG_FILENAME) && !loadDefault)
        {
            lines = File.ReadAllLines(CONFIG_FILENAME);
        }
        else
        {
            Logger.LogWarning("Config not found or corrupt, using default values.");
            string file = ReadTextResource(GetEmbeddedPath() + CONFIG_FILENAME); // Get the default config from the embedded resources
            lines = file.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries); // Split the file into lines
        }

        var optionsDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            foreach (var line in lines)
            {
                if (line.Contains('=')) // Check if the line contains an '=' character so it's a valid config line
                {
                    // Remove all spaces from the line and split it at the first occurrence of '=' into two parts
                    string[] keyValue = line.Replace(" ", "").Split(new[] { '=' }, 2);
                    optionsDict[keyValue[0]] = keyValue[1]; // Add the key and value to the dictionary
                }
            }

            ConfigOptions["ReplaceTitle"] = bool.Parse(optionsDict["ReplaceTitle"]);
            ConfigOptions["ReplaceBackground"] = bool.Parse(optionsDict["ReplaceBackground"]);
        }
        catch (Exception)
        {
            LoadConfig( /*loadDefault =*/ true); // Load config with default values
        }
    }

    private static string GetEmbeddedPath(string folderName = "") // Get the path of embedded resources
    {
        string projectName = Assembly.GetExecutingAssembly().GetName().Name;
        string fullPath = $"{projectName}.{folderName}";
        return fullPath;
    }

    private string ReadTextResource(string fullResourceName)
    {
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullResourceName);
        if (stream == null)
        {
            Logger.LogWarning("Tried to get resource that doesn't exist: " + fullResourceName);
            return null; // Return null if the embedded resource doesn't exist
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd(); // Read and return the embedded resource
    }
}
