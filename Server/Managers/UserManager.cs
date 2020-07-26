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

namespace SectorModel.Server.Managers
{
    public class UserManager : BaseManager
    {
        
        private readonly AppSettings appSettings;

        public UserManager(IMemoryCache _cache, IConfiguration _config, AppSettings _appSettings) : base(_cache, _config)
        {
            appSettings = _appSettings;
        }

        public async Task<List<User>> GetAllUsers()
        {
            List<User> users = new List<User>();
            
            try
            {
                using var db = new ReadContext(appSettings);
                users = await db.Users.Where(u => u.IsActive == true).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error("UserManager::GetAllUsers",ex);
                throw;
            } 

            return users;
        }

        public async Task<User> GetOneById(Guid id)
        {
            User user = new User();
            
            try
            {
                using var db = new ReadContext(appSettings);
                user = await db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                Log.Error("UserManager::GetOneById",ex);
                throw;
            }
            
            return user;
        }

        public async Task<User> GetOneByName(string userName)
        {
            User user = new User();
              
            try
            {
                using var db = new ReadContext(appSettings);
                user = await db.Users.Where(u => u.UserName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                Log.Error("UserManager::GetOneByName",ex);
                throw;
            }
            
            return user;
        }
    

        public async Task<bool> Validate(string userName, string password)
        {
            User user = new User();

            try
            {
                using var db = new ReadContext(appSettings);
                user = await db.Users.Where(u => u.UserName.ToLower() == userName.ToLower()
                                        && u.Password == password
                                        && u.IsActive == true)
                                        .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error("UserManager::Validate",ex);
                throw;
            }
            
            return user != null;
        }

        public async Task<User> Save(User user)
        {           
            try
            {
                using (var db = new WriteContext(appSettings))
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
            catch(Exception ex)
            {
                Log.Error("UserManager::Save",ex);
                throw;
            }
            
            return user;
        }
    }
}
