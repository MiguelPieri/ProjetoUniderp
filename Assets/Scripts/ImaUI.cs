using UnityEngine;
using System.Collections.Generic;

public class ImaUI : MonoBehaviour
{
    public enum TipoIma { Nenhum, Azul, Vermelho, Verde }

    [Header("Configuração do Ímã")]
    public TipoIma tipoAtual = TipoIma.Nenhum;
    public float forcaInteracao = 100f;
    public float raioInteracao = 200f;
    public float distanciaMinimaParada = 50f;

    [Header("Limites da Área de Puzzle")]
    
    public RectTransform limiteAreaPuzzle; 

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
        float deltaTime = Time.unscaledDeltaTime;

        Vector2 forcaTotalAplicadaNesteFrame = Vector2.zero;

        foreach (ImaUI outroIma in todosOsImas)
        {
            if (outroIma == this) continue;
            if (outroIma.rectTransform == null) continue;

            Vector2 direcao = outroIma.rectTransform.anchoredPosition - rectTransform.anchoredPosition;
            float distancia = direcao.magnitude;

            if (distancia < raioInteracao && distancia > distanciaMinimaParada)
            {
                float forcaProporcional = 1f - (distancia / raioInteracao);
                Vector2 forcaDirecional = direcao.normalized * forcaInteracao * forcaProporcional;

                if (tipoAtual == outroIma.tipoAtual)
                {
                    forcaTotalAplicadaNesteFrame -= forcaDirecional;
                }
                else
                {
                    forcaTotalAplicadaNesteFrame += forcaDirecional;
                }
            }
            else if (distancia <= distanciaMinimaParada)
            {
                if (tipoAtual == outroIma.tipoAtual)
                {
                    Vector2 separacaoForca = -direcao.normalized * (forcaInteracao / 2f);
                    forcaTotalAplicadaNesteFrame += separacaoForca;
                }
            }
        }

        Vector2 novaPosicao = rectTransform.anchoredPosition + forcaTotalAplicadaNesteFrame * deltaTime;

        
        if (limiteAreaPuzzle != null)
        {
   
            // Dimensões do limite (pai)
            float limiteWidth = limiteAreaPuzzle.rect.width;
            float limiteHeight = limiteAreaPuzzle.rect.height;

            // Metade do tamanho do próprio ímã
            float halfImaWidth = rectTransform.sizeDelta.x * rectTransform.localScale.x * 0.5f;
            float halfImaHeight = rectTransform.sizeDelta.y * rectTransform.localScale.y * 0.5f;

            // Posição do pivô do ímã (normalizado de 0 a 1)
            Vector2 imaPivot = rectTransform.pivot;

            // Calcula a posição do centro do ímã em relação ao seu pivô
            float imaCenterXOffset = (0.5f - imaPivot.x) * rectTransform.sizeDelta.x * rectTransform.localScale.x;
            float imaCenterYOffset = (0.5f - imaPivot.y) * rectTransform.sizeDelta.y * rectTransform.localScale.y;

            // Calcula os limites reais em coordenadas anchoredPosition
            // Estes são os limites para o anchoredPosition (que é a posição do PIVÔ do ímã)
            float minX = -limiteWidth * limiteAreaPuzzle.pivot.x + halfImaWidth + imaCenterXOffset;
            float maxX = limiteWidth * (1f - limiteAreaPuzzle.pivot.x) - halfImaWidth + imaCenterXOffset;
            float minY = -limiteHeight * limiteAreaPuzzle.pivot.y + halfImaHeight + imaCenterYOffset;
            float maxY = limiteHeight * (1f - limiteAreaPuzzle.pivot.y) - halfImaHeight + imaCenterYOffset;

            // Garante que o item não seja maior que a área. Se for, os limites se ajustam para o centro.
            if (minX > maxX) { minX = maxX = (minX + maxX) / 2f; }
            if (minY > maxY) { minY = maxY = (minY + maxY) / 2f; }

            // Aplica os limites à nova posição
            novaPosicao.x = Mathf.Clamp(novaPosicao.x, minX, maxX);
            novaPosicao.y = Mathf.Clamp(novaPosicao.y, minY, maxY);
        }

        rectTransform.anchoredPosition = novaPosicao;
    }
}