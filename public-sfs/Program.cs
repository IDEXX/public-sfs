using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using smartflowsheet.api.model.objects;

namespace public_sfs
{
    class Program
    {
        /// <summary>
        /// Base server url:
        ///     - sandbox :     "https://sandbox.smartflowsheet.com/api/v3"; 
        ///     - production:   "https://www.smartflowsheet.com/api/v3"
        /// </summary>
        public static string serverUrl = "https://sandbox.smartflowsheet.com/api/v3";

        /// <summary>
        /// Each EMR has special developer key received from Smart Flow Sheet support
        /// </summary>
        public static string emrApiKey = "emrApiKey";

        /// <summary>
        /// Each clinic has a special key to be used for integration with EMR. 
        /// This key is generated after clinic registration and available at 
        /// account page (https://www.smartflowsheet.com/Account/Info)
        /// </summary>
        public static string clinicApiKey = "clinicApiKey";

        /// <summary>
        /// Each clinic has special connection string which used for receive clinic's events via Azure Service Bus.
        /// This connection string received from Smart Flow Sheet support
        /// </summary>
        public static string sbConnectionString = "serviceBusConnectionString";

        /// <summary>
        /// Each clinic has special queue name which used for receive clinic's events via Azure Service Bus.
        /// Queue name received from Smart Flow Sheet support (usually it's equal to clinicApiKey).
        /// </summary>
        public static string sbQueueName = clinicApiKey;


        static void Main(string[] args)
        {
            // Build HTTP headers with auth info
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("emrApiKey", emrApiKey);
            httpClient.DefaultRequestHeaders.Add("clinicApiKey", clinicApiKey);

            // Please always provide "timezoneName" header. 
            // To download reports we must specify the time zone,
            // in which we want all dates to appear. 
            // http://en.wikipedia.org/wiki/List_of_tz_database_time_zones
            httpClient.DefaultRequestHeaders.Add("timezoneName", "Europe/Helsinki");

            // Sample 1. Create inventory item ( http://veterinarium.github.io/#create-or-update-single-inventory-item )
            // CreateInventoryItem(httpClient, "emrId_Cefazolin");

            // Sample 1a. Delete inventory item  ( http://veterinarium.github.io/#delete-single-inventory-item )
            // DeleteInventoryItem(httpClient, "emrId_Cefazolin");

            // Sample 2. Upload doctors ( http://veterinarium.github.io/#create-or-update-multiple-medics )
            // UploadMedics(httpClient);

            // Sample 3. Create hospitalization ( http://veterinarium.github.io/#create-a-patient ) 
            // Hospitalization hosp = CreateHospitalization(httpClient,  "someExternalId");

            // Sample 4. Update hospitalization
            // UpdateHospitalization(httpClient, hosp);

            // Sample 5. Download medical records report for the patient ( http://veterinarium.github.io/#download-the-medical-records-report )
            // DownloadMedicalRecordsReport(httpClient, hosp);

            // Sample 6. Get hospitalization ( http://veterinarium.github.io/#get-hospitalization )
            // GetHospitalization(httpClient, "someExternalId");

            // Sample 7. Discharge hospitalization 
            // DischargeHospitalization(httpClient, "someExternalId");

            // Sample 8. Get treatment templates ( http://veterinarium.github.io/#retreive-active-treatment-templates ) 
            // GetTreatmentTemplates(httpClient);

            // Sample 9. Get departments ( http://veterinarium.github.io/#retreive-existing-departments ) 
            // GetDepartments(httpClient);

            // Sample 9. Get anesthetics ( http://veterinarium.github.io/#retreive-anesthetic-sheet-reports ) 
            // GetAnesthetics(httpClient, "someExternalId");

            // Sample 10. Get EMR events from Azure Service Bus
            // GetEventsFromServiceBus();
        }

