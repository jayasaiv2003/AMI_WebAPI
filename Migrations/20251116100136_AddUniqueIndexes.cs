using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMI_WebAPI.Migrations
{
    public partial class AddUniqueIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Meter_IpAddress",
                schema: "ami",
                table: "Meter",
                column: "IpAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Meter_ICCID",
                schema: "ami",
                table: "Meter",
                column: "ICCID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Meter_IMSI",
                schema: "ami",
                table: "Meter",
                column: "IMSI",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Meter_IpAddress",
                schema: "ami",
                table: "Meter");

            migrationBuilder.DropIndex(
                name: "UQ_Meter_ICCID",
                schema: "ami",
                table: "Meter");

            migrationBuilder.DropIndex(
                name: "UQ_Meter_IMSI",
                schema: "ami",
                table: "Meter");
        }
    }
}
