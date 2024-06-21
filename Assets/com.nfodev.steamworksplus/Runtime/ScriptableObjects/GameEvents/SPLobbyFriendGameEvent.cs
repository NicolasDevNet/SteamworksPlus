using NotTwice.Events.Runtime.ScriptableObjects.Abstract;
using SteamworksPlus.Runtime.Serializables;
using UnityEngine;

namespace SteamworksPlus.Runtime.ScriptableObjects.GameEvents
{
    /// <summary>
    /// Scriptable event for passing information of type <see cref="SPLobbyFriendResponse"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "LobbyFriendGameEvent", menuName = "SteamworksPlus/Events/LobbyFriendGameEvent")]
    public class SPLobbyFriendGameEvent : NTGenericGameEvent<SPLobbyFriendGameEvent, SPLobbyFriendResponse>
	{
	}
}
