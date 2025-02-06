using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class AstroidSpawner : MonoBehaviour
    {
        #region Internal classes
        [System.Serializable]
        public class AstroidSpawnees
        {
            public AstroidSpawnees(AstroidSize _size)
            {
                Size = _size;
            }

            public List<Astroid> Astroids = new List<Astroid>();
            public AstroidSize Size;
        }

        [System.Serializable]
        public class FarAstroidSpawnees
        {
            public FarAstroidSpawnees(AstroidSize _size)
            {
                Size = _size;
            }

            public List<BGAstroid> Astroids = new List<BGAstroid>();
            public AstroidSize Size;
        }

        [System.Serializable]
        public struct AstroidSpawningVars
        {
            public AstroidSize Size;
            [Tooltip("Number of sprites that will pile on the astroid")]
            public int LayersNumber;
            public Vector2 SpeedRange;
            public Vector2 HealthRange;
            public Vector2 DamageRange;
            public ParticlesPooling ExplosionParticles;
            public float Score;
            public ScreenFreezer.FreezeStrength FreezeStrength;
            public Vector2Int CoinsCountRange;
        }

        [System.Serializable]
        public class BackGroundAstroidInfo
        {
            public float Size;
            public int LayerNumber;
            public float Chance;
            public float SpeedMultiplier;
        }
        #endregion

        [SerializeField] private float m_BetweenSpawnsBreak = 0.1f;
        [SerializeField] private float m_YStartPosition = 10f;
        private List<AstroidSpawnees> m_BuiltAstroids = new List<AstroidSpawnees>();
        private List<FarAstroidSpawnees> m_BuiltFarAstroids = new List<FarAstroidSpawnees>();
        [SerializeField] private int m_MinimumAstroids = 10;
        [field: SerializeField, Header("Waves Info")] public bool SpawnAgressiveAstroids { get; private set; } = true;
        [SerializeField] private AgressiveAstroidWaveInfo m_WaveInfo;
        [SerializeField] private bool m_SpawnFarAstroids = true;
        [SerializeField] private FarAstroidWaveInfo m_FarWaveInfo;
        [Header("Standard values")]
        [SerializeField] private List<AstroidSpawningVars> m_AstroidsSpawningInfo;
        [SerializeField] private List<BackGroundAstroidInfo> m_BackgroundLayers;
        private Coroutine m_SpawningCoroutine;
        public bool IsSpawning = true;
        private Transform m_AstroidsParent;

        private WaitForSeconds m_Wait;

        public void SetUp()
        {
            m_Wait = new WaitForSeconds(m_BetweenSpawnsBreak);
            m_AstroidsParent = new GameObject("Astroids").transform;
        }

        private void Start()
        {
            // for pooling
            SpawnAstroids();
            SpawnBGAstroids();

            // start a wave
            if (SpawnAgressiveAstroids) StartCoroutine(ShootingAstroidsProcess(m_WaveInfo));

            // start back ground wave
            if (m_SpawnFarAstroids) StartCoroutine(SpawningFarAstroidsProcess(m_FarWaveInfo));
        }

        #region Spawning Process
        private IEnumerator ShootingAstroidsProcess(AgressiveAstroidWaveInfo _waveInfo)
        {
            while (IsSpawning)
            {
                // select a size based on the chance
                List<AstroidWaveInfo.SizeInfo> chances = _waveInfo.SizeChances.FindAll(a => a.Chance >= Random.value);
                AstroidSize selectedSize = chances.Count == 0 ? _waveInfo.SizeChances.RandomItem().Size : chances.RandomItem().Size;
                Astroid astroid = m_BuiltAstroids.Find(a => a.Size == selectedSize).Astroids.Find(a => !a.IsActive);

                // if there are no inactive astroids with the required size, then create a new one
                if (astroid == null)
                {
                    astroid = CreateAstroid(selectedSize);
                    m_BuiltAstroids.Find(a => a.Size == selectedSize).Astroids.Add(astroid);
                }

                // set the position and rotation
                astroid.transform.position = new Vector2(LevelDimensions.Instance.GetRandomWidth(), m_YStartPosition);
                astroid.transform.eulerAngles = new Vector3(0, 0, (int)Random.Range(0, 3) * (int)90);
                // move the astroid
                astroid.MoveAstroid();
                yield return new WaitForSeconds(_waveInfo.FrequencyRange.RandomValue());
            }
        }

        private IEnumerator SpawningFarAstroidsProcess(FarAstroidWaveInfo _waveInfo)
        {
            while (IsSpawning)
            {
                float chance = Random.value;
                // select a size based on the chance
                List<AstroidWaveInfo.SizeInfo> chances = _waveInfo.SizeChances.FindAll(a => a.Chance >= Random.value);
                AstroidSize selectedSize;

                if (chances.Count == 0) selectedSize = _waveInfo.SizeChances.RandomItem().Size;
                else selectedSize = chances.RandomItem().Size;

                BGAstroid astroid = m_BuiltFarAstroids.Find(a => a.Size == selectedSize).Astroids.Find(a => !a.IsActive);

                // if there are no inactive astroids with the required size, then create a new one
                if (astroid == null)
                {
                    astroid = CreateBGAstroid(selectedSize);
                    m_BuiltFarAstroids.Find(a => a.Size == selectedSize).Astroids.Add(astroid);
                }

                // set the position
                astroid.transform.position = new Vector2(LevelDimensions.Instance.GetRandomWidth(), m_YStartPosition);
                astroid.transform.eulerAngles = new Vector3(0, 0, (int)Random.Range(0, 3) * (int)90);
                astroid.MoveAstroid();

                // set far astroid variables
                BackGroundAstroidInfo bgValues = m_BackgroundLayers.Find(l => Random.value < l.Chance);

                if (bgValues == null) bgValues = m_BackgroundLayers.RandomItem();

                astroid.SetBackGroundData(bgValues.LayerNumber, bgValues.Size, bgValues.SpeedMultiplier);
                yield return new WaitForSeconds(_waveInfo.FrequencyRange.RandomValue());
            }
        }
        #endregion

        #region Setting up process
        public void SpawnAstroids()
        {
            for (int i = 0; i < 3; i++)
            {
                AstroidSize size = (AstroidSize)i;
                m_BuiltAstroids.Add(new AstroidSpawnees(size));
                StartCoroutine(SpawningProcess(m_MinimumAstroids, size));
            }
        }

        private IEnumerator SpawningProcess(int _count, AstroidSize _size)
        {
            List<Astroid> astroids = m_BuiltAstroids.Find(a => a.Size == _size).Astroids;

            for (int i = 0; i < _count; i++)
            {
                astroids.Add(CreateAstroid(_size));
                yield return m_Wait;
            }
        }

        private Astroid CreateAstroid(AstroidSize _size)
        {
            AstroidSpawningVars info = m_AstroidsSpawningInfo.Find(a => a.Size == _size);

            Astroid astroid = AstroidGenerator.i.GenerateBody(info.LayersNumber);
            astroid.transform.parent = m_AstroidsParent;
            astroid.gameObject.SetActive(false);
            astroid.SetUpValues(info.SpeedRange, _size, Mathf.Round(info.HealthRange.RandomValue()),
                Mathf.Round(info.DamageRange.RandomValue()), info.ExplosionParticles, info.Score, info.FreezeStrength, info.CoinsCountRange);

            return astroid;
        }
        #endregion

        #region Spawn Process for BG astroids
        public void SpawnBGAstroids()
        {
            for (int i = 0; i < 3; i++)
            {
                AstroidSize size = (AstroidSize)i;
                m_BuiltFarAstroids.Add(new FarAstroidSpawnees(size));
                StartCoroutine(BGSpawningProcess(m_MinimumAstroids, size));
            }
        }

        private IEnumerator BGSpawningProcess(int _count, AstroidSize _size)
        {
            List<BGAstroid> astroids = m_BuiltFarAstroids.Find(a => a.Size == _size).Astroids;

            for (int i = 0; i < _count; i++)
            {
                astroids.Add(CreateBGAstroid(_size));
                yield return m_Wait;
            }
        }

        private BGAstroid CreateBGAstroid(AstroidSize _size)
        {
            AstroidSpawningVars info = m_AstroidsSpawningInfo.Find(a => a.Size == _size);
            BGAstroid astroid = AstroidGenerator.i.GenerateBGAstroid(info.LayersNumber);
            astroid.transform.parent = m_AstroidsParent;
            astroid.SetUp(info.SpeedRange);
            astroid.gameObject.SetActive(false);
            return astroid;
        }
        #endregion

#if UNITY_EDITOR
        public void EnableSpawnAggressiveAstroids(bool enable)
        {
            SpawnAgressiveAstroids = enable;
        }

        public void SetSpawnFrequencyRange(Vector2 range)
        {
            m_WaveInfo.FrequencyRange = range;
        }

        public Vector2 GetSpawnFrequencyRange()
        {
            return m_WaveInfo.FrequencyRange;
        }
#endif
    }

    public abstract class AstroidWaveInfo
    {
        [System.Serializable]
        public class SizeInfo
        {
            public AstroidSize Size;
            [Range(0, 1)] public float Chance;
        }

        public Vector2 FrequencyRange;
        public List<SizeInfo> SizeChances;
    }

    [System.Serializable]
    public class AgressiveAstroidWaveInfo : AstroidWaveInfo
    {
        [Range(0, 1)] public float PlayerFocusChance = 0.3f;
    }

    [System.Serializable]
    public class FarAstroidWaveInfo : AstroidWaveInfo
    {

    }

    public enum AstroidSize
    {
        Small = 0, Medium = 1, Large = 2
    }
}