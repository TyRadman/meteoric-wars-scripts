using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceWar
{
    public class GameEditor : EditorWindow
    {
        private int m_PlayersNumber;
        private int m_BuildIndex = 0;
        private GameManager m_GameManager;

        [MenuItem("Meteoric Wars/Game Editor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(GameEditor), false, "Game Editor");
        }

        private void OnGUI()
        {
            m_GameManager = FindObjectOfType<GameManager>();

            // header
            RenderHeader("Game Editor", 30);

            #region Players
            GUILayout.BeginVertical(GUI.skin.box);
            RenderHeader("Players", 20, 10);
            GUILayout.BeginHorizontal();
            RenderPlayersNumber();
            RenderPlayersCharacter();
            RenderStartCoins();
            GUILayout.EndHorizontal();
            GUILayout.Label("", GUILayout.Height(10));
            GUILayout.EndVertical();
            GUILayout.Space(20);
            #endregion

            #region Level
            GUILayout.BeginVertical(GUI.skin.box);
            RenderHeader("Level", 20, 10);
            GUILayout.BeginHorizontal();
            RenderSelectingScenes();
            GUILayout.EndHorizontal();
            GUILayout.Label("", GUILayout.Height(10));
            GUILayout.EndVertical();
            GUILayout.Space(20);
            #endregion

            GUILayout.BeginVertical();

            #region Enemies
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUI.skin.box);
            RenderHeader("Enemies", 20, 10);
            GUILayout.BeginHorizontal();
            RenderEnemiesModifier();
            GUILayout.EndHorizontal();
            GUILayout.Label("", GUILayout.Height(10));
            GUILayout.EndVertical();
            #endregion

            #region Astroids
            GUILayout.BeginVertical(GUI.skin.box);
            RenderHeader("Astroids", 20, 10);
            GUILayout.BeginHorizontal();
            RenderAstroidsModifiers();
            GUILayout.EndHorizontal();
            GUILayout.Label("", GUILayout.Height(10));
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Label("", GUILayout.Height(10));
            GUILayout.EndVertical();
        }

        private void RenderAstroidsModifiers()
        {
            GUILayout.Label("Spawn Astroids");
            m_GameManager.AstroidSpawner.EnableSpawnAggressiveAstroids(EditorGUILayout.Toggle(m_GameManager.AstroidSpawner.SpawnAgressiveAstroids));

            GUI.enabled = m_GameManager.AstroidSpawner.SpawnAgressiveAstroids;
            // whether to spawn custom waves or procedural waves
            GUILayout.Label("Spawn Rate Range");
            m_GameManager.AstroidSpawner.SetSpawnFrequencyRange(EditorGUILayout.Vector2Field("", m_GameManager.AstroidSpawner.GetSpawnFrequencyRange()));
            EditorUtility.SetDirty(m_GameManager.AstroidSpawner);
        }

        private void RenderEnemiesModifier()
        {
            GUILayout.Label("Spawn Enemies");
            m_GameManager.SpawnEnemies = EditorGUILayout.Toggle(m_GameManager.SpawnEnemies);

            GUI.enabled = m_GameManager.SpawnEnemies;
            // whether to spawn custom waves or procedural waves
            GUILayout.Label("Spawn Custom waves");
            m_GameManager.WavesGenerator.EnableCustomWaves(EditorGUILayout.Toggle(m_GameManager.WavesGenerator.UseCustomWaves));
            // enemies difficulty
            GUILayout.Label("Difficulty");
            m_GameManager.Difficulty = EditorGUILayout.Slider(m_GameManager.Difficulty, 0f, 2f);
            EditorUtility.SetDirty(m_GameManager.WavesGenerator);
            EditorUtility.SetDirty(m_GameManager);
        }

        private void RenderSelectingScenes()
        {
            GUILayout.Label("Select Scene");

            m_BuildIndex = GUILayout.Toolbar(m_BuildIndex, new string[] { "Player Selection", "Gameplay", "Random Ships" });

            if (GUILayout.Button("Load Scene"))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(m_BuildIndex));
                }
            }
        }

        private void RenderPlayersCharacter()
        {
            GameManager gameManager = FindObjectOfType<GameManager>();

            if (gameManager == null)
            {
                return;
            }

            PlayerShipsData data = gameManager.PlayerShipsData;

            if (data == null)
            {
                return;
            }

            GUILayout.Space(10);

            for (int i = 0; i < 2; i++)
            {
                if(i > m_PlayersNumber - 1)
                {
                    GUI.enabled = false;
                }

                int playerCharacter = data.PlayersCharacters[i];
                string[] shipNames = new string[data.ShipsInfo.Count];

                for (int j = 0; j < shipNames.Length; j++)
                {
                    shipNames[j] = data.ShipsInfo[j].Name;
                }

                GUILayout.Label($"Player {i + 1} character");
                playerCharacter = EditorGUILayout.Popup(playerCharacter, shipNames);
                data.PlayersCharacters[i] = playerCharacter;
                EditorUtility.SetDirty(data);

                if (i > m_PlayersNumber - 1)
                {
                    GUI.enabled = true;
                }
            }
        }

        private void RenderPlayersNumber()
        {
            GameManager gameManager = FindObjectOfType<GameManager>();

            if (gameManager == null)
            {
                return;
            }

            InputControlsCreator controls = gameManager.InputController.ControlsCreator;

            if (controls == null)
            {
                return;
            }

            m_PlayersNumber = controls.PlayersCount - 1;
            GUILayout.Space(10);
            GUILayout.Label("Number of players");
            m_PlayersNumber = GUILayout.Toolbar(m_PlayersNumber, new string[] { "1 Player", "2 Players" }) + 1;
            controls.PlayersCount = m_PlayersNumber;
            EditorUtility.SetDirty(controls);
            GUILayout.Space(10);
        }

        private void RenderStartCoins()
        {

        }

        private void RenderHeader(string title, int fontSize, int beforeSpace = 20, int afterSpace = 20)
        {
            GUILayout.Space(beforeSpace);
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.fontSize = fontSize;
            GUILayout.Label(title, headerStyle);
            GUILayout.Space(afterSpace);
        }
    }
}
