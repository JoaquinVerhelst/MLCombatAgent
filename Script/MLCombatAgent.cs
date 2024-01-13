using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.Animations.Rigging;
using static Unity.MLAgents.Sensors.RayPerceptionOutput;
using static UnityEditor.Rendering.CameraUI;
using Unity.Sentis;
using Random = UnityEngine.Random;
using Unity.MLAgentsExamples;
using System.Linq;

namespace JV
{
    public class MLCombatAgent : Agent
    {


        public MLCombatAgent enemyTarget;
        public List<GameObject> enemies;
        public List<GameObject> friends;


        // Components
        private RayPerceptionSensorComponent3D[] rayPerceptions;
        private Animator animator;
        private CharacterAnimatorManager characterAnimatorManager;
        private CharacterAttackManager characterAttackManager;
        private CharacterManager characterManager;
        private MLHealthManager healthManager;

        // Variables

        public ModelAsset combatBrain;         // Brain to use when no wall is present
        public ModelAsset pushBlockBrain;         // Brain to use when a jumpable wall is present
        public ModelAsset explorationBrain;         // Brain to use when a wall requiring a block to jump over is present

        //[SerializeField] public int teamIndex = 0;
        [SerializeField] MLCombatArea area;
        [SerializeField] float moveSpeed = 9f;

        const float gravityValue = -9.81f;

        string combatBehaviorName = "Combat";
        string pushBlockBehaviorName = "PushBlock";
        string explorationBehaviorName = "Exploration";

        // bool
        bool hasEnemyTarget = false;
        bool hasBlockTarget = false;
        bool isExploring = false;

        // push block
        [HideInInspector]
        public GameObject blockToPush;


        public float yPos = 0;

        float stepCounterCheck = 0;
        [SerializeField] float explorationCheckDistanceSteps = 100;
        Vector3 previousPos = Vector3.zero;

        protected override void Awake()
        {
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            rayPerceptions = GetComponentsInChildren<RayPerceptionSensorComponent3D>();
            characterAttackManager = GetComponent<CharacterAttackManager>();
            characterManager = GetComponent<CharacterManager>();
            animator = GetComponent<Animator>();
            healthManager = GetComponent<MLHealthManager>();

            //


        }

        public override void Initialize()
        {

            // Update model references if we're overriding
            var modelOverrider = GetComponent<ModelOverrider>();
            if (modelOverrider.HasOverrides)
            {
                combatBrain = modelOverrider.GetModelForBehaviorName(combatBehaviorName);
                combatBehaviorName = ModelOverrider.GetOverrideBehaviorName(combatBehaviorName);

                pushBlockBrain = modelOverrider.GetModelForBehaviorName(pushBlockBehaviorName);
                pushBlockBehaviorName = ModelOverrider.GetOverrideBehaviorName(pushBlockBehaviorName);

                explorationBrain = modelOverrider.GetModelForBehaviorName(explorationBehaviorName);
                explorationBehaviorName = ModelOverrider.GetOverrideBehaviorName(explorationBehaviorName);
            }

            stepCounterCheck = 0;

            // important to be random, otherwise the agent will do the same thing over and over and not learn

            hasEnemyTarget = false;
            hasBlockTarget = false;
            enemyTarget = null;
            blockToPush = null;

            isExploring = false;


            AgentHealthReset();

            area.SetUpScene();

            stepCounterCheck += explorationCheckDistanceSteps;
            previousPos = transform.position;
        }

