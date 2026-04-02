using PlatformCore.Infrastructure;
using PlatformCore.Services;

namespace ProjectTemplate.Infrastructure
{
	public static class TemplateStartup
	{
		public static void RegisterServices(ServiceLocator serviceLocator, PersistentSceneContext persistentSceneContext)
		{
			var logger = new LoggerService();
			serviceLocator.Register<ILoggerService, LoggerService>(logger);
		}
	}
}

