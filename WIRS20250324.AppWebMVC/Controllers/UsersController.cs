using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WIRS20250324.AppWebMVC.Models;

namespace WIRS20250324.AppWebMVC.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsersController : Controller
    {
        private readonly Test20250324DbContext _context;

        public UsersController(Test20250324DbContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index(User user, int topRegistro = 10)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(user.Username))
                query = query.Where(s => s.Username.Contains(user.Username));
            if (!string.IsNullOrWhiteSpace(user.Email))
                query = query.Where(s => s.Email.Contains(user.Email));
            if (!string.IsNullOrWhiteSpace(user.Role))
                query = query.Where(s => s.Role.Contains(user.Role));

            if (topRegistro > 0)
                query = query.Take(topRegistro);

            return View(await query.ToListAsync());




        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            // Lista de roles disponibles (Administrador y GERENTE)
            var roles = new List<SelectListItem>
    {
        new SelectListItem { Text = "Administrador", Value = "Administrador" },
        new SelectListItem { Text = "GERENTE", Value = "GERENTE" }
    };

            // Pasar la lista de roles a la vista
            ViewData["Roles"] = roles;

            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Email,Password,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = CalcularHashMD5(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        private string CalcularHashMD5(string passwordHash)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(passwordHash);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // "x2" convierte el byte en una cadena hexadecimal de dos caracteres.
                }
                return sb.ToString();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CerrarSession()
        {
            // Hola mundo
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            user.Password = CalcularHashMD5(user.Password);
            var usuarioAuth = await _context.
                Users.
                FirstOrDefaultAsync(s => s.Email == user.Email && s.Password == user.Password);
            if (usuarioAuth != null && usuarioAuth.Id > 0 && usuarioAuth.Email == user.Email)
            {
                var claims = new[] {
                    new Claim(ClaimTypes.Name, usuarioAuth.Email),
                    new Claim("Id", usuarioAuth.Id.ToString()),
                     new Claim("Nombre", usuarioAuth.Username),
                    new Claim(ClaimTypes.Role, usuarioAuth.Role)
                    };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "El email o contraseña estan incorrectos");
                return View();
            }
        }
        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Email,PasswordHash,Role")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            var userUpdate = await _context.Users
              .FirstOrDefaultAsync(m => m.Id == user.Id);

            try
            {
                userUpdate.Username = user.Username;
                userUpdate.Email = user.Email;
                userUpdate.Role = user.Role;
                _context.Update(userUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                {
                    return NotFound();
                }
                else
                {
                    return View(user);
                }
            }
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

    }
}