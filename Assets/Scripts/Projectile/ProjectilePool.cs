using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ProjectilePool : MonoBehaviour {

    private int _poolLength;
    private LevelInstaller.ProjectileFactory _projectileFactory;
    private Dictionary<ProjectileCategory, Stack<IProjectile>> _projectiles;

    private void Awake()
    {
        Initialize();
    }
    
    [Zenject.Inject]
    private void Inject(LevelInstaller.ProjectileFactory projectileFactory, int poolLength)
    {
        _projectileFactory = projectileFactory;
        _poolLength = poolLength;
    }

    private void Initialize()
    {
        _projectiles = new Dictionary<ProjectileCategory, Stack<IProjectile>>();
        foreach (ProjectileCategory category in Enum.GetValues(typeof(ProjectileCategory)))
        {
            _projectiles.Add(category, new Stack<IProjectile>(_poolLength));
            for (int count = 0; count < _poolLength; ++count)
            {
                _projectiles[category].Push(InstantiateProjectile(category));
            }
        }
    }

    private IProjectile InstantiateProjectile(ProjectileCategory category)
    {
        IProjectile projectile = _projectileFactory.Create(category);
        projectile.Dispose();
        return projectile;
    }

    public IProjectile TakeProjectile(ProjectileCategory category)
    {
        if (_projectiles[category].Count <= 0)
        {
            _projectiles[category].Push(InstantiateProjectile(category));
        }

        IProjectile projectile = _projectiles[category].Pop();
        projectile.SetOnDisposeAction(DisposeProjectile);
        return projectile;
    }

    public void DisposeProjectile(IProjectile projectile)
    {
        if (projectile == null)
        {
            return;
        }

        projectile.Dispose();

        _projectiles[projectile.Category].Push(projectile);
    }

}
