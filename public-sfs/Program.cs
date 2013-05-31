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
        public static string emrApiKey = "test";
        public static string clinicApiKey = "test";
        public static string serverUrl = "https://sfs-public.azurewebsites.net/api/v2";

        static void Main(string[] args)
        {
            Hospitalization hosp = GenerateHospitalization();
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("emrApiKey", emrApiKey);
            httpClient.DefaultRequestHeaders.Add("clinicApiKey", clinicApiKey);
            var url = serverUrl + "/sfshospitalizations";
            Console.WriteLine("Making a web request to " + url);
            var result = httpClient.PostAsJsonAsync<Hospitalization>(url, hosp).Result;
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
