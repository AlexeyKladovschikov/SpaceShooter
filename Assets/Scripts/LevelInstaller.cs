using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [SerializeField]
    private LevelConfig _config;

    private float _worldScreenHeight;
    private float _worldScreenWidth;
    
    
    public override void InstallBindings()
    {
        CalculateScreenSize();
        _config.Arena.Width = ScreenWidthPercentToFloat(_config.Arena.Width);
        _config.Arena.Height = ScreenHeightPercentToFloat(_config.Arena.Height);
        _config.Camera.BorderMove = 1 - 2 * _config.Camera.BorderMove / 100;

        Container.BindInstance(_config.Arena);
        Container.BindInstance(_config.Player);

        Container.BindInstance(_config.Enemies.SpeedyEnemy).WhenInjectedInto<SpeedyEnemy>();
        Container.BindInstance(_config.Enemies.HardyEnemy).WhenInjectedInto<HardyEnemy>();

        Container.BindInstance(_config.Bonuses.HealConfig).WhenInjectedInto<HealBonus>();
        Container.BindInstance(_config.Bonuses.ImmuneConfig).WhenInjectedInto<ImmuneBonus>();
        Container.BindInstance(_config.Bonuses.FireRateConfig).WhenInjectedInto<FireRateBonus>();

        Container.BindInstance(_config.Player.GunWeaponConfig).WhenInjectedInto<GunWeapon>();
        Container.BindInstance(_config.Player.RocketWeaponConfig).WhenInjectedInto<RocketWeapon>();

        Container.BindInstance(_config.Projectiles.RocketProjectile).WhenInjectedInto<RocketProjectile>();
        Container.BindInstance(_config.Projectiles.GunProjectile).WhenInjectedInto<GunProjectile>();

        Container.BindInstance(_config.Camera);

        ProjectilePool projectilePool = transform.Find("ProjectilePool").GetComponent<ProjectilePool>();
        Container.BindInstance(projectilePool);
        EnemyPool enemyPool = transform.Find("EnemyPool").GetComponent<EnemyPool>();
        Container.BindInstance(enemyPool);
        BonusPool bonusPool = transform.Find("BonusPool").GetComponent<BonusPool>();
        Container.BindInstance(bonusPool);

        Container.BindInstance(_config.Projectiles.GunProjectile.PoolSize).WhenInjectedIntoInstance(projectilePool);
        Container.BindInstance(_config.Enemies.HardyEnemy.PoolSize).WhenInjectedIntoInstance(enemyPool);
        Container.BindInstance(2).WhenInjectedIntoInstance(bonusPool);

        Player player = transform.Find("Player").GetComponent<Player>();
        
        Container.BindInstance(player);

        Container.Bind<EnemyFactory>().AsSingle().WithArguments(transform.Find("EnemyPool"));
        Container.Bind<ProjectileFactory>().AsSingle().WithArguments(transform.Find("ProjectilePool"));
        Container.Bind<BonusFactory>().AsSingle().WithArguments(transform.Find("BonusPool"));


        var gunWeapon = player.transform.Find("GunWeapon").GetComponent<GunWeapon>();
        Container.Bind<IWeapon>().FromInstance(gunWeapon);

        var rocketWeapon = player.transform.Find("RocketWeapon").GetComponent<RocketWeapon>();
        Container.Bind<IWeapon>().FromInstance(rocketWeapon);

    }

    private void CalculateScreenSize()
    {
        _worldScreenHeight = Camera.main.orthographicSize * 2f;
        _worldScreenWidth = _worldScreenHeight / Screen.height * Screen.width;
    }

    private float ScreenWidthPercentToFloat(float widthPercent)
    {
        return _worldScreenWidth * (widthPercent / 100);
    }

    private float ScreenHeightPercentToFloat(float heightPercent)
    {
        return _worldScreenHeight * (heightPercent / 100);
    }

    public class EnemyFactory : IFactory<EnemyCategory, IEnemy>
    {
        private DiContainer _container;
        private Transform _parentTransform;

        public EnemyFactory(DiContainer container, Transform parentTransform)
        {
            _container = container;
            _parentTransform = parentTransform;
        }

        public IEnemy Create(EnemyCategory category)
        {
            switch (category)
            {
                case EnemyCategory.Hardy:
                    return InstantiateMonster("Prefabs/HardyEnemy");
                case EnemyCategory.Speedy:
                    return InstantiateMonster("Prefabs/SpeedyEnemy");
                default:
                    Debug.LogErrorFormat("Create monster of category = {0} not implemented!", category);
                    break;
            }

            return Create(EnemyCategory.Hardy);
        }
        
        private IEnemy InstantiateMonster(string prefab)
        {
            GameObject monsterGameObject = _container.InstantiatePrefab(Resources.Load(prefab));
            monsterGameObject.transform.SetParent(_parentTransform);
            return monsterGameObject.GetComponent<IEnemy>();
        }
    }

    public class ProjectileFactory : IFactory<ProjectileCategory, IProjectile>
    {
        private DiContainer _container;
        private Transform _parentTransform;

        public ProjectileFactory(DiContainer container, Transform parentTransform)
        {
            _container = container;
            _parentTransform = parentTransform;
        }

        public IProjectile Create(ProjectileCategory category)
        {
            switch (category)
            {
                case ProjectileCategory.Gun:
                    return InstantiateMonster("Prefabs/GunProjectile");
                case ProjectileCategory.Rocket:
                    return InstantiateMonster("Prefabs/RocketProjectile");
                default:
                    Debug.LogErrorFormat("Create monster of category = {0} not implemented!", category);
                    break;
            }

            return Create(ProjectileCategory.Gun);
        }
        
        private IProjectile InstantiateMonster(string prefab)
        {
            GameObject monsterGameObject = _container.InstantiatePrefab(Resources.Load(prefab));
            monsterGameObject.transform.SetParent(_parentTransform);
            return monsterGameObject.GetComponent<IProjectile>();
        }
    }

    public class BonusFactory : IFactory<BonusCategory, IBonus>
    {
        private DiContainer _container;
        private Transform _parentTransform;

        public BonusFactory(DiContainer container, Transform parentTransform)
        {
            _container = container;
            _parentTransform = parentTransform;
        }

        public IBonus Create(BonusCategory category)
        {
            switch (category)
            {
                case BonusCategory.Heal:
                    return InstantiateMonster("Prefabs/HealBonus");
                case BonusCategory.Immune:
                    return InstantiateMonster("Prefabs/ImmuneBonus");
                case BonusCategory.FireRate:
                    return InstantiateMonster("Prefabs/FireRateBonus");
                default:
                    Debug.LogErrorFormat("Create monster of category = {0} not implemented!", category);
                    break;
            }

            return Create(BonusCategory.Heal);
        }
        
        private IBonus InstantiateMonster(string prefab)
        {
            GameObject monsterGameObject = _container.InstantiatePrefab(Resources.Load(prefab));
            monsterGameObject.transform.SetParent(_parentTransform);
            return monsterGameObject.GetComponent<IBonus>();
        }
    }
}
