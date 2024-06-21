using NotTwice.Events.Runtime.Serializables.Abstract;
using SteamworksPlus.Runtime.ScriptableObjects.GameEvents;
using System;

namespace SteamworksPlus.Runtime.Serializables
{
    /// <summary>
    /// Callback used to interpret a response of type <see cref="SPLobbyFriendResponse"/>.
    /// </summary>
    [Serializable]
	public class SPLobbyFriendCallback : NTGenericEventTypeSwitcher<SPLobbyFriendGameEvent, SPLobbyFriendResponse>
	{

	}
}
