using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace JV
{
    
    public class BlockPush : MonoBehaviour
    {
        [SerializeField]
        private float forceMagnitude = 3f;


        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody rigid = hit.collider.attachedRigidbody;

            if (rigid != null)
            {
                Vector3 forceDir = hit.gameObject.transform.position - transform.position;
                forceDir.y = 0;
                forceDir = forceDir.normalized;

                rigid.AddForceAtPosition(forceDir * forceMagnitude, transform.position, ForceMode.Impulse);


            }
        }



    }
}