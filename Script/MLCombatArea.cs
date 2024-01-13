using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using System.Linq;
using TMPro;
using static Unity.Collections.AllocatorManager;
using Unity.MLAgentsExamples;
using Random = UnityEngine.Random;




namespace JV
{
    public class MLCombatArea : MonoBehaviour
    {
        // SET UP IS FOR 2 AGENTS

        public List<MLCombatAgent> agentsList = new List<MLCombatAgent>();


        [SerializeField] CombatEnv combatEnv;
        [SerializeField] BlockPushEnv pushBlockEnv;
        [SerializeField] ExploreEnv exploreEnv;

        [SerializeField] GameObject agent1SpawnArea;
        [SerializeField] GameObject agent2SpawnArea;

        public TextMeshPro brainTextAgent1;
        public TextMeshPro brainTextAgent2;

        public TextMeshPro succeededText1;
        public TextMeshPro succeededText2;
        int succeededNum1 = 0;
        int succeededNum2 = 0; 



        [HideInInspector]
        public Bounds agent1AreaBounds;
        [HideInInspector]
        public Bounds agent2AreaBounds;


        int agentsGoalReached = 0;


        [SerializeField] bool randomizeConfig = true;
        [Range(0, 3)]
        [SerializeField] int configuration;

        void Start()
        {
            //pushBlockEnv.Init(agentsList);


            //agent1AreaBounds = agent1SpawnArea.GetComponent<Collider>().bounds;
            //agent2AreaBounds = agent2SpawnArea.GetComponent<Collider>().bounds;


            if (randomizeConfig)
            {
                configuration = Random.Range(0, 3);
            }


            succeededText1.text = succeededNum1.ToString();
            succeededText2.text = succeededNum2.ToString();
        }


        public void SetBrainText(string text, MLCombatAgent agent )
        {
            if ( agent == agentsList[0])
            {
                brainTextAgent1.text = text;
            }
            else
            {
                brainTextAgent2.text = text;
            }
        }



        public void LoadPushBlockEnv()
        {
            //Deactivate
            agentsGoalReached = 0;
            exploreEnv.DeactivateEnv();
            combatEnv.DeactivateEnv();

            //Activate
            pushBlockEnv.ActivateEnv(agentsList);

            ActivateAgents();

            for (int i = 0; i < agentsList.Count; i++)
            {
                agentsList[i].yPos = 2f;
            }
        }

        public void LoadCombatEnv()
        {
            //Deactivate
            exploreEnv.DeactivateEnv();
            pushBlockEnv.DeactivateEnv();
            agentsGoalReached = 0;

            //Activate
            combatEnv.ActivateEnv(agentsList);

            ActivateAgents();

            for (int i = 0; i < agentsList.Count; i++)
            {
                agentsList[i].yPos = 0f;
            }

        }

        public void LoadExploreEnv()
        {
            //Deactivate
            combatEnv.DeactivateEnv();
            pushBlockEnv.DeactivateEnv();
            agentsGoalReached = 0;


            //Activate
            exploreEnv.ActivateEnv(agentsList);
            ActivateAgents();

            for (int i = 0; i < agentsList.Count; i++)
            {
                agentsList[i].yPos = 0f;
            }
        }

        public void OnAgentDeath(MLCombatAgent agent)
        {
            agent.gameObject.SetActive(false);

            if (agent == agentsList[0])
            {
                succeededNum2++;
                succeededText2.text = succeededNum2.ToString();
            }
            else
            {
                succeededNum1++;
                succeededText1.text = succeededNum1.ToString();
            }

            ResetScene();
        }


        public void ResetScene()
        {
            for (int i = 0; i < agentsList.Count; i++)
            {
                agentsGoalReached = 0;
                agentsList[i].gameObject.SetActive(true);
                agentsList[i].EndEpisode();
            }
        }

        public void ActivateAgents()
        {
            for (int i = 0; i < agentsList.Count; i++)
            {
                agentsList[i].gameObject.SetActive(true);
            }
        }



        public Vector3 GetRandomSpawnPos(Bounds areaBounds)
        {
            var randomSpawnPos = Vector3.zero;

            var randomPosX = Random.Range(-areaBounds.extents.x, areaBounds.extents.x);

            var randomPosZ = Random.Range(-areaBounds.extents.z, areaBounds.extents.z);

            randomSpawnPos = areaBounds.center + new Vector3(randomPosX, 5f, randomPosZ);

            return randomSpawnPos;
        }



        public void GoalCompletedOrFailed(MLCombatAgent agent, bool succeeded)
        {

            //agentsGoalReached++;

            //if (agentsGoalReached == 2)
            //{
            //    ResetScene();
            //    agentsGoalReached = 0;
            //    return;
            //}


            if (agent == agentsList[0])
            {
                if (succeeded)
                {
                    succeededNum1++;
                    succeededText1.text = succeededNum1.ToString();
                    agent.EndEpisode();
                }
            }
            else
            {
                if (succeeded)
                {
                    succeededNum2++;
                    succeededText2.text = succeededNum2.ToString();
                    agent.EndEpisode();
                }
            }

            //agent.gameObject.SetActive(false);
        }

        public void SetUpScene()
        {

            if (randomizeConfig)
            {
                configuration = Random.Range(0, 3);
            }

            ConfigureEnvironment(configuration);

            //ResetScene();
        }






        void ConfigureEnvironment(int config)
        {
            //set up the enviromnent cooresponding to the brain you want to train
            if (config == 0)
            {
                LoadExploreEnv();
            }
            else if (config == 1)
            {
                LoadPushBlockEnv();
            }
            else
            {
                LoadCombatEnv();
            }
        }
    }
}
