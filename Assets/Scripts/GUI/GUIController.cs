using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIController : MonoBehaviour
{
    [SerializeField]
    private InGamePanel _gamePanel;
    private PlayerConfig _playerConfig;

    [Zenject.Inject]
    private void Inject(PlayerConfig playerConfig)
    {
        _playerConfig = playerConfig;
    }

	void Start ()
	{
	    EventManager.OnPlayerDied += OnPlayerDied;
	    EventManager.OnPlayerHealthChanged += OnPlayerHealthChanged;
        _gamePanel.CreateLifesPanel(_playerConfig.Hp);
    }
	

    public void OnPlayerDied()
    {
        _gamePanel.DisplayGameOver();
        Invoke("ReloadScene", 3f);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnPlayerHealthChanged(int health)
    {
        _gamePanel.UpdateHealth(health);
    }

    private void OnDestroy()
    {
        EventManager.OnPlayerDied -= OnPlayerDied;
        EventManager.OnPlayerHealthChanged -= OnPlayerHealthChanged;
    }


}
