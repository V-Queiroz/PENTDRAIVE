using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PENTDRIVEApi.Migrations
{
    /// <inheritdoc />
    public partial class AjusteFormaPagamentoMapeamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FORMA_PAG",
                table: "venda",
                type: "varchar(8)",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FORMA_PAG",
                table: "venda",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldMaxLength: 8,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
