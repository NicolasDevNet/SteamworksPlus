using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Proxies
{
    /// <summary>
    /// Default flat-pass class used to make calls to Steam, using the facepunch library
    /// </summary>
    public class FacepunchSteamProxy : IFacepunchSteam
	{
		private List<Friend> _cachedFriends;

		public FacepunchSteamProxy()
		{
			_cachedFriends = new List<Friend>();
		}

		#region SteamClient

		public void Init(uint appId, bool asyncCallbacks)
		{
			SteamClient.Init(appId, asyncCallbacks);
		}

		public void Shutdown()
		{
			SteamClient.Shutdown();
		}

		public void RunCallbacks()
		{
			SteamClient.RunCallbacks();
		}

		public string GetName()
		{
			return SteamClient.Name;
		}

		public SteamId GetSteamId()
		{
			return SteamClient.SteamId;
		}

		#endregion

		#region Data

		public void UnlockSuccess(string key)
		{
			try
			{
				var achievement = new Achievement(key);

				if (!achievement.State)
				{
					Debug.Log($"Success {key} unlocked.");
					achievement.Trigger();
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		#endregion

		#region SteamFriends

		public async Task<Image?> GetLargeAvatarAsync()
		{
			return await GetLargeAvatarAsync(SteamClient.SteamId);
		}

		public async Task<Image?> GetLargeAvatarAsync(SteamId targetId)
		{
			return await SteamFriends.GetLargeAvatarAsync(targetId);
		}

		public void OpenGameInviteOverlay(SteamId lobbyId)
		{
			SteamFriends.OpenGameInviteOverlay(lobbyId);
		}

		public List<Friend> GetFriends()
		{

			if (_cachedFriends == null)
			{
				_cachedFriends = new List<Friend>(SteamFriends.GetFriends());
			}

			return _cachedFriends;
		}

		public Friend? GetFriend(SteamId friendId)
		{
			return GetFriends().FirstOrDefault(p => p.Id == friendId);
		}

		public void SetOnGameLobbyJoinRequested(Action<Lobby, SteamId> onGameLobbyJoinRequested)
		{
			SteamFriends.OnGameLobbyJoinRequested += onGameLobbyJoinRequested;
		}

		public void RemoveOnGameLobbyJoinRequested(Action<Lobby, SteamId> onGameLobbyJoinRequested)
		{
			SteamFriends.OnGameLobbyJoinRequested -= onGameLobbyJoinRequested;
		}

		public void SetOnGameRichPresenceJoinRequested(Action<Friend, string> onGameRichPresenceJoinRequested)
		{
			SteamFriends.OnGameRichPresenceJoinRequested += onGameRichPresenceJoinRequested;
		}

		public void RemoveOnGameRichPresenceJoinRequested(Action<Friend, string> onGameRichPresenceJoinRequested)
		{
			SteamFriends.OnGameRichPresenceJoinRequested -= onGameRichPresenceJoinRequested;
		}

		#endregion

		#region Mathmaking

		public async Task<Lobby?> JoinLobby(SteamId lobbyId)
		{
			return await SteamMatchmaking.JoinLobbyAsync(lobbyId);
		}

		public async Task<Lobby[]> GetLobbyList()
		{			
			return await SteamMatchmaking.LobbyList.WithSlotsAvailable(1).FilterDistanceWorldwide().RequestAsync();
		}

		public async Task<Lobby[]> GetFriendsLobbies()
		{
			Lobby[] lobbyList = await GetLobbyList();

			List<Friend> friendList = GetFriends();

			if (lobbyList == null)
			{
				return new Lobby[0];
			}

			return lobbyList.Where(l => friendList.Any(f => f.Id == l.Owner.Id && !f.IsMe)).ToArray();
		}

		public async Task<Lobby?> CreatelobbyAsync(int maxMembers = 100)
		{
			return await SteamMatchmaking.CreateLobbyAsync(maxMembers);
		}

		public void SetOnLobbyCreated(Action<Result, Lobby> onLobbyCreated)
		{
			SteamMatchmaking.OnLobbyCreated += onLobbyCreated;
		}

		public void SetOnLobbyEntered(Action<Lobby> onLobbyEntered)
		{
			SteamMatchmaking.OnLobbyEntered += onLobbyEntered;
		}

		public void SetOnLobbyInvite(Action<Friend, Lobby> onLobbyInvite)
		{
			SteamMatchmaking.OnLobbyInvite += onLobbyInvite;
		}

		public void SetOnLobbyGameCreated(Action<Lobby, uint, ushort, SteamId> onLobbyGameCreated)
		{
			SteamMatchmaking.OnLobbyGameCreated += onLobbyGameCreated;
		}

		public void SetOnLobbyMemberJoined(Action<Lobby, Friend> onLobbyMemberJoined)
		{
			SteamMatchmaking.OnLobbyMemberJoined += onLobbyMemberJoined;
		}

		public void SetOnLobbyMemberKicked(Action<Lobby, Friend, Friend> onLobbyMemberKicked)
		{
			SteamMatchmaking.OnLobbyMemberKicked += onLobbyMemberKicked;
		}

		public void SetOnChatMessage(Action<Lobby, Friend, string> onChatMessage)
		{
			SteamMatchmaking.OnChatMessage += onChatMessage;
		}

		public void SetOnLobbyMemberLeave(Action<Lobby, Friend> onLobbyMemberLeave)
		{
			SteamMatchmaking.OnLobbyMemberLeave += onLobbyMemberLeave;
		}

		public void SetOnLobbyMemberDisconnected(Action<Lobby, Friend> onLobbyMemberDisconnected)
		{
			SteamMatchmaking.OnLobbyMemberDisconnected += onLobbyMemberDisconnected;
		}

		public void SetOnLobbyDataChanged(Action<Lobby> onLobbyDataChanged)
		{
			SteamMatchmaking.OnLobbyDataChanged += onLobbyDataChanged;
		}

		public void RemoveOnLobbyDataChanged(Action<Lobby> onLobbyDataChanged)
		{
			SteamMatchmaking.OnLobbyDataChanged -= onLobbyDataChanged;
		}

		public void RemoveOnLobbyCreated(Action<Result, Lobby> onLobbyCreated)
		{
			SteamMatchmaking.OnLobbyCreated -= onLobbyCreated;
		}

		public void RemoveOnLobbyEntered(Action<Lobby> onLobbyEntered)
		{
			SteamMatchmaking.OnLobbyEntered -= onLobbyEntered;
		}

		public void RemoveOnLobbyInvite(Action<Friend, Lobby> onLobbyInvite)
		{
			SteamMatchmaking.OnLobbyInvite -= onLobbyInvite;
		}

		public void RemoveOnLobbyGameCreated(Action<Lobby, uint, ushort, SteamId> onLobbyGameCreated)
		{
			SteamMatchmaking.OnLobbyGameCreated -= onLobbyGameCreated;
		}

		public void RemoveOnLobbyMemberJoined(Action<Lobby, Friend> onLobbyMemberJoined)
		{
			SteamMatchmaking.OnLobbyMemberJoined -= onLobbyMemberJoined;
		}

		public void RemoveOnLobbyMemberKicked(Action<Lobby, Friend, Friend> onLobbyMemberKicked)
		{
			SteamMatchmaking.OnLobbyMemberKicked -= onLobbyMemberKicked;
		}

		public void RemoveOnLobbyMemberLeave(Action<Lobby, Friend> onLobbyMemberLeave)
		{
			SteamMatchmaking.OnLobbyMemberLeave -= onLobbyMemberLeave;
		}

		public void RemoveOnLobbyMemberDisconnected(Action<Lobby, Friend> onLobbyMemberDisconnected)
		{
			SteamMatchmaking.OnLobbyMemberDisconnected -= onLobbyMemberDisconnected;
		}

		public void RemoveOnChatMessage(Action<Lobby, Friend, string> onChatMessage)
		{
			SteamMatchmaking.OnChatMessage -= onChatMessage;
		}

		#endregion
	}

}
