using System;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;

namespace Unity.Services.Relay
{
    internal static class ApiErrorExtender
    {
        public static RelayExceptionReason GetExceptionReason(this ErrorResponseBody error)
        {
            RelayExceptionReason reason = RelayExceptionReason.Unknown;

            if (error.Code != (int)RelayExceptionReason.NoError)
            {
                if (Enum.IsDefined(typeof(RelayExceptionReason), error.Code))
                {
                    reason = (RelayExceptionReason)error.Code;
                }
            }
            else if (Enum.IsDefined(typeof(RelayExceptionReason), error.Status))
            {
                reason = (RelayExceptionReason)error.Status;
            }

            return reason;
        }

        public static RelayExceptionReason GetExceptionReason(this HttpClientResponse error)
        {
            RelayExceptionReason reason = RelayExceptionReason.Unknown;

            if (error.IsHttpError && Enum.IsDefined(typeof(RelayExceptionReason), (int)error.StatusCode))
            {
                reason = (RelayExceptionReason)error.StatusCode;
            }
            else if (error.IsNetworkError)
            {
                reason = RelayExceptionReason.NetworkError;
            }

            return reason;
        }

        public static string GetExceptionMessage(this ErrorResponseBody error)
        {
            return $"{error.Title}: {error?.Detail}";
        }
    }
}