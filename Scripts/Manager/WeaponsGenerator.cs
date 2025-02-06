using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SpaceWar.Audios;

namespace SpaceWar
{
    public class WeaponsGenerator : Singlton<WeaponsGenerator>
    {
        [System.Serializable]
        public class WeaponHolder
        {
            public BulletWeapon Weapon;
            public ShipRank Rank;
        }

        private BulletWeapon m_CurrentWeapon;
        private ShipRank m_CurrentRank;
        private ShipRankValues m_CurrentValues;
        public List<WeaponHolder> Weapons = new List<WeaponHolder>();
        [SerializeField] private List<Audio> m_Audios;

        public enum WeaponStyles
        {
            OneStraightShot, MultipleStraightShots,
            AngledShots, AngleUpdatingShots, Sniper
        }
        [SerializeField] private WeaponStyles m_Style;
        [Header("Shared Modifiers")]
        [SerializeField] private Vector2 m_BulletAngleRange;
        [SerializeField] private Vector2 m_BulletOffsetRange;


        protected override void Awake()
        {
            base.Awake();
            Weapons.Clear();
        }

        public BulletWeapon CreateWeapon(ShipRank _rank, float _offensive = 0.5f)
        {
            if (GameManager.i.GeneralValues == null) return new BulletWeapon();

            m_CurrentWeapon = ScriptableObject.CreateInstance<BulletWeapon>();
            Weapons.Add(new WeaponHolder() { Weapon = m_CurrentWeapon, Rank = _rank });
            m_CurrentRank = _rank;

            m_CurrentValues = GameManager.i.GeneralValues.RankValues.Find(r => r.Rank == m_CurrentRank);

            m_CurrentWeapon.DamagePerShot = m_CurrentValues.DamageRange.Lerp(_offensive);
            m_CurrentWeapon.MuzzleTag = ParticlePoolTag.StandardMuzzle;

            WeaponStyles newStyle = GetWeaponStyle();

            switch (newStyle)
            {
                case WeaponStyles.OneStraightShot:
                    {
                        CreateStraightBullets(_offensive);
                        break;
                    }
                case WeaponStyles.MultipleStraightShots:
                    {
                        CreateMultipleStraightShots(_offensive);
                        break;
                    }
                case WeaponStyles.AngledShots:
                    {
                        CreateAngledShots(_offensive);
                        break;
                    }
                case WeaponStyles.AngleUpdatingShots:
                    {
                        CreateUpdatedAngledShots(_offensive);
                        break;
                    }
                case WeaponStyles.Sniper:
                    {
                        CreateSniperShots(_offensive);
                        break;
                    }
            }

            if (m_Audios.Count > 0) m_CurrentWeapon.Audio = m_Audios.RandomItem();

            SetValuesBasedOnRank(_offensive, newStyle);
            return m_CurrentWeapon;
        }

        public BulletWeapon CreateWeapon(ShipRank _rank, List<WeaponStyles> _avoidedStyles, float _offensive = 0.5f)
        {
            if (GameManager.i.GeneralValues == null) return new BulletWeapon();

            m_CurrentWeapon = ScriptableObject.CreateInstance<BulletWeapon>();
            Weapons.Add(new WeaponHolder() { Weapon = m_CurrentWeapon, Rank = _rank });
            m_CurrentRank = _rank;

            m_CurrentValues = GameManager.i.GeneralValues.RankValues.Find(r => r.Rank == m_CurrentRank);

            m_CurrentWeapon.DamagePerShot = m_CurrentValues.DamageRange.Lerp(_offensive);
            m_CurrentWeapon.MuzzleTag = ParticlePoolTag.StandardMuzzle;

            WeaponStyles newStyle = GetWeaponStyle(_avoidedStyles);

            switch (newStyle)
            {
                case WeaponStyles.OneStraightShot:
                    {
                        CreateStraightBullets(_offensive);
                        break;
                    }
                case WeaponStyles.MultipleStraightShots:
                    {
                        CreateMultipleStraightShots(_offensive);
                        break;
                    }
                case WeaponStyles.AngledShots:
                    {
                        CreateAngledShots(_offensive);
                        break;
                    }
                case WeaponStyles.AngleUpdatingShots:
                    {
                        CreateUpdatedAngledShots(_offensive);
                        break;
                    }
                case WeaponStyles.Sniper:
                    {
                        CreateSniperShots(_offensive);
                        break;
                    }
            }

            m_CurrentWeapon.Audio = m_Audios.RandomItem();
            SetValuesBasedOnRank(_offensive, newStyle);
            return m_CurrentWeapon;
        }

