using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine;
using SteamworksPlus.Runtime.Providers.Facepunch.Components;

namespace SteamworksPlus.Runtime.Serializables
{
    /// <summary>
    /// Response class used to transmit information related to a lobby message
    /// <para>There is also a method for interpreting a data message</para>
    /// </summary>
	[Serializable]
	public class SPLobbyFriendMessageResponse : SPLobbyFriendResponse
	{
		public SPLobbyFriendMessageResponse(Lobby lobby, Friend friend, string message)
        :base(lobby, friend)
        {
            Message = message;
        }

		public string Message { get; private set; }

        /// <summary>
        /// Method for interpreting information contained in a lobby message
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="data">The output data</param>
        /// <returns>The success of the operation</returns>
        public bool TryReadData<T>(out T data)
            where T : class
		{
            data = null;

            if (string.IsNullOrEmpty(Message))
            {
                return false;
            }

            string[] args = Message.Split(SPLobbyManager.Settings.ChatDataMessageSeparator);

            Type targetType = typeof(T);

            if (!string.Equals(args[0].ToLowerInvariant(), targetType.Name.ToLowerInvariant()))
            {
                return false;
            }

            Debug.Log($"Receive a message of type: {args[0]}");

            data = JsonUtility.FromJson(args[1], targetType) as T;

            return true;
        }
	}
}
