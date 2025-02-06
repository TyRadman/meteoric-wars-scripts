using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public class DialogueManager : Singlton<DialogueManager>
    {
        [SerializeField] private DialogueBox m_DialogueBoxReference;
        [SerializeField] private int m_MaxBoxesNumber = 3;
        private List<DialogueBox> m_DialogueBoxes = new List<DialogueBox>();
        [SerializeField] private float m_DialogueBoxOpeneingDuration = 0.5f;

        protected override void Awake()
        {
            base.Awake();
            CreateDialogueBoxes();
        }

        private void CreateDialogueBoxes()
        {
            for (int i = 0; i < m_MaxBoxesNumber; i++)
            {
                var box = Instantiate(m_DialogueBoxReference, transform);
                m_DialogueBoxes.Add(box);
                box.gameObject.SetActive(false);
            }
        }

        public void TypeMessage(string _message, Vector2 _position, float _duration)
        {
            var box = m_DialogueBoxes.Find(b => !b.gameObject.activeSelf);
            box.gameObject.SetActive(true);
            box.PrintMessage(_message, _position);
            // show the box
            StartCoroutine(ChangeDialogueBoxSizeProcess(box.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), 0f));
            // hide the box
            StartCoroutine(ChangeDialogueBoxSizeProcess(box.transform, new Vector2(1f, 1f), new Vector2(0f, 1f), _duration, true));
        }

        private IEnumerator ChangeDialogueBoxSizeProcess(Transform _box, Vector2 _startSize, Vector2 _endSize, float _delay, bool _deactivate = false)
        {
            yield return new WaitForSeconds(_delay);
            float time = 0f;
            _box.localScale = new Vector2(0f, 1f);
            AnimationCurve curve = GameManager.i.GeneralValues.EntranceMovementSpeedCurve;

            while (time < m_DialogueBoxOpeneingDuration)
            {
                time += Time.deltaTime;
                float t = time / m_DialogueBoxOpeneingDuration;
                _box.localScale = Vector2.Lerp(_startSize, _endSize, curve.Evaluate(t));
                yield return null;
            }

            if (_deactivate) _box.gameObject.SetActive(false);
        }
    }
}