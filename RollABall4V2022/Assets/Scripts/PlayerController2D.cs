using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    public float initialTime;
    public int coins = 0;

    public float moveSpeed;
    public float maxVelocity;

    public float rayDistance;
    public LayerMask groundLayer;

    public float jumpForce;
    
    private GameControls _gameControls;
    private PlayerInput _playerInput;
    private Camera _mainCamera;
    private Rigidbody2D _rigidbody;

    private Vector2 _moveInput;

    private bool _isGrounded;

    private float _timeRemaining;

    private void Start()
    {
        _timeRemaining = initialTime;
    }

    private void OnEnable()
    {
        // inicializacao de variavel
        _gameControls = new GameControls();

        // Referencias dos componentes no mesmo objeto da unity
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody2D>();

        // Referencia para a camera main guardada na classe Camera
        _mainCamera = Camera.main;

        // atribuindo ao delegate do action triggered no player input
        _playerInput.onActionTriggered += OnActionTriggered;
    }

    private void OnDisable()
    {
        // retirando a atribuicao ao delegate
        _playerInput.onActionTriggered -= OnActionTriggered;
    }

    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        // comparando o nome da action que esta chegando com o nome da action de mover
        if (obj.action.name.CompareTo(_gameControls.Gameplay.Movement.name) == 0)
        {
            // atribuir ao moveInput o valor proveniente do input do jogador como um Vector2
            _moveInput = obj.ReadValue<Vector2>();
        }

        if (obj.action.name.CompareTo(_gameControls.Gameplay.Jump.name) == 0)
        {
            if(obj.performed) Jump();
        }
    }

    private void Move()
    {
        // Adiciona a força no objeto atraves do rigidbody, com intensidade definida por moveSpeed
        _moveInput.y = 0;
        _rigidbody.AddForce( _moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        Move();
        LimitVelocity();
    }

    private void LimitVelocity()
    {
        // pegar a velocidade do player
        Vector2 velocity = _rigidbody.velocity;

        // checar se a velocidade está dentro dos limites nos diferentes eixos
        // limitando o eixo x usando ifs, Abs e Sign
        if (Mathf.Abs(velocity.x) > maxVelocity) velocity.x = Mathf.Sign(velocity.x) * maxVelocity;

        
        // alterar a velocidade do player para ficar dentro dos limites
        _rigidbody.velocity = velocity;
    }
    
    /* Como fazer o jogador pular
     * 1 - Checar se o jogador está no chão
     * -- a - Checar colisão a partir da física (usando os eventos de colisão)
     * -- a - vantagem: fácil de implementar (adicionar uma função que já existe na Unity - OnCollisionEnter)
     * -- a - desvantagem: não sabemos a hora exata que a unity vai chamar essa função (pode ser que o jogador
     * toque no chão e demore alguns frames pra o jogo saber que ele está no chão)
     * -- b - Através do raycast: o---| bolinha vai atirar um raio, o raio vai bater em algum objeto e a gente
     * recebe o resultado dessa colisão.
     * -- b - Podemos usar Layers pra definir quais objetos que o raycast deve checar colisão
     * -- b - vantagem: Resposta da colisão é imediata
     * -- b - desvantagem: Um pouco mais trabalhoso de configurar
     * -- Uma variável bool que vai dizer para o resto do codigo se o jogador estará no chão (true) ou não (false)
     * 2 - Jogador precisa apertar o botão de pulo
     * -- Precisamos configurar o botão a ser utilizado para a ação de pular no nosso Input
     * -- Na função OnActionTriggered precisaremos comparar se a ação recebida tem o mesmo nome da ação de pulo
     * -- Precisamos dizer em qual momento do botão ser apertado queremos executar o pulo (started, canceled, performed)
     * 3 - Realizar o pulo através da física
     * -- Vamos criar uma função que vai realizar o pulo
     * -- Se o personagem estiver no chão, iremos aplicar uma força para cima com uma certa magnitude
     */

    private void Jump()
    {
        if(_isGrounded) _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGround()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer);

        _isGrounded = hit2D.collider != null;
    }

    private void Update()
    {
        _timeRemaining -= Time.deltaTime;
        CheckGameOver();
        CheckGround();
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Vector3.down * rayDistance, Color.yellow);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            coins++;
            PlayerObserverManager.PlayerCoinsChanged(coins);
            CheckVictory();
            Destroy(other.gameObject);
        }
    }

    
    private void CheckVictory()
    {
        if (coins >= 10)
        {
            if (GameManager.Instance.gameState != GameState.Victory)
            {
                GameManager.Instance.CallVictory();
            }
        }
    }

    private void CheckGameOver()
    {
        if (_timeRemaining <= 0)
        {
            if (GameManager.Instance.gameState != GameState.GameOver)
            {
                GameManager.Instance.CallGameOver();
                //GameManager.Instance.LoadEnding();
            } 
        }
    }
}
