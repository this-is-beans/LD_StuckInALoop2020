﻿using System;
using UnityEngine;

namespace DefaultNamespace {
    public class TeleportPad : MonoBehaviour {
        public GameObject TargetLocation;
        public BoxCollider2D boxCollider2D;
        private Collider2D[] interactables;

        private void OnTriggerEnter2D(Collider2D obj) {
            obj.transform.parent.transform.position = TargetLocation.transform.position;
        }
    }
}