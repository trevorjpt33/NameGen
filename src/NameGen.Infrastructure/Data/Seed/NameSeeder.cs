using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using NameGen.Core.Models;
using System.Globalization;

namespace NameGen.Infrastructure.Data.Seed;

public static class NameSeeder
{
    public static async Task SeedAsync(AppDbContext context, string csvFolderPath)
    {
        if (await context.HumanNames.AnyAsync())
        {
            Console.WriteLine("Database already seeded — skipping.");
            return;
        }

        var names = new List<HumanName>();

        // --- Seed first names from SSA data ---
        Console.WriteLine("Seeding first names from SSA data...");

        // Track best (lowest) popularity rank per name+gender combination
        // across all years to avoid duplicates
        var firstNameMap = new Dictionary<string, (int popularity, int era)>(
            StringComparer.OrdinalIgnoreCase);

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };

        foreach (var file in Directory.GetFiles(csvFolderPath, "yob*.txt").OrderBy(f => f))
        {
            var yearStr = Path.GetFileNameWithoutExtension(file).Replace("yob", "");
            if (!int.TryParse(yearStr, out int year)) continue;

            using var reader = new StreamReader(file);
            using var csv = new CsvReader(reader, csvConfig);
            var records = csv.GetRecords<SsaNameRecord>().ToList();

            // Rank within this year's file = popularity rank for that year
            int rank = 1;
            foreach (var record in records)
            {
                var key = $"{record.Name}|{record.Gender}";
                if (!firstNameMap.ContainsKey(key) || firstNameMap[key].popularity > rank)
                {
                    firstNameMap[key] = (rank, year);
                }
                rank++;
            }
        }

        foreach (var kvp in firstNameMap)
        {
            var parts = kvp.Key.Split('|');
            var gender = parts[1] == "F" ? Gender.Female : Gender.Male;

            names.Add(new HumanName
            {
                Name = parts[0],
                Component = NameComponent.First,
                Gender = gender,
                Popularity = kvp.Value.popularity,
                Era = kvp.Value.era
            });
        }

        Console.WriteLine($"Parsed {names.Count} unique first names.");

        // --- Seed last names from Census data ---
        Console.WriteLine("Seeding last names from Census data...");

        var surnameFile = Directory.GetFiles(csvFolderPath, "Names_2010Census.csv").FirstOrDefault();
        if (surnameFile != null)
        {
            var surnameConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            using var reader = new StreamReader(surnameFile);
            using var csv = new CsvReader(reader, surnameConfig);
            var records = csv.GetRecords<CensusSurnameRecord>().ToList();

            foreach (var record in records)
            {
                // Skip Census dataset annotations (e.g. "All other names")
                if (record.Name.Contains(' '))
                    continue;
                    
                names.Add(new HumanName
                {
                    Name = char.ToUpper(record.Name[0]) + record.Name.Substring(1).ToLower(),
                    Component = NameComponent.Last,
                    Gender = Gender.Neutral,
                    Popularity = record.Rank
                });
            }

            Console.WriteLine($"Parsed {records.Count} last names.");
        }

        // --- Bulk insert in batches ---
        Console.WriteLine($"Inserting {names.Count} total records...");

        const int batchSize = 1000;
        for (int i = 0; i < names.Count; i += batchSize)
        {
            var batch = names.Skip(i).Take(batchSize).ToList();
            await context.HumanNames.AddRangeAsync(batch);
            await context.SaveChangesAsync();

            if (i % 10000 == 0)
                Console.WriteLine($"  Inserted {i} / {names.Count}...");
        }

        Console.WriteLine("Seeding complete.");
    }
}