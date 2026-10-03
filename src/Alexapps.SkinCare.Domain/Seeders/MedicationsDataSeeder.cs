    using Alexapps.SkinCare.Entities.Medications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Seeders
{
    public class MedicationsDataSeeder : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Medication, Guid> _medicationRepository;

        public MedicationsDataSeeder(IRepository<Medication, Guid> medicationRepository)
        {
            _medicationRepository = medicationRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var medicationsData = new List<(string NameEn, string NameAr, string ActiveIngredients, string Dosage, decimal Price, string Uses, string DescriptionEn, string DescriptionAr)>
            {
                (
                    "Paracetamol 500mg",
                    "باراسيتامول 500 مجم",
                    "Paracetamol",
                    "500 mg",
                    150m,
                    "Fever reduction,Mild to moderate pain relief,Headache,Toothache,Muscle pain",
                    "Paracetamol is a commonly used medication for relieving pain and reducing fever. It is safe when taken within the recommended dosage and suitable for adults and children above 12 years.",
                    "الباراسيتامول هو دواء شائع الاستخدام لتخفيف الألم وخفض الحرارة. وهو آمن عند تناوله ضمن الجرعة الموصى بها ومناسب للبالغين والأطفال فوق 12 عامًا."
                ),
                (
                    "Amoxicillin 500mg",
                    "أموكسيسيلين 500 مجم",
                    "Amoxicillin",
                    "200 mg",
                    150m,
                    "Bacterial infections,Respiratory tract infections,Ear infections,Urinary tract infections",
                    "Amoxicillin is a broad-spectrum antibiotic used to treat bacterial infections. It should only be taken under medical supervision and the full course must be completed.",
                    "أموكسيسيلين هو مضاد حيوي واسع الطيف يستخدم لعلاج الالتهابات البكتيرية. يجب تناوله فقط تحت إشراف طبي ويجب إكمال الدورة الكاملة."
                ),
                (
                    "Hydrating Facial Cleanser",
                    "غسول مرطب للوجه",
                    "Hyaluronic Acid",
                    "5%",
                    450m,
                    "Cleanses skin gently,Removes dirt and oil,Maintains skin hydration,Suitable for dry and sensitive skin",
                    "A gentle facial cleanser that removes impurities without disrupting the skin's natural barrier. Helps keep the skin soft, hydrated, and refreshed.",
                    "غسول لطيف للوجه يزيل الشوائب دون الإخلال بحاجز البشرة الطبيعي. يساعد في الحفاظ على بشرة ناعمة ورطبة ومنتعشة."
                ),
                (
                    "Oil Control Moisturizer",
                    "مرطب للتحكم في الزيوت",
                    "Salicylic Acid",
                    "21%",
                    180m,
                    "Controls excess oil,Prevents acne formation,Hydrates without clogging pores",
                    "An oil-free moisturizer suitable for oily and acne-prone skin. Helps balance sebum production while keeping skin hydrated.",
                    "مرطب خالٍ من الزيوت مناسب للبشرة الدهنية والمعرضة لحب الشباب. يساعد على توازن إنتاج الدهون مع الحفاظ على ترطيب البشرة."
                )
            };

            foreach (var med in medicationsData)
            {
                if (!await _medicationRepository.AnyAsync(x => x.NameEn == med.NameEn))
                {
                    await _medicationRepository.InsertAsync(new Medication
                    {
                        NameEn = med.NameEn,
                        NameAr = med.NameAr,
                        ActiveIngredients = med.ActiveIngredients,
                        Dosage = med.Dosage,
                        Price = med.Price,
                        Uses = med.Uses,
                        DescriptionEn = med.DescriptionEn,
                        DescriptionAr = med.DescriptionAr
                    }, autoSave: true);
                }
            }
        }
    }
}
