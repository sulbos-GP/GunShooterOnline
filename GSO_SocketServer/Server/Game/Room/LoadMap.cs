using Differ;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Humanizer.In;
using static System.Net.Mime.MediaTypeNames;

namespace Server
{

    public class LoadMap
    {


        public int[,] _collisions;
        public int Width;
        public int Height;



        public void loadMap(string mapName, string pathPrefix = "")
        {
/*
            //var Distance = 22;

            //----------------------------------------
            mapName = "Forest";

            // Collision 관련 파일
#if DOCKER
        var text = File.ReadAllText("/app/MapData/" + $"{mapName}.txt");
#else

            var text = File.ReadAllText(" ./../../../../../MapData/" + $"{mapName}.txt");
#endif
            var reader = new StringReader(text);

            var _ = reader.ReadLine();

            var minIndex = reader.ReadLine().Split('/');


            Bleft = new Vector2Int(int.Parse(minIndex[0]), int.Parse(minIndex[1]));
            var size = reader.ReadLine().Split('/');

            roomSize = new Vector2Int(int.Parse(size[0]), int.Parse(size[1]));

            Height = roomSize.x;
            Width = roomSize.y;

            Tright = new Vector2Int(Bleft.x + (roomSize.x - 1), Bleft.y + (roomSize.y - 1));


            _collisions = new int[roomSize.x, roomSize.y];
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
            //}*/
        }

       
    }
    
}
