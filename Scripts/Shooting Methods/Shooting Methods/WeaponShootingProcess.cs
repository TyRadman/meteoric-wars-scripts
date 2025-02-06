using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SpaceWar.Audios;

namespace SpaceWar
{
    public class WeaponShootingProcess : MonoBehaviour
    {
        protected int ShootingPointIndex = 0;
        public List<Transform> ShootingPoints;
        public float CoolingDownTime;
        public WaitForSeconds UpdateRate;
        public UnityAction ShootingAction;
        public Shooter TheShooter;
        public bool HasAudio = false;
        public Audio ShootingAudio;

        protected virtual void Awake()
        {
            UpdateRate = new WaitForSeconds(FindObjectOfType<GeneralValues>().UpdateRate);
        }

        #region Main Functionality 
        public virtual void SetUpComponents(Shooter _shooter)
        {
            ShootingPoints = _shooter.ShootingPoints;
            TheShooter = _shooter;
            SetUpShootingAction();
        }

        public virtual void StartShootingProcess()
        {
            ShootingAction();
        }

        public virtual void SetUpShootingAction()
        {

        }

        public void PlayAudio()
        {
            if (HasAudio)
            {
                GameManager.i.AudioManager.PlayAudio(ShootingAudio);
            }
        }
        #endregion

        #region General Helping Functions
        public Vector3 GetShootingPointPosition(int _index, int _shootingPointIndex)
        {
            Vector3 pos = ShootingPoints[_shootingPointIndex].position + Vector3.left * TheShooter.Weapon().Shots[_index].OffsetX;
            return pos;
        }

        public Vector3 GetShootingPointPosition(int _shootingPointIndex)
        {
            return ShootingPoints[_shootingPointIndex].position;
        }

        public int GetNextShootingPointIndex(int _maxNumber)
        {
            ShootingPointIndex++;

            if (ShootingPointIndex >= _maxNumber)
            {
                ShootingPointIndex = 0;
            }

            return ShootingPointIndex;
        }
        #endregion
    }
}