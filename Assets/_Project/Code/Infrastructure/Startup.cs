using PlatformCore.Infrastructure;
using PlatformCore.Services;

namespace Project.Infrastructure
{
	public static class Startup
	{
		public static void RegisterServices(ServiceLocator serviceLocator, PersistentSceneContext persistentSceneContext)
		{
			var logger = new LoggerService();
			serviceLocator.Register<ILoggerService, LoggerService>(logger);
		}
	}
}
