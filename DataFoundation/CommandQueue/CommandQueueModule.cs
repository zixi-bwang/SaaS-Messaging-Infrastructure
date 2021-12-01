using ContentFoundation.CommandQueue;
using ContentFoundation.CommandQueue.Managers;
using ContentFoundation.Events;
using Decoupling.EventBrokers;
using Decoupling.Module;
using EntityFrameworkCore.BootKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Repository.Enums;
using Repository.Tables;
using Repository.Tables.Field;
using Repository.Tables.User;
using Repository.Taxonomy.Tables;
using System;
using static Utilities.Util;

namespace ContentFoundation.EventWebHook
{
    public class CommandQueueModule : ModuleBase, IModule
    {
        OrderDbContext dc;

        public void RegisterDI(IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<CommandQueueManager>();
            services.AddScoped<IEventHandler<CommandQueueEnqueueEvent, Guid>, CommandQueueManager>();
            services.AddScoped<SampleCommandQueueProcessor>();
        }

        public void SetupDb()
        {
            dc = GetService<OrderDbContext>();

            if (dc.Taxonomies.Find(BuiltInTaxonomy.CommandQueueStatus) != null)
                return;

            dc.Transaction<IOrderTable>(delegate
            {
                AddCommandQueueStatus();
                AddCommandQueueUser();
            });
        }

        void AddCommandQueueStatus()
        {
            dc.Taxonomies.Add(new TaxonomyRecord
            {
                Id = BuiltInTaxonomy.CommandQueueStatus,
                Name = "Command Queue Status"
            });
            dc.SaveChanges();

            foreach (var field in typeof(BuiltInCommandQueueStatus).GetFields())
            {
                dc.TaxonomyTerms.Add(new TaxonomyTermRecord
                {
                    Id = (Guid)field.GetValue(field),
                    TaxonomyId = BuiltInTaxonomy.CommandQueueStatus,
                    Name = SplitCamelCase(field.Name),
                    Enabled = true
                });
            }

            dc.SaveChanges();
        }

        void AddCommandQueueUser()
        {
            if (dc.Users.Find(BuiltInUser.CommandQueue) != null)
                return;

            // command queue account
            var fieldContactId = Guid.Parse("671e0138-8db5-4639-9ef8-ab26de67ecd4");
            dc.FieldContacts.Add(new FieldContactRecord
            {
                Id = fieldContactId,
                FirstName = "Command",
                LastName = "Queue",
                Email = "commandqueue@fixt.com"
            });

            var userRecord = new UserRecord
            {
                Id = BuiltInUser.CommandQueue,
                UserName = "commandqueue",
                ContactId = fieldContactId,
                OrderId = BuiltInOrder.SystemReserved,
                AppTermId = BuiltInAppType.FixtOrderSaaS
            };

            dc.Users.Add(userRecord);

            var securityRecord = new UserSecurityRecord();
            securityRecord.UserId = userRecord.Id;
            securityRecord.PasswordHash = GetHashInSha256("2311251ef30f" + securityRecord.HashSalt);
            dc.UserSecurity.Add(securityRecord);

            dc.SaveChanges();
        }
    }
}
