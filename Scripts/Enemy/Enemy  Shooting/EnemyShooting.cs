using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemyShooting : MonoBehaviour, IController
    {
        [HideInInspector] public ShipComponenets Components;

        public virtual void PerformShooting()
        {

        }

        public void ShootingProcess()
        {
            Components.ShipShooter.Shoot();
        }

        public virtual void StopShooting()
        {

        }

        public virtual void SetUp(IController components)
        {
            Components = components as ShipComponenets;
        }

        public void Activate()
        {
            throw new System.NotImplementedException();
        }

        public void Deactivate()
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}