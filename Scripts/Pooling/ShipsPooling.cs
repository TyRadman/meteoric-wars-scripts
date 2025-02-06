using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ShipsPooling : Singlton<ShipsPooling>
    {
        [System.Serializable]
        public struct ShipPooll
        {
            public SpawnShipAbility Parent;
            public List<EnemyComponents> Ships;
            public int ID;
        }

        public List<ShipPooll> ShipPools_New = new List<ShipPooll>();
        private int IDCounter = 0;

        public List<EnemyComponents> CreateShips(int _shipsNum, GameObject _shipPrefab, SpawnShipAbility _summoner, ShipRank _rank)
        {
            IDCounter++;
            _summoner.ShipsID = IDCounter;
            Transform parent = new GameObject("Ships Number " + IDCounter).transform;
            List<EnemyComponents> ships = new List<EnemyComponents>();

            for (int i = 0; i < _shipsNum; i++)
            {
                string name = _shipPrefab.name;
                // create the ship
                GameObject ship = Instantiate(_shipPrefab, parent);
                ship.name = name;
                // set the ID of the summoner to the summonee
                ship.AddComponent<SummonedShip>().ID = IDCounter;

                EnemyComponents components = ship.GetComponent<EnemyComponents>();

                // add the summonee to the summonee list
                ships.Add(components);
                // add the weapon and set it up
                ship.GetComponent<Shooter>().SetUpWeapon(WeaponsGenerator.i.Weapons.FindAll(w => w.Rank == _rank).RandomItem().Weapon);
                // disables spawning collectables
                components.SpawnCollectables = false;
                // set up health
                //ship.GetComponent<EnemyHealth>().SetUpStats();
            }

            ShipPools_New.Add(new ShipPooll { ID = IDCounter, Ships = ships, Parent = _summoner });
            return ships;
        }

        public void SpawnShip(int _id, Transform _spawnPoint, float _distanceFromSpawningPoint)
        {
            var ships = ShipPools_New.Find(p => p.ID == _id).Ships.FindAll(s => !s.gameObject.activeSelf);

            // select the first ship that is not active and has the specified id
            EnemyComponents ship = ships[0];
            ship.gameObject.SetActive(true);
            ship.transform.localScale = Vector2.one * 1f;

            // if the ship was summoned by an enemy ship then it's required to destroy it before moving to the next wave
            if (ship.ShipStats.GetUserTag() == BulletUser.Enemy)
            {
                WavesManager.i.AddToActiveShips(ship.transform);
            }
            else
            {
                GameManager.i.PlayersManager.PlayerAllies.Add(ship.transform);
            }

            ship.ShipStats.IsInWave = false;

            if (ship == null)
            {
                Debug.LogError($"The ship with the ID {_id} doesn't exist (Ship Pooling)");
                Debug.Break();
                return;
            }

            // if the summoner is the player ship (with the rotation 0 on the Z axis) then we send the summoned ship up. Otherwise, we send it down
            _distanceFromSpawningPoint *= ship.ShipStats.GetUserTag() == BulletUser.Player ? 1 : -1;
            // rotate the ship to face the enemies with its summoner
            ship.transform.eulerAngles = _spawnPoint.eulerAngles;
            // get the end point position
            Vector2 endPoint = new Vector2(_spawnPoint.position.x, _spawnPoint.position.y + _distanceFromSpawningPoint);
            ship.GetComponent<EnemyShipMovement>().EntranceMovement(_spawnPoint.position, endPoint);
            ship.ResetValues();
        }

        public void ShipDestroyed(int _id, BulletUser _userTag, Transform _ship)
        {
            ShipPools_New.Find(s => s.ID == _id).Parent.ShipDestroyed();

            // if the ship is with the player, then remove it from the allies list
            if (_userTag == BulletUser.Player)
            {
                GameManager.i.PlayersManager.PlayerAllies.Remove(_ship);
            }
        }
    }
}