using NaughtyAttributes;
using Steamworks;
using Steamworks.Data;
using SteamworksPlus.Runtime.Serializables;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Components
{
	public abstract class SteamResponseFriendEvent<T> : MonoBehaviour
		where T : class
	{
		[Required]
		public LobbySettings LobbySettings;

		public UnityEvent<Lobby, Friend, T> UnityEvent;

		public void Execute(Lobby lobby, Friend friend, string content)
		{
			string[] args = content.Split(LobbySettings.ChatDataMessageSeparator);

			Type targetType = typeof(T);

			if (!string.Equals(args[0].ToLowerInvariant(), targetType.Name.ToLowerInvariant()))
			{
				return;
			}

			Debug.Log($"Receive a message of type: {args[0]}");

			UnityEvent?.Invoke(lobby, friend, JsonUtility.FromJson(args[1], targetType) as T);
		}
	}
}