        private void SetValuesBasedOnRank(float _difficulty, WeaponStyles _style)
        {
            //print($"Selected style is {_style} for {m_CurrentRank}");
            // cool down time
            m_CurrentWeapon.CoolDownTime = m_CurrentValues.StyleChances.Find(s => s.Style == _style).CoolDownRange.Lerp(1 - _difficulty);

            float perShooterChance = m_CurrentValues.PerShooterChanceRange.Lerp(_difficulty);

            // per shooting point is considered the strongest. Going through specific shooting points should not be considered as enemies have randomized shooting points
            // per shot and per set are the same in terms of damage, so to avoid unnecessary or rather ugly random sets, we'll just use the per shot
            // if the rank allows a per shot property then there's a chance it is used and the ship might get it (just to add variety)
            if (perShooterChance > 0f)
            {
                if (Random.value <= perShooterChance)
                {
                    m_CurrentWeapon.TheShootingMode = ShootingMode.PerAllSP;
                }
                else
                {
                    m_CurrentWeapon.TheShootingMode = ShootingMode.ThroughAllSPPerShot;
                }
            }
            else
            {
                m_CurrentWeapon.TheShootingMode = ShootingMode.ThroughAllSPPerShot;
            }
        }

        #region Shooting methods
        public void CreateStraightBullets(float _difficulty)
        {
            float speed = m_CurrentValues.BulletSpeedRange.Lerp(_difficulty);
            m_CurrentWeapon.Shots.Clear();
            m_CurrentWeapon.Shots.Add(new Shot { Speed = speed, Angle = 0, OffsetX = 0 });
        }

        public void CreateMultipleStraightShots(float _difficulty)
        {
            float speed = m_CurrentValues.BulletSpeedRange.Lerp(_difficulty);
            int shotsNumber = (int)m_CurrentValues.ShotsNumberRange.Lerp(_difficulty);
            bool even = shotsNumber % 2 == 0;
            float offset = m_BulletOffsetRange.Lerp(_difficulty);
            m_CurrentWeapon.Shots.Clear();

            if (even)
            {
                for (int i = 0; i < shotsNumber / 2; i++)
                {
                    m_CurrentWeapon.Shots.Add(new Shot { Speed = speed, Angle = 0, OffsetX = offset * (i + 1) });
                    m_CurrentWeapon.Shots.Add(new Shot { Speed = speed, Angle = 0, OffsetX = -offset * (i + 1) });
                }
            }
            else
            {
                m_CurrentWeapon.Shots.Add(new Shot { Speed = speed, Angle = 0, OffsetX = 0 });

                for (int i = 0; i < (shotsNumber / 2) - 1; i++)
                {
                    m_CurrentWeapon.Shots.Add(new Shot { Speed = speed, Angle = 0, OffsetX = offset * (i + 1) });
                    m_CurrentWeapon.Shots.Add(new Shot { Speed = speed, Angle = 0, OffsetX = -offset * (i + 1) });
                }
            }
        }

