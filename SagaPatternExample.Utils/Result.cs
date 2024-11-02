using SagaPatternExample.Utils.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaPatternExample.Utils
{
    public class Result<T>
    {
        public EnumHttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public bool IsSuccess { get; set; }

        public static Result<T> Success(string message = "Success.") =>
            new Result<T>
            {
                Message = message,
                IsSuccess = true,
                StatusCode = EnumHttpStatusCode.Success,
            };

        public static Result<T> Success(T data, string message = "Success.") =>
            new Result<T>
            {
                Data = data,
                Message = message,
                IsSuccess = true,
                StatusCode = EnumHttpStatusCode.Success,
            };

        public static Result<T> Fail(
            string message = "Fail.",
            EnumHttpStatusCode statusCode = EnumHttpStatusCode.BadRequest
        ) =>
            new Result<T>
            {
                Message = message,
                IsSuccess = false,
                StatusCode = statusCode,
            };

        public static Result<T> Fail(Exception ex) =>
            new Result<T>
            {
                Message = ex.ToString(),
                IsSuccess = false,
                StatusCode = EnumHttpStatusCode.InternalServerError,
            };

        public static Result<T> NotFound(string message = "No data found.") =>
            Result<T>.Fail(message, EnumHttpStatusCode.NotFound);

        public static Result<T> Duplicate(string message = "Duplicate data.") =>
            Result<T>.Fail(message, EnumHttpStatusCode.Conflict);
    }
}
