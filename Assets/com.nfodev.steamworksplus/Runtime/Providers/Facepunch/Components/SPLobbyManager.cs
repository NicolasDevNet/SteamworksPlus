using NaughtyAttributes;
using UnityEngine;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using Steamworks.Data;
using Steamworks;
using SteamworksPlus.Runtime.Providers.Facepunch.Proxies;
using SteamworksPlus.Runtime.Serializables;
using SteamworksPlus.Runtime.ScriptableObjects;
using SteamworksPlus.Runtime.Providers.Facepunch.Extentions;
using System.Threading.Tasks;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Components
{
    /// <summary>
    /// Steam lobby manager for easy event feedback and lobby data transmission
    /// </summary>
    [AddComponentMenu("SteamworksPlus/LobbyManager")]
	[DisallowMultipleComponent]
	public class SPLobbyManager : MonoBehaviour
	{
        #region Fields

        /// <summary>
        /// Shared network manager within the application
        /// </summary>
        [Required, Tooltip("Shared network manager within the application")]
		public NetworkManager NetworkManager;

        /// <summary>
        /// Shared transport within the application
        /// </summary>
        [Required, Tooltip("Shared transport within the application")]
		public FacepunchTransport Transport;

        /// <summary>
        /// Lobby parameters required to read the message as data
        /// </summary>
        [Expandable, Required, Tooltip("Lobby parameters required to read the message as data")]
		public SPLobbySettings LobbySettings;

        /// <summary>
        /// Static instance of global lobby parameters
        /// </summary>
        public static SPLobbySettings Settings { get { return Instance.LobbySettings; } }

        #region Callbacks

        /// <summary>
        /// Callback to invoke when OnLobbyCreated is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyCreated is raised.")]
		public SPLobbyFriendErrCallback OnLobbyCreatedCallback;

        /// <summary>
        /// Callback to invoke when OnGameLobbyJoinRequested is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnGameLobbyJoinRequested is raised.")]
		public SPLobbyFriendErrCallback OnGameLobbyJoinRequestedCallback;

        /// <summary>
        /// Callback to invoke when OnGameRichPresenceJoinRequested is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnGameRichPresenceJoinRequested is raised.")]
		public SPLobbyFriendErrCallback OnGameRichPresenceJoinRequestedCallback;

        /// <summary>
        /// Callback to invoke when OnLobbyMemberJoined is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyMemberJoined is raised.")]
		public SPLobbyFriendCallback OnLobbyMemberJoinedCallback;

        /// <summary>
        /// Callback to invoke when OnLobbyMemberJoined is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyInvite is raised.")]
        public SPLobbyFriendCallback OnLobbyInviteCallback;

        /// <summary>
        /// Callback to invoke when OnLobbyMemberLeave is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyMemberLeave is raised.")]
		public SPLobbyFriendCallback OnLobbyMemberLeaveCallback;

        /// <summary>
        /// Callback to invoke when OnLobbyMemberLeave is raised and the user is lobby's host.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyMemberLeave is raised and the user is lobby's host.")]
		public SPLobbyFriendCallback OnLobbyHostLeaveCallback;

        /// <summary>
        /// Callback to invoke when OnLobbyDataChanged is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyDataChanged is raised.")]
		public SPLobbyFriendCallback OnLobbyDataChangedCallback;

        /// <summary>
        /// Callback to invoke when OnLobbyDataChanged is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyMemberDataChanged is raised.")]
        public SPLobbyFriendCallback OnLobbyMemberDataChangedCallback;

        /// <summary>
        /// Callback to invoke when OnChatMessage is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnChatMessage is raised.")]
		public SPLobbyFriendMessageCallback OnChatMessageCallback;

        /// <summary>
        /// Callback used when a command line is entered at application startup
        /// </summary>
        [Tooltip("Callback used when a command line is entered at application startup")]
        public SPCommandLineCallback OnCommandLineCallback;

        #endregion

        #endregion

        /// <summary>
        /// Static instance of the current lobby manager
        /// </summary>
        public static SPLobbyManager Instance;

        /// <summary>
        /// Current lobby instance joined by a customer
        /// </summary>
        public static Lobby? CurrentLobby;

		private ISPSteam _facepunchSteamInternal;

		private ISPSteam _facepunchSteam
		{
			get
			{
				if(_facepunchSteamInternal == null)
				{
					_facepunchSteamInternal = new SPSteamProxy();
				}

				return _facepunchSteamInternal;
			}
			set
			{
				_facepunchSteamInternal = value;
			}
		}

		#region Unity messages

		private void Start()
		{
			if (Instance == null)
			{
				Debug.Log("Start new FacepunchLobby instance");

				Instance = this;
				DontDestroyOnLoad(gameObject);
				SetLobbyCallbacks();
				TryReadCommandLine();
            }
			else
			{
                Debug.Log("Keep old FacepunchLobby instance");

                //Replace old instance
                Destroy(Instance.gameObject);

				Instance = this;
				DontDestroyOnLoad(gameObject);
				SetLobbyCallbacks();
			}
		}

		private void OnDestroy()
		{
			RemoveLobbyCallbacks();

			if (NetworkManager == null)
			{
				return;
			}

			NetworkManager.OnServerStarted -= OnServerStarted;
			NetworkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
			NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
		}

		private void OnApplicationQuit()
		{
			Disconnect();
		}

        #endregion

        #region Public

        #region Data

        /// <summary>
        /// Method for reading a command line when starting an application with Steam
        /// </summary>
        public void TryReadCommandLine()
		{
			string commandLine = _facepunchSteam.GetCommandLine();

            Debug.Log($"Command line received: {commandLine}");

            if (string.IsNullOrEmpty(commandLine))
			{
                return;
            }	

			OnCommandLineCallback?.Raise(commandLine);
		}

        /// <summary>
        /// Method for sending a message in the lobby
        /// </summary>
        /// <param name="message">Message value</param>
        public void SendLobbyMessage(string message)
        {
            CurrentLobby?.SendChatString(message);
        }

        /// <summary>
        /// Method for sending a typed message in the lobby
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="data">Message data</param>
        public void SendLobbyMessage<T>(T data)
            where T : class
        {
            if (CurrentLobby == null)
            {
                return;
            }

            CurrentLobby?.SendChatString($"{typeof(T).Name}{LobbySettings.ChatDataMessageSeparator}{JsonUtility.ToJson(data)}");
        }

        /// <summary>
        /// Method for defining data for a lobby member
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="key">The associated key</param>
        /// <param name="data">The data to be registered</param>
        public void SetLobbyMemberData<T>(string key, T data)
			where T : class
		{
			if(CurrentLobby == null)
			{
				return;
			}

			CurrentLobby.Value.SetTValueToLobbyMemberData<T>(key, data);
		}

        /// <summary>
        /// Method for defining data for a lobby
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="key">The associated key</param>
        /// <param name="data">The data to be registered</param>
        public void SetLobbyData<T>(string key, T data)
            where T : class
        {
            if (CurrentLobby == null)
            {
                return;
            }

            CurrentLobby.Value.SetTValueToLobbyData<T>(key, data);
        }

        #endregion

        #region Tools

        /// <summary>
        /// Method for starting a host for the client that triggers the method
        /// </summary>
        public async void StartHost()
		{
			Debug.Log($"Trying to start hosting");

			NetworkManager.OnServerStarted += OnServerStarted;

			NetworkManager.StartHost();

            CurrentLobby = await _facepunchSteam.CreateLobbyAsync(LobbySettings.MaxPlayers);
		}

        /// <summary>
        /// How to leave a lobby
        /// </summary>
        public void LeaveLobby()
		{
			Debug.Log("Attempt to leave a lobby");

			if(CurrentLobby == null)
			{
				return;
			}

			CurrentLobby.Value.Leave();
		}

		/// <summary>
		/// Method for opening the steam invitation overlay
		/// </summary>
		public void OpenGameInviteOverlay()
		{
			if (CurrentLobby == null)
			{
				Debug.Log("No lobby created, overlay opening impossible");
				return;
			}

			_facepunchSteam.OpenGameInviteOverlay(CurrentLobby.Value.Id);

			Debug.Log("Steam overlay opening");
		}

        /// <summary>
        /// Method for disconnecting from a lobby and a host/server
        /// </summary>
        public void Disconnect()
		{
			Debug.Log("Trying to disconnect");
			CurrentLobby?.Leave();
			if (NetworkManager == null)
			{
				return;
			}

			if (NetworkManager.IsHost)
			{
				NetworkManager.OnServerStarted -= OnServerStarted;
			}

			if (NetworkManager.IsClient)
			{
				NetworkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
			}

			NetworkManager.Shutdown(true);
			Debug.Log("Disconneted");
		}

        /// <summary>
        /// Method for defining a lobby as public
        /// </summary>
        public void SetPublic()
		{
			CurrentLobby?.SetPublic();
		}

        /// <summary>
        /// Method for defining a lobby as private
        /// </summary>
        public void SetPrivate()
		{
			CurrentLobby?.SetPrivate();
        }

        /// <summary>
        /// Method for determining whether a lobby is reachable or not
        /// </summary>
        public void SetJoinable(bool isJoinable)
		{
			CurrentLobby?.SetJoinable(isJoinable);
        }

        /// <summary>
        /// Method for defining whether the lobby is for friends only or not
        /// </summary>
        public void SetFriendsOnly()
		{
			CurrentLobby?.SetFriendsOnly();
        }

		/// <summary>
		/// Méthode permettant de mettre à jour les paramètres du lobby
		/// </summary>
		public void UpdateLobbySettings()
		{
			UpdateLobbySettings(CurrentLobby.Value);
		}

        /// <summary>
        /// Method for retrieving the client's wide avater as Texture2D
        /// </summary>
        /// <returns>The resulting task</returns>
        public async Task<Texture2D> GetClientLargeAvatarTexture2D()
		{
			Image? result = await _facepunchSteam.GetLargeAvatarAsync();

			return result.ConvertToTexture2D();
		}

        #endregion

        #endregion

        #region Private

        private void UpdateLobbySettings(Lobby lobby)
		{
			if (LobbySettings.IsFriendsOnly)
			{
				lobby.SetFriendsOnly();
			}

			if (LobbySettings.IsPublic)
			{
				lobby.SetPublic();
			}
			else
			{
				lobby.SetPrivate();
			}

			lobby.SetJoinable(LobbySettings.IsJoinable);
		}

		private void StartClient(SteamId steamId)
		{
			NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
			NetworkManager.OnClientDisconnectCallback += OnClientConnectedCallback;

			Transport.targetSteamId = steamId;

			if (NetworkManager.StartClient())
			{
				Debug.Log("Client has started");
			}
		}

		#endregion

		#region Callbacks

		private void SetLobbyCallbacks()
		{
			_facepunchSteam.SetOnLobbyCreated(OnLobbyCreated);
			_facepunchSteam.SetOnGameLobbyJoinRequested(OnGameLobbyJoinRequested);
			_facepunchSteam.SetOnGameRichPresenceJoinRequested(OnGameRichPresenceJoinRequested);
			_facepunchSteam.SetOnLobbyMemberJoined(OnLobbyMemberJoined);
			_facepunchSteam.SetOnLobbyMemberLeave(OnLobbyMemberLeave);
			_facepunchSteam.SetOnLobbyDataChanged(OnLobbyDataChanged);
			_facepunchSteam.SetOnLobbyMemberDataChanged(OnLobbyMemberDataChanged);
            _facepunchSteam.SetOnChatMessage(OnChatMessage);
			_facepunchSteam.SetOnLobbyInvite(OnLobbyInvite);
			_facepunchSteam.SetOnLobbyEntered(OnLobbyEntered);
		}

		private void RemoveLobbyCallbacks()
		{
			_facepunchSteam.RemoveOnLobbyCreated(OnLobbyCreated);
			_facepunchSteam.RemoveOnGameLobbyJoinRequested(OnGameLobbyJoinRequested);
			_facepunchSteam.RemoveOnGameRichPresenceJoinRequested(OnGameRichPresenceJoinRequested);
			_facepunchSteam.RemoveOnLobbyMemberJoined(OnLobbyMemberJoined);
			_facepunchSteam.RemoveOnLobbyMemberLeave(OnLobbyMemberLeave);
			_facepunchSteam.RemoveOnLobbyDataChanged(OnLobbyDataChanged);
			_facepunchSteam.RemoveOnLobbyMemberDataChanged(OnLobbyMemberDataChanged);
            _facepunchSteam.RemoveOnChatMessage(OnChatMessage);
			_facepunchSteam.RemoveOnLobbyInvite(OnLobbyInvite);
			_facepunchSteam.RemoveOnLobbyEntered(OnLobbyEntered);
		}

		private void OnLobbyCreated(Result result, Lobby lobby)
		{
			Debug.Log($"Lobby creation result: {result}");

			if(result == Result.OK || result == Result.AdministratorOK)
			{
				Debug.Log($"Lobby created: {lobby.Id}");

				UpdateLobbySettings(lobby);

				lobby.SetGameServer(lobby.Owner.Id);

				lobby.SetData(SPConstants.AppIdDataKey, _facepunchSteam.GetAppId().ToString());

                OnLobbyCreatedCallback?.Raise(new SPLobbyFriendResponse(lobby, lobby.Owner));

				return;
			}

			Debug.Log("Creation lobby failure");

			OnLobbyCreatedCallback.SteamErrorCallback?.Raise("Creation lobby failure");
		}

		private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId id)
		{
			Debug.Log($"User with id: {id.Value} is trying to join a lobby: {lobby}");

			RoomEnter joinedLobby = await lobby.Join();

			Debug.Log($"Joined lobby: {joinedLobby}");

			if (joinedLobby == RoomEnter.Success)
			{
				Debug.Log("Lobby joined");

				CurrentLobby = lobby;

				OnGameLobbyJoinRequestedCallback?.Raise(new SPLobbyFriendResponse(lobby, new Friend(id)));
			}
			else
			{
				Debug.Log("The client was unable to join the lobby");

				OnGameLobbyJoinRequestedCallback.SteamErrorCallback?.Raise("The client was unable to join the lobby");
			}
		}

		private async void OnGameRichPresenceJoinRequested(Friend friend, string stringParams)
		{
			Debug.Log($"User with id: {friend.Id.Value} is trying to join a lobby by opening the game with params: {stringParams}");
	
			if(!ulong.TryParse(stringParams, out ulong lobbyId))
			{
				Debug.Log($"Impossible to determine the id of the lobby to be joined, current value: {stringParams}");
				OnGameRichPresenceJoinRequestedCallback.SteamErrorCallback?.Raise($"Impossible to determine the id of the lobby to be joined, current value: {stringParams}");
				return;
			}

			Lobby? joinedLobby = await _facepunchSteam.JoinLobby(new SteamId() { Value = lobbyId });

			Debug.Log($"Joined lobby: {joinedLobby}");

			if(joinedLobby == null)
			{
				Debug.Log("The lobby could not be joined");
				OnGameRichPresenceJoinRequestedCallback.SteamErrorCallback?.Raise("The lobby could not be joined");
				return;
			}

			CurrentLobby = joinedLobby;

            Debug.Log("The lobby has been reached");
			OnGameRichPresenceJoinRequestedCallback?.Raise(new SPLobbyFriendResponse(joinedLobby.Value, friend));
		}

		private void OnLobbyEntered(Lobby lobby)
		{
			if (NetworkManager.IsHost)
			{
				//No need to go through this part if the guest is the one who joined the lobby
				return;
			}

			StartClient(lobby.Owner.Id);
		}

		private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
		{
			Debug.Log($"User with id: {friend.Id} has joined the lobby");

			OnLobbyMemberJoinedCallback?.Raise(new SPLobbyFriendResponse(lobby, friend));
		}

		private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
		{
			Debug.Log($"User with id: {friend.Id} has left the lobby");

			if (lobby.IsOwnedBy(friend.Id))
			{
				Debug.Log("Host left the lobby");

				OnLobbyHostLeaveCallback?.Raise(new SPLobbyFriendResponse(lobby, friend));
			}
			else
			{
				OnLobbyMemberLeaveCallback?.Raise(new SPLobbyFriendResponse(lobby, friend));
			}
		}

		private void OnLobbyDataChanged(Lobby lobby)
		{
			Debug.Log($"Lobby data updated. Lobby: {lobby.Id}");

			OnLobbyDataChangedCallback?.Raise(new SPLobbyFriendResponse(lobby, lobby.Owner));
		}

        private void OnLobbyMemberDataChanged(Lobby lobby, Friend friend)
        {
            Debug.Log($"Lobby member data updated. Lobby: {lobby.Id}");

            OnLobbyMemberDataChangedCallback?.Raise(new SPLobbyFriendResponse(lobby, friend));
        }

        private void OnChatMessage(Lobby lobby, Friend friend, string message)
		{
			Debug.Log($"Chat message recieved. Lobby: {lobby.Id} | Sender: {friend.Id} | Message: {message}");

			OnChatMessageCallback?.Raise(new SPLobbyFriendMessageResponse(lobby, friend, message));
		}

		private void OnLobbyInvite(Friend friend, Lobby lobby)
		{
			Debug.Log($"Recieve an invite from: {friend.Id} to join lobby: {lobby.Id}");

			OnLobbyInviteCallback?.Raise(new SPLobbyFriendResponse(lobby, friend));
        }

		private void OnServerStarted()
		{
			Debug.Log("Host started");
		}

		private void OnClientConnectedCallback(ulong ip)
		{
			Debug.Log("Client connected");
		}

		private void OnClientDisconnectCallback(ulong ip)
		{
			Debug.Log("Client disconnected");
		}

		#endregion
	}
}
