using PlatformCore.Infrastructure;
using PlatformCore.Services;
using PlatformCore.Services.AsyncAwaiter;
using PlatformCore.Services.Audio;
using PlatformCore.Services.Factory;
using PlatformCore.Services.Settings;
using PlatformCore.Services.UI;

namespace Project.Infrastructure
{
	public static class Startup
	{
		public static void RegisterServices(ServiceLocator serviceLocator, PersistentSceneContext persistentSceneContext)
		{
			var logger = new LoggerService();
			serviceLocator.Register<ILoggerService, LoggerService>(logger);

			var resourceService = new ResourceService(logger);
			serviceLocator.Register<IResourceService, ResourceService>(resourceService);

			var objectFactory = new ObjectFactory(resourceService, logger);
			serviceLocator.Register<IObjectFactory, ObjectFactory>(objectFactory);

			var sceneService = new SceneService(logger, persistentSceneContext);
			serviceLocator.Register<ISceneService, SceneService>(sceneService);

			var audioService = new AudioBaseService(logger);
			serviceLocator.Register<IAudioService, AudioBaseService>(audioService);

			var uiService = new UIBaseService(logger, resourceService, persistentSceneContext.UICanvasEntries);
			serviceLocator.Register<IUIService, UIBaseService>(uiService);

			var cursorService = new CursorService(uiService, logger);
			serviceLocator.Register<ICursorService, CursorService>(cursorService);

			var asyncAwaiterService = new AsyncAwaiterService();
			serviceLocator.Register<IAsyncAwaiterService, AsyncAwaiterService>(asyncAwaiterService);

			var settingsService = new SettingsService(new PlayerPrefsSettingsPersistence());
			serviceLocator.Register<ISettingsService, SettingsService>(settingsService);
		}
	}
}
