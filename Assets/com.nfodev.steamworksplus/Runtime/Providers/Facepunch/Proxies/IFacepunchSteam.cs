using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SteamworksPlus.Runtime.Providers.Facepunch.Proxies
{
    /// <summary>
    /// Flat-pass interface for implementing common Steam methods
    /// </summary>
    public interface IFacepunchSteam
	{
		Task<Lobby?> CreatelobbyAsync(int maxMembers = 100);
		Friend? GetFriend(SteamId friendId);
		List<Friend> GetFriends();
		Task<Image?> GetLargeAvatarAsync();
		Task<Image?> GetLargeAvatarAsync(SteamId targetId);
		string GetName();
		SteamId GetSteamId();
		void Init(uint appId, bool asyncCallbacks);
		void SetOnGameLobbyJoinRequested(Action<Lobby, SteamId> onGameLobbyJoinRequested);
		void SetOnLobbyCreated(Action<Result, Lobby> onLobbyCreated);
		void SetOnLobbyEntered(Action<Lobby> onLobbyEntered);
		void SetOnLobbyGameCreated(Action<Lobby, uint, ushort, SteamId> onLobbyGameCreated);
		void SetOnLobbyInvite(Action<Friend, Lobby> onLobbyInvite);
		void SetOnLobbyMemberDisconnected(Action<Lobby, Friend> onLobbyMemberDisconnected);
		void SetOnLobbyMemberJoined(Action<Lobby, Friend> onLobbyMemberJoined);
		void SetOnLobbyMemberKicked(Action<Lobby, Friend, Friend> onLobbyMemberKicked);
		void SetOnLobbyMemberLeave(Action<Lobby, Friend> onLobbyMemberLeave);
		void OpenGameInviteOverlay(SteamId lobbyId);
		void RunCallbacks();
		void Shutdown();
		void UnlockSuccess(string key);
		void RemoveOnGameLobbyJoinRequested(Action<Lobby, SteamId> onGameLobbyJoinRequested);
		void RemoveOnLobbyCreated(Action<Result, Lobby> onLobbyCreated);
		void RemoveOnLobbyEntered(Action<Lobby> onLobbyEntered);
		void RemoveOnLobbyGameCreated(Action<Lobby, uint, ushort, SteamId> onLobbyGameCreated);
		void RemoveOnLobbyInvite(Action<Friend, Lobby> onLobbyInvite);
		void RemoveOnLobbyMemberDisconnected(Action<Lobby, Friend> onLobbyMemberDisconnected);
		void RemoveOnLobbyMemberJoined(Action<Lobby, Friend> onLobbyMemberJoined);
		void RemoveOnLobbyMemberKicked(Action<Lobby, Friend, Friend> onLobbyMemberKicked);
		void RemoveOnLobbyMemberLeave(Action<Lobby, Friend> onLobbyMemberLeave);
		void RemoveOnChatMessage(Action<Lobby, Friend, string> onChatMessage);
		void SetOnChatMessage(Action<Lobby, Friend, string> onChatMessage);
		void SetOnLobbyDataChanged(Action<Lobby> onLobbyDataChanged);
		void RemoveOnLobbyDataChanged(Action<Lobby> onLobbyDataChanged);
		void SetOnGameRichPresenceJoinRequested(Action<Friend, string> onGameRichPresenceJoinRequested);
		void RemoveOnGameRichPresenceJoinRequested(Action<Friend, string> onGameRichPresenceJoinRequested);
		Task<Lobby?> JoinLobby(SteamId lobbyId);
        string GetCommandLine();
        AppId GetAppId();
        LobbyQuery CreateLobbyQuery();
        LobbyQuery CreateLobbyQueryWithMaxResult(int maxResult);
        LobbyQuery AddMaxResultToLobbyQuery(LobbyQuery query, int maxResult);
        LobbyQuery CreateLobbyQueryWithKeyValue(string key, string value);
        LobbyQuery AddKeyValueToLobbyQuery(LobbyQuery query, string key, string value);
        LobbyQuery AddAppIdFilterToLobbyQuery(LobbyQuery query);
        Task<Lobby[]> ExecuteLobbyQuery(LobbyQuery query);
        Task<Lobby[]> GetFriendsLobbies(int maxResult);
        void SetOnLobbyMemberDataChanged(Action<Lobby, Friend> onLobbyMemberDataChanged);
        void RemoveOnLobbyMemberDataChanged(Action<Lobby, Friend> onLobbyMemberDataChanged);
    }
}
