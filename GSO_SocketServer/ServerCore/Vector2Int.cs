using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2Int up => new(0, 1);
        public static Vector2Int down => new(0, -1);
        public static Vector2Int left => new(-1, 0);
        public static Vector2Int right => new(1, 0);

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }

        public static Vector2Int operator -(Vector2 a, Vector2Int b)
        {
            return new Vector2Int((int)a.X - b.x, (int)a.Y - b.y);
        }

        public static bool operator ==(Vector2Int a, Vector2Int b)
        {
            return (a.x == b.x) && (a.y == b.y);
        }
        public static bool operator !=(Vector2Int a, Vector2Int b)
        {
            return (a.x != b.x) || (a.y != b.y);
        }


        public float sqrMagnitude => MathF.Sqrt(sqrMagnitude);
        public int Magnitude => x * x + y * y;

        public int cellDistFromZero => Math.Abs(x) + Math.Abs(y);

        public static explicit operator Vector2(Vector2Int v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2Int RoundToInt(Vector2 vector)
        {
            return new Vector2Int(
             (int)Math.Round(vector.X, MidpointRounding.AwayFromZero),
             (int)Math.Round(vector.Y, MidpointRounding.AwayFromZero)
         );
        }
    }

}
