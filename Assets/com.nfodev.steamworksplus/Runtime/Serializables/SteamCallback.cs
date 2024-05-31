using SteamworksPlus.Runtime.Providers.Facepunch.Components;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SteamworksPlus.Runtime.Serializables
{
    /// <summary>
    /// Callback class dedicated to steams callbacks, which handles a response event and an error event.
    /// </summary>
    [Serializable]
	public class SteamCallback
	{
        /// <summary>
        /// Unity response used when the associated steam callback is also triggered
        /// </summary>
        [Tooltip("Unity response used when the associated steam callback is also triggered")]
		public FacepunchSteamEvent Response;

        /// <summary>
        /// Unity response used when an error is raised
        /// </summary>
        [Tooltip("Unity response used when an error is raised")]
		public UnityEvent ErrorHandler;
	}
}
