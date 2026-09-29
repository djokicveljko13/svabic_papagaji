using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Svabic.Api.Podaci.Migracije
{
    /// <inheritdoc />
    public partial class SeedVrste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Vrste",
                columns: new[] { "Slug", "Aktivna", "DanaPoKg", "Naziv", "Redosled" },
                values: new object[,]
                {
                    { "alba-kakadu", true, 30, "Alba kakadu", 13 },
                    { "braunouhi", true, 60, "Braunouhi", 1 },
                    { "edel", true, 30, "Edel", 9 },
                    { "kina-aleksandar", true, 60, "Kina aleksandar", 4 },
                    { "mali-aleksandar", true, 60, "Mali aleksandar", 2 },
                    { "plavo-zuta-ara", true, 15, "Plavo-žuta ara", 14 },
                    { "plavoceli-amazonac", true, 30, "Plavočeli amazonac", 7 },
                    { "roze-kakadu", true, 30, "Roze kakadu", 11 },
                    { "senegalski-papagaj", true, 60, "Senegalski papagaj", 5 },
                    { "veliki-aleksandar", true, 60, "Veliki aleksandar", 3 },
                    { "venecuela-amazonac", true, 30, "Venecuela amazonac", 6 },
                    { "zako", true, 30, "Žako", 10 },
                    { "zelenokrila-ara", true, 15, "Zelenokrila ara", 15 },
                    { "zutoceli-amazonac", true, 30, "Žutočeli amazonac", 8 },
                    { "zutocubi-kakadu", true, 30, "Žutoćubi kakadu", 12 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "alba-kakadu");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "braunouhi");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "edel");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "kina-aleksandar");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "mali-aleksandar");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "plavo-zuta-ara");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "plavoceli-amazonac");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "roze-kakadu");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "senegalski-papagaj");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "veliki-aleksandar");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "venecuela-amazonac");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "zako");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "zelenokrila-ara");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "zutoceli-amazonac");

            migrationBuilder.DeleteData(
                table: "Vrste",
                keyColumn: "Slug",
                keyValue: "zutocubi-kakadu");
        }
    }
}
