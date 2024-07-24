using Docker.DotNet;
using GameServerManager.Servicies.Interfaces;

namespace GameServerManager.Servicies
{
    public class DockerService : IDockerService
    {
        private readonly DockerClient mDockerClient;

        public DockerService()
        {
            mDockerClient = new DockerClientConfiguration(new Uri("http://localhost:8080")).CreateClient();
        }

        public void Dispose()
        {
            mDockerClient.Dispose();
        }

    }
}
