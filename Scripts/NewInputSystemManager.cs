using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceWar
{
    public class NewInputSystemManager : MonoBehaviour
    {
        private PlayerMovement[] m_Movements;
        private PlayerShooting[] m_Shootings;
        private bool m_Assigned = false;
        private int m_PlayersNumber;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);

            var players = GameManager.i.PlayersManager.Players;
            m_PlayersNumber = players.Count;
            m_Movements = new PlayerMovement[m_PlayersNumber];
            m_Shootings = new PlayerShooting[m_PlayersNumber];

            for (int i = 0; i < players.Count; i++)
            {
                m_Movements[i] = players[i].Components.Movement;
                m_Shootings[i] = players[i].Components.ThePlayerShooting;
            }

            m_Assigned = true;
        }

        public void SetUp()
        {

        }

        private void Update()
        {
            for (int i = 0; i < m_PlayersNumber; i++)
            {
                movementProcess(i);
                shootingProcess(i);
            }
        }

        private void movementProcess(int _playerIndex)
        {
            if (!m_Assigned)
            {
                return;
            }

            //if (_playerIndex == 0)
            //{
            //    Vector2 direction = m_PlayerInput.Player1Movement.Movement.ReadValue<Vector2>();
            //    m_Movements[0].Move(direction);
            //}
            //else
            //{
            //    Vector2 direction = m_PlayerInput.Player2Movement.Movement.ReadValue<Vector2>();
            //    m_Movements[1].Move(direction);
            //}
        }

        private void shootingProcess(int _playerIndex)
        {
            //if (!m_Assigned)
            //{
            //    return;
            //}

            //if (_playerIndex == 0)
            //{
            //    float shoot = m_PlayerInput.Player1Movement.Shoot.ReadValue<float>();

            //    if(shoot == 1f)
            //    m_Shootings[0].Shoot();
            //}
            //else
            //{
            //    float shoot = m_PlayerInput.Player2Movement.Shoot.ReadValue<float>();

            //    if (shoot == 1f)
            //        m_Shootings[1].Shoot();
            //}
        }
    }
}