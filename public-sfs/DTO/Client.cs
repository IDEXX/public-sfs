using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace smartflowsheet.PublicApi
{
    /// <summary>
    /// Represent a pet owner object
    /// </summary>
    public class Client
    {
        public string nameLast { get; set; }
        public string nameFirst { get; set; }
        public Guid clientID { get; set; }
        public string homePhone { get; set; }
        public string workPhone { get; set; }
        public string customField { get; set; }
        /// <summary>
        /// Use externalId to associate the entity with your internal id
        /// </summary>
        public string externalID { get; set; }
    }
}