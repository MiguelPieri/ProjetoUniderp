using UnityEngine;
// Removendo TMPro, pois não será mais usado
// using TMPro; // Não precisamos mais desta namespace

public class TriggerProximidade : MonoBehaviour
{
    [Header("Configurações de Interação")]
    public string playerTag = "Player"; // Isso é a tag do player, para saber a proximidade
    public KeyCode interactKey = KeyCode.E; // A tecla que o jogador tem que apertar, no caso E

    // --- REMOVIDO: FeedBack Visual (promptUIGameObject e promptTextComponent) ---
    // Pois não queremos mais feedback de texto/UI

    private bool playerIsNearby = false; // Flag para saber se o jogador está na area de interação

    [Header("Ação do Botão")]
    public UnityEngine.Events.UnityEvent OnButtonPress;

    void Start()
    {
        // --- REMOVIDO: Lógica de ativação/desativação da UI no Start ---
        // Não precisamos mais disso
    }

    // Update is called once per frame
    void Update()
    {
        // A lógica de interação principal permanece
        if (playerIsNearby && Input.GetKeyDown(interactKey))
        {   
            Debug.Log($"Pressionado {interactKey} e playerIsNearby é {playerIsNearby}. Chamando PressButton().");// verificação
            PressButton();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Verifica se o Collider que entrou tem a tag de jogador
         Debug.Log($"OnTriggerEnter disparado! Collider: {other.name}, Tag: {other.tag}");  // verificação

        if (other.CompareTag(playerTag))
        {
            playerIsNearby = true; // O jogador está perto
            // --- REMOVIDO: Lógica para mostrar o prompt de UI ---
            // if (promptUIGameObject != null) { promptUIGameObject.SetActive(true); }
            Debug.Log("Jogador entrou na área do botão de interação."); // Apenas log para depuração
            
        }
    }

    // Chamado quando um Collider sai da área do Trigger do botão
    void OnTriggerExit(Collider other)
    {   
        Debug.Log($"OnTriggerExit disparado! Collider: {other.name}, Tag: {other.tag}"); // verificação
        // Verifica se o Collider que saiu tem a tag de jogador

        if (other.CompareTag(playerTag))
        {
            playerIsNearby = false; // O jogador não está mais perto
            // --- REMOVIDO: Lógica para esconder o prompt de UI ---
            // if (promptUIGameObject != null) { promptUIGameObject.SetActive(false); }
            Debug.Log("Jogador saiu da área do botão de interação."); // Apenas log para depuração
            Debug.Log("Jogador entrou na área do botão de interação! playerIsNearby = true");// verificação
        }
    }

    // --- Método que executa a Ação do Botão ---
    void PressButton()
    {
        Debug.Log("Botão Pressionado! Executando ação...");

        if (OnButtonPress != null)
        {
            OnButtonPress.Invoke();
            Debug.Log("UnityEvent OnButtonPress invocado!"); // verificação
        }
        else
        {
            Debug.LogWarning("UnityEvent OnButtonPress é nulo ou não tem ações configuradas!");// verificação
        }

    }

    // Exemplo de função que pode ser chamada pelo UnityEvent (opcional)
    public void MensagemDeTeste()
    {
        Debug.Log("Ação de teste do botão realizada! (Sem UI de prompt)");
    }
}