using System;

namespace SteamworksPlus.Runtime.Serializables
{
    /// <summary>
    /// Callback used to interpret a response of type <see cref="SPLobbyFriendResponse"/> with additional error handling.
    /// </summary>
    [Serializable]
	public class SPLobbyFriendErrCallback : SPLobbyFriendCallback
	{
		public SPErrorCallback SteamErrorCallback;
	}
}
