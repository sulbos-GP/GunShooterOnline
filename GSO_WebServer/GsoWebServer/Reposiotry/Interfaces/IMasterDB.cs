namespace GsoWebServer.Reposiotry.Interfaces
{
    public interface IMasterDB : IDisposable
    {
        public Task<bool> LoadMasterData();
    }
}
