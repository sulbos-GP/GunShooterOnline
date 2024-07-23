using Collision.Shapes;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class RaycastManager
{
    public static RaycastManager Instance { get; } = new();

    private void Awake()
    {
        Instance = this;
    }


    private Dictionary<int, Raycast> raycasts = new Dictionary<int, Raycast>();


    public void Add(int key, Vector3 origin, Vector3 direction, float distance)
    {
        /*if (!raycasts.ContainsKey(key))
        {
            Raycast newRaycast = new Raycast(origin, direction, distance);
            raycasts.Add(key, newRaycast);
        }
        else
        {
            Debug.LogWarning($"Raycast with key {key} already exists.");
        }*/
    }

    public void Add(int key, Raycast raycast)
    {
        if (!raycasts.ContainsKey(key))
        {
            raycasts.Add(key, raycast);
        }
        else
        {
            Console.WriteLine($"Raycast with key {key} already exists.");
        }
    }



    public void Remove(int key)
    {
        if (raycasts.ContainsKey(key))
        {
            raycasts.Remove(key);
        }
        else
        {
            Console.WriteLine($"Raycast with key {key} does not exist.");
        }
    }

    // 특정 Raycast 가져오는 메서드
    public Raycast GetRaycast(int key)
    {
        if (raycasts.TryGetValue(key, out Raycast raycast))
        {
            return raycast;
        }
        else
        {
            Console.WriteLine($"Raycast with key {key} does not exist.");
            return null;
        }
    }

    Vector2 hitpos;

    private void Update()
    {
        hitpos = Vector2.Zero;
        Shape[] shapes = ObjectManager.Instance.GetValue();

        foreach (Raycast raycast in raycasts.Values)
        {
            RaycastHit2D res = null;

            foreach (Shape shape in shapes)
            {
                RaycastHit2D temp = raycast.Cast(shape);

                if (temp == null)
                    continue;

                if (res != null)
                {
                    if (res.distance > temp.distance)
                    {
                        res = temp;
                        hitpos = res.hitPoint;

                    }
                }
                else
                {
                    res = temp;
                    hitpos = res.hitPoint;
                }




            }
        }

    }


    

}
