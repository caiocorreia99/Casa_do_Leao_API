namespace CDL.Api.Helpers
{
    public class Constants
    {
        /* Route Specification */
        public const string ApiVersionPrefix = "v{v:apiVersion}";
        public const string AuthenticationRoute = $"/api/{ApiVersionPrefix}/core/login";
        public const string UserRoute = $"/api/{ApiVersionPrefix}/core/user";
        public const string EventRoute = $"/api/{ApiVersionPrefix}/core/event";
        public const string ContentRoute = $"/api/{ApiVersionPrefix}/content";
        public const string AdminCmsRoute = $"/api/{ApiVersionPrefix}/admin";
        public const string EventsApiRoute = $"/api/{ApiVersionPrefix}/events";
    }
}
