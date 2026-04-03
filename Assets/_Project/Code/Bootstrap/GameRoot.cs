using Cysharp.Threading.Tasks;
using PlatformCore.Infrastructure;
using Project.Infrastructure;

namespace Project.Bootstrap
{
	public sealed class GameRoot : BaseGameRoot
	{
		protected override void RegisterServices(PersistentSceneContext persistentSceneContext)
		{
			Startup.RegisterServices(_serviceLocator, persistentSceneContext);
		}

		protected override UniTask LaunchGameAsync(PersistentSceneContext persistentSceneContext)
		{
			return UniTask.CompletedTask;
		}
	}
}
