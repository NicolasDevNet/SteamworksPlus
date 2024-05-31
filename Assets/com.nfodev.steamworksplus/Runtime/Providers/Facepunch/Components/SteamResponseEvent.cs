using NaughtyAttributes;
using Steamworks.Data;
using SteamworksPlus.Runtime.Serializables;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Components
{
    /// <summary>
    /// Abstract class for interpreting a Steam lobby message
    /// </summary>
    /// <typeparam name="T">The type of information transmitted</typeparam>
    public abstract class SteamResponseEvent<T> : MonoBehaviour
		where T : class
	{
        /// <summary>
        /// Lobby parameters required to read the message as data
        /// </summary>
        [Expandable, Required, Tooltip("Lobby parameters required to read the message as data")]
        public LobbySettings LobbySettings;

		public UnityEvent<Lobby, T> UnityEvent;

        /// <summary>
        /// Method to execute the callback following a Steam event
        /// </summary>
        /// <param name="lobby">The message's lobby of origin</param>
        /// <param name="content">Chain message content</param>
        public void Execute(Lobby lobby, string content)
		{
			string[] args = content.Split(LobbySettings.ChatDataMessageSeparator);

			Type targetType = typeof(T);

			if (!string.Equals(args[0].ToLowerInvariant(), targetType.Name.ToLowerInvariant()))
			{
				return;
			}

			Debug.Log($"Receive a message of type: {args[0]}");

			UnityEvent?.Invoke(lobby, JsonUtility.FromJson(args[1], targetType) as T);
		}
	}
}
