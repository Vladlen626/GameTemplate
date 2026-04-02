using PlatformCore.Infrastructure;

namespace ProjectTemplate.Bootstrap
{
	public sealed class TemplateBootstrap : BaseBootstrap
	{
		protected override BaseGameRoot CreateGameRoot()
		{
			return new TemplateGameRoot();
		}
	}
}

