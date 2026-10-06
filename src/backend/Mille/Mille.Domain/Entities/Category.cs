using Mille.Domain.Exceptions;

namespace Mille.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description {  get; set; }
        public bool IsDeleted { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        private Category() { }

        public Category(string name,  string description = null)
        {
            Name = name;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateInfo(string name, string description)
        {
            if(IsDeleted) throw new DomainException("Cannot update a deleted category.");

            Name = name;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (!IsDeleted) return;

            IsDeleted = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
