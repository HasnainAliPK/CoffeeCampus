using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CoffeeCampus.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration; // IConfiguration = henter data fra andre filer(Appsetings.json)

        [BindProperty] //Automatisere databinding og forenkler koden
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        // Injectet IConfiguration via kontructeren
        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Email og adgangskode skal udfyldes.";
                return Page();
            }

            // Retrieve the connection string from appsettings.json
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Database connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to retrieve the user role based on email and password
                string query = "SELECT Role, PasswordHash FROM Users WHERE Email = @Email"; // Only fetch the role and hashed password
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@Email", Email);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var storedPasswordHash = reader["PasswordHash"].ToString();
                            var role = reader["Role"].ToString();

                            // Compare the hashed password
                            var passwordHasher = new PasswordHasher<object>(); // Use PasswordHasher to validate hashed password
                            var verificationResult = passwordHasher.VerifyHashedPassword(null, storedPasswordHash, Password);

                            if (verificationResult == PasswordVerificationResult.Failed)
                            {
                                ErrorMessage = "Forkert email eller adgangskode.";
                                return Page();
                            }

                            // Login successful
                            if (role == "Admin")
                                return RedirectToPage("/AdminDashboard");
                            else
                                return RedirectToPage("/UserDashboard");
                        }
                        else
                        {
                            ErrorMessage = "Forkert email eller adgangskode.";
                            return Page();
                        }
                    }
                }
            }
        }
    }
}
