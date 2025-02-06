using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using SpaceWar.UI.HUD;
using SpaceWar.UI.Shop;
using SpaceWar.Shop;
using SpaceWar.Audios;

namespace SpaceWar
{
    public class GameManager : Singlton<GameManager>
    {
        [field: SerializeField] public bool SpawnEnemies { get; set; } = true;
        [field: SerializeField, Range(0f, 2f)] public float Difficulty { get; set; } = 1f;
        private float m_OffensiveDifficulty;
        private float m_DefensiveDifficulty;
        [field: SerializeField, Header("References")] public PlayersManager PlayersManager { get; private set; }
        [field: SerializeField] public InputController InputController { get; private set; }
        [field: SerializeField] public ShopManager ShopManager { get; private set; }
        [field: SerializeField] public GeneralValues GeneralValues { get; private set; }
        [field: SerializeField] public WavesGenerator WavesGenerator { get; private set; }
        [field: SerializeField] public AstroidSpawner AstroidSpawner { get; private set; }
        [field: SerializeField] public AudioManager AudioManager { get; private set; }

        [field: SerializeField, Header("UI")] public AbilitiesShopMenu AbilitiesShop { get; private set; }
        [field: SerializeField] public HUDManager HUDManager { get; private set; }
        [field: SerializeField] public EnemyHealthBar EnemyHealthBar { get; private set; }
        [field: SerializeField] public ScreenDamageIndicator DamageIndicator { get; private set; }

        [field: SerializeField, Header("DataBase")] public PlayerShipsData PlayerShipsData;
        [HideInInspector] public Dictionary<string, bool> FoldoutStates = new Dictionary<string, bool>();

        private void Start()
        {
            SetUp();

            Vector2 difficulties = SetUpDifficulty(Difficulty);
            m_OffensiveDifficulty = difficulties.x;
            m_DefensiveDifficulty = difficulties.y;

            // Create enemy waves, as variables
            LevelEnemySet waves = WavesGenerator.GetEnemyWaves();
            // Create enemy ship references and put them in the waves
            int shipColorsNumber = PlayersManager.DifferentColorPalettes ? 2 : 1;
            List<EnemyWave> createdWaves = EnemySpawner.i.CreateShipsReferences(waves, m_OffensiveDifficulty, m_DefensiveDifficulty, shipColorsNumber);

            if (SpawnEnemies)
            {
                // start the waves
                WavesManager.i.StartWaves(createdWaves, m_DefensiveDifficulty);
            }

            // set up abilities for the level if the ship is not randomly generated
            if (PlayersManager.TheShipType == PlayersManager.ShipType.Premade)
            {
                AbilitiesManager.i.SetAbilitiesForLevel();
            }
        }

        private void SetUp()
        {
            InputController.SetUp();
            HUDManager.SetUp();
            PlayersManager.SetUp(PlayerShipsData);
            AbilitiesShop.SetUp();
            ShopManager.SetUp();
            AstroidSpawner.SetUp();
            AudioManager.SetUp();
        }

        public Vector2 SetUpDifficulty(float _difficulty)
        {
            float offensive = Mathf.Clamp01(Mathf.Lerp(0, _difficulty, Random.value));
            float defensive = Mathf.Clamp01(Mathf.Lerp(0, _difficulty - offensive, Random.value));
            float remainingDifficulty = _difficulty - offensive - defensive;

            for (int i = 0; i < 20; i++)
            {
                if (remainingDifficulty > 0)
                {
                    float offenseToAdd = Mathf.Clamp(remainingDifficulty / 2f, 0f, 1f - offensive);
                    offensive += offenseToAdd;
                    float defenseToAdd = Mathf.Clamp((remainingDifficulty / 2f) + ((remainingDifficulty / 2) - offenseToAdd), 0f, 1f - defensive);
                    defensive += defenseToAdd;
                    remainingDifficulty = _difficulty - offensive - defensive;
                }
            }

            return new Vector2(offensive, defensive);
        }

        public void LevelCompleted()
        {
            StartCoroutine(LevelCompletedProcess());
        }

        private IEnumerator LevelCompletedProcess()
        {
            yield return new WaitForSeconds(1f);
            // play animaiton
            ScreenTitlesManager.Instance.ShowLevelComplete();
            yield return new WaitForSeconds(ScreenTitlesManager.Instance.GetAnimationLength(ScreenTitlesManager.AnimationTag.Wave));
            // show stats
            PlayersManager.Players.ForEach(p => p.Components.Stats.SetGameStatsValues());
            GameStatsManager.i.DisplayStats();
        }

        public void GameOver()
        {
            StartCoroutine(GameOverProcess());
        }

        private IEnumerator GameOverProcess()
        {
            yield return new WaitForSeconds(1f);
            ScreenTitlesManager.Instance.PlayAnimation(ScreenTitlesManager.AnimationTag.GameOver);
            yield return new WaitForSeconds(ScreenTitlesManager.Instance.GetAnimationLength(ScreenTitlesManager.AnimationTag.GameOver));
            // show stats
            PlayersManager.Players.ForEach(p => p.Components.Stats.SetGameStatsValues());
            GameStatsManager.i.DisplayStats();
        }

        public void ReloadScene(InputAction.CallbackContext context)
        {
            // disable the input first
            for (int i = 0; i < PlayersManager.Players.Count; i++)
            {
                //InputController.Instance.DisableControls(i);
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void ReturnToMenuScene(InputAction.CallbackContext context)
        {
            for (int i = 0; i < PlayersManager.Players.Count; i++)
            {
                //InputController.Instance.DisableControls(i);
            }

            SceneManager.LoadScene(0);
        }
    }
}