using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRbackend.Migrations
{
    /// <inheritdoc />
    public partial class NewUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Onboardings_Employees_EmployeeID",
                table: "Onboardings");

            migrationBuilder.DropIndex(
                name: "IX_Onboardings_EmployeeID",
                table: "Onboardings");

            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Onboardings");

            migrationBuilder.DropColumn(
                name: "OnboardingDocumentFilePath",
                table: "Onboardings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Onboardings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "OnboardingDocumentFilePath",
                table: "Onboardings",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Onboardings_EmployeeID",
                table: "Onboardings",
                column: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Onboardings_Employees_EmployeeID",
                table: "Onboardings",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
