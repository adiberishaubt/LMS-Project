using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Library_Managment_System.Data.Migrations
{
    public partial class EventModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Titile = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false),
                    Place = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.ID);
                });
            migrationBuilder.RenameColumn(
            name: "Titile",
            table: "Events",
            newName: "Title");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
