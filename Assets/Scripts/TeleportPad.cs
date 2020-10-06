using System;
using UnityEngine;

namespace DefaultNamespace {
    public class TeleportPad : MonoBehaviour {
        public GameObject TargetLocation;
        public BoxCollider2D boxCollider2D;
        private Collider2D[] interactables;
        public bool isHorizontal;

        private void OnTriggerEnter2D(Collider2D obj) {
            if (obj.gameObject.GetComponent<CharacterController2D>())
                obj.transform.position = TargetLocation.transform.position +
                                         (obj.transform.position - gameObject.transform.position);

        }
    }
}