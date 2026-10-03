using Alexapps.SkinCare.Entities.LABs;
using Alexapps.SkinCare.Entities.Users;
using Alexapps.SkinCare.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Alexapps.SkinCare.Seeders
{
    // Inherit from IDataSeedContributor to ensure it runs during data seeding
    public class LABsSeeder(
     IRepository<Lab, Guid> labRepository,
     IRepository<LabBranch, Guid> branchRepository, 
     IdentityUserManager userManager) : IDataSeedContributor, ITransientDependency
    {
        public async Task SeedAsync(DataSeedContext context)
        {
            

            var labsToSeed = new List<(string Name, string Email, string Address, double Lat, double Lng)>
    {
        ("Alpha Skin Lab", "info@alphalab.com", "12 Tahrir St, Downtown, Cairo", 30.0444, 31.2357),
        ("Perfect Check Lab", "contact@perfectcheck.com", "45 Dokki St, Giza", 29.9931986, 31.1244409)
    };

            foreach (var labData in labsToSeed)
            {
                var existingUser = await userManager.FindByEmailAsync(labData.Email);

                if (existingUser == null)
                {
                
                    var labUser = new User(labData.Name, "012" + new Random().Next(1000000, 9999999), labData.Email, labData.Email);
                    labUser.Activate();
                    labUser.SetEmailConfirmed(true);

                    var createResult = await userManager.CreateAsync(labUser, "Lab@123!");

                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(labUser, RoleEnum.LAB);

                        var newLab = await labRepository.InsertAsync(new Lab
                        {
                            UserId = labUser.Id,
                            IsActive = true
                        }, autoSave: true);

                        await branchRepository.InsertAsync(new LabBranch
                        {
                            LabId = newLab.Id,
                            Name = labData.Name + " - Main Branch",
                            ContactNumber = labUser.PhoneNumber,
                            FullAddress = labData.Address,
                            Latitude = labData.Lat,
                            Longitude = labData.Lng,
                            GoogleMapsUrl = $"https://www.google.com/maps/search/?api=1&query={labData.Lat},{labData.Lng}",
                            IsPrimary = true,
                            IsActive = true
                        }, autoSave: true);
                    }
                }
                else
                {
                    
                    var existingLab = await labRepository.FirstOrDefaultAsync(l => l.UserId == existingUser.Id);

                    if (existingLab != null)
                    {
                        
                        var hasBranches = await branchRepository.AnyAsync(b => b.LabId == existingLab.Id);

                        if (!hasBranches)
                        {
                            
                            await branchRepository.InsertAsync(new LabBranch
                            {
                                LabId = existingLab.Id,
                                Name = labData.Name + " - Main Branch",
                                ContactNumber = existingUser.PhoneNumber ?? "01200000000",
                                FullAddress = labData.Address,
                                Latitude = labData.Lat,
                                Longitude = labData.Lng,
                                GoogleMapsUrl = $"https://www.google.com/maps/search/?api=1&query={labData.Lat},{labData.Lng}",
                                IsPrimary = true,
                                IsActive = true
                            }, autoSave: true);
                        }
                    }
                }
            }
        }
    }
}