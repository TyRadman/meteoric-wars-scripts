using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class Coin : Collectable
    {
        public override void OnTriggerAction(PlayerComponents _player)
        {
            base.OnTriggerAction(_player);
            _player.Coins.AddCoins((int)CollectablePoints);
            PointsPopUp.i.CallText(transform.position, PointsPopUp.PopUpTextType.Coins, CollectablePoints);
        }
    }
}