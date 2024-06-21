using NotTwice.Events.Runtime.ScriptableObjects.Abstract;
using SteamworksPlus.Runtime.Serializables;
using UnityEngine;

namespace SteamworksPlus.Runtime.ScriptableObjects.GameEvents
{
    /// <summary>
    /// Scriptable event for passing information of type <see cref="SPLobbyFriendMessageResponse"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "LobbyFriendMessageGameEvent", menuName = "SteamworksPlus/Events/LobbyFriendMessageGameEvent")]

    public class SPLobbyFriendMessageGameEvent : NTGenericGameEvent<SPLobbyFriendMessageGameEvent, SPLobbyFriendMessageResponse>
	{
	}
}
