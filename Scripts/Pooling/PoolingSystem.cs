using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class PoolingSystem : MonoBehaviour
    {
        public static PoolingSystem Instance;
        public List<BulletPoolingObject> PoolingBullets = new List<BulletPoolingObject>();
        public List<ParticlesPooling> PoolingParticles = new List<ParticlesPooling>();
        public List<CollectablePointsPool> PoolingCollectables = new List<CollectablePointsPool>();
        private Transform m_PoolsParent;

        private void Awake()
        {
            Instance = this;
            m_PoolsParent = new GameObject("Pools Parent").transform;
            createPools();
        }

        #region Initiation
        public void InitialBulletCreation(BulletPoolingObject _pool)
        {
            _pool.Parent = new GameObject($"{_pool.Name} Pools ({_pool.User})").transform;
            _pool.Parent.transform.parent = m_PoolsParent;

            for (int i = 0; i < _pool.InitialNumber; i++)
            {
                var newBullet = Instantiate(_pool.Prefab, _pool.Parent).GetComponent<Bullet>();
                newBullet.SetUpColor(_pool.BulletColor);
                _pool.Objects.Add(newBullet);
                newBullet.DisableBullet();
                newBullet.ImpactParticleTag = _pool.ImpactPoolTag;
            }
        }

        public void InitialParticlesCreation(ParticlesPooling _pool)
        {
            _pool.Parent = new GameObject(_pool.Tag + " Pools").transform;
            _pool.Parent.transform.parent = m_PoolsParent;
            _pool.Objects.Clear();

            for (int i = 0; i < _pool.InitialNumber; i++)
            {
                // create the particles
                var newObject = Instantiate(_pool.Prefab, _pool.Parent).GetComponent<ParticleSystem>();
                // set the color of the particles
                if (_pool.UseColor)
                {
                    var main = newObject.main;
                    main.startColor = _pool.ParticleColor;
                }

                // add the particle to the list
                _pool.Objects.Add(newObject);
            }
        }

        public void InitialCollectableCreation(CollectablePointsPool _pool)
        {
            _pool.Parent = new GameObject(_pool.Prefab.name + "s").transform;
            _pool.Parent.parent = m_PoolsParent;
            _pool.Objects.Clear();

            for (int i = 0; i < _pool.InitialNumber; i++)
            {
                var newObject = Instantiate(_pool.Prefab, _pool.Parent).GetComponent<Collectable>();
                _pool.Objects.Add(newObject);
                newObject.gameObject.SetActive(false);
            }
        }

        private void createPools()
        {
            PoolingBullets.ForEach(p => InitialBulletCreation(p));
            PoolingParticles.ForEach(p => InitialParticlesCreation(p));
            PoolingCollectables.ForEach(p => InitialCollectableCreation(p));
        }

        public void CreateAdditionalBulletPools(BulletPoolingObject _pool)
        {
            PoolingBullets.Add(_pool);
            InitialBulletCreation(_pool);
        }

        public void CreateAdditionalParticlePools(ParticlesPooling _pool)
        {
            PoolingParticles.Add(_pool);
            InitialParticlesCreation(_pool);
        }
        #endregion

        #region Bullets Usage
        public void UseBullet(PoolObjectTag _tag, Vector3 _position, Quaternion _rotation, float _bulletSpeed, BulletUser _user, float _damage, int _userIndex)
        {
            var selectedBullet = GetBullet(_tag, _user);
            selectedBullet.transform.SetPositionAndRotation(_position, _rotation);
            selectedBullet.SetUp(_bulletSpeed, Helper.ReverseUser(_user), _damage, _userIndex);
        }

        public Bullet GetBullet(PoolObjectTag _tag, BulletUser _user)
        {
            // cache selected object
            BulletPoolingObject selectedObject = PoolingBullets.Find(o => o.Name == _tag && o.User == _user);

            Bullet selectedBullet = selectedObject.Objects.Find(o => !o.BulletSprite.enabled);

            // check availability, if there is no sprite then we add a new one
            if (selectedBullet == null)
            {
                var newBullet = Instantiate(selectedObject.Prefab, selectedObject.Parent).GetComponent<Bullet>();
                newBullet.SetUpColor(selectedObject.BulletColor);
                selectedObject.Objects.Add(newBullet);
                selectedBullet = newBullet;
                selectedBullet.ImpactParticleTag = selectedObject.ImpactPoolTag;
            }

            selectedBullet.BulletSprite.enabled = true;
            return selectedBullet;
        }
        #endregion

        #region Particles Usage
        public void UseParticles(ParticlePoolTag _tag, Vector3 _position, Quaternion _rotation)
        {
            ParticlesPooling selectedObject = PoolingParticles.Find(p => p.Tag == _tag);

            if (selectedObject == null)
            {
                print($"No particles of type {_tag}");
                Debug.Break();
            }

            ParticleSystem selectedParticle = selectedObject.Objects.Find(p => !p.isPlaying);

            // if there is no available particle then we create a new one and add it to the list, otherwise, we clear the parent of the selected particle in case it was parented to something else before. This sounds more efficient than clear parents after parenting (I could be very wrong about this but I rock)
            if (selectedParticle == null)
            {
                selectedParticle = CreateNewParticle(selectedObject);
            }
            else
            {
                selectedParticle.transform.parent = selectedObject.Parent;
            }

            selectedParticle.transform.SetPositionAndRotation(_position, _rotation);
            selectedParticle.Play();
        }

        public void UseParticles(ParticlesPooling _particle, Vector3 _position)
        {
            ParticleSystem selectedParticle = _particle.Objects.Find(p => !p.isPlaying);

            // if there is no available particle then we create a new one and add it to the list, otherwise, we clear the parent of the selected particle in case it was parented to something else before. This sounds more efficient than clear parents after parenting (I could be very wrong about this but I rock)
            if (selectedParticle == null)
            {
                selectedParticle = CreateNewParticle(_particle);
            }
            else
            {
                selectedParticle.transform.parent = _particle.Parent;
            }

            selectedParticle.transform.position = _position;
            selectedParticle.Play();
        }

        public void UseParticles(ParticlesPooling _particle, Vector3 _position, Quaternion _rotation)
        {
            ParticleSystem selectedParticle = _particle.Objects.Find(p => !p.isPlaying);

            // if there is no available particle then we create a new one and add it to the list, otherwise, we clear the parent of the selected particle in case it was parented to something else before. This sounds more efficient than clear parents after parenting (I could be very wrong about this but I rock)
            if (selectedParticle == null)
            {
                selectedParticle = CreateNewParticle(_particle);
            }
            else
            {
                selectedParticle.transform.parent = _particle.Parent;
            }

            selectedParticle.transform.SetPositionAndRotation(_position, _rotation);
            selectedParticle.Play();
        }

        public void UseParticles(ParticlesPooling _particle, Vector3 _position, float _particleSize)
        {
            // caching the type of object and the specific particle
            ParticleSystem selectedParticle = _particle.Objects.Find(p => !p.isPlaying);

            // if there is no available particle then we create a new one and add it to the list, otherwise, we clear the parent of the selected particle in case it was parented to something else before. This sounds more efficient than clear parents after parenting (I could be very wrong about this but I rock)
            if (selectedParticle == null)
            {
                selectedParticle = CreateNewParticle(_particle);
            }
            else
            {
                selectedParticle.transform.parent = _particle.Parent;
            }

            selectedParticle.transform.localScale = Vector2.one * _particleSize;
            selectedParticle.transform.position = _position;
            selectedParticle.Play();
        }

        public void UseParticles(ParticlePoolTag _tag, Vector3 _position, Quaternion _rotation, float _particleSize)
        {
            // caching the type of object and the specific particle
            ParticlesPooling selectedObject = PoolingParticles.Find(p => p.Tag == _tag);
            ParticleSystem selectedParticle = selectedObject.Objects.Find(p => !p.isPlaying);

            // if there is no available particle then we create a new one and add it to the list, otherwise, we clear the parent of the selected particle in case it was parented to something else before. This sounds more efficient than clear parents after parenting (I could be very wrong about this but I rock)
            if (selectedParticle == null)
            {
                selectedParticle = CreateNewParticle(selectedObject);
            }
            else
            {
                selectedParticle.transform.parent = selectedObject.Parent;
            }

            selectedParticle.transform.localScale = Vector2.one * _particleSize;
            selectedParticle.transform.SetPositionAndRotation(_position, _rotation);
            selectedParticle.Play();
        }

        public void UseParticles(ParticlePoolTag _tag, Vector3 _position, Quaternion _rotation, float _particleSize, float _duration)
        {
            // caching the type of object and the specific particle
            ParticlesPooling selectedObject = PoolingParticles.Find(p => p.Tag == _tag);
            ParticleSystem selectedParticle = selectedObject.Objects.Find(p => !p.isPlaying);

            // if there is no available particle then we create a new one and add it to the list, otherwise, we clear the parent of the selected particle in case it was parented to something else before. This sounds more efficient than clear parents after parenting (I could be very wrong about this but I rock)
            if (selectedParticle == null)
            {
                selectedParticle = CreateNewParticle(selectedObject);
            }
            else
            {
                selectedParticle.transform.parent = selectedObject.Parent;
            }

            var main = selectedParticle.main;
            main.duration = _duration;
            main.startLifetime = _duration;
            selectedParticle.transform.localScale = Vector2.one * _particleSize;
            selectedParticle.transform.SetPositionAndRotation(_position, _rotation);
            selectedParticle.Play();
        }

        private ParticleSystem CreateNewParticle(ParticlesPooling _selectedObject)
        {
            var newObject = Instantiate(_selectedObject.Prefab, _selectedObject.Parent).GetComponent<ParticleSystem>();
            _selectedObject.Objects.Add(newObject);

            if (_selectedObject.UseColor)
            {
                var main = newObject.main;
                main.startColor = _selectedObject.ParticleColor;
            }

            return newObject;
        }
        #endregion

        #region Shooter Points Usage
        public Collectable UseCollectable(CollectableTag _tag)
        {
            CollectablePointsPool selectedCollection = PoolingCollectables.Find(c => c.Tag == _tag);
            Collectable selectedHolder = selectedCollection.Objects.Find(o => !o.gameObject.activeSelf);

            if (selectedHolder == null)
            {
                Collectable newObject = Instantiate(selectedCollection.Prefab, selectedCollection.Parent);
                selectedCollection.Objects.Add(newObject);
                selectedHolder = newObject;
            }

            return selectedHolder;
        }
        #endregion

        public Bullet[] GetBullets(PoolObjectTag _tag, BulletUser _user)
        {
            return PoolingBullets.Find(o => o.Name == _tag && o.User == _user).Objects.ToArray();
        }
    }

    public enum PoolObjectTag
    {
        NormalShot, Muzzle, ImpactParticles, DeathExplosion, DroneShot, RectangularShot, CrystalShot, RoundShot, YellowRocket, RoundPenetraiting,
        CrazyRocketerBigMissiles, GreenMatterExplodingBall, DroneRedShot
    }

    public enum ParticlePoolTag
    {
        None, StandardMuzzle, StandardExplosion, StandardImpact, AlienShipMuzzle, GreenMatterMuzzle, AlienShipImpact, GreenMatterImpact,
        CrazyRocketorExplosion, CrazyRocketorMuzzle, GreenMatterExplosion, ShootingPointsCollection, HealthPointsCollection,
        DroneSpawnParticle, RedDroneSpawnParticle, RedShieldDestroy, DronesFighterDissolve, RedMuzzle, AstroidImpact, AstroidExplosion,
        AlienShipExplosion, SmallAstroidExplosion, MediumAstroidExplosion, CoinCollection
    }
}