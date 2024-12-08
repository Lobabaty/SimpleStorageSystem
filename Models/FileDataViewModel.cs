namespace FileStorageSys.Models
{
    public class FileDataViewModel
    {
       
        public FileDataViewModel(string Id,byte[] FileData, string FileType, string Name, DateTime? Created_at)
        {
            this.Id = Id;
            this.FileData = FileData;
            this.FileType = FileType;
            this.Name = Name;
            this.Created_at = Created_at;

        }
        public string Id { get; set; }

        public string FileType { get; set; }

        public string Name { get; set; }
        public DateTime? Created_at { get; set; }


        public byte[] FileData { get; set; }
    }
}
