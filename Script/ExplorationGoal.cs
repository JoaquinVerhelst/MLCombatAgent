using System.Collections;
using System.Collections.Generic;
using UnityEngine;





namespace JV
{
    public class ExplorationGoal : MonoBehaviour
    {

        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag("agent"))
            {
                col.gameObject.GetComponent<MLCombatAgent>().ExplorationGoalFound();

            }
        }
    }
}