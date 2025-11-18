using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMI_WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrgUnit_Name",
                schema: "ami",
                table: "OrgUnit",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consumer_Email",
                schema: "ami",
                table: "Consumer",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrgUnit_Name",
                schema: "ami",
                table: "OrgUnit");

            migrationBuilder.DropIndex(
                name: "IX_Consumer_Email",
                schema: "ami",
                table: "Consumer");
        }
    }
}
