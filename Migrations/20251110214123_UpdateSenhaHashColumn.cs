using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PENTDRIVEApi.Migrations
{
    
    public partial class UpdateSenhaHashColumn : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // O AlterDatabase é inofensivo.
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

           
            migrationBuilder.AlterColumn<byte[]>(
                name: "SENHA_HASH",
                table: "usuario",
                type: "VARBINARY(64)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "longblob");

            
            migrationBuilder.AddColumn<string>(
                name: "ROLE",
                table: "usuario",
                type: "longtext",
                nullable: false,
                defaultValue: "Padrao") 
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AlterColumn<byte[]>(
                name: "SENHA_HASH",
                table: "usuario",
                type: "longblob", 
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "VARBINARY(64)");
               
            migrationBuilder.DropColumn(
                name: "ROLE",
                table: "usuario");
        }
    }
}