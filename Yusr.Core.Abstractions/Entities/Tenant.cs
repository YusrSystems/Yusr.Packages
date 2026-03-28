namespace Yusr.Core.Abstractions.Entities
{
    public class Tenant : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public long CurrencyId { get; private set; }
        public string? CompanyBusinessCategory { get; private set; }
        public string? Crn { get; private set; }
        public string? VatNumber { get; private set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? Logo { get; private set; }

        public virtual Currency Currency { get; set; } = null!;

        public static Tenant Create(string email, string name, string phone, long currencyId, string? logo)
        {
            return new Tenant
            {
                Email = email,
                Name = name,
                Phone = phone,
                CreateDate = DateTime.UtcNow,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(15),
                CurrencyId = currencyId,
                Logo = logo,
                IsActive = true
            };
        }

        public Tenant Update(string email, string name, string phone, long currencyId, string? logo)
        {
            Email = email;
            Name = name;
            Phone = phone;
            StartDate = DateTime.UtcNow;
            CurrencyId = currencyId;
            Logo = logo;

            return this;
        }

        public Tenant ChangeStatus(bool isActive)
        {
            IsActive = isActive;
            return this;
        }

        public Tenant ChangeCompanyInfo(string? companyBusinessCategory, string? crn, string? vatNumber)
        {
            CompanyBusinessCategory = companyBusinessCategory;
            Crn = crn;
            VatNumber = vatNumber;
            return this;
        }

        public Tenant Renew(DateTime endDate)
        {
            EndDate = endDate;
            return this;
        }
    }
}