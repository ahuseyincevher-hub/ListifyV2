using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_uyg.Migrations
{
    /// <inheritdoc />
    public partial class MiktarBirimiEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MiktarBirimi",
                table: "AlisverisListesi",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MiktarBirimi",
                table: "AlisverisListesi");
        }
    }
}
