namespace MASZ.Services
{
    public interface IScheduler
    {
        DateTime GetNextCacheSchedule();
        void StartTimers();
        void CacheAll();
        Task<List<ulong>> CacheAllGuildBans(List<ulong> handledUsers);
        Task<List<ulong>> CacheAllGuildMembers(List<ulong> handledUser);
        Task<List<ulong>> CacheAllKnownUsers(List<ulong> handledUser);
        Task CacheAllKnownGuilds();
        void CheckDeletedCases();
    }
}