using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ImmuneBonus : Bonus
{
    [Zenject.Inject]
    private void Inject(ImmuneBonusConfig config)
    {
        Config = config;
    }  

    public override BonusCategory Category
    {
        get { return BonusCategory.Immune; }
    }
}