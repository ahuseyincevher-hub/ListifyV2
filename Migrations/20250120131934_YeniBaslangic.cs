using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace web_uyg.Migrations
{
    /// <inheritdoc />
    public partial class YeniBaslangic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriUrunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrunAdi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    VarsayilanMiktar = table.Column<int>(type: "INTEGER", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriUrunler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlisverisListesi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrunAdi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Miktar = table.Column<int>(type: "INTEGER", nullable: false),
                    AlındiMi = table.Column<bool>(type: "INTEGER", nullable: false),
                    KategoriId = table.Column<int>(type: "INTEGER", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlisverisListesi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlisverisListesi_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Kategoriler",
                columns: new[] { "Id", "Aciklama", "Ad" },
                values: new object[,]
                {
                    { 1, "Taze meyve ve sebzeler", "Meyve & Sebze" },
                    { 2, "Süt, peynir, yoğurt vb.", "Süt & Süt Ürünleri" },
                    { 3, "Bakliyat, un, şeker vb.", "Temel Gıda" },
                    { 4, "Temizlik malzemeleri", "Temizlik" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlisverisListesi_KategoriId",
                table: "AlisverisListesi",
                column: "KategoriId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlisverisListesi");

            migrationBuilder.DropTable(
                name: "FavoriUrunler");

            migrationBuilder.DropTable(
                name: "Kategoriler");
        }
    }
}
