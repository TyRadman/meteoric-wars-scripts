
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class ShipBuilder : Singlton<ShipBuilder>
    {
        #region Structs
        [System.Serializable]
        public struct ShipShapeValues
        {
            public ShipShape Shape;
            public SpriteRandomPointPosition SingleBodyPosition;
            public SpriteRandomPointPosition DoubleBodyPosition;
            public SpriteRandomPointPosition WingsPosition;
            public SpriteRandomPointPosition TailPosition;
            public bool[] PartsToPlaceOn; // body, wing, tail
        }

        [System.Serializable]
        public struct PolygonInfo
        {
            public PolygonCollider2D Polygon;
            public Vector2 Scale;
            public Vector2 Offset;
        }
        #endregion

        [SerializeField] private List<ShipShapeValues> m_Shapes;
        [SerializeField] private ShipShape m_Shape;
        [SerializeField] private bool m_OneColorPalette = true;
        [Header("References")]
        [SerializeField] private ShipBuilderColor m_Colors;
        private ShipColorProfile m_ColorProfile;
        [SerializeField] private EnemySpawner m_EnemySpawner;
        [SerializeField] private List<GameObject> m_Bodies;
        [SerializeField] private List<GameObject> m_Wings;
        [SerializeField] private List<GameObject> m_Tails;
        [SerializeField] private List<GameObject> m_ShooterPrefabs;
        private List<Weapon> m_Weapons;
        [SerializeField] private GameObject m_EnemyShipPrefab;
        [SerializeField] private GameObject m_PlayerShipPrefab;
        [SerializeField] private List<Transform> m_SpacesPosition;
        [Header("Ranks")]
        private GameObject m_LastCreatedShip;
        private Transform m_LastCreatedShipGFX;
        private Transform m_ShipsParent;
        [Header("Modifiers")]
        public ShipRank ShipToBuildRank;
        [HideInInspector] public int CurrentShipPositionIndex = 0;
        [Header("General Values")]
        [SerializeField] private Vector2 m_WingXSize;
        [SerializeField] private Vector2 m_WingYSize;
        [SerializeField] private Vector2 m_DistanceBetweenGraphicsLayers;
        [SerializeField] private Vector2 m_ExtraBodiesOffsetX;
        [SerializeField] private Vector2 m_ExtraBodiesOffsetY;

        // vars used only to cache ship parts
        private ShipBody m_FirstShipBody; // also left body
        private ShipWing m_LeftWing;
        private ShipWing m_RightWing;
        private ShipTail m_Tail;
        private List<ShipShooter> m_Shooters = new List<ShipShooter>();
        private ShipRankValues m_SelectedValues;
        private ShipRank m_SelectedRank;
        private List<ShipPart> m_ShipParts = new List<ShipPart>();
        private List<ShipPart> m_LastShipParts = new List<ShipPart>();
        private List<PolygonInfo> m_ScaledPolygons = new List<PolygonInfo>();
        // random types of shooting to choose from
        private List<System.Type> m_ShootingTypes = new List<System.Type>() { typeof(FrequentShooting) };
        private List<ShipColorProfile> m_ColorProfiles = new List<ShipColorProfile>();
        private float m_OffensiveDifficulty;
        private float m_DefensiveDifficulty;
        // we cache because we create a new one everytime a new color pallete is created
        [SerializeField] private Material m_ShooterMaterial;
        [SerializeField] private Material m_ShooterMaterialReference;

        protected override void Awake()
        {
            base.Awake();
            SetNewColorPalette();
        }

        public void DestroyDisplayedShips()
        {
            if (m_ShipsParent != null) DestroyImmediate(m_ShipsParent.gameObject);

            CurrentShipPositionIndex = 0;
        }

        public GameObject BuildPlayerShip(ShipRank _rank, float _offensive = 1f, float _defensive = 1f, int _colorNumber = 0)
        {
            // set the color 
            m_ColorProfile = m_ColorProfiles[_colorNumber];
            // set up difficulties
            m_OffensiveDifficulty = _offensive;
            m_DefensiveDifficulty = _defensive;

            m_ScaledPolygons.Clear();

            // increment the order layer index

            if (CurrentShipPositionIndex > m_SpacesPosition.Count - 1)
            {
                DestroyDisplayedShips();
            }

            m_SelectedRank = _rank;

            m_SelectedValues = FindObjectOfType<GameManager>().GeneralValues.RankValues.Find(s => s.Rank == _rank);
            m_SelectedValues = FindObjectOfType<GameManager>().GeneralValues.RankValues.Find(s => s.Rank == _rank);

            InstantiateShipAndParts(m_PlayerShipPrefab);
            SetUpWingsAndTail();
            SetUpShooters();
            SetUpColoring(m_ShipParts);
            SetUpPlayerComponents();
            SetUpBodySize();
            AdditionalLayersBuild();
            SetUpColliderAndDamageDetection();

            return m_LastCreatedShip;
        }

        public GameObject BuildShip(ShipRank _rank, float _offensive = 1f, float _defensive = 1f)
        {
            // set up difficulties
            m_OffensiveDifficulty = _offensive;
            m_DefensiveDifficulty = _defensive;

            m_ScaledPolygons.Clear();

            // increment the order layer index

            if (CurrentShipPositionIndex > m_SpacesPosition.Count - 1)
            {
                DestroyDisplayedShips();
            }

            m_SelectedRank = _rank;

            if (GameManager.i == null)
            {
                m_SelectedValues = FindObjectOfType<GameManager>().GeneralValues.RankValues.Find(s => s.Rank == _rank);
            }
            else
            {
                m_SelectedValues = GameManager.i.GeneralValues.RankValues.Find(s => s.Rank == _rank);
            }

            if (CurrentShipPositionIndex == 0 && m_ShipsParent == null)
            {
                m_ShipsParent = new GameObject("Ships").transform;
            }

            InstantiateShipAndParts(m_EnemyShipPrefab);

            SetUpWingsAndTail();

            SetUpShooters();

            SetUpColoring(m_ShipParts);

            SetUpEnemyComponents();

            SetUpBodySize();

            AdditionalLayersBuild();

            OptimizeTransforms(m_ShipParts);

            SetUpColliderAndDamageDetection();

            // rotate towards the player
            m_LastCreatedShip.transform.rotation = m_SpacesPosition[CurrentShipPositionIndex].rotation;
            CurrentShipPositionIndex++;
            return m_LastCreatedShip;
        }

        public void CreateFullSetOfShips()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    BuildShip((ShipRank)i);
                }
            }
        }
        
        public void CreateFullSetOfShipsRandomColors()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    SetNewColorPalette();
                    BuildShip((ShipRank)i);
                }
            }
        }

        public void SetNewColorPalette()
        {
            m_ColorProfile = m_Colors.GetRandomColorPalette();
            m_ColorProfiles.Add(m_ColorProfile);
            // create a new instance material for the shooter
            m_ShooterMaterial = Material.Instantiate(m_ShooterMaterialReference);
        }

