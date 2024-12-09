using NaughtyAttributes;
using UnityEngine;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using Steamworks.Data;
using Steamworks;
using SteamworksPlus.Runtime.Providers.Facepunch.Proxies;
using SteamworksPlus.Runtime.Serializables;
using UnityEngine.Events;

namespace SteamworksPlus.Runtime.Providers.Facepunch.Components
{
	/// <summary>
	/// Gestionnaire de lobby Steam facilitant les retours d'évènement et la transmission de données dans un lobby
	/// </summary>
	[AddComponentMenu("SteamworksPlus/FacepunchLobby")]
	[DisallowMultipleComponent]
	public class FacepunchLobby : MonoBehaviour
	{
        #region Fields

        /// <summary>
        /// Shared transport within the application
        /// </summary>
        [NaughtyAttributes.ReadOnly, Tooltip("Shared transport within the application")]
		public FacepunchTransport Transport;

        /// <summary>
        /// Maximum number of players that can be added to the lobby
        /// </summary>
        [Required, Tooltip("Maximum number of players that can be added to the lobby")]
		public int MaxPlayers;

        /// <summary>
        /// Lobby parameters required to read the message as data
        /// </summary>
        [Expandable, Required, Tooltip("Lobby parameters required to read the message as data")]
		public LobbySettings LobbySettings;

        #region Callbacks

        /// <summary>
        /// Callback to invoke when OnLobbyCreated is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyCreated is raised.")]
		public SteamCallback OnLobbyCreatedCallback;

        /// <summary>
        /// Callback to invoke when OnGameLobbyJoinRequested is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnGameLobbyJoinRequested is raised.")]
		public SteamCallback OnGameLobbyJoinRequestedCallback;

        /// <summary>
        /// Callback to invoke when OnGameRichPresenceJoinRequested is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnGameRichPresenceJoinRequested is raised.")]
		public SteamCallback OnGameRichPresenceJoinRequestedCallback;

        /// <summary>
        /// Callback to invoke when OnLobbyMemberJoined is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyMemberJoined is raised.")]
		public FacepunchSteamEvent OnLobbyMemberJoinedCallback;

        /// <summary>
        /// Callback to invoke when OnLobbyMemberLeave is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyMemberLeave is raised.")]
		public FacepunchSteamEvent OnLobbyMemberLeaveCallback;

        /// <summary>
        /// Callback to invoke when OnLobbyMemberLeave is raised and the user is lobby's host.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyMemberLeave is raised and the user is lobby's host.")]
		public FacepunchSteamEvent OnLobbyHostLeaveCallback;

        /// <summary>
        /// Callback to invoke when OnLobbyDataChanged is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyDataChanged is raised.")]
		public FacepunchSteamEvent OnLobbyDataChangedCallback;

        /// <summary>
        /// Callback to invoke when OnLobbyDataChanged is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnLobbyDataChanged is raised.")]
        public FacepunchSteamEvent OnLobbyMemberDataChangedCallback;

        /// <summary>
        /// Callback to invoke when OnChatMessage is raised.
        /// </summary>
        [Tooltip("Callback to invoke when OnChatMessage is raised.")]
		public FacepunchSteamMessageEvent OnChatMessageCallback;

		public UnityEvent<string> OnCommandLineCallback;

        #endregion

        #endregion

        /// <summary>
        /// Static instance of the current lobby manager
        /// </summary>
        public static FacepunchLobby Instance;

        /// <summary>
        /// Current lobby instance joined by a customer
        /// </summary>
        public static Lobby? CurrentLobby;

		private IFacepunchSteam _facepunchSteamInternal;

		private IFacepunchSteam _facepunchSteam
		{
			get
			{
				if(_facepunchSteamInternal == null)
				{
					_facepunchSteamInternal = new FacepunchSteamProxy();
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

				if(NetworkManager.Singleton != null && Transport == null)
				{
					Transport = NetworkManager.Singleton.GetComponent<FacepunchTransport>();
				}
			}
			else
			{
                Debug.Log("Keep old FacepunchLobby instance");

                //Replace old instance
                Destroy(Instance.gameObject);

				Instance = this;
				DontDestroyOnLoad(gameObject);
				SetLobbyCallbacks();

				if (NetworkManager.Singleton != null && Transport == null)
				{
					Transport = NetworkManager.Singleton.GetComponent<FacepunchTransport>();
				}
			}
		}

		private void OnDestroy()
		{
			RemoveLobbyCallbacks();

			if (NetworkManager.Singleton == null)
			{
				return;
			}

			NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
			NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
			NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
		}

		private void OnApplicationQuit()
		{
			Disconnect();
		}

		#endregion

		#region Public

		public void TryReadCommandLine()
		{
			string commandLine = _facepunchSteam.GetCommandLine();

            Debug.Log($"Command line received: {commandLine}");

            if (string.IsNullOrEmpty(commandLine))
				return;		

			OnCommandLineCallback?.Invoke(commandLine);
		}

		public async void StartHost()
		{
			Debug.Log($"Trying to start hosting");

			NetworkManager.Singleton.OnServerStarted += OnServerStarted;

			NetworkManager.Singleton.StartHost();

			CurrentLobby = await _facepunchSteam.CreatelobbyAsync(MaxPlayers);
		}

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

		public void Disconnect()
		{
			Debug.Log("Trying to disconnect");
			CurrentLobby?.Leave();
			if (NetworkManager.Singleton == null)
			{
				return;
			}

			if (NetworkManager.Singleton.IsHost)
			{
				NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
			}

			if (NetworkManager.Singleton.IsClient)
			{
				NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
			}

			NetworkManager.Singleton.Shutdown(true);
			Debug.Log("Disconneted");
		}

		public void SendLobbyMessage(string message)
		{
			CurrentLobby?.SendChatString(message);
		}

		public void SendLobbyMessage<T>(T data)
			where T : class
		{
			CurrentLobby?.SendChatString($"{typeof(T).Name}{LobbySettings.ChatDataMessageSeparator}{JsonUtility.ToJson(data)}");
		}

		public void SetPublic()
		{
			CurrentLobby?.SetPublic();
		}

		public void SetPrivate()
		{
			CurrentLobby?.SetPrivate();
		}

		public void SetJoinable(bool isJoinable)
		{
			CurrentLobby?.SetJoinable(isJoinable);
		}

		public void SetFriendsOnly()
		{
			CurrentLobby?.SetFriendsOnly();
		}

		public void UpdateLobbySettings()
		{
			UpdateLobbySettings(CurrentLobby.Value);
		}

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
			NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
			NetworkManager.Singleton.OnClientDisconnectCallback += OnClientConnectedCallback;

			Transport.targetSteamId = steamId;

			if (NetworkManager.Singleton.StartClient())
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

                OnLobbyCreatedCallback.Response?.Invoke(lobby, lobby.Owner);

				return;
			}

