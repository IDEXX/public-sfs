using smartflowsheet.api.model.events;
using smartflowsheet.api.model.objects;
using smartflowsheet.queue.api.model.consumers;
using smartflowsheet.queue.api.model.messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace public_sfs
{
    public class QueueConsumer : BaseQueueConsumer<EmrEventMessage>
    {
        public override void ProcessMessage(EmrEventMessage message)
        {
            Console.WriteLine("\r\nMessage Received");
            Console.WriteLine("Clinic API Key:" + message.ClinicApiKey);
            Console.WriteLine("Event type" + message.EventType);

            switch (message.EventType)
            {
                case EventTypes.TreatmentRecordsMultiple:
                    Treatments treatments = message.Object as Treatments;
                    foreach (Treatment t in treatments.Objects)
                    {
                        processTreatment(t);
                    }
                    break;

                case EventTypes.TreatmentRecordEntered:
                    Treatment treatment = message.Object as Treatment;
                    processTreatment(treatment);
                    break;

                case EventTypes.HospitalizationsDischarged:
                    Hospitalizations hospitalizations = message.Object as Hospitalizations;
                    foreach (Hospitalization h in hospitalizations.Objects)
                    {
                        processHospitalization(h);
                    }
                    break;

                default:
                    throw new ApplicationException("Unsuppoted event type");
                    break;
            }
        }

        private void processTreatment(Treatment t)
        {
            Console.WriteLine(String.Format("Treatment: ParameterName - {0}, ParameterTypeName - {1}, Value - {2}", t.ParameterName, t.ParameterTypeName, t.Value));
        }

        private void processHospitalization(Hospitalization h)
        {
            Console.WriteLine(String.Format("Hospitalization: Patient.Name - {0}", h.patient.name));
        }
    }
}
