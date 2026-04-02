using Cysharp.Threading.Tasks;
using PlatformCore.Infrastructure;
using ProjectTemplate.Infrastructure;

namespace ProjectTemplate.Bootstrap
{
	public sealed class TemplateGameRoot : BaseGameRoot
	{
		protected override void RegisterServices(PersistentSceneContext persistentSceneContext)
		{
			TemplateStartup.RegisterServices(_serviceLocator, persistentSceneContext);
		}

		protected override UniTask LaunchGameAsync(PersistentSceneContext persistentSceneContext)
		{
			return UniTask.CompletedTask;
		}
	}
}

