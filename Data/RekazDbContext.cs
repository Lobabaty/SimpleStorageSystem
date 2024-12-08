using FileStorageSys.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileStorageSys.Data
{
    public class RekazDbContext : DbContext
    {
        public RekazDbContext(DbContextOptions<RekazDbContext> options) : base(options)
        {

        }
        public DbSet<FileMetaData> FilesMetaData { get; set; }
        public DbSet<FileLocalFile> FilesLocalFile { get; set; }
        public DbSet<FileDB> FilesDB { get; set; }
    }
}
