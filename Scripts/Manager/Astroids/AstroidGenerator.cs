using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpaceWar
{
    public class AstroidGenerator : Singlton<AstroidGenerator>
    {
        [SerializeField] private List<AstroidPrefab> m_AstroidPrefabs;
        private List<AstroidPrefab> m_AstroidPrefabsInstances = new List<AstroidPrefab>();
        [SerializeField] private int m_LayersNumber;
        [SerializeField] private float m_XPosition = -8;
        [SerializeField] private float m_YPosition = 8;
        [Header("References")]
        [SerializeField] private GameObject m_AstroidParentPrefab;
        [SerializeField] private GameObject m_AstroidEmptyPiecePrefab;
        private Astroid m_CurrentAstroid;
        [SerializeField] private GameObject m_BackGroundAstroidParentPrefab;
        [SerializeField] private Gradient m_Color;

        protected override void Awake()
        {
            base.Awake();
            CreatePrefabInstances();
        }

        private void CreatePrefabInstances()
        {
            for (int i = 0; i < m_AstroidPrefabsInstances.Count; i++)
            {
                if (m_AstroidPrefabsInstances[i] != null)
                {
                    DestroyImmediate(m_AstroidPrefabsInstances[i].gameObject);
                }
            }

            m_AstroidPrefabsInstances.Clear();

            for (int i = 0; i < m_AstroidPrefabs.Count; i++)
            {
                m_AstroidPrefabsInstances.Add(Instantiate(m_AstroidPrefabs[i], transform));
                m_AstroidPrefabsInstances[m_AstroidPrefabsInstances.Count - 1].gameObject.SetActive(false);
            }
        }

        public Astroid GenerateBody(int _layers)
        {
            if (m_AstroidPrefabsInstances.Count == 0 || m_AstroidPrefabsInstances.Exists(p => p == null))
            {
                CreatePrefabInstances();
            }

            SpriteRenderer lastPiece;
            List<PolygonCollider2D> colliders = new List<PolygonCollider2D>();

            Transform astroidParent = Instantiate(m_AstroidParentPrefab).transform;
            AstroidDamageRenderer damageRenderer = astroidParent.GetComponent<AstroidDamageRenderer>();

            SpriteRenderer piece = Instantiate(m_AstroidEmptyPiecePrefab, astroidParent).GetComponent<SpriteRenderer>();
            PolygonCollider2D pieceCollider = piece.GetComponent<PolygonCollider2D>();
            lastPiece = piece;
            Color AstroidColor = m_Color.Evaluate(Random.value);

            piece.transform.localPosition = Vector3.zero;

            var selectedAstroidPrefab = m_AstroidPrefabsInstances.RandomItem();
            SetUpValuesForEmptyAstroidPiece(piece, pieceCollider, selectedAstroidPrefab, AstroidColor);
            damageRenderer.AddDamageRenderer(piece);
            colliders.Add(pieceCollider);

            for (int i = 0; i < _layers; i++)
            {
                piece = Instantiate(m_AstroidEmptyPiecePrefab, astroidParent.position, Quaternion.identity, astroidParent).GetComponent<SpriteRenderer>();
                pieceCollider = piece.GetComponent<PolygonCollider2D>();
                piece.gameObject.name = $"Piece {i}";

                selectedAstroidPrefab = m_AstroidPrefabsInstances.RandomItem();
                SetUpValuesForEmptyAstroidPiece(piece, pieceCollider, selectedAstroidPrefab, AstroidColor);

                piece.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 3) * 90);
                piece.transform.position = Helper.GetRandomPositionOnSprite(lastPiece, SpriteRandomPointPosition.RandonXAndY);
                lastPiece = piece;

                // add the damage renderer to the astroid 
                damageRenderer.AddDamageRenderer(piece);
                // add the colliders to combine them all together
                colliders.Add(pieceCollider);
            }

            // set the parent position at the center of all other children
            colliders.ForEach(c => c.transform.SetParent(null));
            Vector2 position = Vector2.zero;
            colliders.ForEach(c => position += (Vector2)c.transform.position);
            position /= colliders.Count;
            astroidParent.position = position;
            colliders.ForEach(c => c.transform.SetParent(astroidParent));

            m_CurrentAstroid = astroidParent.GetComponent<Astroid>();
            astroidParent.GetComponent<Astroid>().SetUpCollider(colliders);
            return astroidParent.GetComponent<Astroid>();
        }

        public void SetUpValuesForEmptyAstroidPiece(SpriteRenderer _piece, PolygonCollider2D _collider, AstroidPrefab _prefab, Color _color)
        {
            _piece.sprite = _prefab.SpriteRenderer.sprite;
            _piece.color = _prefab.SpriteRenderer.color;
            _piece.color = _color;
            _collider.points = _prefab.Collider.points;
        }

        public void SetUpValuesForEmptyAstroidPiece(SpriteRenderer _piece, AstroidPrefab _prefab)
        {
            _piece.sprite = _prefab.SpriteRenderer.sprite;
            _piece.color = _prefab.SpriteRenderer.color;
        }

        public BGAstroid GenerateBGAstroid(int _layers)
        {
            if (m_AstroidPrefabsInstances.Count == 0 || m_AstroidPrefabsInstances.Exists(p => p == null))
            {
                CreatePrefabInstances();
            }

            SpriteRenderer lastPiece;

            Transform astroidParent = Instantiate(m_BackGroundAstroidParentPrefab).transform;
            SpriteRenderer piece = Instantiate(m_AstroidEmptyPiecePrefab, astroidParent).GetComponent<SpriteRenderer>();
            lastPiece = piece;

            piece.transform.localPosition = Vector3.zero;

            var selectedAstroidPrefab = m_AstroidPrefabsInstances.RandomItem();
            SetUpValuesForEmptyAstroidPiece(piece, selectedAstroidPrefab);

            for (int i = 0; i < _layers; i++)
            {
                piece = Instantiate(m_AstroidEmptyPiecePrefab, astroidParent.position, Quaternion.identity, astroidParent).GetComponent<SpriteRenderer>();
                piece.gameObject.name = $"Piece {i}";

                selectedAstroidPrefab = m_AstroidPrefabsInstances.RandomItem();
                SetUpValuesForEmptyAstroidPiece(piece, selectedAstroidPrefab);

                piece.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 3) * 90);
                piece.transform.position = Helper.GetRandomPositionOnSprite(lastPiece, SpriteRandomPointPosition.RandonXAndY);
                lastPiece = piece;
            }

            Vector2 position = Vector2.zero;
            astroidParent.position = position;

            return astroidParent.GetComponent<BGAstroid>();
        }
    }
}