#region Main Ship Helper Functions
        private void InstantiateShipAndParts(GameObject _shipPrefab)
        {
            m_LastShipParts.Clear();
            m_LastShipParts = new List<ShipPart>();
            m_LastCreatedShip = Instantiate(_shipPrefab, m_ShipsParent);
            m_LastCreatedShip.transform.parent = m_ShipsParent;
            m_LastCreatedShip.transform.position = m_SpacesPosition[CurrentShipPositionIndex].position;
            m_LastCreatedShipGFX = new GameObject("GFX").transform;
            m_LastCreatedShipGFX.parent = m_LastCreatedShip.transform;
            m_LastCreatedShipGFX.localPosition = Vector3.zero;

            if (EnemyShipNamesGenerator.i != null)
            {
                m_LastCreatedShip.name = $"{EnemyShipNamesGenerator.i.GetShipName(m_SelectedRank)}";
            }

            // instantiation
            m_FirstShipBody = Instantiate(m_Bodies[Random.Range(0, m_Bodies.Count)], m_LastCreatedShipGFX).GetComponent<ShipBody>();
            int wingIndex = Random.Range(0, m_Wings.Count);
            m_LeftWing = Instantiate(m_Wings[wingIndex], m_LastCreatedShipGFX).GetComponent<ShipWing>();
            m_RightWing = Instantiate(m_Wings[wingIndex], m_LastCreatedShipGFX).GetComponent<ShipWing>();
            m_Tail = Instantiate(m_Tails[Random.Range(0, m_Tails.Count)], m_FirstShipBody.transform).GetComponent<ShipTail>();
            m_ShipParts = new List<ShipPart> { m_FirstShipBody, m_LeftWing, m_RightWing, m_Tail };
            m_LastShipParts.AddRange(new List<ShipPart> { m_FirstShipBody, m_LeftWing, m_Tail });

            if (GameManager.i == null || GameManager.i.GeneralValues == null)
            {
                m_LastCreatedShip.SetActive(true);
            }
        }

        private void SetUpPlayerComponents()
        {
            PlayerStats stats = m_LastCreatedShip.GetComponent<PlayerStats>();
            PlayerComponents components = m_LastCreatedShip.GetComponent<PlayerComponents>();

            if (WeaponsGenerator.i != null)
            {
                List<WeaponsGenerator.WeaponStyles> avoidedStyles = new List<WeaponsGenerator.WeaponStyles>() { WeaponsGenerator.WeaponStyles.Sniper };
                BulletWeapon weapon = WeaponsGenerator.i.CreateWeapon(m_SelectedRank, avoidedStyles, m_OffensiveDifficulty);
                m_LastCreatedShip.GetComponent<Shooter>().SetUpWeapon(weapon);
            }

            //stats.SetMaxHealth(health);

            m_LastCreatedShip.GetComponent<ShipEffects>().AddDamageRenderer(m_ShipParts);
            m_LastCreatedShip.GetComponent<ShipEffects>().MuzzleEffectTag = ParticlePoolTag.StandardMuzzle;

            // adding damage detectors to parts and extra logic set up
            for (int i = 0; i < m_ShipParts.Count; i++)
            {
                // collect polygon colliders from all ship parts and remove it from the parts (to be assembled later in the main body)
                if (m_ShipParts[i].GetComponent<PolygonCollider2D>() != null)
                {
                    m_ScaledPolygons.Add(new PolygonInfo
                    {
                        Polygon = m_ShipParts[i].GetComponent<PolygonCollider2D>(),
                        Scale = m_ShipParts[i].transform.localScale,
                        Offset = m_ShipParts[i].transform.localPosition
                    });

                    m_ShipParts[i].GetComponent<PolygonCollider2D>().enabled = false;
                }
            }
        }

        private void SetUpEnemyComponents()
        {
            EnemyComponents components = m_LastCreatedShip.GetComponent<EnemyComponents>();

            // setting up weapons
            if (WeaponsGenerator.i != null)
            {
                BulletWeapon weapon = WeaponsGenerator.i.CreateWeapon(m_SelectedRank, m_OffensiveDifficulty);
                m_LastCreatedShip.GetComponent<Shooter>().SetUpWeapon(weapon);
            }

            // selecting a random shooting method for the ship
            m_LastCreatedShip.AddComponent(m_ShootingTypes.RandomItem());
            components.SetShootingMethod(m_LastCreatedShip.GetComponent<EnemyShooting>());

            components.SetUp(null);
            components.SetValues(m_SelectedValues, m_OffensiveDifficulty, m_DefensiveDifficulty);

            // setting up parts of the ship that will change their colors when hit
            components.Effects.AddDamageRenderer(m_ShipParts);

            SetDamageDetectors();

            bool shipHasAbilities = m_SelectedValues.Rank is ShipRank.Strong or ShipRank.Medium or ShipRank.Summoner;

            if (shipHasAbilities)
            {
                components.Abilities = m_LastCreatedShip.AddComponent<EnemyShipAbilities>();
            }
        }

        private void SetDamageDetectors()
        {
            // adding damage detectors to parts and extra logic set up
            for (int i = 0; i < m_ShipParts.Count; i++)
            {
                // collect polygon colliders from all ship parts and remove it from the parts (to be assembled later in the main body)
                if (m_ShipParts[i].GetComponent<PolygonCollider2D>() != null)
                {
                    m_ScaledPolygons.Add(new PolygonInfo
                    {
                        Polygon = m_ShipParts[i].GetComponent<PolygonCollider2D>(),
                        Scale = m_ShipParts[i].transform.localScale,
                        Offset = m_ShipParts[i].transform.localPosition
                    });

                    m_ShipParts[i].GetComponent<PolygonCollider2D>().enabled = false;
                }
            }
        }


        private void SetUpColliderAndDamageDetection()
        {
            // add a polygon collider to the main ship body
            PolygonCollider2D collider = m_LastCreatedShip.GetComponent<PolygonCollider2D>();

            // this function will add all ship part collider paths to the main polygon collider
            Helper.SetPolygonCollider(collider, m_ScaledPolygons);
        }

        private void SetUpWingsAndTail()
        {
            // mirror right wing
            m_RightWing.transform.localScale = new Vector2(-m_RightWing.transform.localScale.x, m_RightWing.transform.localScale.y);
            // getting wings positions
            SpriteRenderer selectedPart = m_FirstShipBody.transform.GetChild(0).GetComponent<SpriteRenderer>();
            m_LeftWing.transform.position = Helper.GetRandomPositionOnSprite(selectedPart, SpriteRandomPointPosition.RandonXAndY);
            // right wing, no need to get the offset since we can just use the position of the left wing
            m_RightWing.transform.localPosition = new Vector2(-m_LeftWing.transform.localPosition.x, m_LeftWing.transform.localPosition.y);
            // tail position
            m_Tail.transform.position = Helper.GetRandomPositionOnSprite(selectedPart, SpriteRandomPointPosition.StableXOuterY);
        }

        private void SetUpShooters()
        {
            m_Shooters.Clear();
            int shootersNum = m_SelectedValues.ShootersNumber.RandomValue();

            // instantiation
            for (int i = 0; i < shootersNum; i++)
            {
                ShipShooter shooter = Instantiate(m_ShooterPrefabs[0], m_LastCreatedShipGFX).GetComponent<ShipShooter>();
                m_Shooters.Add(shooter);
            }

            // selecting an index for the two shooters if they are to be set
            if (shootersNum > 1)
            {
                int wingsShootingPointIndex = Random.Range(0, m_LeftWing.WeaponPointsNumber());
                m_Shooters[m_Shooters.Count - 1].transform.position = m_LeftWing.GetWeaponPoint(wingsShootingPointIndex);
                m_Shooters[m_Shooters.Count - 2].transform.position = m_RightWing.GetWeaponPoint(wingsShootingPointIndex);
            }

            // if the number of shooters is one or three, then we set a shooter in the middle i.e. on the body
            if (shootersNum % 2 == 1)
            {
                m_Shooters[0].transform.localPosition = m_FirstShipBody.WeaponPoint;
            }

            List<Transform> shootingPoints = new List<Transform>();

            m_Shooters.ForEach(s => shootingPoints.Add(s.transform));

            // assigning the shooting points
            m_LastCreatedShip.GetComponent<Shooter>().SetUpShootingPoints(shootingPoints);
            List<ShipPart> shooterParts = new List<ShipPart>();
            m_Shooters.ForEach(s => shooterParts.Add(s));
            SetUpColoring(shooterParts);
            OptimizeTransforms(shooterParts);
        }

        // to be removed as the pixels looks weird when the ship is scaled
        private void SetUpBodySize()
        {
            // overall ship resizing
            float shipSize = m_SelectedValues.ShipSize.RandomValue();
            m_LastCreatedShipGFX.localScale = new Vector3(shipSize, shipSize, 1f);
        }

        private void SetUpColoring(List<ShipPart> _parts)
        {
            // if there is no current color profile, then set one
            if (m_ColorProfile == null)
            {
                SetNewColorPalette();
            }

            // the body takes the main color. Always? 
            Color bodyColor = m_ColorProfile.MainColor;
            Color wingsColor = Random.value > 0.5f ? bodyColor : m_ColorProfile.GetRandomComplementaryColor();
            float value = Random.value;
            Color tailColor = value <= 0.3f ? bodyColor : value <= 0.6f ? wingsColor : m_ColorProfile.GetRandomComplementaryColor();

            for (int i = 0; i < _parts.Count; i++)
            {
                ShipPart part = _parts[i];

                switch (part.Tag)
                {
                    case ShipPart.ShipPartTag.Body:
                            part.SetColor(bodyColor);
                            break;
                    case ShipPart.ShipPartTag.Wing:
                            part.SetColor(wingsColor);
                            break;
                    case ShipPart.ShipPartTag.Tail:
                            part.SetColor(tailColor);
                            break;
                    case ShipPart.ShipPartTag.Weapon:
                            part.SetColor(wingsColor);
                            ShipShooter shooter = (ShipShooter)part;
                            shooter.SetMaterialColor(wingsColor, m_ShooterMaterial);
                            break;
                }
            }
        }

        private void AdditionalLayersBuild()
        {
            int layersNum = m_SelectedValues.AdditionalGraphicsLayersRange.RandomValue();

            // add layers if the rank allows it
            if (layersNum > 0)
            {
                AddAdditionalLayers(layersNum, false, m_ShipParts);
            }
        }

        private void OptimizeTransforms(List<ShipPart> _parts)
        {
            return;

            for (int i = 0; i < _parts.Count; i++)
            {
                Transform part = _parts[i].transform;
                Transform gfx = part.GetChild(0);
                // release the child
                gfx.SetParent(part.parent);
                gfx.name = part.name;

                // move the children to the GFX transfrom
                for (int j = 0; j < part.childCount; j++)
                {
                    part.GetChild(j).parent = gfx;
                }

                Destroy(part.gameObject);
            }
        }


