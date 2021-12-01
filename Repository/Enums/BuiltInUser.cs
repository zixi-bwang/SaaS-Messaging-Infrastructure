using System;

namespace Repository.Enums
{
    public static class BuiltInUser
    {
        public readonly static Guid System = new Guid();
        public readonly static Guid CronJob = new Guid();
        public readonly static Guid CommandQueue = new Guid();
        public readonly static Guid ClientApiUser = new Guid();
    }
}
