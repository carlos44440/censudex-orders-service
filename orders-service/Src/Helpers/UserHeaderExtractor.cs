using Grpc.Core;
using orders_service.Src.DTOs;

namespace orders_service.Src.Helpers
{
    public class UserHeaderExtractor
    {
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
                Id = userId,
                Name = userName,
                Role = userRole,
                EmailAddress = userEmail
            };
        }

        public string GetUserId(ServerCallContext context)
        {
            var headers = context.RequestHeaders;

            var userId = headers.FirstOrDefault(h => h.Key == "x-user-id")?.Value;

            if (string.IsNullOrWhiteSpace(userId)) throw new RpcException(new Status(StatusCode.Unauthenticated, "Missing user id"));

            return userId;
        }
    }
}