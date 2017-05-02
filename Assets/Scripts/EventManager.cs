using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class EventManager
{
    public static Action<IEnemy> OnEnemyDied = (enemy) => { };
    public static Action OnPlayerDied = () => { };
    public static Action<int> OnPlayerHealthChanged = (health) => { };
    public static Action<int> OnScoreUp = (score) => { };

    public static Action Pause = () => { };
    public static Action UnPause = () => { };

}

