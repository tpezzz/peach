using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace Tree.Api {
    internal static class SwaggerConfiguration {
        internal static void ConfigureSwagger(this HttpConfiguration config, string displayedApiVersion) {
            config.EnableSwagger(c => {
                c.SingleApiVersion(displayedApiVersion, "BodhiTree Api");
                c.DocumentFilter<ODataMeasurementOperation>();
                c.OperationFilter<TokenOperation>();
                c.OperationFilter<AssignOAuth2TokenRequirement>();
            }).EnableSwaggerUi(c => { });
        }
    }

    internal class AssignOAuth2TokenRequirement : IOperationFilter {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription) {
            var authorizeAttributes = apiDescription
                .ActionDescriptor.GetCustomAttributes<AuthorizeAttribute>();

            if (!authorizeAttributes.Any())
                return;

            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            operation.parameters.Add(GetOAuthHeaderParameter());
        }

        public static Parameter GetOAuthHeaderParameter() {
            return new Parameter {
                name = "Authorization",
                @in = "header",
                type = "string",
                required = true,
                @default = "Bearer ..."
            };
        }
    }

    internal class TokenOperation : IOperationFilter {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription) {
            if (operation.operationId == "Token_Post") {
                operation.consumes.Clear();
                operation.consumes.Add("application/x-www-form-urlencoded");

                operation.produces.Clear();
                operation.produces.Add("application/x-www-form-urlencoded");

                operation.parameters.Clear();
                operation.parameters.Add(new Parameter {
                    name = "Body",
                    @in = "body",
                    type = "string",
                    required = true,
                    @default = "grant_type=password&access_code=...&username=..."
                });
            }
        }
    }

    internal class ODataMeasurementOperation : IDocumentFilter {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer) {
            var parameters = GetODataQueryParameters();
            parameters.Add(AssignOAuth2TokenRequirement.GetOAuthHeaderParameter());

            swaggerDoc.paths.Add("/odata/Measurement", new PathItem {
                get = new Operation {
                    tags = new List<string> { "ODataReport" },
                    produces = new List<string> { "application/json" },
                    parameters = parameters
                }
            });
        }

        private static List<Parameter> GetODataQueryParameters() {
            return new List<Parameter> {
                GetODataParameter("expand", "Expands related entities inline."),
                GetODataParameter("filter", "Filters the results, based on a Boolean condition."),
                GetODataParameter("select", "Selects which properties to include in the response."),
                GetODataParameter("orderby", "Sorts the results."),
                GetODataParameter("top", "Returns only the first n results."),
                GetODataParameter("skip", "Skips the first n results."),
                GetODataParameter("apply", "Applies a series of consecutive transformations"),
                GetODataParameter("count", "Includes a count of the matching results in the response.", "boolean")
            };
        }

        private static Parameter GetODataParameter(string name, string description, string type = "string") {
            return new Parameter {
                type = type,
                name = "$" + name,
                description = description,
                required = false,
                @in = "query"
            };
        }
    }
}