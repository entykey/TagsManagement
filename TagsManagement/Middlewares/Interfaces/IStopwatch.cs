namespace TagsManagement.Middlewares.Interfaces
{
    public interface IStopwatch
    {
        long ElapsedMilliseconds { get; }
        void Start();
        void Stop();
        void Reset();
    }
}
