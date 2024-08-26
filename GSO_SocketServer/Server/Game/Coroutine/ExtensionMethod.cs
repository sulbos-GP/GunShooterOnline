using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class ExtensionMethod
    {
        private static Random random = new Random();

        private static Stopwatch stopwatch; 
        
        public struct Quaternion
        {
            public float x, y, z, w;

            public Quaternion(float x,float y, float z, float w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

            public static Quaternion Euler(float pitch, float yaw, float roll)
            {
                // 라디안으로 변환
                float radPitch = pitch * (float)(Math.PI / 180.0);
                float radYaw = yaw * (float)(Math.PI / 180.0);
                float radRoll = roll * (float)(Math.PI / 180.0);

                float sinPitch = (float)Math.Sin(radPitch * 0.5f);
                float cosPitch = (float)Math.Cos(radPitch * 0.5f);
                float sinYaw = (float)Math.Sin(radYaw * 0.5f);
                float cosYaw = (float)Math.Cos(radYaw * 0.5f);
                float sinRoll = (float)Math.Sin(radRoll * 0.5f);
                float cosRoll = (float)Math.Cos(radRoll * 0.5f);

                Quaternion q;
                q.x = sinRoll * cosPitch * cosYaw - cosRoll * sinPitch * sinYaw;
                q.y = cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw;
                q.z = cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw;
                q.w = cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw;

                return q;
            }

            public override string ToString()
            {
                return $"Quaternion({x}, {y}, {z}, {w})";
            }
        }
        public static float time
        { get
            {
                return (float)stopwatch.Elapsed.TotalSeconds;
            } }
        public static float Range(float min,float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        public static void Start()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public static void Stop()
        {
            stopwatch.Stop();
        }
        

    }
}
