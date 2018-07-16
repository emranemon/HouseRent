using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace HouseRent.Migrations
{
    public partial class Advertise : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Advertise",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    FlatDetails = table.Column<string>(nullable: true),
                    FlatSize = table.Column<decimal>(nullable: false),
                    Heading = table.Column<string>(nullable: true),
                    OtherBill = table.Column<decimal>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    PostTime = table.Column<DateTime>(nullable: false),
                    Rent = table.Column<decimal>(nullable: false),
                    UserMail = table.Column<string>(nullable: true),
                    UtilitiesBill = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertise", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdvertiseID = table.Column<int>(nullable: false),
                    Anonymous = table.Column<bool>(nullable: false),
                    CommentText = table.Column<string>(nullable: true),
                    CommentTime = table.Column<DateTime>(nullable: false),
                    Commenter = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Comment_Advertise_AdvertiseID",
                        column: x => x.AdvertiseID,
                        principalTable: "Advertise",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdvertiseID = table.Column<int>(nullable: false),
                    FlatImage = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Image_Advertise_AdvertiseID",
                        column: x => x.AdvertiseID,
                        principalTable: "Advertise",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AdvertiseID",
                table: "Comment",
                column: "AdvertiseID");

            migrationBuilder.CreateIndex(
                name: "IX_Image_AdvertiseID",
                table: "Image",
                column: "AdvertiseID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "Advertise");
        }
    }
}