        public static void CreateInventoryItem(HttpClient httpClient, string emrInventoryItemId)
        {
            InventoryItem item = new InventoryItem()
            {
                Id = emrInventoryItemId,
                Name = "Cefazolin",
                Concentration = 100,
                ConcentrationMeasure = "mg",
                ConcentrationVolume = "ml"
            };

            var url = serverUrl + "/inventoryitem";
            Console.WriteLine("Making web request to " + url);
            var result = httpClient.PostAsJsonAsync<InventoryItem>(url, item).Result;

            // Output result

            Console.WriteLine("Http result code: {0}", result.StatusCode);
            Console.WriteLine("Http content:");
            Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            Console.WriteLine("\n\nPress any key to proceed...");
            Console.ReadKey();
        }

        public static void DeleteInventoryItem(HttpClient httpClient, string emrInventoryItemId)
        {
            var url = serverUrl + "/inventoryitem/" + emrInventoryItemId;
            Console.WriteLine("Making web request to " + url);
            var result = httpClient.DeleteAsync(url).Result;

            // Output result

            Console.WriteLine("Http result code: {0}", result.StatusCode);
            Console.WriteLine("Http content:");
            Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            Console.WriteLine("\n\nPress any key to proceed...");
            Console.ReadKey();
        }

        public static void UploadMedics(HttpClient httpClient)
        {
            var medics = new Medics()
            {
                Id = "some-id",
                Objects = new List<Medic>()
            };
            ((List<Medic>)medics.Objects).Add(new Medic()
            {
                ExternalID = "emrIdm3",
                Name = "Ivan",
                MedicType = "doctor"
            });
            ((List<Medic>)medics.Objects).Add(new Medic()
            {
                ExternalID = "emrIdm4",
                Name = "Dr. George",
                MedicType = "doctor"
            });
            var url = serverUrl + "/medics";
            Console.WriteLine("Making web request to " + url);
            var result = httpClient.PostAsJsonAsync<Medics>(url, medics).Result;

            Console.WriteLine("Http result code: {0}", result.StatusCode);
            Console.WriteLine("Http content:");
            Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            Console.WriteLine("\n\nPress any key to proceed...");
            Console.ReadKey();
        }

        public static Hospitalization CreateHospitalization(HttpClient httpClient, string hospitalizationExternalId)
        {
            // Create hospitalization 
            var hosp = new Hospitalization();
            hosp.dateCreated = DateTime.Now;
            hosp.externalID = hospitalizationExternalId;
            hosp.estimatedDaysOfStay = 1;
            hosp.weightUnits = "kg";
            hosp.weight = 5.8;
            hosp.diseases = new List<string>() { "high temperature", "vomiting" };
            hosp.cageNumber = "#14";
            hosp.color = "#ffb4c4";
            hosp.dateMovedToDepartment = DateTime.Now;
            hosp.cageNumber = "123";
            hosp.departmentID = 0;
            hosp.treatmentTemplateName = "Default Q6";
            hosp.resuscitate = ResuscitateStatus.BLS;

            var patient = new Patient();
            patient.birthday = DateTime.Now.AddYears(-2);
            patient.breed = "Hound";
            patient.color = "Brown";
            patient.externalID = "myPatientId1002";
            patient.name = "Jordi";
            patient.species = "Canin";
            patient.sex = "M";

            var owner = new Client();
            owner.externalID = "myClientId1003";
            owner.nameFirst = "Jack";
            owner.nameLast = "Dow";
            owner.workPhone = "555-55-55";

            patient.owner = owner;
            hosp.patient = patient;

            // Send to server and receive response
            var url = serverUrl + "/hospitalization";
            Console.WriteLine("Making web request to " + url);
            var result = httpClient.PostAsJsonAsync<Hospitalization>(url, hosp).Result;

            // Output result
            Console.WriteLine("Http result code: {0}", result.StatusCode);
            Console.WriteLine("Http content:");
            Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            Console.WriteLine("\n\nPress any key to update this hospitalization...");
            Console.ReadKey();

            return result.Content.ReadAsAsync<Hospitalization>().Result;
        }

