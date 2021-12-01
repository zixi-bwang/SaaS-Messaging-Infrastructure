using ContentFoundation.CommandQueue.Models;
using ContentFoundation.Events;
using Decoupling.EventBrokers;
using Newtonsoft.Json;
using Repository.Enums;
using Repository.Tables.EventQueue;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentFoundation.CommandQueue
{
    public class SampleCommandQueueProcessor : ICommandQueueProcessor
    {
        private readonly EventBroker _eventBroker;
        private readonly DbContext _dc;

        public SampleCommandQueueProcessor(DbContext dc,
            EventBroker eventBroker)
        {
            _eventBroker = eventBroker;
            _dc = dc;
        }

        public Guid Proceed(CommandQueueRecord command)
        {

            // enqueue
            var output = _eventBroker.Send<CommandQueueEnqueueEvent, Guid>(new CommandQueueEnqueueEvent
            {
                Data = new CommandQueueEnqueueModel
                {
                    Processor = nameof(SampleCommandQueueProcessor),
                    Subject = "Test Command",
                    AppId = BuiltInAppType.SaaSClient1,
                    Label = "Test label",
                    Content = "{Test content}",
                }
            });

            var eventData = JsonConvert.DeserializeObject<object>(output);

            Serilog.Log.Debug("Your real business logic goes here...");
            _dc.Transaction<IVendorTable>(delegate
            {
                var invoice = _invoiceMgr.GetInvoice(new InvoiceQueryModel { WorkOrderId = eventData.WoModel.Id });
                // TODO: FIXT-3586 remove below two lines when client implement new submit invoice API
                //invoice.InvoiceNumber = clientInvoice.InvoiceNumber;
                //_dc.SaveChanges();

                var invoiceRef = new AppInvoiceRefIdRecord
                {
                    InvoiceId = invoice.Id,
                    InvoiceRefId = clientInvoice.Id,
                    InvoiceRefNum = clientInvoice.InvoiceNumber,
                    AppTermId = eventData.InvoiceModel.AppTermId
                };
                _dc.AppInvoiceRefs.Add(invoiceRef);
                _dc.SaveChanges();
            });

            return command.Id;
        }
    }
}
