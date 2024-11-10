using Microsoft.EntityFrameworkCore;
using System;

namespace iiits_face_recognition_core.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Person> Persons { get; set; }
        public DbSet<RecognizedPerson> RecognizedPersons { get; set; }
    }
}
