using MarketRecipesAPI.Data;
using MarketRecipesAPI.DTO;
using MarketRecipesAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MarketRecipesAPI.Services
{
    public class UserService
    {
        private readonly MarketRecipesContext _context;
        private readonly IConfiguration _configuration;

        public UserService(MarketRecipesContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> CreateUser(User user)
        {
            // Verificando se o nome de usuário já existe
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                throw new System.Exception("O nome de usuário já está em uso.");
            }

            user.Password = HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateUser(int id, UserUpdateDTO userUpdateDTO)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            // Verificando se o novo nome de usuário já está em uso por outro usuário
            if (await _context.Users.AnyAsync(u => u.Username == userUpdateDTO.Username && u.Id != id))
            {
                throw new System.Exception("O nome de usuário já está em uso.");
            }

            existingUser.Username = userUpdateDTO.Username;
            existingUser.Password = HashPassword(userUpdateDTO.Password);

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> AuthenticateUser(string username, string password)
        {
            var hashedPassword = HashPassword(password);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == hashedPassword);

            if (user == null)
                return null;

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
