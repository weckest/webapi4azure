using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using StudentMinimalApi.Models;

namespace StudentMinimalApi.Data;

public class SchoolDbContext : DbContext {
    public DbSet<Student> Students => Set<Student>();

    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
            : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Student>().HasData(GetStudents());
    }

    private static IEnumerable<Student> GetStudents() {
        string[] p = { Directory.GetCurrentDirectory(), "wwwroot", "students.csv" };
        var csvFilePath = Path.Combine(p);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture) {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };

        var data = new List<Student>().AsEnumerable();
        using (var reader = new StreamReader(csvFilePath)) {
            using (var csvReader = new CsvReader(reader, config)) {
                data = csvReader.GetRecords<Student>().ToList();
            }
        }

        return data;
    }

}
