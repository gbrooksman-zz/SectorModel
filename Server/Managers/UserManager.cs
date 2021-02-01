using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using SectorModel.Shared.Entities;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using SectorModel.Server.Data;

namespace SectorModel.Server.Managers
{
	public interface IUserManager
	{
		Task<List<User>> GetAllUsers();
		Task<User> GetOneByEmail(string email);
		Task<User> GetOneById(Guid id);
		Task<User> Save(User user);
		Task<bool> Validate(User user);
	}



	public class UserManager : IUserManager
	{		
		public UserManager()
		{

		}

		public async Task<List<User>> GetAllUsers()
		{
			List<User> users = new();

			try
			{
				using var db = new AppDBContext();
				users = await db.Users.Where(u => u.IsActive == true).ToListAsync();
			}
			catch (Exception ex)
			{
				Log.Error("UserManager::GetAllUsers", ex);
				throw;
			}

			return users;
		}

		public async Task<User> GetOneById(Guid id)
		{
			User user = new();

			try
			{
				using var db = new AppDBContext();
				user = await db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
			}
			catch (Exception ex)
			{
				Log.Error("UserManager::GetOneById", ex);
				throw;
			}

			return user;
		}

		public async Task<User> GetOneByEmail(string email)
		{
			User user = new();

			try
			{
				using var db = new AppDBContext();
				user = await db.Users.Where(u => u.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();
			}
			catch (Exception ex)
			{
				Log.Error("UserManager::GetOneByEmail", ex);
				throw;
			}

			return user;
		}


		public async Task<bool> Validate(User user)
		{
			try
			{
				using var db = new AppDBContext();
				user = await db.Users.Where(u => u.Email.ToLower() == user.Email.ToLower()
										&& u.Password == user.Password
										&& u.IsActive == true)
										.FirstOrDefaultAsync();
			}
			catch (Exception ex)
			{
				Log.Error("UserManager::Validate", ex);
				throw;
			}

			return user != null;
		}

		public async Task<User> Save(User user)
		{
			try
			{
				using (var db = new AppDBContext())
				{
					if (user.Id == Guid.Empty)
					{
						db.Add(user);
					}
					else
					{
						db.Update(user);
					}
					await db.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				Log.Error("UserManager::Save", ex);
				throw;
			}

			return user;
		}
	}
}
