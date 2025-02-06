using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public abstract class AstroidParent : MonoBehaviour
    {
        public AstroidSize Size;
        protected Vector2 SpeedRange;
        public bool IsActive = false;
        [SerializeField] protected float RotationSpeed = 5f;
        [SerializeField] protected Rigidbody2D Rb;

        public void MoveAstroid()
        {
            gameObject.SetActive(true);
            IsActive = true;
            Rb.velocity = Vector2.down * SpeedRange.RandomValue();
            Rb.rotation = RotationSpeed;
        }
    }
}