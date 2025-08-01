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

        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("MovimentoUI: Canvas pai não encontrado. Este script deve estar em um elemento UI dentro de um Canvas.");
        }
    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("MovimentoUI: Item clicado! Prepare para arrastar.");
       
    }
    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("MovimentoUI: Item sendo arrastado..."); // Cuidado: isso pode gerar muitos logs!

        // Se o Canvas Render Mode for Screen Space - Overlay ou Screen Space - Camera:
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            // Move o item diretamente para a posição do ponteiro do mouse.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPos);
            rectTransform.anchoredPosition = localPointerPos;
        }
        else
        {

            Debug.LogWarning("MovimentoUI: Arrastar em Canvas World Space é mais complexo e não totalmente suportado por este script simples.");
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("MovimentoUI: Item solto!");
    }
}