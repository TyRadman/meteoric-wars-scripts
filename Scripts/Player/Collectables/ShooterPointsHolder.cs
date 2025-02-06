using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ShooterPointsHolder : Collectable
    {
        public override void OnTriggerAction(PlayerComponents _player)
        {
            _player.Stats.AddXPPoints(CollectablePoints);
            PointsPopUp.i.CallText(transform.position, PointsPopUp.PopUpTextType.ShooterPoints, CollectablePoints);
            base.OnTriggerAction(_player);
        }
    }
}