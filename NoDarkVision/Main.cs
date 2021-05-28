using System;
using UnityModManagerNet;
using HarmonyLib;
using System.Reflection;

namespace NoDarkVision
{
    public class Main
    {
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Log(string msg)
        {
            if (logger != null) logger.Log(msg);
        }
        public static void Error(Exception ex)
        {
            if (logger != null) logger.Error(ex.ToString());
        }
        public static void Error(string msg)
        {
            if (logger != null) logger.Error(msg);
        }

        public static UnityModManager.ModEntry.ModLogger logger;
        public static bool enabled;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                logger = modEntry.Logger;
                var harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Error(ex);
                throw ex;
            }
            return true;
        }

        //[HarmonyPatch(typeof(MainMenuScreen), "RuntimeLoaded")]
        //static class MainMenuScreen_RuntimeLoaded_Patch

        [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
        internal static class GameManager_BindPostDatabase_Patch
        {
            static void Postfix()
            {
                try
                {

                    Main.Log("Running Check for Dark Vision");
                    foreach (CharacterRaceDefinition characterRaceDefinition in DatabaseRepository.GetDatabase<CharacterRaceDefinition>().GetAllElements())
                    {
                        if (characterRaceDefinition.FeatureUnlocks.Find(x => x.FeatureDefinition.name == "SenseDarkvision") != null)
                        {
                            Main.Log("DV Race: " + characterRaceDefinition.Name);
                            characterRaceDefinition.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition.name == "SenseDarkvision");
                            Main.Log("Has dark vision been removed? - " + (characterRaceDefinition.FeatureUnlocks.Find(x => x.FeatureDefinition.name == "SenseDarkvision") == null));

                        }

                        if (characterRaceDefinition.FeatureUnlocks.Find(x => x.FeatureDefinition.name == "SenseSuperiorDarkvision") != null)
                        {
                            Main.Log(" SDV Race: " + characterRaceDefinition.Name);
                            characterRaceDefinition.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition.name == "SenseSuperiorDarkvision");
                            Main.Log("Has superior dark vision been removed? - " + (characterRaceDefinition.FeatureUnlocks.Find(x => x.FeatureDefinition.name == "SenseSuperiorDarkvision") == null));

                        }
                    }
                }

                catch (Exception ex)
                {
                    Error(ex);
                    throw ex;
                }
            }
        }
    }
}
