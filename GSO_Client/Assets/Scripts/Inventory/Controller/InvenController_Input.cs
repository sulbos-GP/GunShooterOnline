using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = System.Numerics.Vector2;

public partial class InventoryController
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
        InvenBtn();
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
            position.X -= (selectedItem.Width - 1) * GridObject.WidthOfTile / 2;
            position.Y += (selectedItem.Height - 1) * GridObject.HeightOfTile / 2;
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
            InvenInputOn();

        }
        else
        {
            if (InvenHighLight.highlightObj != null)
            {
                invenHighlight.DestroyHighlighter();
            }

            Debug.Log("touchinput Off");
            InvenInputOff();
            playerInput.Player.Enable();
        }

        inventoryUI.SetActive(isActive);
    }

    private void InvenInputOff()
    {
        playerInput.UI.TouchPosition.performed -= OnTouchPos;
        playerInput.UI.TouchPress.started -= OnTouchStart;
        playerInput.UI.TouchPress.canceled -= OnTouchEnd;
        playerInput.UI.MouseRightClick.performed -= OnMouseRightClickInput;
        playerInput.UI.InventoryControl.performed -= InvenUIControlInput;
        playerInput.UI.Disable();
    }

    private void InvenInputOn()
    {
        playerInput.UI.Enable();
        playerInput.UI.TouchPosition.performed += OnTouchPos;
        playerInput.UI.TouchPress.started += OnTouchStart;
        playerInput.UI.TouchPress.canceled += OnTouchEnd;
        playerInput.UI.MouseRightClick.performed += OnMouseRightClickInput;
        playerInput.UI.InventoryControl.performed += InvenUIControlInput;
    }

    

    public void InvenBtn()
    {
        //�ڽ��� �κ��丮 ��û
        if (!isActive) {
            //�κ��丮�� ų ���
            SendLoadInvenPacket(0);
        }
        else
        {
            //�κ��丮�� ���� ���
            if (otherInvenUI.instantGrid != null) { SendCloseInvenPacket(otherInvenUI.instantGrid.objectId); return; }
            if (playerInvenUI.instantGrid != null) { SendCloseInvenPacket(); }
        }
    }

    public void RotateBtn()
    {
        RotateItemRight();
    }
}
