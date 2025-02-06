using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SpaceWar
{
    public class GameStatsManager : Singlton<GameStatsManager>
    {
        [System.Serializable]
        public struct EndGameStats
        {
            public List<Stat> Stats;
        }

        //public List<EndGameStats> PlayersStats;
        public List<StatsReferences> PlayerStatsReferences;
        private const float STAT_FILLING_TIME = 1f;
        private const float STAT_TEXT_SHOW_TIME = 0.5f;
        [Header("Stats menu buttons")]
        [SerializeField] private TextMeshProUGUI m_RetryText;
        [SerializeField] private TextMeshProUGUI m_QuitText;

        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < PlayerStatsReferences.Count; i++)
            {
                for (int j = 0; j < PlayerStatsReferences[i].Stats.Count; j++)
                {
                    FillStat(0f, PlayerStatsReferences[i].Stats[j]);
                }
            }
        }

        #region Display Info
        public void DisplayStats()
        {
            ScreenTitlesManager.Instance.ShowStats(true);

            for (int i = 0; i < GameManager.i.PlayersManager.Players.Count; i++)
            {
                //InputController.Instance.SwitchControls(i, InputState.StatsMenu);
            }

            for (int i = 0; i < GameManager.i.PlayersManager.Players.Count; i++)
            {
                StartCoroutine(DisplayStatsProcess(PlayerStatsReferences[i].Stats));
            }

            // set the input keys to the buttons
            //List<string> keys = GameManager.i.InputController.GetStatsMenuButtonsKeys();
            //m_RetryText.text = $"Retry [{keys[0]}]";
            //m_QuitText.text = $"Exit [{keys[1]}]";
        }

        private IEnumerator DisplayStatsProcess(List<Stat> _stats)
        {
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < _stats.Count; i++)
            {
                float time = 0f;
                float amount = 0;
                float processTime = STAT_FILLING_TIME;

                // fade the stat text in
                while (time < STAT_TEXT_SHOW_TIME)
                {
                    time += Time.deltaTime;
                    _stats[i].SetAlpha(Mathf.Lerp(0f, 1f, time / STAT_TEXT_SHOW_TIME));
                    yield return null;
                }

                time = 0f;

                // if the stat value is zero, then skip the process of adding the value gradually by setting the process time to 0
                if (_stats[i].Value == 0)
                {
                    processTime = 0f;
                }

                while (time < processTime)
                {
                    time += Time.deltaTime;
                    amount += _stats[i].Value * Time.deltaTime;
                    // use a function because the way we add a value to time is different from the other values
                    FillStat(amount, _stats[i]);
                    yield return null;
                }

                yield return null;
            }
        }

        private void FillStat(float _amount, Stat _stat)
        {
            if (_stat.Tag == StatTag.Time)
            {
                _stat.SetValue(Helper.GetTimeValue(_amount));
            }
            else
            {
                _stat.SetValue(_amount.ToString("0"));
            }
        }
        #endregion

        #region Values Setters
        public void AddStats(int _amount, int _playerIndex, StatTag _statTag)
        {
            PlayerStatsReferences[_playerIndex].Stats.Find(s => s.Tag == _statTag).Value += _amount;
        }

        public void SetMaxStat(int _amount, int _playerIndex, StatTag _statTag)
        {
            if (_amount > PlayerStatsReferences[_playerIndex].Stats.Find(s => s.Tag == _statTag).Value)
            {
                PlayerStatsReferences[_playerIndex].Stats.Find(s => s.Tag == _statTag).Value = _amount;
            }
        }

        public void SetStat(int _amount, int _playerIndex, StatTag _statTag)
        {
            PlayerStatsReferences[_playerIndex].Stats.Find(s => s.Tag == _statTag).Value = _amount;
        }
        #endregion
    }

    public enum StatTag
    {
        Score, Deaths, ShipsDestroyed, LevelUps, HighestLevel, Time
    }
}
