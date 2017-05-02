using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusPool : MonoBehaviour {

    private int _poolLength;
    private LevelInstaller.BonusFactory _bonusFactory;
    private Dictionary<BonusCategory, Stack<IBonus>> _bonuses;

    private void Awake()
    {
        Initialize();
    }

    [Zenject.Inject]
    private void Inject(LevelInstaller.BonusFactory bonusFactory, int poolLength)
    {
        _bonusFactory = bonusFactory;
        _poolLength = poolLength;
    }

    private void Initialize()
    {
        _bonuses = new Dictionary<BonusCategory, Stack<IBonus>>();
        foreach (BonusCategory category in Enum.GetValues(typeof(BonusCategory)))
        {
            _bonuses.Add(category, new Stack<IBonus>(_poolLength));
            for (int count = 0; count < _poolLength; ++count)
            {
                _bonuses[category].Push(InstantiateBonus(category));
            }
        }
    }

    private IBonus InstantiateBonus(BonusCategory category)
    {
        IBonus projectile = _bonusFactory.Create(category);
        projectile.Dispose();
        return projectile;
    }

    public IBonus TakeBonus(BonusCategory category)
    {
        if (_bonuses[category].Count <= 0)
        {
            _bonuses[category].Push(InstantiateBonus(category));
        }

        IBonus bonus = _bonuses[category].Pop();
        bonus.SetOnDisposeAction(DisposeBonus);

        return bonus;
    }

    public void DisposeBonus(IBonus bonus)
    {
        if (bonus == null)
        {
            return;
        }

        bonus.Dispose();

        _bonuses[bonus.Category].Push(bonus);
    }
}
