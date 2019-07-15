using smartflowsheet.api.model.events;
using smartflowsheet.api.model.objects;
using smartflowsheet.queue.api.model.consumers;
using smartflowsheet.queue.api.model.messages;
using System;
using System.Threading.Tasks;

namespace public_sfs
{
    public class QueueConsumer : BaseQueueConsumer<EmrEventMessage>
    {
        public override async Task ProcessMessage(EmrEventMessage message)
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

                case EventTypes.MedicsImported:
                    Medics medics = message.Object as Medics;
                    foreach(Medic m in medics.Objects)
                    {
                        processMedic(m);
                    }
                    break;

                case EventTypes.InventoryItemsImported:
                    InventoryItems inventoryItems = message.Object as InventoryItems;
                    foreach(InventoryItem ii in inventoryItems.Objects)
                    {
                        processInventoryItem(ii);
                    }
                    break;

                case EventTypes.NotesEntered:
                    Notes notes = message.Object as Notes;
                    foreach(Note n in notes.Objects)
                    {
                        processNote(n);
                    }
                    break;

                default:
                    throw new ApplicationException("Unsuppoted event type");
            }
        }

        private void processNote(Note n)
        {
            Console.WriteLine($"Note: Id - {n.NoteID.ToString()}, Text - {n.Text}, Status - {n.NoteStatus}");
        }

        private void processTreatment(Treatment t)
        {
            Console.WriteLine($"Treatment: ParameterName - {t.ParameterName}, ParameterTypeName - {t.ParameterTypeName}, Value - {t.Value}");
        }

        private void processHospitalization(Hospitalization h)
        {
            Console.WriteLine($"Hospitalization: Patient.Name - {h.patient.name}");
        }

        private void processMedic(Medic m)
        {
            Console.WriteLine($"Medic.Name - {m.Name}");
        }

        private void processInventoryItem(InventoryItem ii)
        {
            Console.WriteLine($"InventoryItem.Name - {ii.Name}");
        }
    }
}
