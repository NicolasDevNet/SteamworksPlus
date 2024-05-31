namespace SteamworksPlus.Runtime.Serializables
{
    /// <summary>
    /// Class used to manage typed messages in a Steam lobby, it allows information to be passed on.
    /// </summary>
    public abstract class SteamDataLobbyMessage : SteamLobbyMessage
	{
        /// <summary>
        /// The type of data to be serialized
        /// </summary>
		public string DataType { get; set; }
	}
}
