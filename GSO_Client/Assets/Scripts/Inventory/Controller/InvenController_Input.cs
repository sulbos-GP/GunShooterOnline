using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = System.Numerics.Vector2;

public partial class InventoryController
{
    #region PlayerInput 액션
    private void OnTouchPos(InputAction.CallbackContext context)
    {
        UnityEngine.Vector2 pos = context.ReadValue<UnityEngine.Vector2>();
        mousePosInput = new Vector2(pos.x, pos.y);
        
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        isPress = true;
        //여기서 아이템 이벤트를 하는게 베스트이나 여기가 실행되고 다음 프레임에 그리드가 할당됨.
        //gridInteract를 대체하여 터치위치에 UI를 얻는 코드가 있을시 수정할것
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
        //에디터 디버그 용
        RotateItemRight();
    }

    private void InvenUIControlInput(InputAction.CallbackContext context)
    {
        InventoryActiveBtn();
    }

    #endregion


    /// <summary>
    /// 마우스의 위치를 Grid상의 타일 위치로 변환
    /// </summary>
    private Vector2Int WorldToGridPos()
    {
        Vector2 position = mousePosInput;
        //아이템을 들고있다면
        if (selectedItem != null)
        {
            //아이템이 마우스 중앙에 오도록 보정
            position.X -= (selectedItem.Width - 1) * GridObject.WidthOfTile / 2;
            position.Y += (selectedItem.Height - 1) * GridObject.HeightOfTile / 2;
        }

        return selectedGrid.MouseToGridPosition(position);
    }

    /// <summary>
    /// 컨트롤러에서 아이템 우회전 명령
    /// </summary>
    public void RotateItemRight()
    {
        if (!isItemSelected) { return; }
        selectedItem.RotateRight();
    }

    public void invenUIControl(bool tf)
    {
        isActive = tf;

        if (isActive)
        {
            if (invenHighlight.highlightObj == null)
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
            if (invenHighlight.highlightObj != null)
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

    

    public void InventoryActiveBtn()
    {
        //자신의 인벤토리 요청
        if (!isActive) {
            //인벤토리를 킬 경우
            InventoryPacket.SendLoadInvenPacket();
        }
        else
        {
            //인벤토리를 닫을 경우
            if (otherInvenUI.instantGrid != null) { InventoryPacket.SendCloseInvenPacket(otherInvenUI.instantGrid.objectId); return; }
            if (playerInvenUI.instantGrid != null) { InventoryPacket.SendCloseInvenPacket(); }
        }
    }

    public void RotateBtn()
    {
        RotateItemRight();
    }
}
