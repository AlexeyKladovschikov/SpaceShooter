using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour {

    private int _poolLength;
    private LevelInstaller.EnemyFactory _enemyFactory;
    private Dictionary<EnemyCategory, Stack<IEnemy>> _enemies;
    private void Awake()
    {
        Initialize();
    }

    [Zenject.Inject]
    private void Inject(LevelInstaller.EnemyFactory enemyFactory, int poolLength)
    {
        _enemyFactory = enemyFactory;
        _poolLength = poolLength;
    }

    private void Initialize()
    {
        _enemies = new Dictionary<EnemyCategory, Stack<IEnemy>>();
        foreach (EnemyCategory category in Enum.GetValues(typeof (EnemyCategory)))
        {
            _enemies.Add(category, new Stack<IEnemy>(_poolLength));
            for (int count = 0; count < _poolLength; ++count)
            {
                _enemies[category].Push(InstantiateEnemy(category));
            }
        }
        
    }

    private IEnemy InstantiateEnemy(EnemyCategory category)
    {
        IEnemy enemy = _enemyFactory.Create(category);
        enemy.Dispose();
        return enemy;
    }

    public IEnemy TakeEnemy(EnemyCategory category)
    {
        if (_enemies[category].Count <= 0)
        {
            _enemies[category].Push(InstantiateEnemy(category));
        }

        IEnemy enemy = _enemies[category].Pop();
        enemy.SetOnDisposeAction(DisposeEnemy);

        return enemy;
    }

    public void DisposeEnemy(IEnemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.Dispose();

        _enemies[enemy.Category].Push(enemy);
    }
}
