using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace smartflowsheet.PublicApi
{
    /// <summary>
    /// Represent a patient entity
    /// </summary>
    public class Patient
    {
        public Guid patientID { get; set; }
        public string name { get; set; }
        public string customField { get; set; }
        public DateTime birthday { get; set; }
        public string species { get; set; }
        public Client owner { get; set; }
        public string color { get; set; }
        public string sex { get; set; }
        public string breed { get; set; }
        /// <summary>
        /// Use externalId to associate the entity with your internal id
        /// </summary>
        public string externalID { get; set; }
    }
}