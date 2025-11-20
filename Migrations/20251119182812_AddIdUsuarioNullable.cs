using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PENTDRIVEApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIdUsuarioNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AddColumn<int>(
            //     name: "ID_USUARIO",
            //     table: "venda",
            //     type: "int",
            //     nullable: true);

            // migrationBuilder.CreateIndex(
            //     name: "IX_venda_ID_USUARIO",
            //     table: "venda",
            //     column: "ID_USUARIO");

            migrationBuilder.AddForeignKey(
                name: "FK_venda_usuario_ID_USUARIO",
                table: "venda",
                column: "ID_USUARIO",
                principalTable: "usuario",
                principalColumn: "ID_USUARIO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_venda_usuario_ID_USUARIO",
                table: "venda");

            migrationBuilder.DropIndex(
                name: "IX_venda_ID_USUARIO",
                table: "venda");

            migrationBuilder.DropColumn(
                name: "ID_USUARIO",
                table: "venda");
        }
    }
}
