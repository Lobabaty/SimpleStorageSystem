using System.ComponentModel.DataAnnotations;

namespace FileStorageSys.Entities
{
    public class FileMetaData
    {
        [Key]
        public long Id { get; set; }
        public Guid FileId { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public string Extension { get; set; }
        public string Description { get; set; }

    }
}
