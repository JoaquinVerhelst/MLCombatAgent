using JV;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MLHealthManager : MonoBehaviour
{

    public int currentHealth = 3;

    [SerializeField] int maxHealth;

    public bool isDeath = true;

    private CharacterAnimatorManager characterAnimatorManager;


    // Start is called before the first frame update
    void Awake()
    {
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public bool OnDamage()
    {
        currentHealth -= 1;
        characterAnimatorManager.PlayTargetActionAnimation("OnDamage", true, false, false, true);

      
        if (currentHealth <= 0)
        {
            Death();
            return true;
        }

        return false;
    }


    public void Death()
    { 
        if (GetComponent<MLCombatAgent>() != null)
        {
            GetComponent<MLCombatAgent>().OnDeath();
        }

        //gameObject.SetActive(false);
        isDeath = true;
    }

    public void ResetAgent()
    {
        currentHealth = maxHealth;


        //gameObject.SetActive(true);
    }


}

