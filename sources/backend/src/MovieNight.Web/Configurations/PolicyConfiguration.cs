using Microsoft.AspNetCore.Authorization;
using MovieNight.Common.Authorization;

namespace MovieNight.Web.Configurations
{
    public class PolicyConfiguration
    {
        public static void ConfigurePolicies(AuthorizationOptions options)
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

            options.AddPolicy(Policies.IsAdmin.ToString(), policy =>
            {
                policy
                    .RequireAuthenticatedUser()
                    .RequireClaim(Claims.EmployeeId.ToString(), "true");
            });
        }
    }
}
