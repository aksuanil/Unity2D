using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] private bool combatEnabled;
    [SerializeField] private float inputTimer, attack1Radius, attack1Damage;
    [SerializeField] private Transform attack1HitBoxPos;
    [SerializeField] private LayerMask Damageable;


    private PlayerController controllerScript;
    public bool gotInput;
    public bool isAttacking;
    private bool isFirstAttack;

    private float lastInputTime;

    private Animator anim;

    private void Start() 
{   
    anim = GetComponent<Animator>();
}
    private void Update() 
    {
        CheckCombatInput();
        CheckAttacks();
        controllerScript.ChangeAnimationState("Attack1");
    }
    
private void CheckCombatInput() 
{
    if(Input.GetButtonDown("Fire1"))
    {
        if(combatEnabled)
        {
            gotInput = true;
            lastInputTime = Time.time; 
        }
    }
}
public void CheckAttacks()
{
    if(gotInput)
    {   
        if(!isAttacking)
        {   
            anim.Play("Attack1");
            gotInput = false;
            isAttacking = true;
            isFirstAttack = !isFirstAttack;

        }
        if(Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }
    }
}

private void CheckAttackHitBox()
{
    Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, Damageable);

    foreach (Collider2D collider in detectedObjects)
    {
        collider.transform.parent.SendMessage("Damage",attack1Damage);
    }
}

public void FinistAttack1()
    {
        isAttacking = false;

    }

private void OnDrawGizmos() 
{
    Gizmos.DrawWireSphere(attack1HitBoxPos.position,attack1Radius);
}

}