        private void Update()
        {
            if (StepCount >= MaxStep)
            {
                area.GoalCompletedOrFailed(this, false);
                AddReward(-1f);
            }


            ApplyGravity();


            // Get the observations
            if (hasEnemyTarget)
            {
                //if (enemyTarget.GetComponent<MLHealthManager>().isDeath)
                //{
                //    enemyTarget = null;
                //    hasEnemyTarget = false;
                //}

                return;
            }

            List<RayOutput[]> rayOutputs = new List<RayOutput[]>();

            for (int i = 0; i < rayPerceptions.Length; i++)
            {
                rayOutputs.Add(rayPerceptions[i].RaySensor.RayPerceptionOutput.RayOutputs);
            }


            if (rayOutputs != null)
            {
                for (int i = 0; i < rayOutputs.Count; i++)
                {
                    if (rayOutputs[i] == null)
                        continue;

                    for (int j = 0; j < rayOutputs[i].Length; j++)
                    {

                        //if (rayOutputs[i][j] == null)
                        //    return;


                        GameObject hitObj = rayOutputs[i][j].HitGameObject;

                        if (hitObj == null)
                            continue;

                        if (hitObj == this.gameObject)
                            continue;

                        if (!hasEnemyTarget && hitObj.CompareTag("agent") /*|| hitObj.CompareTag("Player")*/)
                        {

                            enemyTarget = hitObj.GetComponent<MLCombatAgent>();
                            hasEnemyTarget = true;
                            isExploring = false;
                            hasBlockTarget = false;
                            SetModel(combatBehaviorName, combatBrain);
                            area.SetBrainText("Combat", this);
                        }
                        if (!hasBlockTarget && hitObj.CompareTag("block"))
                        {
                            blockToPush = hitObj;
                            hasBlockTarget = true;
                            isExploring = false;
                            SetModel(pushBlockBehaviorName, pushBlockBrain);
                            area.SetBrainText("Block Push", this);
                        }
                    }


                }
            }

            if (!isExploring && !hasEnemyTarget && !hasBlockTarget)
            {

                isExploring = true;
                SetModel(explorationBehaviorName, explorationBrain);
                area.SetBrainText("Explore", this);
                
            }


            if (isExploring)
            {
                if (StepCount >= stepCounterCheck)
                {
                    stepCounterCheck += explorationCheckDistanceSteps;

                    // save current pos
                    float distance = Vector3.Distance(previousPos, transform.position);

                    if (distance < 1.5f)
                    {
                        AddReward(-0.15f);
                    }
                    else
                    {
                        AddReward(distance / 20f);
                    }

                    previousPos = transform.position;

                }



            }
        }
        

        public override void OnEpisodeBegin()
        {

            stepCounterCheck = 0;

            // important to be random, otherwise the agent will do the same thing over and over and not learn

            hasEnemyTarget = false;
            hasBlockTarget = false;
            enemyTarget = null;
            blockToPush = null;

            isExploring = false;


            AgentHealthReset();

           area.SetUpScene();

            stepCounterCheck += explorationCheckDistanceSteps;
            previousPos = transform.position;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Add observations to the sensor  // 3
            sensor.AddObservation(transform.position);

            // direction of this agent // 3
            sensor.AddObservation(transform.forward);


            if (hasEnemyTarget && enemyTarget != null) // 9 total + 3 placeholder
            {
                sensor.AddObservation((enemyTarget.transform.position)); // 3
                sensor.AddObservation(Vector3.Distance(enemyTarget.transform.position, transform.position)); //distance to target // 1

                Vector3 directionToTarget = (enemyTarget.transform.position - transform.position).normalized;
                sensor.AddObservation(directionToTarget); // Direction to target // 3

                // Combat booleans // 2
                sensor.AddObservation(enemyTarget.GetComponent<CharacterAttackManager>().isAttacking);
                sensor.AddObservation(enemyTarget.GetComponent<CharacterAttackManager>().isBlocking);

                sensor.AddObservation(Vector3.zero);
            }
            else if (hasBlockTarget && blockToPush != null) // 10 + 2 placeholder
            {



                sensor.AddObservation(blockToPush.transform.position); // 3

                Vector3 directionToTarget = (blockToPush.transform.position - transform.position).normalized;
                sensor.AddObservation(directionToTarget); // Direction to target // 3

                Vector3 goalPos = blockToPush.GetComponent<GoalDetectCombatAgent>().goal.transform.position;

                Vector3 directionTogoal = (blockToPush.transform.position - goalPos).normalized;
                sensor.AddObservation(directionToTarget); // Direction to target // 3
                //distance bet5ween block and goal // 1
                sensor.AddObservation(Vector3.Distance(blockToPush.transform.position, goalPos));


                sensor.AddObservation(0f); // Placeholder value for combat boolean 1
                sensor.AddObservation(0f); // Placeholder value for combat boolean 2
            }
            else // 12 placeholder
            {
                // Add placeholder values when there is no enemy or block to push
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(0f); // Distance to target // 1
                sensor.AddObservation(Vector3.zero); // Direction to target // 3
                sensor.AddObservation(Vector3.zero); // targts pos // 3

                sensor.AddObservation(0f); // Placeholder value for combat boolean 1
                sensor.AddObservation(0f); // Placeholder value for combat boolean 2
            }

            // 12 + 6 = 18 obs total

        }


