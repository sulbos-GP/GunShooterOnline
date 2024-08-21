using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = System.Numerics.Vector2;

public partial class InventoryController : MonoBehaviour
{
    #region PlayerInput �׼�
    private void OnTouchPos(InputAction.CallbackContext context)
    {
        UnityEngine.Vector2 pos = context.ReadValue<UnityEngine.Vector2>();
        mousePosInput = new Vector2(pos.x, pos.y);
        
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        isPress = true;
        //���⼭ ������ �̺�Ʈ�� �ϴ°� ����Ʈ�̳� ���Ⱑ ����ǰ� ���� �����ӿ� �׸��尡 �Ҵ��.
        //gridInteract�� ��ü�Ͽ� ��ġ��ġ�� UI�� ��� �ڵ尡 ������ �����Ұ�
    }

    private void OnTouchEnd(InputAction.CallbackContext context)
    {
        isPress = false;
        if (isItemSelected)
        {
            ItemEvent();
        }
    }


    private void OnMouseRightClickInput(InputAction.CallbackContext context)
    {
        //�ӽ�
        //���콺 ��Ŭ����. ���õ� ������ ���ο� ���� �� ������ ���� Ȥ�� ������ ȸ��
        RotateItemRight();
    }

    private void InvenUIControlInput(InputAction.CallbackContext context)
    {
        invenUIControl();
    }

    #endregion


    /// <summary>
    /// ���콺�� ��ġ�� Grid���� Ÿ�� ��ġ�� ��ȯ
    /// </summary>
    private Vector2Int WorldToGridPos()
    {
        Vector2 position = mousePosInput;
        //�������� ����ִٸ�
        if (selectedItem != null)
        {
            //�������� ���콺 �߾ӿ� ������ ����
            position.X -= (selectedItem.Width - 1) * InventoryGrid.WidthOfTile / 2;
            position.Y += (selectedItem.Height - 1) * InventoryGrid.HeightOfTile / 2;
        }

        return selectedGrid.MouseToGridPosition(position);
    }

    /// <summary>
    /// ��Ʈ�ѷ����� ������ ��ȸ�� ���
    /// </summary>
    public void RotateItemRight()
    {
        if (!isItemSelected) { return; }
        selectedItem.RotateRight();
    }

    public void invenUIControl()
    {
        isActive = !isActive;

        if (isActive)
        {
            if (InvenHighLight.highlightObj == null)
            {
                invenHighlight.InstantHighlighter();
            }

            Debug.Log("touchinput On");
            playerInput = Managers.Object.MyPlayer.playerInput;
            playerInput.Player.Disable();
            playerInput.UI.Enable();
            playerInput.UI.TouchPosition.performed += OnTouchPos;
            playerInput.UI.TouchPress.started += OnTouchStart;
            playerInput.UI.TouchPress.canceled += OnTouchEnd;
            playerInput.UI.MouseRightClick.performed += OnMouseRightClickInput;
            playerInput.UI.InventoryControl.performed += InvenUIControlInput;

        }
        else
        {
            if (InvenHighLight.highlightObj != null)
            {
                invenHighlight.DestroyHighlighter();
            }

            Debug.Log("touchinput Off");
            playerInput.UI.TouchPosition.performed -= OnTouchPos;
            playerInput.UI.TouchPress.started -= OnTouchStart;
            playerInput.UI.TouchPress.canceled -= OnTouchEnd;
            playerInput.UI.MouseRightClick.performed -= OnMouseRightClickInput;
            playerInput.UI.InventoryControl.performed -= InvenUIControlInput;
            playerInput.UI.Disable();
            playerInput.Player.Enable();
        }

        inventoryUI.SetActive(isActive);
    }

    public void RotateBtn()
    {
        RotateItemRight();
    }
}
