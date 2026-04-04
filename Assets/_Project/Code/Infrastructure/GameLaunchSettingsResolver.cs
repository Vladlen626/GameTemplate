using System;

namespace Project.Infrastructure
{
	public static class GameLaunchSettingsResolver
	{
		private const string HostArg = "-host";
		private const string ClientArg = "-client";
		private const string AddressArg = "-address";
		private const string PortArg = "-port";

		public static GameLaunchSettings Resolve()
		{
			var args = Environment.GetCommandLineArgs();
			if (args == null || args.Length == 0)
			{
				return new GameLaunchSettings(
					GameLaunchMode.Singleplayer,
					GameLaunchSettings.DefaultAddress,
					GameLaunchSettings.DefaultPort);
			}

			var mode = GameLaunchMode.Singleplayer;
			var address = GameLaunchSettings.DefaultAddress;
			var port = GameLaunchSettings.DefaultPort;

			for (var i = 0; i < args.Length; i++)
			{
				var arg = args[i];
				if (string.Equals(arg, HostArg, StringComparison.OrdinalIgnoreCase))
				{
					mode = GameLaunchMode.MultiplayerHost;
					continue;
				}

				if (string.Equals(arg, ClientArg, StringComparison.OrdinalIgnoreCase))
				{
					mode = GameLaunchMode.MultiplayerClient;
					continue;
				}

				if (string.Equals(arg, AddressArg, StringComparison.OrdinalIgnoreCase))
				{
					var next = TryReadNext(args, i);
					if (!string.IsNullOrWhiteSpace(next))
					{
						address = next;
					}

					continue;
				}

				if (string.Equals(arg, PortArg, StringComparison.OrdinalIgnoreCase))
				{
					var next = TryReadNext(args, i);
					if (ushort.TryParse(next, out var parsedPort))
					{
						port = parsedPort;
					}
				}
			}

			return new GameLaunchSettings(mode, address, port);
		}

		private static string TryReadNext(string[] args, int index)
		{
			var nextIndex = index + 1;
			if (nextIndex >= args.Length)
			{
				return string.Empty;
			}

			return args[nextIndex];
		}
	}
}
