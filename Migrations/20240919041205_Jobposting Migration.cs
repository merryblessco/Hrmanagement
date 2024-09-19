using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRbackend.Migrations
{
    /// <inheritdoc />
    public partial class JobpostingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyAddress",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobCode",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobMode",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Qualifications",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Responsibilities",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalaryRange",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkMode",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Coverletter",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DOB",
                table: "Applicants",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Fullname",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Benefits",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "CompanyAddress",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "JobCode",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "JobMode",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Qualifications",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Responsibilities",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "SalaryRange",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "WorkMode",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Coverletter",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "DOB",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Fullname",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Applicants");
        }
    }
}
