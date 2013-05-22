using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace smartflowsheet.PublicApi
{
    /// <summary>
    /// Represents a hospitalization object
    /// </summary>
    public class Hospitalization
    {
        public Guid hospitalizationID { get; set; }
        public DateTime dateCreated { get; set; }
        public bool finished { get; set; }
        public Guid treatmentTemplateID { get; set; }
        public bool isMetricUnitSystem { get; set; }
        public string weight { get; set; }
        public int estimatedDaysOfStay { get; set; }
        public ICollection<string> diseases { get; set; }
        public Patient patient { get; set; }
        /// <summary>
        /// Use externalId to associate the entity with your internal id
        /// </summary>
        public string externalId { get; set; }
    }
}