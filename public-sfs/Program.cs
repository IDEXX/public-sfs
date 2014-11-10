using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using smartflowsheet.api.model.objects;
using smartflowsheet.api.model.events;

namespace public_sfs
{
    class Program
    {
        /// <summary>
        /// Base server url:
        ///     - sandbox :     "https://sfs-public.azurewebsites.net/api/v3"; 
        ///     - production:   "https://www.smartflowsheet.com/api/v3"
        /// </summary>
        public static string serverUrl = "https://sfs-public.azurewebsites.net/api/v3";

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

        static void Main(string[] args)
        {
            // Build HTTP headers with auth info
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("emrApiKey", emrApiKey);
            httpClient.DefaultRequestHeaders.Add("clinicApiKey", clinicApiKey);
            
            // Sample 1. Create inventory item
            CreateInventoryItem(httpClient);

            // Sample 2. Create hospitalization 
            Hospitalization hosp = CreateHospitalization(httpClient);

            // Sample 3. Update hospitalization
            UpdateHospitalization(httpClient, hosp);

            // Sample 4. Download medical records report for the patient
            DownloadMedicalRecordsReport(httpClient, hosp);
        }

        public static void CreateInventoryItem(HttpClient httpClient)
        {
            InventoryItem item = new InventoryItem()
            {
                Id = "emrId_Cefazolin",
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

        public static Hospitalization CreateHospitalization(HttpClient httpClient)
        {
            // Create hospitalization 
            var hosp = new Hospitalization();
            hosp.dateCreated = DateTime.Now;
            hosp.externalID = "myDbId1001";
            hosp.estimatedDaysOfStay = 1;
            hosp.weightUnits = "kg";
            hosp.weight = 5.8;
            hosp.diseases = new List<string>() { "high temperature", "vomiting" };

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

        public static void DownloadMedicalRecordsReport(HttpClient httpClient, Hospitalization hosp)
        {
            // To download the report we must specify the time zone,
            // in which we want all dates to appear. 
            // http://en.wikipedia.org/wiki/List_of_tz_database_time_zones
            httpClient.DefaultRequestHeaders.Add("timezoneName", "Europe/Helsinki");

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
    }
}
