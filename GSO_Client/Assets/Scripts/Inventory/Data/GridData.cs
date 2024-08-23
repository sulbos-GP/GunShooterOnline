using Google.Protobuf.Protocol;
using JetBrains.Annotations;
using NPOI.POIFS.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

[System.Serializable]
//[CreateAssetMenu(fileName = "gridData", menuName = "InventoryUI/gridData")]
public class GridData //: MonoBehaviour
{
    public Vector2Int gridSize;
    public List<ItemData> itemList;
}
