using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace PdfApi.Handling
{
    public class HandlerResult<T>
    {
        public bool IsSuccess { get; set; }
        public T Value { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }

        public IActionResult ToActionResult()
        {
            if(IsSuccess)
            {
                return new OkObjectResult(this.Value);
            }
            else
            {
                if(Exception is null)
                {
                    return new ObjectResult(this.ErrorMessage)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                else
                {
                    return new ObjectResult(new { this.ErrorMessage, this.Exception })
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
            }
        }

        public static HandlerResult<T> CreateFailureResult(string errorMessge, Exception exception = null)
            => new HandlerResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessge,
                Exception = exception,
                Value = default
            };

        public static HandlerResult<T> CreateSuccessResult(T value)
            => new HandlerResult<T>
            {
                IsSuccess = true,
                ErrorMessage = null,
                Exception = null,
                Value = value
            };
    }
}
