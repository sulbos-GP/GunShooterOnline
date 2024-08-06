using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Vector2 = System.Numerics.Vector2;
using Differ;

namespace Server
{

    public class DDA
    {
        LoadMap loadMap = new LoadMap();

        public static readonly Vector2 unityOffset = new Vector2(0.5f, 0.5f);
        //public static readonly Vector2 unityOffset = Vector2.Zero;

        int mapWidth;
        int mapHeight;
        int[,] vecMap;
        public void Init()
        {
            loadMap.loadMap(0);

            mapWidth = loadMap.Width;
            mapHeight = loadMap.Height;
            vecMap = loadMap._collisions;
        }
        Vector2 vIntersection;
        bool bTileFound;
        float fMaxDistance;



        /*int mapWidth = 10; // Example map width
        int mapHeight = 10; // Example map height
        int[] vecMap = new int[mapWidth * mapHeight]; // Example map*/




        Vector2 offeset;

        /*
        private void Update()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;

            // 화면 좌표를 월드 좌표로 변환 (z축을 0으로 설정)
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y));

            // z축 값을 조정 (2D 게임의 경우 z=0)
            mouseWorldPosition.z = 0;
            new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);
            RaycastHit2D t = RayCast2Map(new Vector2(0.0f, 0.0f), new Vector2(mouseWorldPosition.x, mouseWorldPosition.y));

            vIntersection = t != null ? (Vector2)t.hitPoint : Vector2.Zero;

        }*/
        public RaycastHit2D RayCast2Map(Vector2 _startPos, Vector2? _endPos)
        {
            if (_endPos == null)
                return null;

            offeset = new Vector2((int)(loadMap.Width / 2), (int)(loadMap.Height / 2));


            Vector2 vtargetPos = (Vector2)_endPos + offeset + unityOffset;
            Vector2 vRayStart = _startPos + offeset + unityOffset;

            fMaxDistance = Vector2.Distance(vRayStart, vtargetPos); ;

            Vector2 vRayDir = Vector2.Normalize(vtargetPos - vRayStart);

            //Debug.Log("Dir" +  vRayDir);

            float fDistance = 0.0f;
            bTileFound = false;



            Vector2 vRayUnitStepSize = new Vector2(
                MathF.Sqrt(1 + (vRayDir.Y / vRayDir.X) * (vRayDir.Y / vRayDir.X)),
                MathF.Sqrt(1 + (vRayDir.X / vRayDir.Y) * (vRayDir.X / vRayDir.Y))
            );

            Vector2 vMapCheck = new Vector2((int)vRayStart.X, (int)vRayStart.Y);
            Vector2 vRayLength1D = new Vector2();
            Vector2 vStep = new Vector2();

            if (vRayDir.X < 0)
            {
                vStep.X = -1;
                vRayLength1D.X = (vRayStart.X - vMapCheck.X) * vRayUnitStepSize.X;
            }
            else
            {
                vStep.X = 1;
                vRayLength1D.X = (vMapCheck.X + 1 - vRayStart.X) * vRayUnitStepSize.X;
            }

            if (vRayDir.Y < 0)
            {
                vStep.Y = -1;
                vRayLength1D.Y = (vRayStart.Y - vMapCheck.Y) * vRayUnitStepSize.Y;
            }
            else
            {
                vStep.Y = 1;
                vRayLength1D.Y = (vMapCheck.Y + 1 - vRayStart.Y) * vRayUnitStepSize.Y;
            }





            /*// Example map initialization (1 represents a wall)
            for (int i = 0; i < mapWidth * mapHeight; i++)
            {
                vecMap[i] = 0;
            }
            vecMap[5 * mapWidth + 5] = 1; // Set a wall at (5, 5)*/

            while (!bTileFound && fDistance < fMaxDistance)
            {
                if (vRayLength1D.X < vRayLength1D.Y)
                {
                    vMapCheck.X += vStep.X;
                    fDistance = vRayLength1D.X;
                    vRayLength1D.X += vRayUnitStepSize.X;
                }
                else
                {
                    vMapCheck.Y += vStep.Y;
                    fDistance = vRayLength1D.Y;
                    vRayLength1D.Y += vRayUnitStepSize.Y;
                }

                if (vMapCheck.X >= 0 && vMapCheck.X < mapWidth && vMapCheck.Y >= 0 && vMapCheck.Y < mapHeight)
                {
                    //if (vecMap[(int)vMapCheck.Y * mapWidth + (int)vMapCheck.X] == 1)
                    if (vecMap[(int)vMapCheck.X, (int)vMapCheck.Y] == 1)
                    {
                        //Debug.Log($"x = {vMapCheck.X} y = {vMapCheck.Y} is {vecMap[(int)vMapCheck.X, (int)vMapCheck.Y]}");
                        bTileFound = true;
                    }
                }
            }


            if (bTileFound)
            {
                RaycastHit2D res = new RaycastHit2D();
                res.hitPoint = _startPos + vRayDir * fDistance;
                res.distance = fDistance;
                return res;
                //Debug.Log($"Intersection found at: ({vIntersection.X}, {vIntersection.Y})");
            }
            else
            {
                return null;
                //Debug.Log("No intersection found.");
            }
        }


        //Vector2 unityOffset = new Vector2(0.5f, 0.5f);

        /*private void OnDrawGizmos()
        {

            Gizmos.color = Color.blue;
            //Gizmos.DrawSphere(new UnityEngine.Vector3(startPos.X, startPos.Y, 0), 0.5f);

            Gizmos.color = Color.green;
            //Gizmos.DrawSphere(new UnityEngine.Vector3(targetPos.X, targetPos.Y, 0), 0.5f);





            Gizmos.color = Color.red;
            UnityEngine.Vector3 position = new UnityEngine.Vector3(vIntersection.X - unityOffset.X, vIntersection.Y - unityOffset.Y, 0);
            if (bTileFound)
                Gizmos.DrawSphere(position, 0.5f); 
        }*/

    }


}