        public override void OnActionReceived(ActionBuffers actionBuffers)
        {

            MoveAgent(actionBuffers.DiscreteActions);

            if (hasEnemyTarget)
            {
                Combat(actionBuffers.DiscreteActions);
            }


            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(0.8f, 0.1f, 0.8f));
            if (hitColliders.Length > 0)
            {
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    if (hitColliders[i].CompareTag("wall"))
                    {
                        AddReward(-0.0001f);
                        //Debug.Log("Hitting Wall");
                    }
                }
            }


            AddReward(-1f / MaxStep);

        }


        private void OnCollisionEnter(Collision collision)
        {
            //if (collision.collider.CompareTag("block"))
            //{
            //    AddReward(0.05f);
            //    //Debug.Log("Hitting blovk");
            //}
        }


        public override void Heuristic(in ActionBuffers actionsOut)
        {
            //var continuousActions = actionsOut.ContinuousActions;
            var discreteActions = actionsOut.DiscreteActions;


            if (Input.GetKey(KeyCode.W))
            {
                discreteActions[0] = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                discreteActions[0] = 2;
            }
            if (Input.GetKey(KeyCode.A))
            {
                discreteActions[0] = 3;
            }
            if (Input.GetKey(KeyCode.D))
            {
                discreteActions[0] = 4;
            }
            if (Input.GetKey(KeyCode.E))
            {
                discreteActions[0] = 5;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                discreteActions[0] = 6;
            }

            if (Input.GetKey(KeyCode.R))
            {
                discreteActions[1] = 1;
            }
            else if (Input.GetKey(KeyCode.F))
            {
                discreteActions[1] = 2;
            }




            if (Input.GetKey(KeyCode.L))
            {
                area.SetUpScene();
            }
        }


        private void MoveAgent(ActionSegment<int> act)
        {
            var dirToGo = Vector3.zero;
            var rotateDir = Vector3.zero;

            int forward = 0;
            int right = 0;


            var forwardAxis = act[0];
            //var rightAxis = act[1];
            //var rotateAxis = act[2];

            switch (forwardAxis)
            {
                case 1:
                    dirToGo += transform.forward * moveSpeed;
                    forward = 1;
                    break;
                case 2:
                    dirToGo += transform.forward * -moveSpeed;
                    forward = -1;
                    break;
            //}

            //switch (rightAxis)
            //{
                case 3:
                    dirToGo += transform.right * moveSpeed;
                    right = 1;
                    break;
                case 4:
                    dirToGo += transform.right * -moveSpeed;
                    right = -1;
                    break;
            //}

            //switch (rotateAxis)
            //{
                case 5:
                    rotateDir = transform.up * -2f;
                    break;
                case 6:
                    rotateDir = transform.up * 2f;
                    break;
            }


            transform.Rotate(rotateDir, Time.deltaTime * 100f);

            characterManager.charController.Move(dirToGo * Time.deltaTime);

            characterAnimatorManager.UpdateAnimatorMovementParameters(right, forward, false);



            // discourage doing nothing
            AddReward(-0.0005f);

        }


        private void Combat(ActionSegment<int> act)
        {
            if (act[1] == 1)
            {
                characterAttackManager.Attack();
            }
            if (act[1] == 2)
            {
                characterAttackManager.Block();
            }
            else if (animator.GetBool("IsBlocking"))
            {
                characterAttackManager.isBlocking = false;
                animator.SetBool("IsBlocking", false);
            }
        }


        public void AgentHealthReset()
        {
            healthManager.ResetAgent();
        }

        private void ApplyGravity()
        {
            Vector3 pos = transform.position;
            pos.y = yPos;
            transform.position = pos;
        }

        public void OnHitEnemy()
        {
            AddReward(0.5f);
        }

        public void OnBlockEnemy()
        {
            AddReward(0.5f);
        }

        public void OnGettingHit()
        {
            AddReward(-0.2f);
        }


        public void OnKillEnemy()
        {
           AddReward(1f);
            enemyTarget = null;
            hasEnemyTarget = false;
        }

        public void OnMissEnemy()
        {
            AddReward(-0.01f);
        }

        public void OnDeath()
        {
            AddReward(-0.5f);
            area.OnAgentDeath(this);
            enemyTarget = null;
            hasEnemyTarget = false;
        }



        public void BlockPushedInGoal()
        {
           AddReward(5f);

            blockToPush = null;
            hasBlockTarget = false;

            area.GoalCompletedOrFailed(this,true);
        }

        public void ExplorationGoalFound()
        {
           AddReward(5f);

            area.GoalCompletedOrFailed(this,true);
        }


    }

}