using smartflowsheet.PublicApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace public_sfs
{
    class Program
    {
        /// <summary>
        /// Base server url:
        ///     - sandbox :     "https://sfs-public.azurewebsites.net/api/v2"; 
        ///     - production:   "https://www.smartflowsheet.com/api/v2"
        /// </summary>
        public static string serverUrl = "https://sfs-public.azurewebsites.net/api/v2";

        /// <summary>
        /// Each EMR has special developer key received from Smart Flow Sheet support
        /// </summary>
        public static string emrApiKey = "emrApiKey"; 
        
        /// <summary>
        /// Each clinic has a special key to be used for integration with EMR. 
        /// This key is generated after clinic registration and available at 
        /// account page (https://www.smartflowsheet.com/Account/Info)
        /// </summary>
        public static string clinicApiKey = "clinicApiKey"; // each clinic has

        static void Main(string[] args)
        {
            // Build HTTP headers with auth info
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("emrApiKey", emrApiKey);
            httpClient.DefaultRequestHeaders.Add("clinicApiKey", clinicApiKey);
            
            // Create dto with hospitalization information
            Hospitalization hosp = GenerateHospitalization();

            // Send to server and receive response
            var url = serverUrl + "/sfshospitalizations";
            Console.WriteLine("Making a web request to " + url);
            var result = httpClient.PostAsJsonAsync<Hospitalization>(url, hosp).Result;

            // Output result
            Console.WriteLine("Http result code: {0}", result.StatusCode);
            Console.WriteLine("Http content:");
            Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }

        public static Hospitalization GenerateHospitalization()
        {
            var hosp = new Hospitalization();
            hosp.dateCreated = DateTime.Now;
            hosp.diseases = new List<string>() { "high temperature", "vomiting" };
            hosp.externalId = "myDbId";
            hosp.finished = false;
            hosp.isMetricUnitSystem = true;
            hosp.estimatedDaysOfStay = 1;
            hosp.weight = ((Double)2.5).ToString();

            var patient = new Patient();
            patient.birthday = DateTime.Now.AddYears(-2);
            patient.breed = "Hound";
            patient.color = "Brown";
            patient.externalID = "myPatientId";
            patient.name = "Rocky";
            patient.sex = "M";
            patient.species = "Dog";

            var owner = new Client();
            owner.externalID = "myClientId";
            owner.nameFirst = "John";
            owner.nameLast = "Doe";
            owner.workPhone = "555-55-55";

            patient.owner = owner;
            hosp.patient = patient;

            return hosp;
        }
    }
}
