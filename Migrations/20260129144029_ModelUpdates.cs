using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace web_uyg.Migrations
{
    /// <inheritdoc />
    public partial class ModelUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Fiyat",
                table: "AlisverisListesi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ListeId",
                table: "AlisverisListesi",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Not",
                table: "AlisverisListesi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResimUrl",
                table: "AlisverisListesi",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiraNo",
                table: "AlisverisListesi",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AlisverisListeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Renk = table.Column<string>(type: "TEXT", nullable: false),
                    VarsayilanMi = table.Column<bool>(type: "INTEGER", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SonDegistirilmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlisverisListeler", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Kategoriler",
                columns: new[] { "Id", "Aciklama", "Ad" },
                values: new object[,]
                {
                    { 5, "Sıcak ve soğuk içecekler", "İçecek" },
                    { 6, "Tatlı ve tatlı ürünler", "Tatlı" },
                    { 7, "Kahvaltı için gerekli ürünler", "Kahvaltı" },
                    { 8, "Yiyecek ürünleri", "Yiyecek" },
                    { 9, "Süs eşyaları", "Süs Eşyası" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlisverisListesi_ListeId_SiraNo",
                table: "AlisverisListesi",
                columns: new[] { "ListeId", "SiraNo" });

            migrationBuilder.CreateIndex(
                name: "IX_AlisverisListeler_VarsayilanMi",
                table: "AlisverisListeler",
                column: "VarsayilanMi");

            migrationBuilder.AddForeignKey(
                name: "FK_AlisverisListesi_AlisverisListeler_ListeId",
                table: "AlisverisListesi",
                column: "ListeId",
                principalTable: "AlisverisListeler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlisverisListesi_AlisverisListeler_ListeId",
                table: "AlisverisListesi");

            migrationBuilder.DropTable(
                name: "AlisverisListeler");

            migrationBuilder.DropIndex(
                name: "IX_AlisverisListesi_ListeId_SiraNo",
                table: "AlisverisListesi");

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Kategoriler",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DropColumn(
                name: "Fiyat",
                table: "AlisverisListesi");

            migrationBuilder.DropColumn(
                name: "ListeId",
                table: "AlisverisListesi");

            migrationBuilder.DropColumn(
                name: "Not",
                table: "AlisverisListesi");

            migrationBuilder.DropColumn(
                name: "ResimUrl",
                table: "AlisverisListesi");

            migrationBuilder.DropColumn(
                name: "SiraNo",
                table: "AlisverisListesi");
        }
    }
}
