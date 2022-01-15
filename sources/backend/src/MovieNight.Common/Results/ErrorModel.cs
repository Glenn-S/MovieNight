using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MovieNight.Common.Results
{
    /// <summary>
    /// Model for holding a collection of errors returned from an api result.
    /// </summary>
    public class ErrorModel
    {
        public IEnumerable<ErrorDetail> Errors { get; set; }
    }

    /// <summary>
    /// A class for holding the specific error and relevant details
    /// regarding the failure.
    /// </summary>
    public class ErrorDetail
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }

        public static ErrorDetail MN4000(string detail)
        {
            return new ErrorDetail
            {
                Code = "MN-4000",
                Message = "The form provided was null",
                Detail = detail
            };
        }

        public static ErrorDetail MN4001(string detail)
        {
            return new ErrorDetail
            {
                Code = "MN-4001",
                Message = "An authentication error has occurred",
                Detail = detail
            };
        }

        public static ErrorDetail MN4004(string detail)
        {
            return new ErrorDetail
            {
                Code = "MN-4004",
                Message = "User could not be found",
                Detail = detail
            };
        }

        public static ErrorDetail MN4220(string detail)
        {
            return new ErrorDetail
            {
                Code = "MN-4220",
                Message = "A validation error has occurred",
                Detail = detail
            };
        }

        public static ErrorDetail MN5000(string detail)
        {
            return new ErrorDetail
            {
                Code = "MN-5000",
                Message = "A system level error has occurred while trying to fullfill the request",
                Detail = detail
            };
        }

        public static IEnumerable<ErrorDetail> GetValidationErrors(IEnumerable<ValidationFailure> errors)
        {
            foreach (var error in errors)
                yield return MN4220($"Error Code: {error.ErrorCode} Attempted Value: {error.AttemptedValue} Message: {error.ErrorMessage}");
        }

        public static IEnumerable<ErrorDetail> GetIdentityErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
                yield return MN4220($"Error Code: {error.Code} Message: {error.Description}");
        }

        public static IEnumerable<ErrorDetail> GetIdentityInternalErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
                yield return MN5000($"Error Code: {error.Code} Message: {error.Description}");
        }
    }
}
