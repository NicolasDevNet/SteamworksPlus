using Steamworks;
using Steamworks.Data;
using System;

namespace SteamworksPlus.Runtime.Serializables
{
    /// <summary>
    /// Response class used to transmit information related to a lobby data
    /// </summary>
    [Serializable]
	public class SPLobbyFriendResponse
	{
		public SPLobbyFriendResponse(Lobby lobby, Friend friend)
		{
			Lobby = lobby;
			Friend = friend;
		}

		public Lobby Lobby { get; private set; }

		public Friend Friend { get; private set; }
		
	}
}
