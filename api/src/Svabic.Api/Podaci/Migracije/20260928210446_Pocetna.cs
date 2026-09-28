using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Svabic.Api.Podaci.Migracije
{
    /// <inheritdoc />
    public partial class Pocetna : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kategorije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Naziv = table.Column<string>(type: "text", nullable: false),
                    Redosled = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategorije", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proizvodi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Naziv = table.Column<string>(type: "text", nullable: false),
                    KategorijaId = table.Column<int>(type: "integer", nullable: false),
                    KratakOpis = table.Column<string>(type: "text", nullable: false),
                    JeZaAre = table.Column<bool>(type: "boolean", nullable: false),
                    Aktivan = table.Column<bool>(type: "boolean", nullable: false),
                    Redosled = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proizvodi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proizvodi_Kategorije_KategorijaId",
                        column: x => x.KategorijaId,
                        principalTable: "Kategorije",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pakovanja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProizvodId = table.Column<int>(type: "integer", nullable: false),
                    Oznaka = table.Column<string>(type: "text", nullable: false),
                    TezinaGrama = table.Column<int>(type: "integer", nullable: true),
                    CenaRsd = table.Column<int>(type: "integer", nullable: false),
                    PostarinaRsd = table.Column<int>(type: "integer", nullable: true),
                    Aktivno = table.Column<bool>(type: "boolean", nullable: false),
                    Redosled = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pakovanja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pakovanja_Proizvodi_ProizvodId",
                        column: x => x.ProizvodId,
                        principalTable: "Proizvodi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pakovanja_ProizvodId",
                table: "Pakovanja",
                column: "ProizvodId");

            migrationBuilder.CreateIndex(
                name: "IX_Proizvodi_KategorijaId",
                table: "Proizvodi",
                column: "KategorijaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pakovanja");

            migrationBuilder.DropTable(
                name: "Proizvodi");

            migrationBuilder.DropTable(
                name: "Kategorije");
        }
    }
}
