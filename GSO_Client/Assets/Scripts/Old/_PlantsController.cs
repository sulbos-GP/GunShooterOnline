using System.Collections.Generic;
using UnityEngine;

public class _PlantsController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _tourablePlants = new();

    public bool isSpawnPoint;
    public int radius = -1;
    public int Id = -1;

    public List<GameObject> EnemyList => _tourablePlants;

    //private void Update()
    //{

    //    foreach (Transform t in transform)
    //    {
    //        if (t == null)
    //            continue;

    //        int side = -1;
    //        MonsterController mc;
    //        if (t.TryGetComponent<MonsterController>(out mc) == true)
    //            side = mc.GetComponent<MonsterController>().PlanetSide;
    //        else
    //            continue;


    //        //Debug.Log("Side +" + side);
    //        //Debug.Log("rotation +" + transform.rotation.eulerAngles.z);
    //        //Debug.Log("rotation +" + transform.rotation.z);
    //        //Debug.Log("rotation :" + transform.rotation.eulerAngles.z + "Side =" + side);

    //        if (transform.rotation.eulerAngles.z == 0 && side == 0)
    //        {
    //            OnSide(t, true);
    //        }
    //        else if (transform.rotation.eulerAngles.z == 90 && side == 1)
    //        {
    //            OnSide(t, true);
    //        }
    //        else if (transform.rotation.eulerAngles.z == 180 && side == 2)
    //        {
    //            OnSide(t, true);
    //        }
    //        else if (transform.rotation.eulerAngles.z == 270 && side == 3)
    //        {
    //            OnSide(t, true);

    //        }
    //        else
    //        {
    //            OnSide(t, false);

    //            //Debug.Log($"{t.gameObject.name} ħ��");
    //        }

    //    }
    //}


    //private void OnSide(Transform go, bool isRightSide)
    //{

    //    MonsterController mc;

    //    if (isRightSide)
    //    {
    //        //Debug.Log($"{go.gameObject.name} �߷�");

    //        go.GetComponent<Rigidbody2D>().gravityScale = 1;

    //        if (go.TryGetComponent<MonsterController>(out mc) == true)
    //        {
    //            mc.CanAttackDirect = true;
    //        }
    //        //go.transform.gameObject.SetActive(true);

    //    }
    //    else
    //    {
    //        if (go.transform.gameObject.activeSelf == false)
    //            return;

    //        go.GetComponent<Rigidbody2D>().gravityScale = 0;
    //        go.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

    //        if (go.TryGetComponent<MonsterController>(out mc) == true)
    //        {
    //            mc.CanAttackDirect = false;
    //        }
    //        //go.transform.gameObject.SetActive(false);
    //    }
    //}

    //public string GetTourablePlantsId()
    //{
    //    string res = "";

    //    for (int i = 0; i < _tourablePlants.Count; i++)
    //    {
    //        if (i == _tourablePlants.Count - 1)
    //            res += _tourablePlants[i].GetComponent<PlantsController>().Id.ToString();
    //        else
    //            res += _tourablePlants[i].GetComponent<PlantsController>().Id.ToString() + "_";
    //    }


    //    return res;
    //}


    //public void AddEenemy()
    //{

    //}


    //public void ClearPlants()
    //{
    //    _tourablePlants.Clear();
    //    Id = -1;
    //    radius = -1;
    //}

    //public void AddPlants(GameObject go)
    //{
    //    if (_tourablePlants.Contains(go) == true)
    //        return;

    //    _tourablePlants.Add(go);
    //}

    //private void Start()
    //{
    //    GetComponentInParent<MapController>()?.MapInit();
    //}


    //private void OnDrawGizmos()
    //{
    //    if (_tourablePlants == null && _tourablePlants.Count < 0)
    //        return;

    //    foreach (GameObject go in _tourablePlants)
    //    {
    //        if (go == null)
    //        {
    //            Debug.Log("go is missing");
    //            GetComponentInParent<MapController>()?.MapInit();
    //            break;
    //        }
    //        Gizmos.DrawLine(go.transform.position, transform.position);
    //    }

    //    if (isSpawnPoint)
    //    {
    //        Gizmos.DrawIcon(transform.position, "Cheak", false, Color.white);
    //    }

    //}
}