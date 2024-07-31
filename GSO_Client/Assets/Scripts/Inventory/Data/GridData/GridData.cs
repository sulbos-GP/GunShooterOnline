using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

[CreateAssetMenu(fileName = "new GridData", menuName = "Inventory/GridData")]
public class GridData : ScriptableObject
{
    public int gridId;
    public Vector2Int gridSize;
    public Vector2 gridPos;
    public List<ItemData> itemList;

    public bool createRandomItem;
    public int randomItemAmount;
}
