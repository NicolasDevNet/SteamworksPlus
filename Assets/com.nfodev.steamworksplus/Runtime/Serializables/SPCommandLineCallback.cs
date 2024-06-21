using NotTwice.Events.Runtime.ScriptableObjects.Events;
using NotTwice.Events.Runtime.Serializables.Abstract;
using System;

namespace SteamworksPlus.Runtime.Serializables
{
    /// <summary>
    /// Callback used to interpret a command line received at application startup
    /// </summary>
    [Serializable]
    public class SPCommandLineCallback : NTGenericEventTypeSwitcher<NTStringGameEvent, string>
    {
    }
}
