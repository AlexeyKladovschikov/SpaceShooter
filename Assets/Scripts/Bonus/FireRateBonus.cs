using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class FireRateBonus : Bonus
{
    [Zenject.Inject]
    private void Inject(FireRateBonusConfig config)
    {
        Config = config;
    }

    public override BonusCategory Category
    {
        get { return BonusCategory.FireRate; }
    }
}

