using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SpaceWar
{
    public class BGAstroid : AstroidParent
    {
        public SortingGroup Sort;

        private void Update()
        {
            if (!IsActive) return;

            if (transform.position.y <= -LevelDimensions.Instance.LevelHeight - 2f)
            {
                gameObject.SetActive(false);
                IsActive = false;
            }
        }

        public void SetBackGroundData(int _layer, float _size, float _speed)
        {
            Sort.sortingOrder = _layer;
            transform.localScale = Vector3.one * _size;
            Rb.velocity *= _speed;
        }

        public void SetUp(Vector2 _speedRange)
        {
            SpeedRange = _speedRange;
        }
    }
}