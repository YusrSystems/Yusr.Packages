namespace Yusr.Core.Abstractions.Entities
{
    public interface ITenant
    {
        public long Id { get; set; }
        string Email { get; set; }
        string Name { get; set; }
        DateTime CreateDate { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        bool IsActive { get; set; }
    }
}