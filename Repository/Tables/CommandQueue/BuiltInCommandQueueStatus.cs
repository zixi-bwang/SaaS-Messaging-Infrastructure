using System;
using System.ComponentModel;

namespace Repository.Enums
{
    public static class BuiltInCommandQueueStatus
    {
        [Description("Enqueued")]
        public static readonly Guid Enqueued = Guid.Parse("76941667-b901-4334-883d-3a8bf4580ae6");
        [Description("Dequeued")]
        public static readonly Guid Dequeued = Guid.Parse("97381bfa-f3d4-4774-9fba-ae9833e061ff");
        [Description("Processing")]
        public static readonly Guid Processing = Guid.Parse("781eca28-ac10-4c8e-8bf9-2ae225134d02");
        [Description("Completed")]
        public static readonly Guid Completed = Guid.Parse("7c52fbd5-ba8d-475b-b1ed-9d23ea931af3");
        [Description("Failed")]
        public static readonly Guid Failed = Guid.Parse("fa0d5ca4-182e-4dd9-b0f8-d07451a89f72");
    }
}
