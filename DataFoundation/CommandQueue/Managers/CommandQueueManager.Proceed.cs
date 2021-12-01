using System.Linq;
using System;
using static Utilities.Util;
using Repository.Enums;
using Decoupling.Decouplers;
using Microsoft.Extensions.Configuration;
using Utilities.Exceptions;

namespace ContentFoundation.CommandQueue.Managers
{
    public partial class CommandQueueManager
    {
        public void Proceed(Guid id)
        {
            var command = _dc.CommandQueue.FirstOrDefault(x => x.Id == id);
            if (command == null)
                return;

            var processor = GetProcessorByName(command.Processor);
            
            try
            {
                command.UpdatedTime = DateTime.UtcNow;
                command.StatusTermId = BuiltInCommandQueueStatus.Processing;
                _dc.SaveChanges();

                if (processor == null)
                    throw new Exception($"Can't find processor {command.Processor}");

                processor.Proceed(command);
                command.StatusTermId = BuiltInCommandQueueStatus.Completed;
                command.StatusMessage = null;
            }
            catch (FixtClientApiException ex)
            {
                command.StatusTermId = BuiltInCommandQueueStatus.Failed;
                command.StatusMessage = ex.Message?.SubstringMax(256);
            }
            catch (Exception ex)
            {
                command.StatusTermId = BuiltInCommandQueueStatus.Failed;
                command.StatusMessage = ex.Message?.SubstringMax(256);
            }
            finally
            {
                command.UpdatedTime = DateTime.UtcNow;
                command.NumOfExecution++;
                _dc.SaveChanges();
            }
        }

        ICommandQueueProcessor GetProcessorByName(string processor)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var serviceProvider = Decoupler.ServiceProvider;
            var config = (IConfiguration)serviceProvider.GetService(typeof(IConfiguration));
            var decouplerSettings = (DecouplerSettings)serviceProvider.GetService(typeof(DecouplerSettings));
            var myAssemblies = assemblies.Where(x => decouplerSettings.Assemblies.Contains(x.GetName().Name)).ToList();

            for (var i = 0; i < myAssemblies.Count; i++)
            {
                var processorType = myAssemblies[i].GetTypes()
                    .Where(x => x.GetInterface(nameof(ICommandQueueProcessor)) != null)
                    .FirstOrDefault(x => x.Name == processor);
                if (processorType != null)
                {
                    var processorService = (ICommandQueueProcessor)serviceProvider.GetService(processorType);
                    if (processorService != null)
                        return processorService;
                }
            }

            Serilog.Log.Error($"Could not resolve a processor of '{processor}'.");
            return null;
        }
    }
}
