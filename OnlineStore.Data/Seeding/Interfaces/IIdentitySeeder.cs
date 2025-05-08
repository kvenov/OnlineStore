﻿using Microsoft.AspNetCore.Identity;

namespace OnlineStore.Data.Seeding.Interfaces
{
    public interface IIdentitySeeder<TUser, TRole>
        where TUser : class, new()
        where TRole : class, new()
    {

        UserManager<TUser> UserManager { get; }

        RoleManager<TRole> RoleManager { get; }
    }
}
