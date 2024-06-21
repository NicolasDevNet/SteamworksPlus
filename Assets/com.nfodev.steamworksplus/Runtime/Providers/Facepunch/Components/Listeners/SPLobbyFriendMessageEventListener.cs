using NotTwice.Events.Runtime.Components.Abstract;
using SteamworksPlus.Runtime.ScriptableObjects.GameEvents;
using SteamworksPlus.Runtime.Serializables;
using UnityEngine;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Components.Listeners
{
    /// <summary>
    /// Listener event for transitioning information of type <see cref="SPLobbyFriendMessageResponse"/> when a <see cref="SPLobbyFriendMessageGameEvent"/> event is raised.
    /// </summary>
    [AddComponentMenu("SteamworksPlus/Listeners/LobbyFriendMessageEventListener")]
    [DisallowMultipleComponent]
    public class SPLobbyFriendMessageEventListener : NTGenericEventListener<SPLobbyFriendMessageGameEvent, SPLobbyFriendMessageResponse>
	{
	}
}
