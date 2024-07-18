using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private RectTransform lever;

    [SerializeField] [Range(10, 150)] private float leverRange;

    private Vector2 _dir;
    private RectTransform _rectTransform;
    private MyPlayerController playerController;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ControlJoyStickLever(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ControlJoyStickLever(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lever.anchoredPosition = Vector2.zero;
        _dir = Vector2.zero;
    }

    public void StartJoystick(bool isable, MyPlayerController _playerController)
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();

        if (isable)
        {
            _rectTransform.gameObject.SetActive(true);

            playerController = _playerController;
        }
        else
        {
            transform.gameObject.SetActive(false);
        }
    }

    private void ControlJoyStickLever(PointerEventData eventData)
    {
        var inputpos = (eventData.position - _rectTransform.anchoredPosition) / 2;
        var inputVector = inputpos.magnitude <= leverRange ? inputpos : inputpos.normalized * leverRange;
        lever.anchoredPosition = inputVector;
        //Debug.Log($"{Math.Round(inputVector.normalized.x,3)},{Math.Round(inputVector.normalized.y, 3)}");

        if (playerController == null)
            return;
        {
            inputVector.Normalize();
            if (inputVector.y > 0.3f) inputVector.y = 1;
            if (inputVector.y < -0.3f) inputVector.y = -1;
            if (inputVector.x > 0.3f) inputVector.x = 1;
            if (inputVector.x < -0.3f) inputVector.x = -1;
            _dir = inputVector;
        }
    }

    public Vector2 GetDir()
    {
        return _dir;
    }
}