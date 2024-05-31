namespace SteamworksPlus.Runtime.Serializables
{
    /// <summary>
    /// Abstract class for managing steam lobby messages
    /// </summary>
    public abstract class SteamLobbyMessage
	{
        /// <summary>
        /// Message type
        /// </summary>
        public string MessageType { get; set; }
	}
}
