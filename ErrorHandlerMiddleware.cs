using HMS_Data_Layer.HMS_IData;
using HMS_View_Models.Models;
using Realms.Sync.Exceptions;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace HMS_Api
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                UserContext user = new UserContext();

                // If the user is authenticated and we have the identity, extract user details
                if (context?.User?.Identity is ClaimsIdentity identity)
                {
                    TimeZoneInfo timeZone;
                    string timeZoneId = context?.Request?.Headers?["TimeZone"];

                    if (!string.IsNullOrEmpty(timeZoneId))
                    {
                        try
                        {
                            // Attempt to find the system timezone by the provided timeZoneId header
                            timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                        }
                        catch (TimeZoneNotFoundException)
                        {
                            // Fallback if the TimeZoneId in the request is invalid
                            timeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                        }
                    }
                    else
                    {
                        // Use "India Standard Time" as the default timezone if the "TimeZone" header doesn't exist
                        timeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    }

                    // Create the user context based on identity claims
                    user = new UserContext
                    {
                        AppUserId = Convert.ToInt32(identity.FindFirst("AppUserID")?.Value),
                        AppUserName = identity.FindFirst("AppUserName")?.Value,
                        Role_Id = Convert.ToInt32(identity.FindFirst("RoleId")?.Value),
                        RoleName = identity.FindFirst("RoleName")?.Value,
                        isAuthenticated = Convert.ToBoolean(identity.FindFirst("isAuthenticated")?.Value),
                        LastLoginTime = Convert.ToDateTime(identity.FindFirst("LastLoginTime")?.Value),
                        TimeZone = timeZone,
                        UserLocalTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone)
                    };
                }

                // Set response status based on the type of exception
                switch (error)
                {
                    case AppException e:  // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:  // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:  // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                // Error logging (for debugging and audit purposes)
                string allerror = "Message =" + error?.Message + "\\n" + error?.StackTrace;
                //await userManager.SaveError(allerror, user);

                // Return the error message in the response body
                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
