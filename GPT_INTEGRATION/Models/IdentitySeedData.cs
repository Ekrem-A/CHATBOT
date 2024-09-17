using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GPT_INTEGRATION.Models
{    public static class IdentitySeedData
    {
        // Admin kullanıcısı oluşturuldu
        private const string adminUser = "Admin";

        private const string adminPassword = "Secret123";
        // IdentityUser oluşturuldu
        public static async void IdentityUser(IApplicationBuilder app)
        {
            // Veritabanı oluşturuldu
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();

            // Veritabanı yoksa oluşturuldu
            if (context.Database.GetAppliedMigrations().Any())
            {
                context.Database.Migrate();
            }
            //User bilgisi oluşturuldu
            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(adminUser);
            // Kullanıcı yoksa oluşturuldu
            if (user == null)
            {

                user = new IdentityUser(adminUser)
                {
                    UserName = adminUser,
                    Email = "admin@hotmail.com",
                    PhoneNumber = "1234567890"
                };
            }


            await userManager.CreateAsync(user, adminPassword);
        }
    }
}
