using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float chaseRange = 6f;
    [SerializeField] float turnSpeed = 5f; 
    float distanceToTarget = Mathf.Infinity;
    bool isProvoked = false;
    AudioSource audioSource1;

    // bool m_Play;
    // bool m_ToggleChange;
    EnemyHealth health;
    NavMeshAgent navMeshAgent;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent= GetComponent<NavMeshAgent>();
        health=GetComponent<EnemyHealth>();
        audioSource1=GetComponent<AudioSource>();
        // m_Play = false;
        audioSource1.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToTarget=Vector3.Distance(target.position,transform.position);

        if(distanceToTarget <= chaseRange)
        {
            isProvoked=true;
        }
        else{
            isProvoked=false;
        }

        if(health.IsDead())
        {
            isProvoked=false;
            toggleAudio(false);
            enabled=false;
            navMeshAgent.enabled=false;
            
            
        }

        if(isProvoked)
        {
            EngageTarget();
            if(!audioSource1.isPlaying)
            {
                toggleAudio(true);
            }
        }
        else{
            toggleAudio(false);
        }

        
        
    }

    public void OnDamageTaken()
    {
        isProvoked=true;
    }

    public void toggleAudio(bool cond){
        if(cond==true){
            audioSource1.Play();
        }
        else{
            audioSource1.Stop();
        }
    }

    

    private void EngageTarget()
    {
        FaceTarget();
        if(distanceToTarget>=navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }

        if(distanceToTarget<=navMeshAgent.stoppingDistance)
        {
            AttackTarget();
        }
        
    }

    private void ChaseTarget()
    {
        GetComponent<Animator>().SetBool("Attack",false);
        GetComponent<Animator>().SetTrigger("Move");
        navMeshAgent.SetDestination(target.position);
    }

    private void AttackTarget()
    {
        GetComponent<Animator>().SetBool("Attack",true);
        
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position-transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
        transform.rotation=Quaternion.Slerp(transform.rotation,lookRotation,Time.deltaTime*turnSpeed);
    }

    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