			Debug.Log($"Creation lobby failure");

			OnLobbyCreatedCallback.ErrorHandler?.Invoke();
		}

		private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId id)
		{
			Debug.Log($"User with id: {id.Value} is trying to join a lobby: {lobby}");

			RoomEnter joinedLobby = await lobby.Join();

			Debug.Log($"Operation status: {joinedLobby}");

			if (joinedLobby == RoomEnter.Success)
			{
				Debug.Log($"Lobby joined");

				CurrentLobby = lobby;

				OnGameLobbyJoinRequestedCallback.Response?.Invoke(lobby, new Friend(id));
			}
			else
			{
				Debug.Log($"The client was unable to join the lobby");

				OnGameLobbyJoinRequestedCallback.ErrorHandler?.Invoke();
			}
		}

		private async void OnGameRichPresenceJoinRequested(Friend friend, string stringParams)
		{
			Debug.Log($"User with id: {friend.Id.Value} is trying to join a lobby by opening the game with params: {stringParams}");
	
			if(!ulong.TryParse(stringParams, out ulong lobbyId))
			{
				Debug.Log($"Impossible to determine the id of the lobby to be joined, current value: {stringParams}");
				OnGameRichPresenceJoinRequestedCallback.ErrorHandler?.Invoke();
				return;
			}

			Lobby? joinedLobby = await _facepunchSteam.JoinLobby(new SteamId() { Value = lobbyId });

			Debug.Log($"Operation status: {joinedLobby}");

			if(joinedLobby == null)
			{
				Debug.Log($"The lobby could not be joined");
				OnGameRichPresenceJoinRequestedCallback.ErrorHandler?.Invoke();
				return;
			}

			Debug.Log($"The lobby has been reached");
			OnGameRichPresenceJoinRequestedCallback.Response?.Invoke(joinedLobby.Value, friend);
		}

		private void OnLobbyEntered(Lobby lobby)
		{
			if (NetworkManager.Singleton.IsHost)
			{
				//No need to go through this part if the guest is the one who joined the lobby
				return;
			}

			StartClient(lobby.Owner.Id);
		}

		private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
		{
			Debug.Log($"User with id: {friend.Id} has joined the lobby");

			OnLobbyMemberJoinedCallback?.Invoke(lobby, friend);
		}

		private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
		{
			Debug.Log($"User with id: {friend.Id} has left the lobby");

			if (lobby.IsOwnedBy(friend.Id))
			{
				Debug.Log($"Host left the lobby");

				OnLobbyHostLeaveCallback?.Invoke(lobby, friend);
			}
			else
			{
				OnLobbyMemberLeaveCallback?.Invoke(lobby, friend);
			}
		}

		private void OnLobbyDataChanged(Lobby lobby)
		{
			Debug.Log($"Lobby data updated. Lobby: {lobby.Id}");

			OnLobbyDataChangedCallback?.Invoke(lobby, lobby.Owner);
		}

		public void OnLobbyMemberDataChanged(Lobby lobby, Friend friend)
		{
            Debug.Log($"Lobby data member updated. Lobby: {lobby.Id}");

            OnLobbyMemberDataChangedCallback?.Invoke(lobby, friend);
        }

        private void OnChatMessage(Lobby lobby, Friend friend, string message)
		{
			Debug.Log($"Chat message recieved. Lobby: {lobby.Id} | Sender: {friend.Id} | Message: {message}");

			OnChatMessageCallback?.Invoke(lobby, friend, message);
		}

		private void OnLobbyInvite(Friend friend, Lobby lobby)
		{
			Debug.Log($"Recieve an invite from: {friend.Id} to join lobby: {lobby.Id}");
		}

		private void OnServerStarted()
		{
			Debug.Log($"Host started");
		}

		private void OnClientConnectedCallback(ulong ip)
		{
			Debug.Log($"Client connected");
		}

		private void OnClientDisconnectCallback(ulong ip)
		{
			Debug.Log($"Client disconnected");
		}

		#endregion
	}
}
