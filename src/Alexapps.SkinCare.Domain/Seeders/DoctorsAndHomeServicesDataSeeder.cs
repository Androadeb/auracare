using Alexapps.SkinCare.Entities.HomeServices;
using Alexapps.SkinCare.Entities.Users;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Alexapps.SkinCare.Seeders
{
    public class DoctorsAndHomeServicesDataSeeder : ITransientDependency
    {
        private readonly IRepository<Doctor, Guid> _doctorRepository;
        private readonly IRepository<HomeService, Guid> _homeServiceRepository;
        private readonly IRepository<HomeServiceProvider, Guid> _homeServiceProviderRepository;
        private readonly IRepository<Specialty, Guid> _specialtyRepository;
        private readonly IRepository<DoctorSchedule, Guid> _doctorScheduleRepository;
        private readonly IRepository<HomeServiceSchedule, Guid> _homeServiceScheduleRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IDataFilter _dataFilter;

        public DoctorsAndHomeServicesDataSeeder(
            IRepository<Doctor, Guid> doctorRepository,
            IRepository<HomeService, Guid> homeServiceRepository,
            IRepository<HomeServiceProvider, Guid> homeServiceProviderRepository,
            IRepository<Specialty, Guid> specialtyRepository,
            IRepository<DoctorSchedule, Guid> doctorScheduleRepository,
            IRepository<HomeServiceSchedule, Guid> homeServiceScheduleRepository,
            IdentityUserManager userManager,
            IDataFilter dataFilter)
        {
            _doctorRepository = doctorRepository;
            _homeServiceRepository = homeServiceRepository;
            _homeServiceProviderRepository = homeServiceProviderRepository;
            _specialtyRepository = specialtyRepository;
            _doctorScheduleRepository = doctorScheduleRepository;
            _homeServiceScheduleRepository = homeServiceScheduleRepository;
            _userManager = userManager;
            _dataFilter = dataFilter;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _doctorRepository.AnyAsync() || await _homeServiceRepository.AnyAsync())
            {
                return;
            }

            await SeedHomeServicesAsync();
            await SeedDoctorsAsync();
        }

        private async Task SeedDoctorsAsync()
        {
            using (_dataFilter.Disable<ISoftDelete>())
            {
                var existingHomeProviders = await _homeServiceProviderRepository.GetListAsync();
                foreach (var hp in existingHomeProviders) await _homeServiceProviderRepository.HardDeleteAsync(hp, autoSave: true);

                var existingSchedules = await _doctorScheduleRepository.GetListAsync();
                foreach (var sch in existingSchedules) await _doctorScheduleRepository.HardDeleteAsync(sch, autoSave: true);

                var existingDoctors = await _doctorRepository.GetListAsync();
                foreach (var doc in existingDoctors)
                {
                    var user = await _userManager.FindByIdAsync(doc.UserId.ToString());
                    if (user != null) await _userManager.DeleteAsync(user);
                    await _doctorRepository.HardDeleteAsync(doc, autoSave: true);
                }

                var existingSpecialties = await _specialtyRepository.GetListAsync();
                foreach (var spec in existingSpecialties) await _specialtyRepository.HardDeleteAsync(spec, autoSave: true);
            }

            var homeLaser = await _homeServiceRepository.FirstOrDefaultAsync(s => s.NameEn == "Home Laser");
            var royalSkinCleaning = await _homeServiceRepository.FirstOrDefaultAsync(s => s.NameEn == "Royal Skin Cleaning");

            // تم إزالة Qualifications من قائمة البيانات
            var doctorUsers = new List<(string Name, string Email, string SpecialtyEn, string SpecialtyAr, double Rating, string Image, string DescriptionEn, string DescriptionAr, int NumberOfPatients, int NumberOfReviews, int YearsOfExperience, string Gender, DoctorTypeEnum Type, Guid? HomeServiceId)>
            {
                ("Dr. Sarah Johnson", "sarah.johnson@skincare.com", "Dermatologist", "طبيب جلدية", 4.9, "/assets/img/doctors/sarah.jpg",
                    "Board-certified dermatologist specializing in skin conditions, acne treatment, and anti-aging therapies.",
                    "طبيبة جلدية معتمدة متخصصة في أمراض الجلد وعلاج حب الشباب والعلاجات المضادة للشيخوخة.",
                    1250, 320, 12, "female", DoctorTypeEnum.SessionProvider, null),

                ("Dr. Michael Chen", "michael.chen@skincare.com", "Skin Surgeon", "جراح تجميل", 4.8, "/assets/img/doctors/michael.jpg",
                    "Expert skin surgeon with extensive experience in reconstructive and cosmetic skin procedures.",
                    "جراح جلد خبير ذو خبرة واسعة في الإجراءات الجلدية الترميمية والتجميلية.",
                    980, 245, 15, "male", DoctorTypeEnum.SessionProvider, null),

                ("Dr. Emily Davis", "emily.davis@skincare.com", "Cosmetic Dermatologist", "طبيب تجميل", 4.7, "/assets/img/doctors/emily.jpg",
                    "Specialist in cosmetic dermatology, laser treatments, and non-invasive rejuvenation procedures.",
                    "متخصصة في طب الجلد التجميلي وعلاجات الليزر وإجراءات التجديد غير الجراحية.",
                    850, 198, 8, "female", DoctorTypeEnum.SessionProvider, null),

                ("Dr. James Wilson", "james.wilson@skincare.com", "Laser Specialist", "أخصائي ليزر", 4.9, "/assets/img/doctors/sarah.jpg",
                    "Specialist in home laser treatments and advanced skin therapy.",
                    "أخصائي في علاجات الليزر المنزلية والعلاج المتقدم للبشرة.",
                    500, 120, 10, "male", DoctorTypeEnum.ServiceProvider, homeLaser?.Id),

                ("Dr. Linda White", "linda.white@skincare.com", "Skin Care Expert", "خبير عناية بالبشرة", 4.8, "/assets/img/doctors/emily.jpg",
                    "Expert in deep cleansing and anti-aging home services.",
                    "خبيرة في التنظيف العميق وخدمات مكافحة الشيخوخة المنزلية.",
                    400, 95, 7, "female", DoctorTypeEnum.ServiceProvider, royalSkinCleaning?.Id),

                ("Dr. Robert Fox", "robert.fox@skincare.com", "Laser Expert", "خبير ليزر", 4.8, "/assets/img/doctors/michael.jpg",
                    "Specialist in therapeutic laser treatments and skin rejuvenation.",
                    "متخصص في علاجات الليزر العلاجية وتجديد شباب الجلد.",
                    600, 150, 12, "male", DoctorTypeEnum.ServiceProvider, homeLaser?.Id),

                ("Dr. Jane Cooper", "jane.cooper@skincare.com", "Esthetician", "أخصائية تجميل", 4.9, "/assets/img/doctors/emily.jpg",
                    "Specialist in medical-grade facials and advanced skin cleaning.",
                    "متخصصة في علاجات الوجه الطبية وتنظيف البشرة المتقدم.",
                    750, 180, 9, "female", DoctorTypeEnum.ServiceProvider, royalSkinCleaning?.Id),

                ("Dr. Cody Fisher", "cody.fisher@skincare.com", "Laser Specialist", "أخصائي ليزر", 4.7, "/assets/img/doctors/sarah.jpg",
                    "Expert in home-based laser hair removal and skin care.",
                    "خبير في إزالة الشعر بالليزر في المنزل والعناية بالبشرة.",
                    450, 110, 6, "male", DoctorTypeEnum.ServiceProvider, homeLaser?.Id),

                ("Dr. Esther Howard", "esther.howard@skincare.com", "Skin Therapist", "معالج جلدية", 4.8, "/assets/img/doctors/michael.jpg",
                    "Clinical skin therapist focusing on holistic home skin care.",
                    "معالج جلدي إكلينيكي يركز على العناية الشاملة بالبشرة في المنزل.",
                    550, 135, 11, "female", DoctorTypeEnum.ServiceProvider, royalSkinCleaning?.Id),

                ("Dr. Jenny Wilson", "jenny.wilson@skincare.com", "Dermatology Specialist", "أخصائي جلدية", 4.9, "/assets/img/doctors/emily.jpg",
                    "Dedicated to providing high-quality dermatological home services.",
                    "مكرسة لتقديم خدمات جلدية منزلية عالية الجودة.",
                    900, 220, 14, "female", DoctorTypeEnum.ServiceProvider, homeLaser?.Id)
            };

            foreach (var docData in doctorUsers)
            {
                var specialty = await _specialtyRepository.FirstOrDefaultAsync(s => s.NameEn == docData.SpecialtyEn);
                if (specialty == null)
                {
                    specialty = await _specialtyRepository.InsertAsync(new Specialty { NameEn = docData.SpecialtyEn, NameAr = docData.SpecialtyAr }, autoSave: true);
                }

                var newUser = new User(docData.Name, "000000" + new Random().Next(1000, 9999), docData.Email, docData.Email);
                newUser.ProfileImage = docData.Image;
                newUser.Gender = docData.Gender;

                var result = await _userManager.CreateAsync(newUser, "Doctor@123!");
                if (!result.Succeeded) continue;

                var doctor = new Doctor
                {
                    UserId = newUser.Id,
                    SpecialtyId = specialty.Id,
                    Rating = docData.Rating,
                    DescriptionEn = docData.DescriptionEn,
                    DescriptionAr = docData.DescriptionAr,
                    NumberOfPatients = docData.NumberOfPatients,
                    NumberOfReviews = docData.NumberOfReviews,
                    YearsOfExperience = docData.YearsOfExperience,
                    Type = docData.Type,
                    // تم ترك القائمة فارغة ليتم إضافتها لاحقاً عبر الـ Endpoint
                    Qualifications = new List<DoctorQualification>()
                };

                var insertedDoctor = await _doctorRepository.InsertAsync(doctor, autoSave: true);

                if (docData.Type == DoctorTypeEnum.ServiceProvider && docData.HomeServiceId.HasValue)
                {
                    await _homeServiceProviderRepository.InsertAsync(new HomeServiceProvider
                    {
                        DoctorId = insertedDoctor.Id,
                        HomeServiceId = docData.HomeServiceId.Value
                    }, autoSave: true);
                }

                var random = new Random();
                foreach (var day in (DayOfWeek[])Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (day == DayOfWeek.Friday) continue;
                    await _doctorScheduleRepository.InsertAsync(new DoctorSchedule
                    {
                        DoctorId = insertedDoctor.Id,
                        DayOfWeek = day,
                        StartTime = new TimeSpan(random.Next(8, 11), 0, 0),
                        EndTime = new TimeSpan(random.Next(16, 19), 0, 0),
                        IsAvailable = true
                    }, autoSave: true);
                }
            }
        }

        private async Task SeedHomeServicesAsync()
        {
            var existingServiceSchedules = await _homeServiceScheduleRepository.GetListAsync();
            foreach (var cur in existingServiceSchedules) await _homeServiceScheduleRepository.HardDeleteAsync(cur, autoSave: true);

            var services = new List<HomeService>
            {
                new HomeService
                {
                    NameAr = "ليزر منزلي",
                    NameEn = "Home Laser",
                    Price = 299,
                    Duration = "60-90",
                    DescriptionAr = "خدمة إزالة الشعر بالليزر في منزلك مع أحدث الأجهزة المعتمدة والآمنة.",
                    DescriptionEn = "Laser hair removal service at your home with the latest approved and safe devices.",
                    ServicesAr = "تنظيف وتعقيم المنطقة, جلسة ليزر كاملة بأحدث الأجهزة, كريم مهدئ بعد الجلسة, استشارة مجانية",
                    ServicesEn = "Area leaning and sterilization, Full laser session with latest devices, Soothing cream after session, Free consultation",
                    NotesAr = "تجنب التعرض للشمس قبل الجلسة بـ 48 ساعة, حلاقة المنطقة قبل الجلسة بيوم واحد",
                    NotesEn = "Avoid sun exposure 48 hours before session, Shave the area one day before session",
                    Image = "/assets/img/homeservices/laser.jpg"
                },
                new HomeService
                {
                    NameAr = "تنظيف بشرة ملكي",
                    NameEn = "Royal Skin Cleaning",
                    Price = 350,
                    Duration = "45-60",
                    DescriptionAr = "تنظيف عميق للبشرة مع ماسكات مغذية ومرطبة لإشراقة فورية.",
                    DescriptionEn = "Deep skin cleaning with nourishing and moisturizing masks for instant glow.",
                    ServicesAr = "تنظيف عميق, تقشير, ماسك ذهب, مساج للوجه",
                    ServicesEn = "Deep cleaning, Peeling, Gold mask, Face massage",
                    NotesAr = "إزالة المكياج بالكامل قبل الجلسة",
                    NotesEn = "Remove all makeup before session",
                    Image = "/assets/img/homeservices/skincleaning.jpg"
                }
            };

            foreach (var service in services)
            {
                var existing = await _homeServiceRepository.FirstOrDefaultAsync(s => s.NameEn == service.NameEn);
                if (existing == null)
                {
                    existing = await _homeServiceRepository.InsertAsync(service, autoSave: true);
                }

                foreach (var day in (DayOfWeek[])Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (day == DayOfWeek.Friday) continue;
                    await _homeServiceScheduleRepository.InsertAsync(new HomeServiceSchedule
                    {
                        HomeServiceId = existing.Id,
                        DayOfWeek = day,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(21, 0, 0),
                        IsAvailable = true
                    }, autoSave: true);
                }
            }
        }
    }
}