using Differ;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Server
{

    internal class LoadMap
    {


        public int[,] _collisions;
        public int Width;
        public int Height;



        public void loadMap(int mapId, string pathPrefix = "../../../../../Common/MapData")
        {

            //var Distance = 22;

            //----------------------------------------
            var mapName = "Map_2";

            // Collision 관련 파일
            var text = File.ReadAllText(" ./../../../../GSO_Client/Assets/Resources/ "+ $"/ Data/{mapName}.txt");
            var reader = new StringReader(text);

            var _ = reader.ReadLine();

            var minIndex = reader.ReadLine().Split('/');


            var Bleft = new Vector2Int(int.Parse(minIndex[0]), int.Parse(minIndex[1]));
            var roomSize = int.Parse(reader.ReadLine());

            Width = roomSize;
            Height = roomSize;

            var Tright = new Vector2Int(Bleft.x + (roomSize - 1), Bleft.y + (roomSize - 1));


            _collisions = new int[roomSize, roomSize];
            //_objects = new GameObject[roomSize, roomSize];


            //for (var x = roomSize - 1; x >= 0; x--)
            for (var x = 0; x < roomSize; x++)
                Buffer.BlockCopy(
                    Array.ConvertAll(reader.ReadLine().Split(','), s => int.Parse(s)),
                    0, _collisions, x * roomSize * sizeof(int), roomSize * sizeof(int));

            //디버깅
            string t = "";

            for (int i = 0; i < roomSize; i++)
            {

                for (int j = 0; j < roomSize; j++)
                {
                    //Console.Write(_collisions[i, j]);
                    t += _collisions[i, j];
                }
                //Console.WriteLine();
                t += "\n";
            }
            //Debug.Log(t);

            //Debug.Log(_collisions.ToString());




            //         while (true)
            //         {
            //	int x = int.Parse(Console.ReadLine());
            //	int y = int.Parse(Console.ReadLine());
            //	bool k = CanGo(new Vector2Int(x, y), false);
            //             Console.WriteLine(k);
            //}
        }

    }
    
}
