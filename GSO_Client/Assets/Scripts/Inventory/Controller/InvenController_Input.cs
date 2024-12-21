using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = System.Numerics.Vector2;

public partial class InventoryController
{
    #region PlayerInput
    private void OnTouchPos(InputAction.CallbackContext context)
    {
        UnityEngine.Vector2 pos = context.ReadValue<UnityEngine.Vector2>();
        mousePosInput = new Vector2(pos.x, pos.y);
        
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        isPress = true;
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
        RotateItemRight();
    }

    private void InvenUIControlInput(InputAction.CallbackContext context)
    {
        InventoryActiveBtn();
    }

    #endregion
    private Vector2Int WorldToGridPos()
    {
        Vector2 position = mousePosInput;
        if (selectedItem != null)
        {
            position.X -= (selectedItem.Width - 1) * GridObject.WidthOfTile / 2;
            position.Y += (selectedItem.Height - 1) * GridObject.HeightOfTile / 2;
        }

        return selectedGrid.GetGridPosByMousePos(position);
    }

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

    public int? interactBoxId;

    public void InventoryActiveBtn()
    {
        if (OnWaitSwitchPacket)
        {
            return;
        }

        if (!isActive) {
            InventoryPacket.SendLoadInvenPacket();
            AudioManager.instance.PlaySound("ShowInventory",AudioManager.instance.GetComponent<AudioSource>());
        }
        else
        {
            if (interactBoxId != null) 
            { 
                InventoryPacket.SendCloseInvenPacket((int)interactBoxId); 

            }
            else 
            { 
                InventoryPacket.SendCloseInvenPacket(); 
            }
        }
    }

    public void RotateBtn()
    {
        RotateItemRight();
    }
}
