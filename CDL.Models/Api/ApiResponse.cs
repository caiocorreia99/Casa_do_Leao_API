using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static CDL.Models.Helpers.Constants;

namespace CDL.Models.Api
{
    public class ApiResponse<T> where T : class, new()
    {
        public DateTimeOffset ResponseDate { get; set; } = DateTimeOffset.Now;
        public HttpStatusCode HttpCode { get; set; }
        public string HttpCodeName => HttpCode.ToString();
        public InternalCode Code { get; set; }
        public string CodeName => Code.ToString();
        public int? AuxCode { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public string? Exception { get; set; }
        public string? InnerException { get; set; }
        public string? Stack { get; set; }

        public static ApiResponse<T> GetSuccessResponse(string message = "OK", Exception exception = default, T? data = default, bool success = true)
            => GetResponse(InternalCode.Success, HttpStatusCode.OK, message, exception, data, success);

        public static ApiResponse<T> GetErrorResponse(InternalCode code, HttpStatusCode httpCode, string message, Exception exception, T? data = null, bool success = false)
            => GetResponse(code, httpCode, message, exception, data, success);

        public static ApiResponse<T> GetResponse(InternalCode code, HttpStatusCode httpCode, string message, Exception exception, T? data, bool success)
        {
            var response = new ApiResponse<T>()
            {
                Code = code,
                HttpCode = httpCode,
                Message = message,
                Success = success,
                Data = data,
            };

            // Bind Stack
            if (exception != null)
            {
                response.Exception = exception.GetType().Name;

#if DEBUG
                response.InnerException = exception.InnerException?.ToString();
                response.Stack = exception.StackTrace;
#endif
            }

            return response;
        }
    }
}
