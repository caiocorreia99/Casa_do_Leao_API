namespace CDL.Api.Helpers
{
    public class Constants
    {
        /* Route Specification */
        public const string ApiVersionPrefix = "v{v:apiVersion}";
        public const string AuthenticationRoute = $"/api/{ApiVersionPrefix}/core/login";
        public const string UserRoute = $"/api/{ApiVersionPrefix}/core/user";        

    }
}
