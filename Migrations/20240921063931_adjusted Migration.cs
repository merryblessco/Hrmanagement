using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRbackend.Migrations
{
    /// <inheritdoc />
    public partial class adjustedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryRange",
                table: "JobPostings");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxSalaryRange",
                table: "JobPostings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinSalaryRange",
                table: "JobPostings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxSalaryRange",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "MinSalaryRange",
                table: "JobPostings");

            migrationBuilder.AddColumn<string>(
                name: "SalaryRange",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
