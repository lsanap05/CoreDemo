using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreDemo.Controllers
{
    [Authorize(Roles = "Administration")]
    [AutoValidateAntiforgeryToken]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private UserManager<IdentityUser> userManager;
        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = roleManager.Roles.ToList();
           
            List<Role> roleIndex = new List<Role>();
            foreach (var item in roles)
            {
                Role role1 = new Role();
                List<string> names = new List<string>();
                IdentityRole role = await roleManager.FindByIdAsync(item.Id);
                role1.RoleId = item.Id;
                role1.RoleName = item.Name;
                if (role != null)
                {
                    foreach (var user in userManager.Users)
                    {
                        if (user != null && await userManager.IsInRoleAsync(user, item.Name))
                            names.Add(user.UserName);
                    }
                }

                // output.Content.SetContent(names.Count == 0 ? "No Users" : string.Join(", ", names));
                role1.UserName = names.Count == 0 ? "No Users" : string.Join(", ", names);
                roleIndex.Add(role1);
            }
         
            return View(roleIndex);
        }
        [HttpGet]
        public IActionResult create()
        {
            var role =new IdentityRole();
            return View(role);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult create(IdentityRole identityRole)
        {
            var role = roleManager.CreateAsync(identityRole).Result;
            if (role.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View(role);
        }
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "No role found");
            return View("Index", roleManager.Roles);
        }
        public async Task<IActionResult> Update(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            List<IdentityUser> members = new List<IdentityUser>();
            List<IdentityUser> nonMembers = new List<IdentityUser>();
            foreach (IdentityUser user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }
            return View(new RoleEdit
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(RoleModification model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    IdentityUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            Errors(result);
                    }
                }
                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    IdentityUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            Errors(result);
                    }
                }
            }

            if (ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            else
                return await Update(model.RoleId);
        }
    }
}
