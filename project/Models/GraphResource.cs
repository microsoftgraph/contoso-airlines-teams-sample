using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoAirlines.Models
{
    public class ResultList<T>
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public T[] value { get; set; }
    }
}
