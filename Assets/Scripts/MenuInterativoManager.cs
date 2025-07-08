using UnityEngine;

public class MenuInterativoManager : MonoBehaviour
{
    [Header("Configuração da Tela Interativa")]
    public GameObject telaInterativaPanel; // Arraste seu GameObject "TelaInterativa" (o painel principal) para cá
    public KeyCode exitKey = KeyCode.Space; // Tecla para sair da tela interativa

    private bool isTelaInterativaOpen = false;
    private MovimentoJogador playerMovementScript; // Referência ao script de movimento do jogador

    void Start()
    {
        // Garante que a TelaInterativa comece desativada
        if (telaInterativaPanel != null)
        {
            telaInterativaPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Tela Interativa Panel não atribuída no MenuInterativoManager!");
        }

        // Encontra o script de movimento do jogador na cena
        playerMovementScript = FindFirstObjectByType<MovimentoJogador>();
        if (playerMovementScript == null)
        {
            Debug.LogError("Script MovimentoJogador não encontrado na cena! Não será possível desativar o movimento.");
        }

        // Garante que o cursor esteja travado e invisível no início do jogo (estado padrão de gameplay)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Lógica para sair da tela interativa com a tecla de saída
        if (isTelaInterativaOpen && Input.GetKeyDown(exitKey))
        {
            CloseTelaInterativa();
        }

        // --- Gerenciamento de Estado do Jogo (Pausa, Cursor, Movimento do Jogador) ---
        if (isTelaInterativaOpen)
        {
            // Se a tela interativa estiver aberta:
            Cursor.lockState = CursorLockMode.None; // Libera o cursor para interagir com a UI
            Cursor.visible = true; // Torna o cursor visível
            Time.timeScale = 0f; // Pausa o tempo do jogo (tudo para, exceto a UI que não é afetada por timeScale)

            if (playerMovementScript != null)
            {
                playerMovementScript.SetMovementEnabled(false); // Desativa o movimento do jogador
            }
        }
        else
        {
            // Se a tela interativa estiver fechada:
            Cursor.lockState = CursorLockMode.Locked; // Trava o cursor
            Cursor.visible = false; // Esconde o cursor
            Time.timeScale = 1f; // Retoma o tempo do jogo

            if (playerMovementScript != null)
            {
                playerMovementScript.SetMovementEnabled(true); // Reativa o movimento do jogador
            }
        }
    }

    // --- Métodos Públicos para Abrir/Fechar a Tela (chamados por outros scripts) ---

    // Método para ser chamado por um botão ou outro script (como seu BotãoScript) para ABRIR a tela
    public void OpenTelaInterativa()
    {
        if (!isTelaInterativaOpen) // Só abre se já não estiver aberta
        {
            isTelaInterativaOpen = true;
            telaInterativaPanel.SetActive(true);
            Debug.Log("Tela Interativa Aberta.");
            // O resto da lógica (cursor, tempo, movimento) será tratado no Update
        }
    }

    // Método para ser chamado por um botão ou outro script (ou a tecla de saída) para FECHAR a tela
    public void CloseTelaInterativa()
    {
        if (isTelaInterativaOpen) // Só fecha se já estiver aberta
        {
            isTelaInterativaOpen = false;
            telaInterativaPanel.SetActive(false);
            Debug.Log("Tela Interativa Fechada.");
            // O resto da lógica será tratado no Update
        }
    }

}