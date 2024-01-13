using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;



namespace JV
{
    public class ExploreEnv : MonoBehaviour
    {

        [SerializeField] GameObject exploreEnv;

        [SerializeField] List<GameObject> goals1 = new List<GameObject>();
        [SerializeField] List<GameObject> goals2 = new List<GameObject>();

        [SerializeField] List<GameObject> spawnLoc1 = new List<GameObject>();
        [SerializeField] List<GameObject> spawnLoc2 = new List<GameObject>();

        [SerializeField] GameObject middleWall;

        public void ActivateEnv(List<MLCombatAgent> agentList)
        {
            exploreEnv.SetActive(true);
            middleWall.SetActive(true);

            ActivateRandomGoals();

            SpawnRandomLocation(agentList);
        }

        public void DeactivateEnv()
        {
            exploreEnv.SetActive(false);


            for (int i = 0; i < goals1.Count; i++)
            {
                goals1[i].SetActive(false);

            }
            for (int i = 0; i < goals2.Count; i++)
            {
                goals2[i].SetActive(false);
            }
        }


        private void ActivateRandomGoals()
        {
            int index1 = Random.Range(0, goals1.Count);
            int index2 = Random.Range(0, goals2.Count);

            for (int i = 0; i < goals1.Count; i++)
            {
                goals1[i].SetActive(false);

            }
            for (int i = 0; i < goals2.Count; i++)
            {
                goals2[i].SetActive(false);
            }

            goals1[index1].SetActive(true);
            goals2[index2].SetActive(true);
        }


        private void SpawnRandomLocation(List<MLCombatAgent> agentList)
        {
            int index1 = Random.Range(0, spawnLoc1.Count);
            int index2 = Random.Range(0, spawnLoc2.Count);


            agentList[0].transform.position = spawnLoc1[index1].transform.position;
            agentList[1].transform.position = spawnLoc2[index2].transform.position;
        }




    }
}