        public static void UpdateHospitalization(HttpClient httpClient, Hospitalization hosp)
        {
            // Add disease, and set custom field for the patient.
            // Fields that should not be updated, must be nullified. 
            // If you want to reset some text fields then pass empty string ""
            hosp.diseases.Add("diarrhea");
            hosp.color = "#d3b2f1";

            hosp.patient.species = null;                // species will not be updated
            hosp.patient.owner = null;                  // owner will not be updated
            hosp.patient.customField = "CS 123";        // new value for customField
            hosp.patient.color = "";                    // reset color

            var url = serverUrl + "/hospitalization";
            Console.WriteLine("Making web request to " + url);
            var result = httpClient.PutAsJsonAsync<Hospitalization>(url, hosp).Result;

            // Output result
            Console.WriteLine("Http result code: {0}", result.StatusCode);
            Console.WriteLine("Http content:");
            Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void GetHospitalization(HttpClient httpClient, string hospitalizationExternalId)
        {
            var url = serverUrl + "/hospitalization/" + hospitalizationExternalId;
            Console.WriteLine("Making web request to " + url);
            var result = httpClient.GetAsync(url).Result;

            // Output result
            Console.WriteLine("Http result code: {0}", result.StatusCode);
            Console.WriteLine("Http content:");
            string resultString = result.Content.ReadAsStringAsync().Result;
            //File.WriteAllText("hospitalization.json", resultString);
            Console.WriteLine(resultString);
            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void DischargeHospitalization(HttpClient httpClient, string hospitalizationExternalId)
        {
            var url = serverUrl + "/hospitalization/discharge/" + hospitalizationExternalId;
            Console.WriteLine("Making web request to " + url);
            var result = httpClient.PostAsync(url, null).Result;

            // Output result
            Console.WriteLine("Http result code: {0}", result.StatusCode);
            Console.WriteLine("Http content:");
            string resultString = result.Content.ReadAsStringAsync().Result;
            File.WriteAllText("hospitalization.json", resultString);
            Console.WriteLine(resultString);
            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void GetDepartments(HttpClient httpClient)
        {
            var url = serverUrl + "/departments/";
            Console.WriteLine("Making web request to " + url);
            var result = httpClient.GetAsync(url).Result;

            // Output result
            Console.WriteLine("Http result code: {0}", result.StatusCode);
            Console.WriteLine("Http content:");
            Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void GetTreatmentTemplates(HttpClient httpClient)
        {
            var url = serverUrl + "/treatmenttemplates/";
            Console.WriteLine("Making web request to " + url);
            var result = httpClient.GetAsync(url).Result;

            // Output result
            Console.WriteLine("Http result code: {0}", result.StatusCode);
            Console.WriteLine("Http content:");
            Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void DownloadMedicalRecordsReport(HttpClient httpClient, Hospitalization hosp)
        {
            var url = serverUrl + "/hospitalization/" + hosp.externalID + "/medicalrecordsreport";
            Console.WriteLine("Start downloading medical records report...");
            Stream pdfStream = httpClient.GetStreamAsync(url).Result;
            using (var fileStream = File.Create("report.pdf"))
            {
                pdfStream.CopyTo(fileStream);
            }
            Console.WriteLine("File downloaded. You can find it in the /bin/{Configuration}/ folder");
            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }

        public static void GetAnesthetics(HttpClient httpClient, string hospitalizationExternalId)
        {
            var url = serverUrl + "/hospitalization/" + hospitalizationExternalId + "/anesthetics";
            Console.WriteLine("Making web request to " + url);
            var result = httpClient.GetAsync(url).Result;

            // Output result
            Console.WriteLine("Http result code: {0}", result.StatusCode);
            Console.WriteLine("Http content:");
            string resultString = result.Content.ReadAsStringAsync().Result;
            File.WriteAllText("anesthetics.json", resultString);
            Console.WriteLine(resultString);
            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void GetEventsFromServiceBus()
        {
            Task.Factory.StartNew(() =>
            {
                Queue.BeginProcessingMessages(sbConnectionString, sbQueueName);
            });

            // NOTE: Thread.Sleep used only for demo purposes
            Thread.Sleep(TimeSpan.FromSeconds(100));
            Queue.EndProcessingMessages();

            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
