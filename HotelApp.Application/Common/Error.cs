using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HotelApp.Application.Common
{
    public sealed record Error(string Code, string Description, ErrorType ErrorType = ErrorType.Failure)
    {
        public static Error Failure(string Code = "General.Failure", string Description = "General Failure Has Occurred")
            => new Error(Code, Description, ErrorType.Failure);

        public static Error Validation(string Code = "General.Validation", string Description = "General Validation Has Occurred")
            => new Error(Code, Description, ErrorType.Validation);

        public static Error NotFound(string Code = "General.NotFound", string Description = "Record Not Found")
            => new Error(Code, Description, ErrorType.NotFound);

        public static Error Conflict(string Code = "General.Conflict", string Description = "General Conflict Has Occurred")
            => new Error(Code, Description, ErrorType.Conflict);

        public static Error Unauthorized(string Code = "General.Unauthorized", string Description = "Access Is Denied Due To Bad Authorization")
            => new Error(Code, Description, ErrorType.Unauthorized);
        public static Error Forbidden(string Code = "General.Forbidden", string Description = "This Operation Is Forbidden")
             => new Error(Code, Description, ErrorType.Forbidden);
        public static Error InvaildCredentias(string Code = "General.InvaildCredentias", string Description = "Provided Credentias Are Invaild")
             => new Error(Code, Description, ErrorType.InvaildCredentias);



    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4,
        Forbidden = 5,
        InvaildCredentias = 6

    }
}