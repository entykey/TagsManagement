using Microsoft.AspNetCore.Mvc;
using TagsManagement.Repositories.Filters;

namespace TagsManagement.Repositories
{
    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute() : base(typeof(CustomAuthorizeFilter))
        {
        }
    }
}
