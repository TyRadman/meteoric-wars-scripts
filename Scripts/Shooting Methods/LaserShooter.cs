using SpaceWar.Audios;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class LaserShooter : MonoBehaviour
    {
        [System.Serializable]
        public struct CreatedLaserWeaponInfo
        {
            public LaserBullet LaserObject;
            public LaserWeapon Weapon;
        }

        public enum ShooterType
        {
            Normal, Ability
        }

        public ShooterType TheShooterType = ShooterType.Normal;
        [SerializeField] private List<CreatedLaserWeaponInfo> m_CreatedLasers = new List<CreatedLaserWeaponInfo>();
        public LaserWeapon CurrentWeapon;
        [SerializeField] private ParticleSystem m_LineParticles;
        public BulletUser UserTag;
        private Transform m_ShootingPoint;
        private LaserBullet m_LaserLine;
        private Shooter m_Shooter;
        private bool m_CanShoot = true;
        public bool IsShooting = false;
        [SerializeField] private Audio m_ShootingAudio;

        private void Awake()
        {
            if (GetComponent<Shooter>() != null)
            {
                m_Shooter = GetComponent<Shooter>();
            }
        }

        public void Shoot()
        {
            if (!m_CanShoot || IsShooting)
            {
                return;
            }

            m_CanShoot = false;

            StartCoroutine(ShootingProcess());
        }

        private IEnumerator ShootingProcess()
        {
            AudioSource source = GameManager.i.AudioManager.PlayAudioAndGetSource(m_ShootingAudio);

            IsShooting = true;
            m_LaserLine.Activate(true);
            float resizingTime = CurrentWeapon.ResizingTime;
            float time = 0f;
            float frequency = CurrentWeapon.Frequency;
            float amplitude = CurrentWeapon.Amplitude;
            float width = CurrentWeapon.Width;
            float duration = CurrentWeapon.ShotDuration;

            // scaling up
            while (time < resizingTime)
            {
                time += Time.deltaTime;
                float t = time / resizingTime;
                m_LaserLine.transform.position = m_ShootingPoint.position;
                m_LaserLine.SetWidth(Mathf.Lerp(0f, width, t));
                yield return null;
            }

            time = 0f;

            // stabalizing
            while (time < duration)
            {
                time += Time.deltaTime;
                m_LaserLine.transform.position = m_ShootingPoint.position;
                m_LaserLine.SetWidth(width + Mathf.Sin(Time.time * frequency) * amplitude);
                yield return null;
            }

            time = 0f;
            float currentWidth = m_LaserLine.GetLaserWidth();

            // scaling down
            while (time < resizingTime)
            {
                time += Time.deltaTime;
                float t = time / resizingTime;
                m_LaserLine.transform.position = m_ShootingPoint.position;
                m_LaserLine.SetWidth(Mathf.Lerp(currentWidth, 0f, t));
                yield return null;
            }

            if (source != null)
            {
                source.Stop();
            }

            m_LaserLine.Activate(false);
            Invoke(nameof(EnableShooting), CurrentWeapon.CoolDownTime);
            IsShooting = false;
        }

        private void EnableShooting()
        {
            m_CanShoot = true;
        }

        public void StopShooting()
        {
            StopAllCoroutines();
            m_LaserLine.Activate(false);
            return;
            StartCoroutine(StopShootingProcess());
        }

        private IEnumerator StopShootingProcess()
        {
            float resizingTime = CurrentWeapon.ResizingTime;
            float time = 0f;
            float currentWidth = m_LaserLine.GetLaserWidth();

            // scaling down
            while (time < resizingTime)
            {
                time += Time.deltaTime;
                float t = time / resizingTime;
                m_LaserLine.transform.position = m_ShootingPoint.position;
                m_LaserLine.SetWidth(Mathf.Lerp(currentWidth, 0f, t));
                yield return null;
            }

            m_LaserLine.Activate(false);
            Invoke(nameof(EnableShooting), CurrentWeapon.CoolDownTime);
            IsShooting = false;
        }

        #region Set Ups
        public void SetWeapon(LaserWeapon _weapon)
        {
            // if this laser shooter already has set up this weapon then there is no need to start it again
            if (m_CreatedLasers.Exists(w => w.Weapon == _weapon))
            {
                var info = m_CreatedLasers.Find(l => l.Weapon == _weapon);
                m_LaserLine = info.LaserObject;
                CurrentWeapon = info.Weapon;
                return;
            }

            CurrentWeapon = _weapon;
            m_ShootingAudio = _weapon.Audio;
            m_LaserLine = Instantiate(GameManager.i.GeneralValues.LaserPrefab, m_ShootingPoint);
            m_LaserLine.transform.localPosition = Vector2.zero;

            int userIndex = -1;

            if (m_Shooter.TryGetComponent(out PlayerComponents components))
            {
                userIndex = components.PlayerIndex;
            }

            //print($"Set up the laser bullet, player index is {userIndex}");
            m_LaserLine.SetUp(CurrentWeapon.DamagePerSecond, Helper.ReverseUser(UserTag).ToString(), userIndex, _weapon.Color);
            m_LaserLine.Activate(false);
            m_CreatedLasers.Add(new CreatedLaserWeaponInfo { LaserObject = m_LaserLine, Weapon = _weapon });
        }

        public void SetUpShootingPoints(Transform _point)
        {
            m_ShootingPoint = _point;
        }

        public void SetShooter(Shooter _shooter)
        {
            m_Shooter = _shooter;
        }
        #endregion
    }
}