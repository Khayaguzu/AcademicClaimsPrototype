using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace AcademicClaimsPrototype.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : Attribute, IAsyncActionFilter
    {
        private readonly HashSet<string> _roles;
        public const string SessionEmail = "CurrentUserEmail";
        public const string SessionRole = "CurrentUserRole";


        public AuthorizeRoleAttribute(string rolesCsv)
        {
            _roles = rolesCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var http = context.HttpContext;
            var email = http.Session.GetString(SessionEmail);
            var role = http.Session.GetString(SessionRole);


            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }


            if (_roles.Count > 0 && !_roles.Contains(role))
            {
                // Not authorized for this role
                context.Result = new RedirectToActionResult("Login", "Account", new { denied = true });
                return;
            }


            await next();
        }
    }
}