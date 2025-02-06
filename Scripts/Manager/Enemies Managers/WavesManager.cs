using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SpaceWar.Shop;

namespace SpaceWar
{
    public class WavesManager : Singlton<WavesManager>
    {
        [SerializeField] private float m_InitialDelay = 1f;
        [SerializeField] private int m_EnemiesToDestroy = 0;
        [HideInInspector] public List<Transform> ActiveShips = new List<Transform>();
        private const float BEFORE_TITLE_DELAY = 1f;
        private const float BEFORE_SHOP_DELAY = 2f;
        private float m_Difficulty = 0.5f;


        /// <summary>
        /// Main Waves methods
        /// </summary>
        /// <param name="_waves"></param>
        public void StartWaves(List<EnemyWave> _waves, float _difficulty = 0.5f)
        {
            m_Difficulty = _difficulty;
            StartCoroutine(WavesProcess(_waves));
        }

        private IEnumerator WavesProcess(List<EnemyWave> _waves)
        {
            for (int i = 0; i < _waves.Count; i++)
            {
                // show the screen
                yield return new WaitForSeconds(BEFORE_TITLE_DELAY);
                ScreenTitlesManager.Instance.ShowNextWave(i);
                yield return new WaitForSeconds(ScreenTitlesManager.Instance.GetAnimationLength(ScreenTitlesManager.AnimationTag.Wave));

                for (int j = 0; j < _waves[i].EnemyPushes.Count; j++)
                {
                    // start the push
                    SpawnPush(_waves[i].EnemyPushes[j]);
                    // wait until all the enemies of the push are destroyed before spawning the next one or move to the next wave
                    while (m_EnemiesToDestroy > 0) yield return null;

                }

                // if this wave has a shop to display and the wave isn't the last one then spawn the shop
                if (_waves[i].ShowShop && i != _waves.Count - 1 && i != 0)
                {
                    yield return new WaitForSeconds(BEFORE_SHOP_DELAY);
                    // spawn the shop here
                    GameManager.i.ShopManager.SpawnShop();
                    // while the shop is still active
                    while (GameManager.i.ShopManager.ShopIsPresent()) yield return null;
                    yield return new WaitForSeconds(BEFORE_SHOP_DELAY);
                }
            }

            GameManager.i.LevelCompleted();
        }

        private void SpawnPush(EnemyPush _push)
        {
            m_EnemiesToDestroy = _push.Enemies.Count;
            ActiveShips.Clear();
            // set up the movement of the ships in this push
            var enemies = EnemySpawner.i.GetShipsFromPush(_push);
            var delays = EnemySpawner.i.GetPushDelays(_push);
            ActiveShips = EnemyMovementGenerator.i.SetMovementForPush(enemies, delays, m_Difficulty);
        }

        public void EnemyDestroyed(Transform _ship)
        {
            // reduce the number of ships that the player must destroy
            m_EnemiesToDestroy--;
            // remove it from the list
            ActiveShips.Remove(_ship);
        }

        public Vector2 GetClosestEnemyToPoint(Transform _bullet)
        {
            if (ActiveShips.Count > 0)
            {
                // orders the active ships according to how close they are from the bullet
                return ActiveShips.OrderBy(s => Vector2.Distance(_bullet.position, s.position)).FirstOrDefault().position;
            }

            // if there are ships then the bullet goes up
            return _bullet.up * 100f;
        }

        public void AddToActiveShips(Transform _ship)
        {
            // increase the number of enemies to destroy
            m_EnemiesToDestroy++;
            // add it to the list
            ActiveShips.Add(_ship);
        }
    }

    #region Structures

    [System.Serializable]
    public struct SingleEnemyPush
    {
        public ShipRank Rank;
        [Tooltip("If no type2 exists then it's automatically set to type1.")]
        public int TypeNumber;
        public float InitialDelay;
    }

    [System.Serializable]
    public struct EnemyPush
    {
        public int Value;
        public List<SingleEnemyPush> Enemies;
    }

    [System.Serializable]
    public struct EnemyWave
    {
        public int Value;
        public List<EnemyPush> EnemyPushes;
        public bool ShowShop;
    }
    #endregion
}