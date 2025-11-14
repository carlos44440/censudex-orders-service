using Grpc.Core;
using orders_service.Src.DTOs;

namespace orders_service.Src.Helpers
{
    /// <summary>
    /// Utilidad para extraer información del usuario desde los encabezados enviados en llamadas gRPC.
    /// </summary>
    public class UserHeaderExtractor
    {
        /// <summary>
        /// Obtiene todos los datos del usuario desde los headers de la solicitud gRPC.
        /// </summary>
        /// <param name="context">Contexto de la llamada gRPC.</param>
        /// <returns>Retorna un DTO con los datos del usuario.</returns>
        /// <exception cref="RpcException">
        /// Se lanza cuando:
        /// <list type="bullet">
        /// <item>No se encuentra alguno de los headers requeridos: <c>x-user-id</c>, <c>x-user-name</c>, <c>x-user-role</c>, <c>x-user-email</c>.</item>
        /// </list>
        /// </exception>
        public UserDataDto GetUserData(ServerCallContext context)
        {
            var headers = context.RequestHeaders;

            var userId = headers.FirstOrDefault(h => h.Key == "x-user-id")?.Value;
            var userName = headers.FirstOrDefault(h => h.Key == "x-user-name")?.Value;
            var userRole = headers.FirstOrDefault(h => h.Key == "x-user-role")?.Value;
            var userEmail = headers.FirstOrDefault(h => h.Key == "x-user-email")?.Value;

            // Validar que existan los headers críticos
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userEmail))
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Missing user context headers"));
            }

            return new UserDataDto
            {
                Id = Guid.Parse(userId),
                Name = userName,
                Role = userRole,
                EmailAddress = userEmail
            };
        }

        /// <summary>
        /// Obtiene únicamente el Id del usuario desde los headers de la solicitud gRPC.
        /// </summary>
        /// <param name="context">Contexto de la llamada gRPC.</param>
        /// <returns>Retorna el identificador del usuario.</returns>
        /// <exception cref="RpcException">
        /// Se lanza cuando:
        /// <list type="bullet">
        /// <item>No se encuentra el header requerido: <c>x-user-id</c>.</item>
        /// </list>
        /// </exception>
        public string GetUserId(ServerCallContext context)
        {
            var headers = context.RequestHeaders;

            var userId = headers.FirstOrDefault(h => h.Key == "x-user-id")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Missing user id"));
            }

            return userId;
        }
    }
}
