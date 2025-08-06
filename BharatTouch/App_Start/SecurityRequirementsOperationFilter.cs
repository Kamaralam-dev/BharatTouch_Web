using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Swashbuckle.Swagger;
using System.Web.Http.Description;

namespace BharatTouch.App_Start
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.security == null)
                operation.security = new List<IDictionary<string, IEnumerable<string>>>();

            var authRequirements = new Dictionary<string, IEnumerable<string>>
            {
                { "Bearer", new string[] { } }
            };

            operation.security.Add(authRequirements);
        }
    }
}