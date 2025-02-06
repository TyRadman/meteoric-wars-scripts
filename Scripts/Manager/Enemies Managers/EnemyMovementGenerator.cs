using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    // responsible for giving enemies the ability to move and orders their positions upon enterance
    public class EnemyMovementGenerator : Singlton<EnemyMovementGenerator>
    {
        public enum MovementStyle
        {
            NormalParade = 0, NormalParadeWithDelays = 1, RandomParade = 2, RandomParadeWithDelays = 3, ContinuousMovement = 4,
            RandomMovement = 5
        }

        [SerializeField] private Vector2 m_LevelHorLimits;
        [SerializeField] private Vector2 m_LevelVerLimits;
        private const float STARTING_Y_POSITION = 10f;
        private const float DIFFICULTY_TWIST_THRESHOLD = 0.3f;
        [SerializeField] private float m_Difficulty = 0.5f;

        public List<Transform> SetMovementForPush(List<EnemyComponents> _enemies, List<float> _delays, float _difficulty)
        {
            m_Difficulty = _difficulty;
            List<Transform> shipsTransform = new List<Transform>();

            for (int i = 0; i < 5; i++)
            {
                var shipsOfRank = _enemies.FindAll(s => s.Rank == (ShipRank)i);

                if (shipsOfRank != null)
                {
                    SetGroupMovement(shipsOfRank, _delays, (ShipRank)i);
                }
            }

            _enemies.ForEach(s => shipsTransform.Add(s.transform));
            return shipsTransform;
        }

        #region For Summoned Ships
        public void SetUpMovementForSummonedShips(List<EnemyComponents> _ships)
        {
            var style = Random.value > 0.5f ? HorizontalMovement.HorizontalMovementStyle.Parade : HorizontalMovement.HorizontalMovementStyle.ContinuousMovement;
            NormalHorizontalMovement(_ships, style, ShipRank.Small, false);
        }
        #endregion

        private void SetGroupMovement(List<EnemyComponents> _ships, List<float> _delays, ShipRank _rank)
        {
            MovementStyle style = (MovementStyle)Random.Range(0, 6);
            //MovementStyle style = MovementStyle.RandomMovement;

            switch (style)
            {
                case MovementStyle.NormalParade:
                    {
                        NormalHorizontalMovement(_ships, HorizontalMovement.HorizontalMovementStyle.Parade, _rank);
                        break;
                    }
                case MovementStyle.NormalParadeWithDelays:
                    {
                        NormalParadeWithDelays(_ships, _delays, HorizontalMovement.HorizontalMovementStyle.Parade, _rank);
                        break;
                    }
                case MovementStyle.RandomParade:
                    {
                        RandomParade(_ships, HorizontalMovement.HorizontalMovementStyle.Parade, _rank);
                        break;
                    }
                case MovementStyle.RandomParadeWithDelays:
                    {
                        RandomParadeWithDelays(_ships, _delays, HorizontalMovement.HorizontalMovementStyle.Parade, _rank);
                        break;
                    }
                case MovementStyle.ContinuousMovement:
                    {
                        NormalHorizontalMovement(_ships, HorizontalMovement.HorizontalMovementStyle.ContinuousMovement, _rank);
                        break;
                    }
                case MovementStyle.RandomMovement:
                    {
                        // to create values for this ship
                        AddRandomMovement(_ships, _rank, true);
                        break;
                    }
            }

            StartCoroutine(AddSpecialValues(_ships, _rank));
        }

        private IEnumerator AddSpecialValues(List<EnemyComponents> _ships, ShipRank _rank)
        {
            yield return new WaitForSeconds(1f);

            if (_rank == ShipRank.Strong || _rank == ShipRank.Summoner)
            {
                for (int i = 0; i < _ships.Count; i++)
                {
                    EnemyShipAbilities abilities = _ships[i].GetComponent<EnemyShipAbilities>();
                    EnemyAbilitiesManager.i.AddAbilities(abilities, _rank);
                    abilities.ActivateShip();
                }
            }
        }

        #region Horizontal Methods
        private HorizontalMovementValues CreateHorizontalMovementValues(float _difficulty, ShipRank _rank)
        {
            var value = GameManager.i.GeneralValues.RankValues.Find(r => r.Rank == _rank);
            // the total value of all properties for the horizontal movement is the number of properties that depend on the difficulty value multiplied by the difficulty value
            float[] values = new float[3];
            float threshold = _difficulty * DIFFICULTY_TWIST_THRESHOLD;

            for (int i = 0; i < values.Length; i++)
            {
                float toAdd = _difficulty + Random.Range(-threshold, threshold);
                values[i] = toAdd;
            }

            float speed = Mathf.Lerp(value.HorizontalMovementInfo.SpeedRange.x, value.HorizontalMovementInfo.SpeedRange.y, values[0]);
            float timeBetweenSteps = Mathf.Lerp(value.HorizontalMovementInfo.WaitingTime.x, value.HorizontalMovementInfo.WaitingTime.y, 1 - values[2]);
            int stepsOverScreen = (int)Mathf.Lerp(value.HorizontalMovementInfo.StepsRange.x, value.HorizontalMovementInfo.StepsRange.y, values[1]);
            return new HorizontalMovementValues { Speed = speed, TimeBetweenSteps = timeBetweenSteps, StepsOverScreen = stepsOverScreen };
        }

        private void NormalHorizontalMovement(List<EnemyComponents> _enemies, HorizontalMovement.HorizontalMovementStyle _style, ShipRank _rank, bool _spawnEnemy = true)
        {
            Vector2 levelLimits = GetCoverageArea(Random.Range(0.5f, 0.9f));
            HorizontalMovementValues HorizontalMovementValuesHolder = CreateHorizontalMovementValues(m_Difficulty, _rank);

            for (int i = 0; i < _enemies.Count; i++)
            {
                float t = GetTValue(i, _enemies.Count);
                var shipComponent = _enemies[i];
                shipComponent.gameObject.SetActive(true);

                // spawning the ship
                Vector2 endingPosition = new Vector2(Mathf.Lerp(m_LevelHorLimits.x, m_LevelHorLimits.y, t)
                    , Mathf.Lerp(levelLimits.x, levelLimits.y, t));
                Vector2 startingPosition = new Vector2(Mathf.Lerp(m_LevelHorLimits.x, m_LevelHorLimits.y, t),
                    STARTING_Y_POSITION);

                shipComponent.Movement = shipComponent.gameObject.AddComponent<HorizontalMovement>();
                // we need to cache the movement as a horizontal movement script to access the setStyle method since it's not a member of the base class
                shipComponent.GetComponent<HorizontalMovement>().SetHorizontalMovementProperties(HorizontalMovementValuesHolder);
                shipComponent.GetComponent<HorizontalMovement>().SetStyle(_style);
                shipComponent.Movement.SetUpValues(shipComponent.transform, shipComponent.Rank);
                EnemySpawner.i.ShipRankAddtions(shipComponent.Rank, shipComponent);

                if (_spawnEnemy)
                {
                    shipComponent.Movement.EntranceMovement(startingPosition, endingPosition);
                }
            }
        }

        private void NormalParadeWithDelays(List<EnemyComponents> _enemies, List<float> _delays, HorizontalMovement.HorizontalMovementStyle _style, ShipRank _rank)
        {
            StartCoroutine(NormalParadeWithDelaysProcess(_enemies, _delays, _style, _rank));
        }

        private IEnumerator NormalParadeWithDelaysProcess(List<EnemyComponents> _enemies, List<float> _delays, HorizontalMovement.HorizontalMovementStyle _style, ShipRank _rank)
        {
            Vector2 levelLimits = GetCoverageArea(Random.Range(0.5f, 0.9f));
            HorizontalMovementValues HorizontalMovementValuesHolder = CreateHorizontalMovementValues(m_Difficulty, _rank);

            for (int i = 0; i < _enemies.Count; i++)
            {
                yield return new WaitForSeconds(_delays[i]);

                float t = GetTValue(i, _enemies.Count);
                var shipComponent = _enemies[i];
                shipComponent.gameObject.SetActive(true);
                Vector2 endingPosition = new Vector2(Mathf.Lerp(m_LevelHorLimits.x, m_LevelHorLimits.y, t), Mathf.Lerp(levelLimits.x, levelLimits.y, t));
                Vector2 startingPosition = new Vector2(endingPosition.x, STARTING_Y_POSITION);

                shipComponent.Movement = shipComponent.gameObject.AddComponent<HorizontalMovement>();
                shipComponent.GetComponent<HorizontalMovement>().SetHorizontalMovementProperties(HorizontalMovementValuesHolder);
                shipComponent.GetComponent<HorizontalMovement>().SetStyle(_style);
                shipComponent.Movement.SetUpValues(shipComponent.transform, shipComponent.Rank);
                EnemySpawner.i.ShipRankAddtions(shipComponent.Rank, shipComponent);
                shipComponent.Movement.EntranceMovement(startingPosition, endingPosition);
            }
        }

        private void RandomParade(List<EnemyComponents> _enemies, HorizontalMovement.HorizontalMovementStyle _style, ShipRank _rank)
        {
            HorizontalMovementValues HorizontalMovementValuesHolder = CreateHorizontalMovementValues(m_Difficulty, _rank);

            for (int i = 0; i < _enemies.Count; i++)
            {
                var shipComponent = _enemies[i];
                shipComponent.gameObject.SetActive(true);
                Vector2 endingPosition = new Vector2(m_LevelHorLimits.RandomValue(), m_LevelVerLimits.RandomValue());
                Vector2 startingPosition = new Vector2(endingPosition.x, STARTING_Y_POSITION);

                shipComponent.Movement = shipComponent.gameObject.AddComponent<HorizontalMovement>();
                shipComponent.GetComponent<HorizontalMovement>().SetHorizontalMovementProperties(HorizontalMovementValuesHolder);
                shipComponent.GetComponent<HorizontalMovement>().SetStyle(_style);
                shipComponent.Movement.SetUpValues(shipComponent.transform, shipComponent.Rank);
                EnemySpawner.i.ShipRankAddtions(shipComponent.Rank, shipComponent);
                shipComponent.Movement.EntranceMovement(startingPosition, endingPosition);
            }
        }

        private void RandomParadeWithDelays(List<EnemyComponents> _enemies, List<float> _delays, HorizontalMovement.HorizontalMovementStyle _style, ShipRank _rank)
        {
            StartCoroutine(RandomParadeWithDelaysProcess(_enemies, _delays, _style, _rank));
        }

        private IEnumerator RandomParadeWithDelaysProcess(List<EnemyComponents> _enemies, List<float> _delays, HorizontalMovement.HorizontalMovementStyle _style, ShipRank _rank)
        {
            HorizontalMovementValues HorizontalMovementValuesHolder = CreateHorizontalMovementValues(m_Difficulty, _rank);

            for (int i = 0; i < _enemies.Count; i++)
            {
                yield return new WaitForSeconds(_delays[i]);
                EnemyComponents shipComponent = _enemies[i];
                shipComponent.gameObject.SetActive(true);
                Vector2 endingPosition = new Vector2(m_LevelHorLimits.RandomValue(), m_LevelVerLimits.RandomValue());
                Vector2 startingPosition = new Vector2(endingPosition.x, STARTING_Y_POSITION);

                shipComponent.Movement = shipComponent.gameObject.AddComponent<HorizontalMovement>();
                shipComponent.GetComponent<HorizontalMovement>().SetHorizontalMovementProperties(HorizontalMovementValuesHolder);
                shipComponent.GetComponent<HorizontalMovement>().SetStyle(_style);
                shipComponent.Movement.SetUpValues(shipComponent.transform, shipComponent.Rank);
                EnemySpawner.i.ShipRankAddtions(shipComponent.Rank, shipComponent);
                shipComponent.Movement.EntranceMovement(startingPosition, endingPosition);
            }
        }
        #endregion

        #region Random Movement
        private RandomMovementValues CreateRandomMovementValues(float _difficulty, ShipRank _rank)
        {
            var value = GameManager.i.GeneralValues.RankValues.Find(r => r.Rank == _rank);
            // the total value of all properties for the horizontal movement is the number of properties that depend on the difficulty value multiplied by the difficulty value
            float[] values = new float[3];
            // to add variations
            float threshold = _difficulty * DIFFICULTY_TWIST_THRESHOLD;

            for (int i = 0; i < values.Length; i++)
            {
                float toAdd = _difficulty + Random.Range(-threshold, threshold);
                values[i] = toAdd;
            }

            float speed = Mathf.Lerp(value.RandomMovementInfo.SpeedRange.x, value.RandomMovementInfo.SpeedRange.y, values[0]);
            float timeBetweenSteps = Mathf.Lerp(value.RandomMovementInfo.WaitingTime.x, value.RandomMovementInfo.WaitingTime.y, 1 - values[2]);
            float chasingPlayerChance = Mathf.Lerp(value.RandomMovementInfo.ChasingPlayerChance.x, value.RandomMovementInfo.ChasingPlayerChance.y, values[1]);
            return new RandomMovementValues { Speed = speed, TimeBetweenSteps = timeBetweenSteps, ChasingPlayerChances = chasingPlayerChance };
        }

        private void AddRandomMovement(List<EnemyComponents> _enemies, ShipRank _rank, bool _spawnEnemy = true)
        {
            Vector2 levelLimits = GetCoverageArea(Random.Range(0.5f, 0.9f));
            RandomMovementValues randomMovementValues = CreateRandomMovementValues(m_Difficulty, _rank);

            for (int i = 0; i < _enemies.Count; i++)
            {
                float t = GetTValue(i, _enemies.Count);
                var shipComponent = _enemies[i];
                shipComponent.gameObject.SetActive(true);

                // spawning the ship
                Vector2 endingPosition = new Vector2(Mathf.Lerp(m_LevelHorLimits.x, m_LevelHorLimits.y, t)
                    , Mathf.Lerp(levelLimits.x, levelLimits.y, t));
                Vector2 startingPosition = new Vector2(Mathf.Lerp(m_LevelHorLimits.x, m_LevelHorLimits.y, t),
                    STARTING_Y_POSITION);

                shipComponent.Movement = shipComponent.gameObject.AddComponent<RandomStepsMovement>();
                // we need to cache the movement as a horizontal movement script to access the setStyle method since it's not a member of the base class
                shipComponent.GetComponent<RandomStepsMovement>().SetProperties(randomMovementValues);
                shipComponent.Movement.SetUpValues(shipComponent.transform, shipComponent.Rank);
                EnemySpawner.i.ShipRankAddtions(shipComponent.Rank, shipComponent);

                if (_spawnEnemy)
                {
                    shipComponent.Movement.EntranceMovement(startingPosition, endingPosition);
                }
            }
        }
        #endregion

        #region Helping Methods
        private Vector2 GetCoverageArea(float _coveragePerc)
        {
            Vector2 area = new Vector2();
            float levelLength = m_LevelVerLimits.y - m_LevelVerLimits.x;
            float deploymentArea = levelLength * _coveragePerc;
            area.x = m_LevelVerLimits.x + levelLength / 2 - deploymentArea / 2;
            area.y = area.x + deploymentArea;
            return area;
        }

        private float GetTValue(float _index, float _count)
        {
            if (_count > 2f)
            {
                return _index / (_count - 1f);
            }
            else
            {
                if (_count == 1f)
                {
                    return 0.5f;
                }
                else
                {
                    if (_index == 0)
                    {
                        return 0.25f;
                    }
                    else
                    {
                        return 0.75f;
                    }
                }
            }
        }

        //private void SetShipMovementStyle(EnemyComponents _ship)
        //{
        //    _ship.GetComponent<HorizontalMovement>().SetHorizontalMovementProperties(m_HorizontalMovementValuesHolder);
        //    // _ship.GetComponent<HorizontalMovement>().SetStyle(_style);
        //}
        #endregion
    }
    public struct HorizontalMovementValues
    {
        public float Speed;
        public int StepsOverScreen;
        public float TimeBetweenSteps;
    }

    public struct RandomMovementValues
    {
        public float Speed;
        public float TimeBetweenSteps;
        public float ChasingPlayerChances;
    }
}