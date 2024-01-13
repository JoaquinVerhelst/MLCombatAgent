using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace JV
{
    public class CombatEnv : MonoBehaviour
    {

        [SerializeField] GameObject middleWall;
        [SerializeField] GameObject env;
        [SerializeField] GameObject trainEnv;


        public void ActivateEnv(List<MLCombatAgent> agentList)
        {
            middleWall.SetActive(false);
            env.SetActive(true);


            agentList[0].transform.localPosition = GetAgentRandomSpawnPos();
            agentList[1].transform.localPosition = GetAgentRandomSpawnPos();

        }

        public void DeactivateEnv()
        {
            middleWall.SetActive(true);
            env.SetActive(false);
        }



        public Vector3 GetAgentRandomSpawnPos()
        {
            var randomSpawnPos = Vector3.zero;

            var randomPosX = Random.Range(-6f, 6f);
            var randomPosZ = Random.Range(-6f, 6f);

            randomSpawnPos = new Vector3(randomPosX, 5f, randomPosZ);

            return randomSpawnPos;
        }
    }
}