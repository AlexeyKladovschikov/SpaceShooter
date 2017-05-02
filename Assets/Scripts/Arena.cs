using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Arena : MonoBehaviour
{
    [SerializeField]
    private Floor _floor;
    [SerializeField]
    private Parallax _background;
    [SerializeField]
    private Parallax _stars;
    [SerializeField]
    private Parallax _yellowStars;

    public Bounds Bounds;
    private ArenaConfig _config;
    private EnemyPool _enemyPool;
    private BonusPool _bonusPool;
    private List<IEnemy> _enemies;
    private float _spawnTimer;
    private float _spawnDelay;
    private float _levelUpTimer;
    private int _score;
    private int _currentLevel;
    private Player _player;
    private bool _playMode;
    private Action _onPlayerDied;
    public const string PlayerTag = "Player";
    public const string WallTag = "Wall";
    public const string ProjectileTag = "Projectile";
    public const string EnemyProjectileLayer = "EnemyProjectile";
    public const string PlayerProjectileLayer = "PlayerProjectile";
    public const string EnemyTag = "Enemy";
    public const string FloorTag = "Floor";
    public const string BonusTag = "Bonus";

    private Vector2 _savedOffset;
    void Start()
    {
        Initialize();
    }

    [Zenject.Inject]
    private void Inject(ArenaConfig config, Player player, EnemyPool enemyPool, BonusPool bonusPool)
    {
        _config = config;
        _player = player;
        _enemyPool = enemyPool;
        _bonusPool = bonusPool;
    }

    private void Initialize()
    {
        _spawnTimer = 0;
        _spawnDelay = _config.EnemySpawnDelay;
        _levelUpTimer = _config.LevelUpTimer;
        _score = 0;
        _currentLevel = 0;
        _enemies = new List<IEnemy>();
        _playMode = true;
        _onPlayerDied = () => _playMode = false;
        EventManager.OnEnemyDied += OnEnemyDied;
        EventManager.OnPlayerDied += _onPlayerDied;
        EventManager.Pause += Pause;
        EventManager.UnPause += UnPause;
        _floor.Initialize(OnEnemyLeftArea, OnBonusLeftArea);

        _background.Initialize(_player);
        _stars.Initialize(_player);
        _yellowStars.Initialize(_player);

        SetBounds();
        EventManager.Pause();

    }

    private void SetBounds()
    {
        Bounds bounds = GetComponent<Renderer>().bounds;
        float ratioScaleH = _config.Height / bounds.size.y;
        float ratioScaleW = _config.Width / bounds.size.x;
        transform.localScale = new Vector3(ratioScaleW, ratioScaleH, 1);

        Bounds = new Bounds(Vector3.zero, new Vector3(bounds.size.x * transform.localScale.x, bounds.size.y * transform.localScale.y));
    }

    void Update()
    {
        if (!_playMode) return;

        PlayerControl();

        if (_spawnTimer < 0)
        {
            SpawnEnemy();
        }

        if (_levelUpTimer < 0)
        {
            LevelUp();
        }

        _levelUpTimer -= Time.deltaTime;
        _spawnTimer -= Time.deltaTime;

    }

    private void PlayerControl()
    {
        float dx = Input.GetAxis("Horizontal");
        _player.MoveTo(dx);

        if (Input.GetButtonDown("Jump"))
        {
            _player.StartFire();
        }
        else if (Input.GetButtonUp("Jump"))
        {
            _player.EndFire();
        }
        else if (_player.HasFire && !Input.GetButton("Jump"))
        {
            _player.EndFire();
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            _player.PrevWeapon();
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            _player.NextWeapon();
        }
    }

    private void LevelUp()
    {
        _currentLevel++;
        _spawnDelay *= 0.9f;
        _levelUpTimer = _config.LevelUpTimer;
        Debug.Log(_currentLevel + " " + _spawnDelay);
    }

    private void SpawnEnemy()
    {
        Array categories = Enum.GetValues(typeof(EnemyCategory));

        IEnemy enemy = _enemyPool.TakeEnemy((EnemyCategory)UnityEngine.Random.Range(0, categories.Length));

        enemy.Reset(GetRandomPointInSpawnRadius(), _currentLevel);
        _enemies.Add(enemy);

        _spawnTimer = _spawnDelay;
    }

    private Vector2 GetRandomPointInSpawnRadius()
    {
        Vector2 dir = new Vector2(Random.Range(Bounds.min.x * 0.75f, Bounds.max.x * 0.75f), Bounds.max.y * 1.25f);

        return dir;
    }

    private void OnEnemyDied(IEnemy enemy)
    {
        SpawnBonus(enemy.Position);
        ScoreUp();
    }

    private void ScoreUp()
    {
        _score++;
        EventManager.OnScoreUp(_score);
    }

    private void OnEnemyLeftArea(IEnemy enemy)
    {
        if (enemy.IsActive)
        {
            _enemyPool.DisposeEnemy(enemy);
        }
    }

    private void OnBonusLeftArea(IBonus bonus)
    {
        if (bonus.IsActive)
        {
            _bonusPool.DisposeBonus(bonus);
        }
    }
    
    private void SpawnBonus(Vector2 position)
    {
        int roll = Random.Range(0, 100);
        if (roll >= 20) return;

        Array categories = Enum.GetValues(typeof(BonusCategory));
        IBonus bonus = _bonusPool.TakeBonus((BonusCategory)UnityEngine.Random.Range(0, categories.Length));
        bonus.Reset(position);
    }

    private void OnDestroy()
    {
        EventManager.OnEnemyDied -= OnEnemyDied;
        EventManager.OnPlayerDied -= _onPlayerDied;
        EventManager.Pause -= Pause;
        EventManager.UnPause -= UnPause;
    }

    private void Pause()
    {
        Time.timeScale = 0f;
    }

    private void UnPause()
    {
        Time.timeScale = 1f;
    }
}
