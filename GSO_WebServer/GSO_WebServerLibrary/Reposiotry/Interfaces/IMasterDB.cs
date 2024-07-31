namespace GSO_WebServerLibrary.Reposiotry.Interfaces
{
    public interface IMasterDB : IDisposable
    {
        public Task<bool> LoadMasterData();
    }
}
