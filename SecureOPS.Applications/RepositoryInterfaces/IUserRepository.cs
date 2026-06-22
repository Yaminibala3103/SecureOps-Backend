using SecureOPS.Domain.Data;
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Applications.RepositoryInterfaces
{
	public interface IUserRepository
	{
		User? GetByEmail(string email);
		User? GetByResetToken(string token);
		void Add(User user);
		void Update(User user);

		Task<User> GetByIdAsync(int id);


		Task<User> GetUserWithNotificationsAsync(int userId);

		Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
	}
}



