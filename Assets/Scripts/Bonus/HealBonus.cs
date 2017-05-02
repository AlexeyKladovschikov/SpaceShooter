using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class HealBonus : Bonus
{
    [Zenject.Inject]
    private void Inject(HealBonusConfig config)
    {
        Config = config;
    }

    public override BonusCategory Category
    {
        get { return BonusCategory.Heal; }
    }
}

