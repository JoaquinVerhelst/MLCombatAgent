using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace JV
{
    public class GoalDetectCombatAgent : MonoBehaviour
    {

        [HideInInspector]
        public MLCombatAgent agent;

        [SerializeField] public GameObject goal;


        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag("goal"))
            {
                agent.BlockPushedInGoal();
            }
        }
    }
}