using System;
using System.Reflection;
using PlatformCore.Infrastructure;
using PlatformCore.Services;

namespace Project.Infrastructure
{
	public static class MultiplayerRuntimeBridge
	{
		private const string EntryPointTypeName = "Project.Network.MultiplayerRuntimeEntryPoint, _Project.Network";
		private const string StartMethodName = "Start";

		public static bool TryStart(
			ServiceLocator serviceLocator,
			LifecycleService lifecycleService,
			GameLaunchSettings launchSettings,
			ILoggerService loggerService)
		{
			if (!launchSettings.IsMultiplayer)
			{
				return false;
			}

			var entryType = Type.GetType(EntryPointTypeName);
			if (entryType == null)
			{
				loggerService?.LogError(
					"[Multiplayer] Multiplayer mode requested, but _Project.Network entry point is unavailable. " +
					"Install FishNet and reimport assemblies.");
				return false;
			}

			var startMethod = entryType.GetMethod(StartMethodName, BindingFlags.Public | BindingFlags.Static);
			if (startMethod == null)
			{
				loggerService?.LogError("[Multiplayer] Multiplayer entry point exists, but Start method is missing.");
				return false;
			}

			object result;
			try
			{
				result = startMethod.Invoke(null, new object[]
				{
					serviceLocator,
					lifecycleService,
					launchSettings,
					loggerService,
				});
			}
			catch (Exception exception)
			{
				loggerService?.LogError($"[Multiplayer] Failed to start multiplayer runtime: {exception}");
				return false;
			}

			if (result is bool started)
			{
				return started;
			}

			loggerService?.LogError("[Multiplayer] Multiplayer Start returned invalid result.");
			return false;
		}
	}
}
