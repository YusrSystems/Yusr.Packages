namespace Yusr.Core.Abstractions.Entities
{
    public class Tenant : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public static Tenant Create(string email, string name)
        {
            return new Tenant
            {
                Email = email,
                Name = name,
                CreateDate = DateTime.UtcNow,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                IsActive = true
            };
        }

        public Tenant Update(string email, string name, DateTime endDate, bool isActive)
        {
            Email = email;
            Name = name;
            StartDate = DateTime.UtcNow;
            EndDate = endDate;
            IsActive = isActive;

            return this;
        }
    }
}