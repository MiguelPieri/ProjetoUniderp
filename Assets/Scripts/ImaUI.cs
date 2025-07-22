using UnityEngine;
using System.Collections.Generic; // Para usar List

public class ImaUI : MonoBehaviour
{
    public enum TipoIma { Nenhum, Azul, Vermelho, Verde }

    [Header("Configuração do Ímã")]
    public TipoIma tipoAtual = TipoIma.Nenhum;
    public float forcaInteracao = 100f; // Força/Velocidade de movimento (pixels por segundo)
    public float raioInteracao = 200f; // Raio em pixels para detectar outros ímãs
    public float distanciaMinimaParada = 50f; // Nova: Distância mínima para ímãs pararem de se mover/repelir

    [Header("Limites da Área de Puzzle")]
    public RectTransform limiteAreaPuzzle; // Arraste o RectTransform do FundoPuzzle ou TelaInterativa para cá

    private RectTransform rectTransform;
    private static List<ImaUI> todosOsImas = new List<ImaUI>();

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("ImaUI: RectTransform não encontrado no GameObject. Este script deve estar em um elemento de UI (Image ou RawImage).");
        }
    }

    void OnEnable()
    {
        if (!todosOsImas.Contains(this))
        {
            todosOsImas.Add(this);
            Debug.Log($"ImaUI: {this.name} adicionado à lista. Total de ímãs: {todosOsImas.Count}");
        }
    }

    void OnDisable()
    {
        todosOsImas.Remove(this);
        Debug.Log($"ImaUI: {this.name} removido da lista. Total de ímãs: {todosOsImas.Count}");
    }

    void Update()
    {
        float deltaTime = Time.unscaledDeltaTime; // Movimento independente da pausa do jogo

        Vector2 forcaTotalAplicadaNesteFrame = Vector2.zero;

        foreach (ImaUI outroIma in todosOsImas)
        {
            if (outroIma == this) continue; // Não interagir consigo mesmo
            if (outroIma.rectTransform == null) continue; // Garante que o outro ímã tem um RectTransform

            Vector2 direcao = outroIma.rectTransform.anchoredPosition - rectTransform.anchoredPosition;
            float distancia = direcao.magnitude;

            // --- Lógica de Interação de Força ---
            // Só interage se estiver dentro do raio E acima da distância mínima de parada.
            if (distancia < raioInteracao && distancia > distanciaMinimaParada)
            {
                // Calcular a força proporcional: mais perto = mais força
                // A força é máxima (1) quando a distância é 0, e zero quando a distância é igual ao raioInteracao
                float forcaProporcional = 1f - (distancia / raioInteracao);

                // Calcular a força base direcional
                Vector2 forcaDirecional = direcao.normalized * forcaInteracao * forcaProporcional;

                // Lógica de Atração (tipos diferentes) ou Repulsão (tipos iguais)
                if (tipoAtual == outroIma.tipoAtual)
                {
                    // === CORREÇÃO 1: INVERTER LÓGICA - REPELIR IGUAIS ===
                    forcaTotalAplicadaNesteFrame -= forcaDirecional; // Repelir: Subtrai força
                    // Debug.Log($"Repelindo: {this.name} ({tipoAtual}) de {outroIma.name} ({outroIma.tipoAtual})");
                }
                else // Tipos diferentes
                {
                    // === CORREÇÃO 1: INVERTER LÓGICA - ATRAIR DIFERENTES ===
                    forcaTotalAplicadaNesteFrame += forcaDirecional; // Atrair: Adiciona força
                    // Debug.Log($"Atraindo: {this.name} ({tipoAtual}) para {outroIma.name} ({outroIma.tipoAtual})");
                }
            }
            // === CORREÇÃO 2: ESTACIONAR QUANDO PRÓXIMOS O SUFICIENTE ===
            else if (distancia <= distanciaMinimaParada)
            {
                // Se tipos iguais e muito próximos, aplicar uma repulsão forte para afastá-los um pouco,
                // mas não deixá-los flutuar infinitamente.
                if (tipoAtual == outroIma.tipoAtual)
                {
                    // Se estiverem muito próximos e forem iguais, repelir suavemente até a distanciaMinimaParada
                    Vector2 separacaoForca = -direcao.normalized * (forcaInteracao / 2f); // Força de separação
                    forcaTotalAplicadaNesteFrame += separacaoForca;
                }
                // Se tipos diferentes e muito próximos, eles já deveriam estar parados/juntos.
                // Não adicionamos força aqui para evitar o flicker.
                // Podemos até forçar a posição se quisermos que eles "grudem" perfeitamente.
                // Exemplo de "grudar" perfeitamente:
                // if (tipoAtual != outroIma.tipoAtual && distancia <= 5f) // Se muito perto e diferentes
                // {
                //     rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, outroIma.rectTransform.anchoredPosition - direcao.normalized * distanciaMinimaParada, deltaTime * suavizacaoMovimento);
                // }
            }
        }

        // Aplica o movimento resultante da força total
        Vector2 novaPosicao = rectTransform.anchoredPosition + forcaTotalAplicadaNesteFrame * deltaTime;

        // === CORREÇÃO 3: LIMITAR POSIÇÃO DENTRO DA ÁREA DO PUZZLE ===
        if (limiteAreaPuzzle != null)
        {
            // Calcula os limites locais do pai do RectTransform
            // A posição do ímã é relativa ao seu pai.
            Vector2 paiLocalMin = rectTransform.parent.InverseTransformPoint(limiteAreaPuzzle.TransformPoint(limiteAreaPuzzle.rect.min));
            Vector2 paiLocalMax = rectTransform.parent.InverseTransformPoint(limiteAreaPuzzle.TransformPoint(limiteAreaPuzzle.rect.max));

            // Tamanho do próprio ímã para não deixar ele sair completamente
            Vector2 halfSize = rectTransform.sizeDelta * 0.5f;

            // Calcula os limites permitidos para o centro do ímã
            float minX = paiLocalMin.x + halfSize.x;
            float maxX = paiLocalMax.x - halfSize.x;
            float minY = paiLocalMin.y + halfSize.y;
            float maxY = paiLocalMax.y - halfSize.y;
            
            // Garante que o item não seja maior que a área, senão min/max ficam invertidos
            if (minX > maxX) { minX = (paiLocalMin.x + paiLocalMax.x) / 2f; maxX = minX; }
            if (minY > maxY) { minY = (paiLocalMin.y + paiLocalMax.y) / 2f; maxY = minY; }


            // Grampeia a nova posição dentro dos limites
            novaPosicao.x = Mathf.Clamp(novaPosicao.x, minX, maxX);
            novaPosicao.y = Mathf.Clamp(novaPosicao.y, minY, maxY);
        }

        rectTransform.anchoredPosition = novaPosicao;
    }
}