#endregion

#region Additional Layers Helper Functions
        public void AddAdditionalLayers(int _loops, bool _builtTwoBodies, List<ShipPart> _previousParts)
        {
            if (_loops == 0) return;

            var shape = m_Shapes.Find(s => s.Shape == m_Shape);

            bool twoBodies = !_builtTwoBodies && Random.value > 0.5f;
            List<ShipPart> parts;
            int bodyIndex = Random.Range(0, m_Bodies.Count);
            var leftBody = Instantiate(m_Bodies[bodyIndex], m_LastCreatedShipGFX).GetComponent<ShipBody>();
            int wingIndex = Random.Range(0, m_Wings.Count);
            var leftWing = Instantiate(m_Wings[wingIndex], m_LastCreatedShipGFX).GetComponent<ShipWing>();
            var rightWing = Instantiate(m_Wings[wingIndex], m_LastCreatedShipGFX).GetComponent<ShipWing>();
            int tailIndex = Random.Range(0, m_Tails.Count);
            var leftTail = Instantiate(m_Tails[tailIndex], m_LastCreatedShipGFX).GetComponent<ShipTail>();
            parts = new List<ShipPart> { leftBody, leftWing, leftTail, rightWing };
            SpriteRenderer selectedPart;

            // set up parts that don't get affected by whether the ship has two bodies or not
            selectedPart = _previousParts.RandomItem().transform.GetChild(0).GetComponent<SpriteRenderer>();
            leftWing.transform.position = Helper.GetRandomPositionOnSprite(selectedPart, shape.WingsPosition);
            rightWing.transform.localScale = new Vector2(-1, 1);
            rightWing.transform.localPosition = new Vector2(-leftWing.transform.localPosition.x, leftWing.transform.localPosition.y); ;

            // tail position
            selectedPart = leftBody.transform.GetChild(0).GetComponent<SpriteRenderer>();
            leftTail.transform.position = Helper.GetRandomPositionOnSprite(selectedPart, shape.TailPosition);

            if (!_builtTwoBodies)
            {
                selectedPart = m_FirstShipBody.transform.GetChild(0).GetComponent<SpriteRenderer>();
                leftBody.transform.position = Helper.GetRandomPositionOnSprite(selectedPart, shape.SingleBodyPosition);
            }
            else
            {
                var rightBody = Instantiate(m_Bodies[bodyIndex], m_LastCreatedShipGFX).GetComponent<ShipBody>();
                var rightTail = Instantiate(m_Tails[tailIndex], m_LastCreatedShipGFX).GetComponent<ShipTail>();
                parts.AddRange(new List<ShipPart>() { rightBody, rightTail });

                selectedPart = _previousParts.RandomItem().transform.GetChild(0).GetComponent<SpriteRenderer>();
                leftBody.transform.position = Helper.GetRandomPositionOnSprite(selectedPart, shape.DoubleBodyPosition);
                rightBody.transform.localPosition = new Vector2(-leftBody.transform.localPosition.x, leftBody.transform.localPosition.y);
                // tail
                rightTail.transform.localPosition = new Vector2(-leftTail.transform.localPosition.x, leftTail.transform.localPosition.y);
            }

            // coloring
            SetUpColoring(parts);
            // add the previous parts depending on the ship style
            _previousParts = new List<ShipPart>();

            for (int i = 0; i < shape.PartsToPlaceOn.Length; i++)
            {
                if (shape.PartsToPlaceOn[i])
                {
                    _previousParts.Add(parts[i]);
                }
            }

            // just in case for some reason it was zero
            if (_previousParts.Count == 0) _previousParts.Add(leftBody);

            m_LastCreatedShip.GetComponent<ShipEffects>().AddDamageRenderer(parts);

            // adding damage detectors to parts and extra logic set up
            for (int i = 0; i < parts.Count; i++)
            {
                // collect polygon colliders from all ship parts and remove it from the parts (to be assembled later in the main body)
                if (parts[i].GetComponent<PolygonCollider2D>() != null)
                {
                    m_ScaledPolygons.Add(new PolygonInfo
                    {
                        Polygon = parts[i].GetComponent<PolygonCollider2D>(),
                        Scale = parts[i].transform.localScale,
                        Offset = parts[i].transform.localPosition
                    });

                    parts[i].GetComponent<PolygonCollider2D>().enabled = false;
                }
            }

            AddAdditionalLayers(--_loops, _builtTwoBodies, _previousParts);
            m_ShipParts.AddRange(_previousParts);
        }
#endregion
    }

    public enum SpriteRandomPointPosition
    {
        RandonXAndY, RandomXStableY, StableXRandomY, OuterXRandomY, RandomXOuterY, OuterXOuterY, StableXOuterY, OuterXStableY
    }

    public enum ShipShape
    {
        Random, HorizontalExpanding, VerticalExpanding
    }
}