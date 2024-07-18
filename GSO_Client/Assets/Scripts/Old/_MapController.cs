using UnityEngine;

public class _MapController : MonoBehaviour
{
    //public int MapNumer;
    //public int ChildCount { get { return transform.childCount; } }

    //public List<GameObject> Plants;

    //public Vector2Int Center;

    //public Vector2Int Size;


    //#region ���ڵ� ����
    //public int DistancePlants = 50;

    //[ContextMenu("MapInit")]
    //public void MapInit()
    //{
    //    SetClear();
    //    SetOrder();
    //    SetTourable();
    //}

    //public void SetClear()
    //{
    //    //Debug.Log($"����{transform.childCount}");
    //    foreach (Transform go in transform)
    //    {
    //        go.GetComponent<PlantsController>()?.ClearPlants();
    //    }
    //    Plants.Clear();
    //    //Debug.Log(Plants.Count);
    //}

    //public void SetOrder()
    //{
    //    int _count = 1;

    //    var transforms =
    //        GetComponentsInChildren<Transform>()
    //        .OrderBy(t => t.position.x)
    //        .ThenBy(t => t.position.y);

    //    Transform[] t = transforms.ToArray();

    //    foreach (Transform item in t)
    //    {
    //        PlantsController pc;
    //        if (item.TryGetComponent<PlantsController>(out pc) == true)
    //        {
    //            pc.Id = _count++;
    //            Plants.Add(item.gameObject);

    //            pc.radius = (int)item.localScale.x;
    //        }

    //    }

    //}

    //public void SetTourable()
    //{
    //    foreach (Transform go in transform)
    //    {
    //        PlantsController plants = go.GetComponent<PlantsController>();
    //        foreach (Transform target in transform)
    //        {
    //            if (go.gameObject == target.gameObject)
    //                continue;
    //            if (DistancePlants + 1 > Vector2.Distance((Vector2)go.transform.position, (Vector2)target.transform.position))
    //            {
    //                plants.AddPlants(target.gameObject);
    //            }

    //        }
    //    }


    //}


    //#endregion


    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = new Color(0, 1, 1, 0.1f);
    //    Gizmos.DrawCube(new Vector3(Center.x, Center.y), new Vector3(Size.x, Size.y));
    //}
}