using Cysharp.Threading.Tasks;
using FishNet;
using PlatformCore.Infrastructure;
using PlatformCore.Infrastructure.Network.FishNet;
using PlatformCore.Services;
using PlatformCore.Services.Network;
using Project.Infrastructure;

namespace Project.Network
{
	public static class MultiplayerRuntimeEntryPoint
	{
		public static bool Start(
			ServiceLocator serviceLocator,
			LifecycleService lifecycleService,
			GameLaunchSettings launchSettings,
			ILoggerService loggerService)
		{
			if (!launchSettings.IsMultiplayer)
			{
				return false;
			}

			var networkManager = InstanceFinder.NetworkManager;
			if (!networkManager)
			{
				loggerService?.LogError(
					"[Multiplayer] FishNet NetworkManager is missing in loaded scenes. " +
					"Add NetworkManager + PlayerSpawner to Persistent scene.");
				return false;
			}

			if (!serviceLocator.TryGet<INetworkSessionService>(out _))
			{
				serviceLocator.RegisterFishNetFoundation(loggerService);
			}

			var sessionController = new NetworkSessionFlowController(networkManager, launchSettings, loggerService);
			lifecycleService.RegisterAsync(sessionController).Forget();

			loggerService?.Log($"[Multiplayer] Requested mode: {launchSettings.Mode}, address={launchSettings.Address}, port={launchSettings.Port}");
			return true;
		}
	}
}
