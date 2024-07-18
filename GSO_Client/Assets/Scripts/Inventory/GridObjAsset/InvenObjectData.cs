using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new GridObj", menuName = "Inventory/GridObjData")]
public class GridObjectData : ScriptableObject
{
    [Header("gridSize")]
    public Vector2Int gridSize;
}
