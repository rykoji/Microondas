using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microondas.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgramasCustomizados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Seconds = table.Column<int>(type: "int", nullable: false),
                    PowerLevel = table.Column<int>(type: "int", nullable: false),
                    Alimento = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Instrucoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CaracterAquecimento = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramasCustomizados", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramasCustomizados_Nome",
                table: "ProgramasCustomizados",
                column: "Nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramasCustomizados");
        }
    }
}
