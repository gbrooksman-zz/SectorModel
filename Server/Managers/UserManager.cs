using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using SectorModel.Shared.Entities;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace SectorModel.Server.Managers
{
    public class UserManager : BaseManager
    {
        public UserManager(IMemoryCache _cache, IConfiguration _config) : base(_cache, _config)
        {

        }

        public async Task<List<User>> GetAllUsers()
        {
            List<User> users = new List<User>();
            
            try
            {
                using var db = new ReadContext();
                users = await db.Users.Where(u => u.Active == true).ToListAsync();

                //using (NpgsqlConnection db = new NpgsqlConnection(connString))
                //{
                //    var qResult = await db.QueryAsync<User>(@"SELECT * 
                //                                            FROM users 
                //                                            WHERE active = True").ConfigureAwait(false);
                //    mgrResult = qResult.ToList();

                //}
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
                using var db = new ReadContext();
                user = await db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //{
                //    mgrResult = await db.QueryFirstOrDefaultAsync<User>(@"SELECT * FROM users 
                //                                                                 WHERE id = @p1", 
                //                                                                 new { p1 = id }).ConfigureAwait(false);
                //}
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
            //User mgrResult = new User();
            User user = new User();
              
            try
            {
                using var db = new ReadContext();
                user = await db.Users.Where(u => u.UserName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();

                //using (NpgsqlConnection db = new NpgsqlConnection(connString))
                //{
                //   user = await db.QueryFirstOrDefaultAsync<User>(@"SELECT * 
                //                                FROM users 
                //                                WHERE user_name = @p1", 
                //                                new { p1 = userName }).ConfigureAwait(false);
                //}
                // mgrResult = user;
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
                using var db = new ReadContext();
                user = await db.Users.Where(u => u.UserName.ToLower() == userName.ToLower()
                                        && u.Password == password
                                        && u.Active == true)
                                        .FirstOrDefaultAsync();


                //using (NpgsqlConnection db = new NpgsqlConnection(connString))
                //{
                //    User user = await db.QueryFirstOrDefaultAsync<User>(@"SELECT * 
                //                            FROM users 
                //                            WHERE user_name = @p1 
                //                            AND password = @p2 
                //                            AND active = true", 
                //                            new { p1 = userName, p2 = password }).ConfigureAwait(false);

                //    mgrResult = (user != default(User));
                //}
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
                if (user.Id == Guid.Empty)
                {
                    using var db = new WriteContext();
                    db.Add(user);
                    await db.SaveChangesAsync();

                    //using NpgsqlConnection db = new NpgsqlConnection(connString);
                    //{                        
                    //    string sql = $"INSERT INTO USERS (USER_NAME, PASSWORD)  VALUES ( @p1 , @p2)";
                    //    await db.ExecuteAsync(sql, new { p1 = user.UserName, p2 = user.Password}).ConfigureAwait(false);
                    //}
                }
                else
                {
                    using var db = new WriteContext();
                    db.Update(user);
                    await db.SaveChangesAsync();

                    //using NpgsqlConnection db = new NpgsqlConnection(connString);
                    //{
                    //    await db.UpdateAsync(user).ConfigureAwait(false);
                    //}
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
