using NotTwice.Events.Runtime.Components.Abstract;
using SteamworksPlus.Runtime.ScriptableObjects.GameEvents;
using SteamworksPlus.Runtime.Serializables;
using UnityEngine;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Components.Listeners
{
    /// <summary>
    /// Listener event for transitioning information of type <see cref="SPLobbyFriendResponse"/> when a <see cref="SPLobbyFriendGameEvent"/> event is raised.
    /// </summary>
    [AddComponentMenu("SteamworksPlus/Listeners/LobbyFriendEventListener")]
	[DisallowMultipleComponent]
    public class SPLobbyFriendEventListener : NTGenericEventListener<SPLobbyFriendGameEvent, SPLobbyFriendResponse>
	{
	}
}
