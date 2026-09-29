using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Svabic.Api.Podaci.Migracije
{
    /// <inheritdoc />
    public partial class VrstePapagaja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vrste",
                columns: table => new
                {
                    Slug = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Naziv = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DanaPoKg = table.Column<int>(type: "integer", nullable: false),
                    Aktivna = table.Column<bool>(type: "boolean", nullable: false),
                    Redosled = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vrste", x => x.Slug);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vrste");
        }
    }
}
