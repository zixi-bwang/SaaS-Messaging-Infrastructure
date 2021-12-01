using EntityFrameworkCore.Triggers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Tables
{
    public abstract class TimedBaseRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public DateTime UpdatedTime { get; set; }

        public TimedBaseRecord()
        {
            Id = Guid.NewGuid();
            UpdatedTime = DateTime.UtcNow;
        }

        static TimedBaseRecord()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Triggers<TimedBaseRecord>.Inserting += entry =>
            {

            };

            Triggers<TimedBaseRecord>.Inserted += entry =>
            {
#if DEBUG
                Console.WriteLine($"Inserted {entry.Entity.Id} to {GetTableName(entry.Entity)} at {entry.Entity.UpdatedTime}");
#endif
            };

            Triggers<TimedBaseRecord>.Updating += entry =>
            {

            };

            Triggers<TimedBaseRecord>.Updated += entry =>
            {
                Console.WriteLine($"Updated {entry.Entity.Id} in {GetTableName(entry.Entity)} at {entry.Entity.UpdatedTime}");
            };

            Triggers<TimedBaseRecord>.Deleting += entry =>
            {

            };

            Triggers<TimedBaseRecord>.Deleted += entry =>
            {
                Console.WriteLine($"Deleted {entry.Entity.Id} from {GetTableName(entry.Entity)} at {entry.Entity.UpdatedTime}");
            };
        }

        // Has issue, disabled
        /*static IRecordValidator GetValidator(Assembly[] assemblies, IServiceProvider service, TimedBaseRecord entity)
        {
            var validatorTypeName = $"{entity.GetType().Name}Validator";

            // return from cache
            if (recordValidatorsCache.ContainsKey(validatorTypeName))
                return recordValidatorsCache[validatorTypeName];

            IRecordValidator validator = null;

            var config = (IConfiguration)service.GetService(typeof(IConfiguration));
            var assemblyNames = config.GetValue<string>("Decoupler:Assemblies").Split(',');
            var myAssemblies = assemblies.Where(x => assemblyNames.Contains(x.GetName().Name)).ToList();

            for (var i = 0; i < myAssemblies.Count; i++)
            {
                var validatorType = myAssemblies[i].GetTypes().FirstOrDefault(x => x.Name == validatorTypeName);
                if (validatorType != null)
                {
                    validator = (IRecordValidator)service.GetService(validatorType);
                    if (validator == null)
                        throw new Exception($"Could not resolve a service of '{validatorType.Name}', register the dependency injection before using it.");
                    break;
                }
            }

            if (validator == null)
                Console.WriteLine($"Could not find a validator for {entity.GetType().Name}.", Color.Red);

            recordValidatorsCache[validatorTypeName] = validator;
            return validator;
        }*/

        static string GetTableName(object entity)
        {
            return entity.GetType().Name.Replace("Record", "");
        }
    }
}
