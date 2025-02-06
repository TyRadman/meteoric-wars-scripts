using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceWar.UI.HUD;
using SpaceWar.Shop;

namespace SpaceWar
{
    public class PlayerStats : Stats
    {
        private PlayerComponents m_Components;
        public bool CanTakeDamage = false;
        private int m_PlayerIndex = -1;
        [SerializeField] private int m_XPPointsLevelIndex;
        public float MaxShooterPoints;
        private float m_CurrentShooterPoints;
        private int m_Lives;
        #region Stats
        private float m_Score;
        private int m_Deaths;
        private int m_ShipsDestroyed;
        #endregion
        public float MissChance;
        public float Resilience;
        private float m_Scale;
        private bool m_EnableComponents = false;
        private static string[] m_LevelsNumbering = new string[10] { "1", "2", "3", "4", "5", "5 i", "5 ii", "5 iii", "5 iv", "5 v" };
        private Collider2D m_Collider;

        public override void SetUp(IController components)
        {
            base.SetUp(components);

            m_Collider = GetComponent<Collider2D>();
            m_Components = components as PlayerComponents;

            m_PlayerIndex = m_Components.PlayerIndex;
            m_Scale = transform.localScale.x;
            m_XPPointsLevelIndex = 0;

            // just to distinguish between the main menu and the gameplay scene
            if (PlayerLevelUp.i == null)
            {
                return;
            }

            MaxShooterPoints = PlayerLevelUp.i.ShooterPointsLevels[m_XPPointsLevelIndex];
            m_CurrentShooterPoints = 0f;
            AddXPPoints(0f);
            GameManager.i.HUDManager.UpdateScoreText(0f, m_PlayerIndex);
            GameManager.i.HUDManager.UpdatePlayerShootingPoints(m_CurrentShooterPoints / MaxShooterPoints, m_PlayerIndex, GetShooterPointsAmountText(), m_LevelsNumbering[m_XPPointsLevelIndex]);
            GetComponent<Shooter>().UserIndex = m_PlayerIndex;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                AddXPPoints(1000f);
                m_Components.Coins.AddCoins(1000);
            }
        }

        #region Setting and resetting
        public override void SetDamage(float _damage)
        {
            base.SetDamage(_damage);

            m_Components.ShipShooter.Damage = _damage;
        }

        public void ResetStats()
        {
            m_Components.Health.ResetHealth();

            // shooting points
            // reset the shooting points
            //m_CurrentShooterPoints = 0f;
            // reset the level of the shooter
            //m_XPPointsLevelIndex = 0;
            // reset the max shooting points
            //MaxShooterPoints = PlayerLevelUp.Instance.ShooterPointsLevels[m_XPPointsLevelIndex];
            // set UI
            //GameManager.i.PlayerHUDnstance.UpdatePlayerShootingPoints(m_CurrentShooterPoints / MaxShooterPoints, PlayerIndex, GetShooterPointsAmountText(), m_LevelsNumbering[m_XPPointsLevelIndex]);
            // reset the weapon of the shooter to the first weapon
            //GameManager.Instance.PlayersManager.UpgradePlayerShooting(m_XPPointsLevelIndex, PlayerIndex);
        }
        #endregion

        #region Health

        #region Shooter Points Functions
        public void AddXPPoints(float _points)
        {
            if (m_XPPointsLevelIndex >= PlayerLevelUp.i.ShooterPointsLevels.Count)
            {
                return;
            }

            m_CurrentShooterPoints += _points;

            // if we level up
            if (m_CurrentShooterPoints >= MaxShooterPoints && m_XPPointsLevelIndex <= PlayerLevelUp.i.ShooterPointsLevels.Count - 1)
            {
                m_Components.PlayLevelUpParticles();
                m_XPPointsLevelIndex++;
                // for stats
                GameStatsManager.i.AddStats(1, m_PlayerIndex, StatTag.LevelUps);
                GameStatsManager.i.SetMaxStat(m_XPPointsLevelIndex, m_PlayerIndex, StatTag.HighestLevel);
                m_CurrentShooterPoints -= MaxShooterPoints;

                if (m_XPPointsLevelIndex <= PlayerLevelUp.i.ShooterPointsLevels.Count - 1)
                {
                    MaxShooterPoints = PlayerLevelUp.i.ShooterPointsLevels[m_XPPointsLevelIndex];
                }
                else
                {
                    m_CurrentShooterPoints = MaxShooterPoints;
                }

                // level up
                GameManager.i.PlayersManager.UpgradePlayerShooting(m_XPPointsLevelIndex, m_PlayerIndex);

                // just in case the amount was enough to level up twice which would only occur if the game was hacked
                if (m_CurrentShooterPoints >= MaxShooterPoints)
                {
                    AddXPPoints(0);
                }
            }

            GameManager.i.HUDManager.UpdatePlayerShootingPoints(m_CurrentShooterPoints / MaxShooterPoints, m_PlayerIndex, GetShooterPointsAmountText(), m_LevelsNumbering[m_XPPointsLevelIndex]);
        }

