using NotTwice.Events.Runtime.Serializables.Abstract;
using SteamworksPlus.Runtime.ScriptableObjects.GameEvents;
using System;

namespace SteamworksPlus.Runtime.Serializables
{
    /// <summary>
    /// Callback used to interpret a response of type <see cref="SPLobbyFriendMessageResponse"/>.
    /// </summary>
    [Serializable]
	public class SPLobbyFriendMessageCallback : NTGenericEventTypeSwitcher<SPLobbyFriendMessageGameEvent, SPLobbyFriendMessageResponse>
	{

	}
}
