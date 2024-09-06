using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Docker
{
    public static class DockerUtil
    {
        public static string GetContainerId()
        {
#if DOCKER
            string[] cgroupLines = File.ReadAllLines("/proc/self/cgroup");
            string cgroupLine = cgroupLines.FirstOrDefault(line => line.Contains("cpu:"));

            if (cgroupLine != null)
            {
                return cgroupLine.Split('/').Last();
            }

            return string.Empty;
#else
            return "DummyContainer";
#endif
        }

        public static string GetHostIP()
        {
#if DOCKER
            string ip = Environment.GetEnvironmentVariable("HOST_IP");
            if (ip == null)
            {
                return string.Empty;
            }
            return ip;
#else
            return "127.0.0.1";
#endif
        }

        public static int GetHostPort()
        {
#if DOCKER
            string port = Environment.GetEnvironmentVariable("HOST_PORT");
            if (port == null)
            {
                return 0;
            }
            return int.Parse(port);
#else
            return 7777;
#endif
        }

        public static int GetRegister()
        {
            return Convert.ToInt32(Environment.GetEnvironmentVariable("REGISTER"));
        }

        public static int GetBacklog()
        {
            return Convert.ToInt32(Environment.GetEnvironmentVariable("BACKLOG"));
        }
    }
}