        private string GetShooterPointsAmountText()
        {
            return $"{m_CurrentShooterPoints:00} / {MaxShooterPoints:00}";
        }
        #endregion

        #region Score
        public void AddScore(float _score)
        {
            m_ShipsDestroyed++;
            m_Score += _score;
            GameManager.i.HUDManager.UpdateScoreText(m_Score, m_PlayerIndex);
        }
        #endregion

        #region Lives


        public void SetStartingLives(int _lives)
        {
            m_Lives = _lives;
            GameManager.i.HUDManager.SetLivesCount(m_Lives, m_PlayerIndex);
        }

        public override void OnDeath()
        {
            base.OnDeath();
            // deaths is only for stats
            m_Deaths++;
            GameManager.i.ShopManager.StopAggressiveMode();
            m_Components.Effects.PlayDeathParticles();
            SpawnAndKillManager.i.ShipDeath(m_Components.Effects.GetSprites(), transform);
            // play the explosion particles using the method overload that takes size into account to resize the explosion
            SwitchVisuals(false);
            m_EnableComponents = false;
            SwitchComponents();
            StartCoroutine(AddLiveAfterTime(SpawnAndKillManager.SHRINK_TIME));
        }

        private IEnumerator AddLiveAfterTime(float _time)
        {
            yield return new WaitForSeconds(_time);
            AddLives(-1);
        }

        public void AddLives(int _lives)
        {
            m_Lives += _lives;
            GameManager.i.HUDManager.SetLivesCount(m_Lives, m_PlayerIndex);

            // if there are no lives left then the player is dead, otherwise, it revives
            if (m_Lives < 0)
            {
                // game over 
                m_Components.IsAlive = false;
                // stop regeneration
                m_Components.Health.StopRegeneration();
                m_Components.gameObject.SetActive(false);
                // StopCoroutine(m_RegeneratingCoroutine);

                // Game over for both players
                if (GameManager.i.PlayersManager.AllPlayersDead())
                {
                    GameManager.i.GameOver();
                }
            }
            else
            {
                // revive the player
                // reset values
                SwitchVisuals(true);
                ResetStats();
                m_EnableComponents = true;
                Invoke(nameof(SwitchComponents), SpawnAndKillManager.i.GetRevivingDuration());
                // enable graphics
                SpawnAndKillManager.i.RevivePlayerShip(m_Components.Effects.GetSprites(), transform, m_Scale, m_PlayerIndex);
                // animate the player back in
                SpawnAndKillManager.i.MovePlayerToSpawnPosition(m_Components);
            }
        }

        public void SetLives(int livesCount)
        {
            m_Lives = livesCount;
        }

        private void SwitchVisuals(bool _enable)
        {
            m_Components.Effects.DamageRenderers[0].PartRenderer.transform.parent.gameObject.SetActive(_enable);

            if (!_enable)
            {
                m_Components.Effects.Thrusters.ForEach(t => t.Stop());
            }
            else
            {
                m_Components.Effects.Thrusters.ForEach(t => t.Play());
            }
        }

        private void SwitchComponents()
        {
            CanTakeDamage = m_EnableComponents;
            m_Components.ShipStats.enabled = m_EnableComponents;
            m_Components.ShipShooter.enabled = m_EnableComponents;
            m_Components.ThePlayerShooting.CanShoot = m_EnableComponents;
            m_Components.Movement.Enable(m_EnableComponents);
            m_Components.Movement.enabled = m_EnableComponents;
            m_Components.Abilities.enabled = m_EnableComponents;
            m_Components.EnableColliders(m_EnableComponents);
        }

        public void SetGameStatsValues()
        {
            GameStatsManager.i.SetStat((int)m_Score, m_PlayerIndex, StatTag.Score);
            GameStatsManager.i.SetStat(m_Deaths, m_PlayerIndex, StatTag.Deaths);
            // this is currently inaccurate as the player can spend forever selecting an ability
            GameStatsManager.i.SetStat((int)Time.time, m_PlayerIndex, StatTag.Time);
            GameStatsManager.i.SetStat(m_ShipsDestroyed, m_PlayerIndex, StatTag.ShipsDestroyed);
        }
        #endregion

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Collectable collectable))
            {
                collectable.OnTriggerAction(m_Components);
                return;
            }
        }

        #region IController
        public override void Activate()
        {
            base.Activate();

        }

        public override void Deactivate()
        {
            base.Deactivate();

        }

        public override void Dispose()
        {
            base.Dispose();

        }
        #endregion
    }
    #endregion
}