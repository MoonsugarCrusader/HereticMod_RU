using UnityEngine;
using RoR2;
using BepInEx.Configuration;

namespace HereticMod
{
    public class Squawk
    {
        public static bool initialized = false;

        public static void Init()
        {
            if (initialized) return;
            initialized = false;
        }


        //Taken from https://github.com/ToastedOven/CustomEmotesAPI/blob/main/CustomEmotesAPI/CustomEmotesAPI/CustomEmotesAPI.cs
        private static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
    }
}
