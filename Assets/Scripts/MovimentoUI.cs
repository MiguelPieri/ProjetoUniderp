using UnityEngine;
using UnityEngine.EventSystems; // Importante para as interfaces de eventos de UI

// As interfaces IPointerDownHandler, IDragHandler e IPointerUpHandler são essenciais
// para detectar quando o mouse clica no item, arrasta o item e solta o item.
public class MovimentoUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform; // Referência ao RectTransform do item de UI
    private Canvas canvas; // Referência ao Canvas pai do item de UI

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>(); // Obtém o RectTransform deste GameObject
        if (rectTransform == null)
        {
            Debug.LogError("MovimentoUI: RectTransform não encontrado no GameObject. Este script deve estar em um elemento de UI (como Image).");
        }

        // Procura o Canvas pai. É importante para a conversão de coordenadas.
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("MovimentoUI: Canvas pai não encontrado. Este script deve estar em um elemento UI dentro de um Canvas.");
        }
    }

    // --- IPointerDownHandler: Chamado quando o botão do mouse é pressionado sobre este elemento ---
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("MovimentoUI: Item clicado! Prepare para arrastar.");
        // Opcional: Se você quiser que o item vá para o topo da ordem de desenho quando clicado
        // rectTransform.SetAsLastSibling();
    }

    // --- IDragHandler: Chamado enquanto o mouse está sendo arrastado sobre este elemento ---
    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("MovimentoUI: Item sendo arrastado..."); // Cuidado: isso pode gerar muitos logs!

        // Se o Canvas Render Mode for Screen Space - Overlay ou Screen Space - Camera:
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            // Move o item diretamente para a posição do ponteiro do mouse.
            // eventData.position já é a posição do mouse em coordenadas de tela.
            // Para RectTransform, 'anchoredPosition' é a posição relativa ao seu pivô/âncora.
            // Precisamos converter a posição da tela para a posição local do Canvas.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPos);
            rectTransform.anchoredPosition = localPointerPos;
        }
        else // Se o Canvas Render Mode for World Space (mais complexo para arrastar diretamente)
        {
            // Para World Space, o arrasto é mais complexo e pode envolver raios e Physics.Raycast.
            // Para simplicidade, este script assume Screen Space.
            // Você pode adicionar uma lógica mais sofisticada aqui se precisar.
            Debug.LogWarning("MovimentoUI: Arrastar em Canvas World Space é mais complexo e não totalmente suportado por este script simples.");
        }
    }

    // --- IPointerUpHandler: Chamado quando o botão do mouse é liberado sobre este elemento ---
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("MovimentoUI: Item solto!");
    }
}