        private void CreateAngledShots(float _difficulty)
        {
            //if (m_CurrentRank != ShipRank.Strong)
            //{
            //    print($"returned because {m_CurrentRank}");
            //    return;
            //}
            
            float speed = m_CurrentValues.BulletSpeedRange.Lerp(_difficulty);
            int shotsNumber = (int)m_CurrentValues.AngledShotsInfo.ShotsRange.Lerp(_difficulty);
            // making sure the number of shots is odd
            shotsNumber += shotsNumber % 2 == 0 ? 1 : 0;
            int anglesDividor = (int)((shotsNumber - 1) * 0.5f);
            // the total angle's value will be determined by the number of shots. The more shots the weapon has, the wider the angle should be, otherwise it'll be impossible to escape the bullets
            float totalAngle = m_CurrentValues.AngledShotsInfo.AnglesRange.Lerp(Random.Range(_difficulty, 1f));
            float anglePerShot = totalAngle / anglesDividor;

            // add a shot in the middle that goes straight
            Shot newShot = new Shot();
            newShot.SetUp(0f, speed, 0f);
            m_CurrentWeapon.Shots.Add(newShot);
            int loops = (int)((float)(shotsNumber - 1f) / 2f);

            for (int i = 0; i < loops; i++)
            {
                newShot = new Shot();
                newShot.SetUp(anglePerShot * (i + 1), speed, 0f);
                m_CurrentWeapon.Shots.Add(newShot);
                newShot = new Shot();
                newShot.SetUp(-anglePerShot * (i + 1), speed, 0f);
                m_CurrentWeapon.Shots.Add(newShot);
            }

            print($"{m_CurrentRank}: Shots range is {shotsNumber} and weapon has {m_CurrentWeapon.Shots.Count} shots. Loops: {loops}. Speed: {speed}");
            m_CurrentWeapon.TheShootingMode = ShootingMode.ThroughAllSPPerSet;
        }

        private void CreateUpdatedAngledShots(float _difficulty)
        {
            // this too
            float speed = m_CurrentValues.BulletSpeedRange.Lerp(_difficulty);
            m_CurrentWeapon.Shots.Add(new Shot { Speed = speed, Angle = 0, OffsetX = 0 });
            m_CurrentWeapon.ShootingType = BulletWeapon.ShootingTypes.AngleUpdatePerIndex;
            m_CurrentWeapon.UpdatedShotsVariables.AngleRange = m_CurrentValues.UpdatedAnglesValues.AngleRange.Lerp(_difficulty);
            m_CurrentWeapon.UpdatedShotsVariables.ShotsNumber = (int)m_CurrentValues.UpdatedAnglesValues.ShotsNumberRange.Lerp(_difficulty);
            m_CurrentWeapon.UpdatedShotsVariables.CoolDownTimeBetweenShots = m_CurrentValues.UpdatedAnglesValues.CoolDownRange.Lerp(_difficulty);
            m_CurrentWeapon.UpdatedShotsVariables.HalfLoop = m_CurrentValues.UpdatedAnglesValues.HalfLoop;
        }

