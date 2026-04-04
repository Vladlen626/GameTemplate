using PlatformCore.Core;
using PlatformCore.Core.Lifecycle;
using PlatformCore.Services.Audio;
using PlatformCore.Services.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class UISettingsController : BaseContextController<UISettings>, IUpdatable
{
	private readonly IAudioService _audioService;
	private readonly ICursorService _cursorService;

	private bool _cursorUnlockedBySettings;

	public UISettingsController(IUIService uiService, IAudioService audioService, ICursorService cursorService)
		: base(uiService)
	{
		_audioService = audioService;
		_cursorService = cursorService;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		_context.Hide();

		_context.OnMasterChanged += OnMasterChangedHandler;
		_context.OnMusicChanged += OnMusicChangedHandler;
		_context.OnSfxChanged += OnSfxChangedHandler;
		_context.OnCloseClicked += OnCloseClickedHandler;
		_context.OnMainMenuClicked += OnMainMenuClickedHandler;
	}

	protected override void OnDeactivate()
	{
		_context.OnMainMenuClicked -= OnMainMenuClickedHandler;
		_context.OnCloseClicked -= OnCloseClickedHandler;
		_context.OnSfxChanged -= OnSfxChangedHandler;
		_context.OnMusicChanged -= OnMusicChangedHandler;
		_context.OnMasterChanged -= OnMasterChangedHandler;

		base.OnDeactivate();
	}

	public void OnUpdate(float deltaTime)
	{
		if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
		{
			if (_context.IsShown())
			{
				HideContext();
			}
			else
			{
				ShowContext();
			}
		}
	}

	private void ShowContext()
	{
		_context.SetValues(_audioService.MasterVolume, _audioService.MusicVolume, _audioService.SfxVolume);
		_context.Show();

		_cursorUnlockedBySettings = false;
		if (_cursorService.IsCursorLocked)
		{
			_cursorService.UnlockCursor();
			_cursorUnlockedBySettings = !_cursorService.IsCursorLocked;
		}
	}

	private void HideContext()
	{
		_context.Hide();

		if (_cursorUnlockedBySettings && !_cursorService.IsCursorLocked)
		{
			_cursorService.LockCursor();
		}

		_cursorUnlockedBySettings = false;
	}

	private void OnMasterChangedHandler(float value)
	{
		_audioService.SetMasterVolume(value);
	}

	private void OnMusicChangedHandler(float value)
	{
		_audioService.SetMusicVolume(value);
	}

	private void OnSfxChangedHandler(float value)
	{
		_audioService.SetSfxVolume(value);
	}

	private void OnCloseClickedHandler()
	{
		HideContext();
	}

	private void OnMainMenuClickedHandler()
	{
		HideContext();
	}
}
