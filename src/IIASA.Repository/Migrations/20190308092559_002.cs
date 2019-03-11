using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IIASA.Repository.Migrations
{
    public partial class _002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateCreated",
                table: "Image",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<string>(
                name: "MetaData",
                table: "Image",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaData",
                table: "Image");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Image",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));
        }
    }
}
