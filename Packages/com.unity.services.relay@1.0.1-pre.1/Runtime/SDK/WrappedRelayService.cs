using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Apis.Allocations;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;

namespace Unity.Services.Relay
{
    /// <summary>
    /// The Allocations Service enables clients to connect to relay servers. Once connected, they are able to communicate with each other, via the relay servers, using the bespoke relay binary protocol.
    /// </summary>
    internal class WrappedRelayService : IRelayServiceSDK
    {
        internal IAllocationsApiClient m_AllocationsApiClient { get; set; }

        internal Configuration m_Configuration;

        private readonly IAuthenticationService m_AuthenticationService;

        internal WrappedRelayService(IAllocationsApiClient allocationsApiClient, Configuration configuration, IAuthenticationService authenticationService = null)
        {
            m_AllocationsApiClient = allocationsApiClient;
            m_Configuration = configuration;
            m_AuthenticationService = authenticationService == null ? AuthenticationService.Instance : authenticationService;
        }

        /// <inheritdoc/>
        public async Task<Allocation> CreateAllocationAsync(int maxConnections, string region = null)
        {
            EnsureSignedIn();

            try
            {
                var response = await m_AllocationsApiClient.CreateAllocationAsync(new Allocations.CreateAllocationRequest(new AllocationRequest(maxConnections, region)), m_Configuration);

                return response.Result.Data.Allocation;
            }
            catch (HttpException<ErrorResponseBody> e)
            {
                throw new RelayServiceException(e.ActualError.GetExceptionReason(), e.ActualError.GetExceptionMessage(), e);
            }
            catch (HttpException e)
            {
                if (e.Response.IsHttpError)
                {
                    throw new RelayServiceException(e.Response.GetExceptionReason(), e.Response.ErrorMessage, e);
                }

                if (e.Response.IsNetworkError)
                {
                    throw new RelayServiceException(RelayExceptionReason.NetworkError, e.Response.ErrorMessage);
                }

                throw new RequestFailedException((int)RelayExceptionReason.Unknown, "Something went wrong.", e);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="RelayServiceException">Thrown when the request successfully reach the Relay Allocation Service but results in an errorr.</exception>
        /// <exception cref="RequestFailedException">Thrown when the request does not reach the Relay Allocation Service.</exception>
        public async Task<string> GetJoinCodeAsync(Guid allocationId)
        {
            EnsureSignedIn();

            try
            {
                var response = await m_AllocationsApiClient.CreateJoincodeAsync(new Allocations.CreateJoincodeRequest(new JoinCodeRequest(allocationId)), m_Configuration);
                return response.Result.Data.JoinCode;
            }
            catch (HttpException<ErrorResponseBody> e)
            {
                throw new RelayServiceException(e.ActualError.GetExceptionReason(), e.ActualError.GetExceptionMessage(), e);
            }
            catch (HttpException e)
            {
                if (e.Response.IsHttpError)
                {
                    throw new RelayServiceException(e.Response.GetExceptionReason(), e.Response.ErrorMessage, e);
                }

                if (e.Response.IsNetworkError)
                {
                    throw new RelayServiceException(RelayExceptionReason.NetworkError, e.Response.ErrorMessage);
                }

                throw new RequestFailedException((int)RelayExceptionReason.Unknown, "Something went wrong.", e);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="RelayServiceException">Thrown when the request successfully reach the Relay Allocation Service but results in an error.</exception>
        /// <exception cref="RequestFailedException">Thrown when the request does not reach the Relay Allocation Service.</exception>
        public async Task<JoinAllocation> JoinAllocationAsync(string joinCode)
        {
            EnsureSignedIn();
            try
            {
                var response = await m_AllocationsApiClient.JoinRelayAsync(new Allocations.JoinRelayRequest(new JoinRequest(joinCode)), m_Configuration);

                return response.Result.Data.Allocation;
            }
            catch (HttpException<ErrorResponseBody> e)
            {
                throw new RelayServiceException(e.ActualError.GetExceptionReason(), e.ActualError.GetExceptionMessage(), e);
            }
            catch (HttpException e)
            {
                if (e.Response.IsHttpError)
                {
                    throw new RelayServiceException(e.Response.GetExceptionReason(), e.Response.ErrorMessage, e);
                }

                if (e.Response.IsNetworkError)
                {
                    throw new RelayServiceException(RelayExceptionReason.NetworkError, e.Response.ErrorMessage);
                }

                throw new RequestFailedException((int)RelayExceptionReason.Unknown, "Something went wrong.", e);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="RelayServiceException">Thrown when the request successfully reach the Relay Allocation Service but results in an error.</exception>
        /// <exception cref="RequestFailedException">Thrown when the request does not reach the Relay Allocation Service.</exception>
        public async Task<List<Region>> ListRegionsAsync()
        {
            EnsureSignedIn();

            try
            {
                var response = await m_AllocationsApiClient.ListRegionsAsync(new Allocations.ListRegionsRequest(), m_Configuration);

                return response.Result.Data.Regions;
            }
            catch (HttpException<ErrorResponseBody> e)
            {
                throw new RelayServiceException(e.ActualError.GetExceptionReason(), e.ActualError.GetExceptionMessage(), e);
            }
            catch (HttpException e)
            {
                if (e.Response.IsHttpError)
                {
                    throw new RelayServiceException(e.Response.GetExceptionReason(), e.Response.ErrorMessage, e);
                }

                if (e.Response.IsNetworkError)
                {
                    throw new RelayServiceException(RelayExceptionReason.NetworkError, e.Response.ErrorMessage);
                }

                throw new RequestFailedException((int)RelayExceptionReason.Unknown, "Something went wrong.", e);
            }
        }

        private void EnsureSignedIn()
        {
            if (!m_AuthenticationService.IsSignedIn)
            {
                throw new RelayServiceException(RelayExceptionReason.Unauthorized, "You are not signed in to the Authentication Service. Please sign in.");
            }
        }
    }
}