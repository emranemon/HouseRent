using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HouseRent.Migrations
{
    public partial class utubeandfk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlatType",
                table: "Advertise",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeLink",
                table: "Advertise",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_Email",
                table: "User",
                column: "Email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_Email",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FlatType",
                table: "Advertise");

            migrationBuilder.DropColumn(
                name: "YoutubeLink",
                table: "Advertise");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
