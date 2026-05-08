using EclatantLaundry.Models;
using System.Linq;

namespace EclatantLaundry.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public static Supervisor? CurrentUser { get; set; }

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public Supervisor Login(string username, string password)
        {
            try
            {
                var user = _context.Supervisor
                    .FirstOrDefault(s => s.USERNAME == username && s.PASSWORD == password);

                if (user != null)
                {
                   CurrentUser = user;
                   UserSession.ID_SUPERVISOR = user.ID_SUPERVISOR;
                   UserSession.NAMA_SUPERVISOR = user.NAMA_SUPERVISOR;

                }

                return user!;
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public void Register(string nama, string username, string password)
        {
            var isUsernameTaken = _context.Supervisor.Any(s => s.USERNAME == username);

            if (isUsernameTaken)
            {
                throw new Exception("Username Sudah Digunakan!");
            }

            var supervisor = new Supervisor
            {
                NAMA_SUPERVISOR = nama,
                USERNAME = username,
                PASSWORD = password
            };

            _context.Supervisor.Add(supervisor);
            _context.SaveChanges();
        }
    }

}

