using System.ComponentModel.DataAnnotations;

namespace FileStorageSys.Entities
{
    public class FileEntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime? Created_at { get; set; }
        public FileEntityBase()
        {
            Created_at = DateTime.UtcNow;
        }
    }
}
