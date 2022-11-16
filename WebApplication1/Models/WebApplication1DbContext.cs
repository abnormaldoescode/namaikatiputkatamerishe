using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplication1.Models
{

    public class WebApplication1DbContext : IdentityDbContext<ApplicationUser>
    {
        public WebApplication1DbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public virtual IDbSet<Car> Cars { get; set; }

        public static WebApplication1DbContext Create()
        {
            return new WebApplication1DbContext();
        }
    }
}