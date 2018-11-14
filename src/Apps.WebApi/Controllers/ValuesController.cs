using Apps.Common;
using Apps.IBLL;
using Apps.WebApi.Core;
using Unity.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Apps.WebApi.Controllers
{
    public class ValuesController : BaseApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        [HttpGet]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/put/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/delete/5
        public void Delete(int id)
        {
        }
    }
}
