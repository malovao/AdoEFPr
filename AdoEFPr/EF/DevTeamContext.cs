using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.SqlDbType;

namespace AdoEFPr.EF
{
    internal class DevTeamContext: DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Problem> Problems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=DevTeam;Trusted_Connection=True;");
            base.OnConfiguring(optionsBuilder);
        }

        public void CreateDbIfNotExist()
        {
            this.Database.EnsureCreated();
        }

        public class Role
        {
            public int Id { get; set; }

            [MaxLength(50)]
            public string Title { get; set; }
        }

        public class Person
        {
            public int Id { get; set; }

            [MaxLength(50)]
            public string Name { get; set; }

            public Role Role { get; set; }
        }

        public class Problem
        {
            public int Id { get; set; }

            [MaxLength(50)]
            public string Title { get; set; }

            [Column(TypeName = "DATE")]
            public DateTime DateStart {get; set;}

            [Column(TypeName = "DATE")]
            public DateTime? DateFinish { get; set; }

            public Person Person { get; set; } 
        }

    }
}