        int count = 0;
        private void CreateSniperShots(float _difficulty)
        {
            #region Locals
            int numberOfValuesToSet = 6;
            float anglePerShot = 0f;
            float timeBetweenShots = 0f;
            List<float> values = new List<float>();

            // a local method to keep things clean
            float GetValue()
            {
                int selectedValueIndex = Random.Range(0, values.Count);
                float value = values[selectedValueIndex];
                values.RemoveAt(selectedValueIndex);
                return value;
            }
            #endregion

            count++;
            // get random values that will be randomly selected for each weapon attribute
            for (int i = 0; i < numberOfValuesToSet; i++)
            {
                values.Add(Random.Range(0.1f, 0.3f));
            }

            #region Setting Values
            // speed of the shots
            float t = GetValue();
            float speed = m_CurrentValues.SniperMachineGunValues.Speed.Lerp(t * _difficulty);


            // bullets number
            t = GetValue();
            int bulletsNumber = (int)m_CurrentValues.SniperMachineGunValues.BulletsPerShot.Lerp(t * _difficulty);


            // shots number
            t = GetValue();
            int shotsNumber = (int)m_CurrentValues.SniperMachineGunValues.NumberOfShots.Lerp(t * _difficulty);


            // time between shots
            if (shotsNumber > 1)
            {
                t = GetValue();
                timeBetweenShots = m_CurrentValues.SniperMachineGunValues.TimeBetweenShots.Lerp(t * _difficulty);
            }

            // angle
            if (bulletsNumber > 1)
            {
                t = GetValue();
                float angle = m_CurrentValues.SniperMachineGunValues.Angle.Lerp(t * _difficulty);
                int anglesDividor = (int)((bulletsNumber - 1) * 0.5f);
                anglePerShot = angle / anglesDividor;
            }
            #endregion

            // we can either have same number of bullets for all shots or have them gradually increment over shots
            List<ShotGroup> shots = new List<ShotGroup>();
            Vector2Int shotsValues;

            // if we have more than one bullet, there is a chance that we will have the weapon increment the bullets count with each shot
            if (Random.value > 0.5f && bulletsNumber > 1)
            {
                shotsValues = new Vector2Int(1, bulletsNumber);
            }
            else
            {
                shotsValues = new Vector2Int(bulletsNumber, bulletsNumber);
            }

            for (int i = 0; i < shotsNumber; i++)
            {
                float bulletsNumberT = i / shotsNumber;
                int currentBulletsNumber = (int)shotsValues.Lerp(bulletsNumberT);
                ShotGroup currentShots = new ShotGroup();
                shots.Add(currentShots);
                currentShots.Shots = new List<Shot>();

                // if the number of bullets is odd then we set the first bullet in the middle with no angle and decrease the number of total bullets in the shot
                if (currentBulletsNumber % 2 == 1)
                {
                    currentShots.Shots.Add(new Shot { Speed = speed, Angle = 0, OffsetX = 0 });
                    currentBulletsNumber--;
                }

                int loops = (currentBulletsNumber / 2) - 1;

                for (int j = 0; j < loops; j++)
                {
                    currentShots.Shots.Add(new Shot { Speed = speed, Angle = anglePerShot * (j + 1), OffsetX = 0 });
                    currentShots.Shots.Add(new Shot { Speed = speed, Angle = -anglePerShot * (j + 1), OffsetX = 0 });
                }
            }

            m_CurrentWeapon.TheSniperMachineGunValues.ShotsGroup = shots;
            m_CurrentWeapon.TheSniperMachineGunValues.TimeBetweenShots = timeBetweenShots;
            m_CurrentWeapon.TheShootingMode = (ShootingMode)Random.Range(0, 5);
            m_CurrentWeapon.ShootingType = BulletWeapon.ShootingTypes.SniperMachineGun;
            m_CurrentWeapon.CoolDownTime = m_CurrentValues.SniperMachineGunValues.CoolDownTime.Lerp(_difficulty);
        }
        #endregion

        private WeaponStyles GetWeaponStyle()
        {
            List<WeaponStyleChances> stylesChances = GameManager.i.GeneralValues.RankValues.Find(s => s.Rank == m_CurrentRank).StyleChances;
            float chance = Random.value * stylesChances.OrderByDescending(s => s.Chance).FirstOrDefault().Chance;
            // we select the weapon styles that have the chance that is smaller than or equals the random number
            List<WeaponStyleChances> selectedStyles = stylesChances.FindAll(w => w.Chance >= chance);

            // if there are no weapons matching this chance then we take a random weapon where the chance is not zero
            if (selectedStyles.Count == 0)
            {
                return stylesChances[Random.Range(0, stylesChances.Count)].Style;
            }
            else
            {
                return selectedStyles[Random.Range(0, selectedStyles.Count)].Style;
            }
        }

        private WeaponStyles GetWeaponStyle(List<WeaponStyles> _avoidedStyles)
        {
            List<WeaponStyleChances> stylesChances = GameManager.i.GeneralValues.RankValues.Find(s => s.Rank == m_CurrentRank).StyleChances;
            List<WeaponStyleChances> selectedChances = stylesChances.FindAll(s => !_avoidedStyles.Contains(s.Style));

            float chance = Random.value;
            // we select the weapon styles that have the chance that is smaller than or equals the random number
            List<WeaponStyleChances> selectedStyles = stylesChances.FindAll(w => w.Chance >= chance);

            // if there are no weapons matching this chance then we take a random weapon where the chance is not zero
            if (selectedStyles.Count == 0)
            {
                return stylesChances[Random.Range(0, stylesChances.Count)].Style;
            }
            else
            {
                return selectedStyles[Random.Range(0, selectedStyles.Count)].Style;
            }
        }
    }

    [System.Serializable]
    public struct ChasingWeaponValues
    {
        public float Precision;
    }
}