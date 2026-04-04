using System.Reflection;
using FishNet.Managing;
using PlatformCore.Core;
using PlatformCore.Core.Lifecycle;
using PlatformCore.Services;
using Project.Infrastructure;

namespace Project.Network
{
	public sealed class NetworkSessionFlowController : IBaseController, IActivatable, IDeactivatable
	{
		private readonly NetworkManager _networkManager;
		private readonly GameLaunchSettings _launchSettings;
		private readonly ILoggerService _loggerService;

		public NetworkSessionFlowController(
			NetworkManager networkManager,
			GameLaunchSettings launchSettings,
			ILoggerService loggerService)
		{
			_networkManager = networkManager;
			_launchSettings = launchSettings;
			_loggerService = loggerService;
		}

		public void Activate()
		{
			ConfigureTransport();

			switch (_launchSettings.Mode)
			{
				case GameLaunchMode.MultiplayerHost:
					StartServer();
					StartClient();
					break;

				case GameLaunchMode.MultiplayerClient:
					StartClient();
					break;
			}
		}

		public void Deactivate()
		{
			StopClient();
			StopServer();
		}

		private void ConfigureTransport()
		{
			var transport = _networkManager.TransportManager?.Transport;
			if (transport == null)
			{
				return;
			}

			SetWritableProperty(transport, "Port", _launchSettings.Port);

			if (_launchSettings.Mode == GameLaunchMode.MultiplayerClient)
			{
				SetWritableProperty(transport, "ClientAddress", _launchSettings.Address);
			}
		}

		private void StartServer()
		{
			if (GetStarted(_networkManager.ServerManager))
			{
				return;
			}

			if (!InvokeParameterlessMethod(_networkManager.ServerManager, "StartConnection"))
			{
				_loggerService?.LogError("[Multiplayer] Failed to start server connection.");
				return;
			}

			_loggerService?.Log("[Multiplayer] Server started.");
		}

		private void StartClient()
		{
			if (GetStarted(_networkManager.ClientManager))
			{
				return;
			}

			if (!InvokeParameterlessMethod(_networkManager.ClientManager, "StartConnection"))
			{
				_loggerService?.LogError("[Multiplayer] Failed to start client connection.");
				return;
			}

			_loggerService?.Log("[Multiplayer] Client started.");
		}

		private void StopServer()
		{
			if (!GetStarted(_networkManager.ServerManager))
			{
				return;
			}

			if (!InvokeStopMethod(_networkManager.ServerManager))
			{
				_loggerService?.LogWarning("[Multiplayer] Failed to stop server connection.");
				return;
			}

			_loggerService?.Log("[Multiplayer] Server stopped.");
		}

		private void StopClient()
		{
			if (!GetStarted(_networkManager.ClientManager))
			{
				return;
			}

			if (!InvokeStopMethod(_networkManager.ClientManager))
			{
				_loggerService?.LogWarning("[Multiplayer] Failed to stop client connection.");
				return;
			}

			_loggerService?.Log("[Multiplayer] Client stopped.");
		}

		private static bool GetStarted(object target)
		{
			if (target == null)
			{
				return false;
			}

			var property = target.GetType().GetProperty("Started", BindingFlags.Public | BindingFlags.Instance);
			if (property == null || property.PropertyType != typeof(bool))
			{
				return false;
			}

			if (property.GetValue(target) is bool started)
			{
				return started;
			}

			return false;
		}

		private static bool InvokeStopMethod(object target)
		{
			if (target == null)
			{
				return false;
			}

			var type = target.GetType();
			var parameterlessMethod = type.GetMethod("StopConnection", BindingFlags.Public | BindingFlags.Instance, null, System.Type.EmptyTypes, null);
			if (parameterlessMethod != null)
			{
				parameterlessMethod.Invoke(target, null);
				return true;
			}

			var boolMethod = type.GetMethod("StopConnection", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(bool) }, null);
			if (boolMethod != null)
			{
				boolMethod.Invoke(target, new object[] { true });
				return true;
			}

			return false;
		}

		private static bool InvokeParameterlessMethod(object target, string methodName)
		{
			if (target == null)
			{
				return false;
			}

			var method = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, null, System.Type.EmptyTypes, null);
			if (method == null)
			{
				return false;
			}

			method.Invoke(target, null);
			return true;
		}

		private static void SetWritableProperty(object target, string propertyName, object value)
		{
			if (target == null || value == null)
			{
				return;
			}

			var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
			if (property == null || !property.CanWrite)
			{
				return;
			}

			var valueType = value.GetType();
			if (!property.PropertyType.IsAssignableFrom(valueType))
			{
				if (property.PropertyType == typeof(int) && valueType == typeof(ushort))
				{
					property.SetValue(target, (int)(ushort)value);
				}

				return;
			}

			property.SetValue(target, value);
		}
	}
}
