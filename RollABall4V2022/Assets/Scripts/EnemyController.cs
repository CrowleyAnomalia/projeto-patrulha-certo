using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public EnemyDataSO enemyData;

    public PatrolRouteManager myPatrolRoute;
    
    private float _moveSpeed;

    private int _maxHealthPoints;

    private GameObject _enemyMesh;

    public float FollowDistance => _followDistance;
    public float _followDistance; //x = distancia minima pro inimigo seguir o jogador

    public float ReturnDistance => _returnDistance;
    public float _returnDistance; //z = distancia maxima pro inimigo seguir o jogador

    public float AttackDistance => _attackDistance;
    public float _attackDistance; //y = distancia minima pro inimigo atacar o jogador

    public float GiveUpDistance => _giveUpDistance;
    public float _giveUpDistance; //w = distancia maxima pro inimigo atacar o jogador

    private int _currentHealthPoints;

    private float _currentMoveSpeed;

    private Animator _enemyFSM;
    // mapeia obstaculo
    private NavMeshAgent _navMeshAgent;

    private SphereCollider _sphereCollider;

    private Transform _playerTransform;

    private Transform _currentPatrolPoint;

    private Transform _nextPatrolPoint;

    private int _currentPatrolIndex;
    

    private void Awake()
    {
        _moveSpeed = enemyData.moveSpeed;
        _maxHealthPoints = enemyData.maxHealthPoints;

        

        _followDistance = enemyData.followDistance;
        _returnDistance = enemyData.returnDistance;
        _attackDistance = enemyData.attackDistance;
        _giveUpDistance = enemyData.giveUpDistance;

        _currentHealthPoints = _maxHealthPoints;
        _currentMoveSpeed = _moveSpeed;

        _enemyFSM = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _sphereCollider = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // primeiro ponto de patrulha
        _currentPatrolIndex = 0;

        _currentPatrolPoint = myPatrolRoute.patrolRoutePoints[_currentPatrolIndex];
    }

    public void Patrulha()
    {
        if(myPatrolRoute.patrolRoutePoints.Count > 0)
        {
            
            //movimenta o inimigo
            _navMeshAgent.destination = myPatrolRoute.patrolRoutePoints[_currentPatrolIndex].position;
            // incrementa o array, indo para a proxima posição
            _currentPatrolIndex++;

            // verifica se chegou a quarta posição
            if(_currentPatrolIndex == myPatrolRoute.patrolRoutePoints.Count
            )
            {
                // seta de volta para a posição zero
                _currentPatrolIndex = 0;
            }
        }
        else
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemyFSM.GetCurrentAnimatorStateInfo(0).IsName("Return"))
        {
            _enemyFSM.SetFloat("ReturnDistance",
                Vector3.Distance(transform.position, _currentPatrolPoint.position));
        }
        // distancia do ponto de patrulha e inimigo
        if (_enemyFSM.GetCurrentAnimatorStateInfo(0).IsName("Patrol"))
        {
            if (_navMeshAgent.remainingDistance < 0.1f)
            {
                Patrulha();
            }
        }
    }

    public void SetSphereRadius(float value)
    {
        _sphereCollider.radius = value;
    }

    public void SetDestinationToPlayer()
    {
        transform.position += (_playerTransform.position - transform.position).normalized * _moveSpeed * Time.deltaTime;
        
        _navMeshAgent.SetDestination(_playerTransform.position);
    }

    public void SetDestinationToPatrol()
    {   //passa a posição do atual ponto do array
        _navMeshAgent.SetDestination(_currentPatrolPoint.position);
        _currentPatrolIndex = 0;
    }

    public void ResetPlayerTransform()
    {
        _playerTransform = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerTransform = other.transform;
            _enemyFSM.SetTrigger("OnPlayerEntered");
            Debug.Log("Jogador entrou na area");
        }
    }

    private IEnumerator DoAttack()
    {
        _meshRenderer.material.color = attackColor;
        _playerTransform.GetComponent<PlayerController>().TakeDamage(10, Transform.position)
        _currentCoolDownTime = AttackCoolDown + 1f;
        yield return new WaitForSeconds(1f);
        _meshRenderer.material.color = cooldownColor;
    }
    private void Attack()
    {
        StartCourotine("DoAttack")
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _enemyFSM.SetTrigger("OnPlayerExited");
            Debug.Log("Jogador saiu da area");
        }
    }
}
