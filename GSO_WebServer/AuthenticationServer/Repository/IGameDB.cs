namespace AuthenticationServer.Repository
{
    public interface IGameDB : IDisposable
    {
        public Task<Tuple<ErrorCode, Int64>> SignIn(String id, String service);
        public Task<Tuple<ErrorCode, Int64>> SignUp(String id, String service);
    }
}
