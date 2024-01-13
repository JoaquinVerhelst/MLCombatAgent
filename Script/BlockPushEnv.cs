using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace JV
{
    public class BlockPushEnv : MonoBehaviour
    {

        [SerializeField] GameObject pushBlockArea1;
        [SerializeField] GameObject pushBlockArea2;


        [SerializeField] public GameObject blockToPush1;
        [SerializeField] public GameObject blockToPush2;

        [SerializeField] GameObject middleWall;

        [SerializeField] public GameObject trainEnv;

        void Start()
        {
        }

        public void ActivateEnv(List<MLCombatAgent> agentList)
        {
            // Activate the neccesay Env
            pushBlockArea1.SetActive(true);
            pushBlockArea2.SetActive(true);

            middleWall.SetActive(true);

            // Randomize things
            RandomizePushBlockEnvsRotation();
            RandomizeBlockPos();

            agentList[0].transform.localPosition = GetAgentRandomSpawnPos(pushBlockArea1.transform.localPosition);
            agentList[1].transform.localPosition = GetAgentRandomSpawnPos(pushBlockArea2.transform.localPosition);
        }

        public void DeactivateEnv()
        {
            pushBlockArea1.SetActive(false);
            pushBlockArea2.SetActive(false);



        }


        public void Init(List<MLCombatAgent> agentList)
        {
            blockToPush1.GetComponent<GoalDetectCombatAgent>().agent = agentList[0];
            blockToPush2.GetComponent<GoalDetectCombatAgent>().agent = agentList[1];
        }


        public void RandomizeBlockPos()
        {
            blockToPush1.transform.localPosition = GetBlockRandomSpawnPos();
            blockToPush2.transform.localPosition = GetBlockRandomSpawnPos();

            // Reset block velocity back to zero.
            Rigidbody block1 = blockToPush1.gameObject.GetComponent<Rigidbody>();
            Rigidbody block2 = blockToPush2.gameObject.GetComponent<Rigidbody>();

            block1.velocity = Vector3.zero;
            block2.velocity = Vector3.zero;

            // Reset block angularVelocity back to zero.
            block1.angularVelocity = Vector3.zero;
            block2.angularVelocity = Vector3.zero;
        }

        private void RandomizePushBlockEnvsRotation()
        {
            var rotation = Random.Range(0, 4);
            var rotationAngle = rotation * 90f;

            pushBlockArea1.transform.Rotate(new Vector3(0f, rotationAngle, 0f));

            rotation = Random.Range(0, 4);
            rotationAngle = rotation * 90f;

            pushBlockArea2.transform.Rotate(new Vector3(0f, rotationAngle, 0f));
        }

        // I know magic numbers , but areabounds doesnt work, so I only can do this atm
        // ^^^^ I just realized its because I used position instead of localPosition to set the position, too late to change now

        public Vector3 GetBlockRandomSpawnPos()
        {
            var randomSpawnPos = Vector3.zero;

            var randomPosX = Random.Range(-3f, 3f);

            var randomPosZ = Random.Range(-3f, 3f);

            randomSpawnPos = new Vector3(randomPosX, 5f, randomPosZ);

            return randomSpawnPos;
        }


        public Vector3 GetAgentRandomSpawnPos(Vector3 centerPosition)
        {
            var randomSpawnPos = Vector3.zero;

            var randomPosX = Random.Range(-3f, 3f);
            var randomPosZ = Random.Range(-3f, 3f);

            randomSpawnPos = centerPosition + new Vector3(randomPosX, 5f, randomPosZ);

            return randomSpawnPos;
        }



    }
}