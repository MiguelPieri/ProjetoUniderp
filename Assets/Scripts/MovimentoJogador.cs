using UnityEngine;

public class MovimentoJogador : MonoBehaviour
{
    // --- Referências de Componentes ---
    private CharacterController controller;
    private Transform myCamera; // Referência à câmera principal
    private Animator animator;
    private CapsuleCollider col; // Continua sendo usada para a verificação de chão mais detalhada

    // --- Variáveis de Movimento ---
    public float VelMovimento = 5f; // Velocidade horizontal do personagem
    public float ForçaPulo = 8f; // Força do pulo
    public float Gravidade = -9.81f; // Valor padrão da gravidade. Ajuste se quiser
    private float velocidadeVertical; // Controla a velocidade Y para pulo e gravidade

    // --- Controle de Movimento ---
    private bool canMove = true; // Flag para controlar se o jogador pode se mover
    private UnityEngine.Vector3 movimentoHorizontal; // Declarar aqui para ser acessível em ambos os blocos do if/else

    // --- Verificação de Chão ---
    public LayerMask GroundLayer; // Camada que representa o chão. Configure no Inspector.
    // Variáveis para ajuste fino da cápsula de checagem do chão (opcional, se TaParado() for usada)
    [Tooltip("Offset vertical da base do collider para os pontos da cápsula de checagem do chão.")]
    public float groundCheckOffset = 0.1f;
    [Tooltip("Multiplicador do raio do collider do personagem para o raio da cápsula de checagem do chão.")]
    [Range(0.1f, 1.0f)] // Limita o valor entre 0.1 e 1.0
    public float groundCheckRadiusMultiplier = 0.9f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>(); // Obtém o CapsuleCollider do próprio GameObject

        // Encontra a câmera principal. Certifique-se de que sua câmera no Unity tem a tag "MainCamera"
        myCamera = Camera.main.transform;

        // Verificações para garantir que os componentes existem
        if (controller == null)
        {
            Debug.LogError("CharacterController não encontrado! Adicione um CharacterController ao seu personagem.");
        }
        if (animator == null)
        {
            Debug.LogWarning("Animator não encontrado! Animações não serão reproduzidas.");
        }
        if (col == null)
        {
            Debug.LogError("CapsuleCollider não encontrado! Adicione um CapsuleCollider ao seu personagem.");
        }
        if (myCamera == null)
        {
            Debug.LogError("Câmera principal (com a tag 'MainCamera') não encontrada!");
        }
    }

    void Update()
    {
        bool estaNoChao = controller.isGrounded; // Verificação de chão sempre é feita

        // --- Lógica de Input e Movimento Horizontal (Condicional a 'canMove') ---
        if (canMove)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            movimentoHorizontal = new UnityEngine.Vector3(horizontal, 0, vertical);
            movimentoHorizontal = myCamera.TransformDirection(movimentoHorizontal);
            movimentoHorizontal.y = 0; // Garante que o movimento horizontal não afete a altura
            movimentoHorizontal.Normalize(); // Normaliza para evitar velocidade diagonal maior
            movimentoHorizontal *= VelMovimento; // Aplica a velocidade de movimento

            // --- Rotação do Personagem ---
            if (movimentoHorizontal.magnitude > 0.1f) // Usa magnitude para evitar rotação quando parado
            {
                UnityEngine.Quaternion targetRotation = UnityEngine.Quaternion.LookRotation(movimentoHorizontal);
                transform.rotation = UnityEngine.Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10);
            }

            // --- Animação de Movimento ---
            if (animator != null)
            {
                // Define o parâmetro de animação "MovFrente" (para andar/correr)
                animator.SetBool("MovFrente", movimentoHorizontal.magnitude > 0.1f); // True se estiver se movendo
            }

            // --- Lógica de Pulo (Condicional a 'canMove' e 'estaNoChao') ---
            if (estaNoChao && Input.GetKeyDown(KeyCode.Space))
            {
                velocidadeVertical = ForçaPulo; // Define a velocidade vertical para o pulo
                if (animator != null)
                {
                    animator.SetTrigger("Jump"); // Dispara o trigger de animação "Jump"
                }
            }
        }
        else // Se canMove for false (player está pausado)
        {
            movimentoHorizontal = UnityEngine.Vector3.zero; // Zera o movimento horizontal
            if (animator != null)
            {
                animator.SetBool("MovFrente", false); // Garante que a animação de movimento pare
            }
            // Não permitimos input de pulo aqui, mas a gravidade continua.
        }

        // --- Lógica de Gravidade (Sempre Aplicada, independentemente de 'canMove') ---
        // Aplica gravidade se não estiver no chão, ou se a velocidade vertical for negativa (caindo)
        // Se estiver no chão, zera a velocidade vertical para evitar acúmulo de gravidade
        if (estaNoChao && velocidadeVertical < 0)
        {
            velocidadeVertical = -0.5f; // Um valor pequeno negativo para garantir que ele "grude" no chão
        }
        else
        {
            velocidadeVertical += Gravidade * Time.deltaTime; // Aplica a gravidade continuamente
        }

        // --- Aplica o Movimento Final ao CharacterController ---
        // Combina o movimento horizontal (que pode ser zero se canMove for false) e a velocidade vertical
        UnityEngine.Vector3 movimentoFinal = movimentoHorizontal;
        movimentoFinal.y = velocidadeVertical; // Adiciona a velocidade vertical

        // Move o CharacterController
        controller.Move(movimentoFinal * Time.deltaTime);
     
    }

    public void SetMovementEnabled(bool enable)
    {
        canMove = enable;
        // Opcional: Se quiser resetar a velocidade horizontal imediatamente ao pausar
        // if (!enable)
        // {
        //     movimentoHorizontal = UnityEngine.Vector3.zero;
        // }
    }


    // --- Função de Verificação de Chão (Opcional, se controller.isGrounded não for suficiente) ---
    // Esta função é mais detalhada e usa Physics.CheckCapsule.
    // Eu a mantive porque você a tinha, mas controller.isGrounded é mais simples para CharacterController.
    // Use uma ou outra, não as duas para a mesma coisa.
    private bool TaParado() // Renomeei para IsGrounded() para melhor clareza.
    {
        if (col == null) return false; // Evita erro se o collider não for encontrado

        // Ponto central da base do Collider do personagem
        UnityEngine.Vector3 colliderBottomCenter = new UnityEngine.Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z);

        // Define os dois pontos para a CheckCapsule
        UnityEngine.Vector3 point1 = colliderBottomCenter + UnityEngine.Vector3.up * groundCheckOffset;
        UnityEngine.Vector3 point2 = colliderBottomCenter - UnityEngine.Vector3.up * groundCheckOffset;

        // O raio da cápsula de checagem
        float checkRadius = col.radius * groundCheckRadiusMultiplier; // Usa o raio do CapsuleCollider

        // Desenha a cápsula no Scene View para depuração
        Debug.DrawLine(point1, point2, Color.cyan);
        Debug.DrawRay(point2, UnityEngine.Vector3.down * 0.05f, Color.red);

        // Realiza a checagem da cápsula com a camada do chão
        return Physics.CheckCapsule(point1, point2, checkRadius, GroundLayer);
    }
}