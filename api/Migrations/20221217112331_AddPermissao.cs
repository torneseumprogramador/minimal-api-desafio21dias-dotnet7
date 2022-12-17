using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minimalapidesafio.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Permissao",
                table: "administradores",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permissao",
                table: "administradores");
        }
    }
}
