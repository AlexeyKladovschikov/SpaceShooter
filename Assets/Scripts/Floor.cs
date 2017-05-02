using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour {

    private Action<IEnemy> _onEnemyLeftArea = (enemy) => { };
    private Action<IBonus> _onBonusLeftArea = (enemy) => { };

    public void Initialize(Action<IEnemy> onEnemyLeftArea, Action<IBonus> onBonusLeftArea)
    {
        _onEnemyLeftArea = onEnemyLeftArea;
        _onBonusLeftArea = onBonusLeftArea;
    }

   private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collosoinGameObject = collision.gameObject;
        if (collosoinGameObject.CompareTag(Arena.EnemyTag) && collision.enabled)
        {
            _onEnemyLeftArea(collosoinGameObject.transform.parent.GetComponent<IEnemy>());
        }
        else if (collosoinGameObject.CompareTag(Arena.BonusTag) && collision.enabled)
        {
            _onBonusLeftArea(collosoinGameObject.GetComponent<IBonus>());
        }
        
    }
    
}
