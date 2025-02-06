using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class EnemiesDisplayer : MonoBehaviour
    {
        [SerializeField] private Vector2 m_WaitBetweenShipsTimeRange;
        [SerializeField] private Vector2 m_ColorPaletteDurationRange;
        [SerializeField] private Vector2 m_SpeedRange;
        [SerializeField] private float m_SpeedMultiplier = 0.2f;
        private const float m_DistanceBetweenShips = 5f;
        private Transform m_Parent;
        private Coroutine m_SpawnCoroutine;
        private Coroutine m_ColorCoroutine;

        private void Start()
        {
            m_Parent = new GameObject("Ships parent").transform;
            m_SpawnCoroutine = StartCoroutine(SpawningProcess());
            m_ColorCoroutine = StartCoroutine(ChangingColorPaletteProcess());
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                StopCoroutine(m_SpawnCoroutine);
                StopCoroutine(m_ColorCoroutine);
            }
        }

        private IEnumerator SpawningProcess()
        {
            float previousX = 0f;

            while (true)
            {
                int rank = Random.Range(0, 5);
                GameObject ship = ShipBuilder.i.BuildShip((ShipRank)rank);
                ship.SetActive(true);
                ship.transform.parent = m_Parent;

                previousX = GetNewXPosition(previousX);
                ship.transform.position = new Vector2(previousX, LevelDimensions.Instance.HalfHeight);
                StartCoroutine(MoveShip(ship.transform, (rank) * m_SpeedMultiplier));
                yield return new WaitForSeconds(m_WaitBetweenShipsTimeRange.RandomValue());
            }
        }

        private float GetNewXPosition(float _previousX)
        {
            Vector2 range = Vector2.zero;

            // if there is no space on the right side
            if (_previousX + m_DistanceBetweenShips >= LevelDimensions.Instance.HalfWidth)
            {
                range = new Vector2(-LevelDimensions.Instance.HalfWidth, _previousX - m_DistanceBetweenShips);
                return range.RandomValue();
            }
            else if (_previousX - m_DistanceBetweenShips <= -LevelDimensions.Instance.HalfWidth)
            {
                range = new Vector2(_previousX + m_DistanceBetweenShips, LevelDimensions.Instance.HalfWidth);
                return range.RandomValue();
            }
            else
            {
                if (Random.value > 0.5f)
                {
                    range = new Vector2(-LevelDimensions.Instance.HalfWidth, _previousX - m_DistanceBetweenShips);
                    return range.RandomValue();
                }
                else
                {
                    range = new Vector2(_previousX + m_DistanceBetweenShips, LevelDimensions.Instance.HalfWidth);
                    return range.RandomValue();
                }
            }
        }

        private IEnumerator ChangingColorPaletteProcess()
        {
            while (true)
            {
                ShipBuilder.i.SetNewColorPalette();
                yield return new WaitForSeconds(m_ColorPaletteDurationRange.RandomValue());
            }
        }

        private IEnumerator MoveShip(Transform _ship, float _speedMultiplier)
        {
            float time = 0f;
            float duration = _speedMultiplier * m_SpeedRange.RandomValue();
            Vector2 startPoint = _ship.transform.position;
            Vector2 endpoint = new Vector2(startPoint.x, -startPoint.y);

            while (time < duration)
            {
                time += Time.deltaTime;
                _ship.position = Vector2.Lerp(startPoint, endpoint, time / duration);
                yield return null;
            }

            Destroy(_ship.gameObject);
            // _ship.gameObject.SetActive(false);
        }

        private IEnumerator MoveShip(Transform _ship)
        {
            float time = 0f;
            float duration = m_SpeedRange.RandomValue();
            Vector2 startPoint = _ship.transform.position;
            Vector2 endpoint = new Vector2(startPoint.x, -startPoint.y);

            while (time < duration)
            {
                time += Time.deltaTime;
                _ship.position = Vector2.Lerp(startPoint, endpoint, time / duration);
                yield return null;
            }

            Destroy(_ship.gameObject);
            // _ship.gameObject.SetActive(false);
        }
    }
}