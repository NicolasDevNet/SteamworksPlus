using Steamworks.Data;
using SteamworksPlus.Runtime;
using SteamworksPlus.Runtime.Providers.Facepunch.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Samples.Scripts
{
    public class LobbiesDisplayer : MonoBehaviour
    {
        private ISPSteam _facepunchSteamInternal;

        private ISPSteam _facepunchSteam
        {
            get
            {
                if (_facepunchSteamInternal == null)
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

        public TMP_Text TextComponent;

        public UnityEvent<Lobby[]> PostLobbiesLoadingEvent;

        public async void RequestLobbies()
        {
            LobbyQuery lobbyQuery = _facepunchSteam.CreateLobbyQueryWithMaxResult(20);
            lobbyQuery = _facepunchSteam.AddAppIdFilterToLobbyQuery(lobbyQuery);

            var result = await _facepunchSteam.ExecuteLobbyQuery(lobbyQuery);

            PostLobbiesLoadingEvent?.Invoke(result);
        }

        public void Display(Lobby[] lobbies)
        {
            if(lobbies == null)
            {
                Debug.Log("0 lobby to display");
                return;
            }

            Debug.Log("Lobbies found:");

            TextComponent.text = string.Empty;

            foreach (var lobby in lobbies)
            {
                string result = "Lobby id: " + lobby.Id;
                Debug.Log(result);
                TextComponent.text += $"{result} \n";

                foreach(var data in lobby.Data)
                {
                    Debug.Log(data.Key + ":" + data.Value);
                    TextComponent.text += data.Key + ":" + data.Value + "\n";
                }
            }
        }
    }
}
