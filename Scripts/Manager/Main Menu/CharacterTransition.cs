using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class CharacterTransition : MonoBehaviour
    {
        [System.Serializable]
        public class Cloud
        {
            [HideInInspector] public Material Material;
            public SpriteRenderer Sprite;

            public void SetUp()
            {
                Material = Sprite.material;
            }
        }

        [System.Serializable]
        public struct ColorPair
        {
            public Color FirstColor;
            public Color SecondColor;
        }

        [SerializeField] private List<ColorPair> m_Colors;
        [SerializeField] private List<Cloud> m_FirstClouds;
        [SerializeField] private List<Cloud> m_SecondClouds;
        private bool m_TransitionToFirstClouds = false;
        [SerializeField] private float m_TransitionDuration = 2f;
        [SerializeField] private float m_CloudsSpeedRatio = 0.1f;
        [SerializeField] private float m_MaxTransitionScale = 3f;
        [Header("Sizes")]
        [SerializeField] private float m_MinSize;
        [SerializeField] private float m_MaxSize;
        [SerializeField] private float m_NormalSize;

        private void Awake()
        {
            m_FirstClouds.ForEach(c => c.SetUp());
            m_SecondClouds.ForEach(c => c.SetUp());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                var col = m_Colors.RandomItem();
                Transition(col.FirstColor, col.SecondColor, new Vector2(Random.Range(0f, 0.2f), Random.Range(0.01f, 2f)));
            }
        }

        public void Transition(Color _firstColor, Color _secondColor, Vector2 _cloudsSpeed)
        {
            // choose what clouds to color. the first or the second ones
            List<Cloud> clouds = m_TransitionToFirstClouds ? m_SecondClouds : m_FirstClouds;
            // change the color of the other clouds and set their speed
            clouds[0].Sprite.color = _firstColor;
            clouds[1].Sprite.color = _secondColor;

            // set the speed
            for (int i = 0; i < clouds.Count; i++)
            {
                float speed = _cloudsSpeed.y - m_CloudsSpeedRatio * i;
                speed = speed < 0 ? 0 : speed;
                clouds[i].Material.SetVector("_Speed", new Vector2(_cloudsSpeed.x, speed));
            }

            StartCoroutine(TransitioningProcess());
        }

        private IEnumerator TransitioningProcess()
        {
            List<Cloud> vanishingClouds = m_TransitionToFirstClouds ? m_FirstClouds : m_SecondClouds;
            List<Cloud> newClouds = m_TransitionToFirstClouds ? m_SecondClouds : m_FirstClouds;
            float time = 0f;
            Vector2 normalSize = Vector2.one * m_NormalSize;
            Vector2 maxSize = Vector2.one * m_MaxSize;
            Vector2 minSize = Vector2.one * m_MinSize;
            Color firstCloudColor = vanishingClouds[0].Sprite.color;
            Color secondCloudColor = vanishingClouds[1].Sprite.color;
            Color thirdCloudColor = newClouds[0].Sprite.color;
            Color fourthCloudColor = newClouds[1].Sprite.color;
            m_TransitionToFirstClouds = !m_TransitionToFirstClouds;

            while (time < m_TransitionDuration)
            {
                time += Time.deltaTime;
                float t = time / m_TransitionDuration;

                // scale up first clouds
                vanishingClouds.ForEach(c => c.Sprite.transform.localScale = Vector2.Lerp(normalSize, maxSize, t));
                // make clouds more transparent
                firstCloudColor.a = Mathf.Lerp(1, 0, t);
                vanishingClouds[0].Sprite.color = firstCloudColor;
                secondCloudColor.a = Mathf.Lerp(1, 0, t);
                vanishingClouds[1].Sprite.color = secondCloudColor;

                // scale up the 
                newClouds.ForEach(c => c.Sprite.transform.localScale = Vector2.Lerp(minSize, normalSize, t));
                thirdCloudColor.a = Mathf.Lerp(0, 1, t);
                newClouds[0].Sprite.color = thirdCloudColor;
                fourthCloudColor.a = Mathf.Lerp(0, 1, t);
                newClouds[1].Sprite.color = fourthCloudColor;

                yield return null;
            }
        }
    }
}