﻿using HM.BLL.Extensions;
using HM.BLL.Interfaces;
using HM.BLL.Models.Common;
using HM.BLL.Models.Users;
using HM.DAL.Data;
using HM.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HM.BLL.Services;

public class UserService(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    HmDbContext context
    ) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetUsersAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, string?> allRoles = await context.Roles
            .Where(r => r.Name != null)
            .AsNoTracking()
            .ToDictionaryAsync(r => r.Id, r => r.Name, cancellationToken);
        List<IdentityUserRole<string>> allUsersRoles = await context.UserRoles
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        List<User> users = await userManager.Users
            .Include(u => u.Profiles)
            .AsNoTracking().ToListAsync(cancellationToken);
        List<UserDto> userDtos = [];
        foreach (var user in users)
        {
            List<string> userRoles = [];
            foreach (var r in allUsersRoles.Where(ur => ur.UserId == user.Id))
            {
                userRoles.Add(allRoles[r.RoleId]!);
            }
            UserDto userDto = user.ToUserDto(userRoles);
            userDtos.Add(userDto);
        }
        return userDtos;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        User? user = await userManager.Users
            .Include(u => u.Profiles)
            .FirstOrDefaultAsync(u => u.Id == userId);
        UserDto? userDto = null;
        if (user != null)
        {
            IEnumerable<string> roles = await userManager.GetRolesAsync(user);
            userDto = user.ToUserDto(roles);
        }
        return userDto;
    }

    public async Task<OperationResult<UserDto>> ChangeUserRolesAsync(string userId, string[] roles)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new OperationResult<UserDto>(false, "User with such an id does not exist");
        }
        var allowedRoles = await GetAllRolesAsync();
        if (Array.Exists(roles, r => !allowedRoles.Contains(r)))
        {
            string allowed = string.Join(", ", allowedRoles);
            return new OperationResult<UserDto>(false, $"Not all roles in the list exist. Allowed roles include: {allowed}");
        }

        var oldRoles = await userManager.GetRolesAsync(user);
        var removeResult = await userManager.RemoveFromRolesAsync(user, oldRoles);
        var addResult = await userManager.AddToRolesAsync(user, roles);

        if (removeResult.Succeeded && addResult.Succeeded)
        {
            IEnumerable<string> newRoles = await userManager.GetRolesAsync(user);
            UserDto userDto = user.ToUserDto(newRoles);
            return new OperationResult<UserDto>(true, userDto);
        }
        else
        {
            string removeErrors = string.Join(" ", removeResult.Errors);
            string addErrors = string.Join(" ", addResult.Errors);
            return new OperationResult<UserDto>(false, removeErrors + " " + addErrors);
        }
    }

    public async Task<OperationResult> DeleteUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new OperationResult(false, "User with such an id does not exist.");
        }
        var profiles = await context.Profiles.Where(p => p.UserId == userId).ToListAsync();
        context.Profiles.RemoveRange(profiles);
        await context.SaveChangesAsync();
        var result = await userManager.DeleteAsync(user);
        return result.Succeeded
            ? new OperationResult(true)
            : new OperationResult(false, string.Join(" ", result.Errors));
    }

    public async Task<IEnumerable<string>> GetAllRolesAsync()
    {
        return await roleManager.Roles
            .Where(r => r.Name != null)
            .Select(r => r.Name!).ToListAsync();
    }
}
