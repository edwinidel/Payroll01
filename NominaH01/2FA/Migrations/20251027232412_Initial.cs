using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MaxCompanies = table.Column<int>(type: "int", nullable: true),
                    MaxEmployees = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostCenters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CountryTimeZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryTimeZones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Creditors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RUC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Dv = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    FixedPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CellPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creditors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HoliDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoliDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityDocumentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityDocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObservationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OverTimeFactors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Factor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Formula = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Identify = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverTimeFactors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentFrequencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QuantityOfDays = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentFrequencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Phases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GmtOffset = table.Column<int>(type: "int", nullable: false),
                    DaylightSavingOffset = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransitBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TransitId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeOfDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeOfDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeOfWorkers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeOfWorkers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeOfWorkSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeOfWorkSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessGroupId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_BusinessGroups_BusinessGroupId",
                        column: x => x.BusinessGroupId,
                        principalTable: "BusinessGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaseHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentFrequencyId = table.Column<int>(type: "int", nullable: false),
                    LastPayDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAbsensestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtraTimeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuantityOfDays = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentGroups_PaymentFrequencies_PaymentFrequencyId",
                        column: x => x.PaymentFrequencyId,
                        principalTable: "PaymentFrequencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LegalDeductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmployeeDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployerDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayrollTypeId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PayrollTypeEntityId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalDeductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalDeductions_PayrollTypes_PayrollTypeEntityId",
                        column: x => x.PayrollTypeEntityId,
                        principalTable: "PayrollTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LegalDeductions_PayrollTypes_PayrollTypeId",
                        column: x => x.PayrollTypeId,
                        principalTable: "PayrollTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shifts_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Iso3 = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Iso2 = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Numeric_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone_code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Capital = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Currency_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Currency_symbol = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Tld = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Native = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Region_id = table.Column<int>(type: "int", nullable: false),
                    Subregion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subregion_id = table.Column<int>(type: "int", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TimeZoneId = table.Column<int>(type: "int", nullable: true),
                    Latitude = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Longitude = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Emoji = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmojiU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name_es = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name_en = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name_fr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Flag = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CoatOfArms = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CountryEntityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Countries_Countries_CountryEntityId",
                        column: x => x.CountryEntityId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Countries_TimeZones_TimeZoneId",
                        column: x => x.TimeZoneId,
                        principalTable: "TimeZones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TransitBankId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banks_TransitBanks_TransitBankId",
                        column: x => x.TransitBankId,
                        principalTable: "TransitBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftSegments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    SegmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ShiftEntityId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftSegments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftSegments_Shifts_ShiftEntityId",
                        column: x => x.ShiftEntityId,
                        principalTable: "Shifts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShiftSegments_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryEntityCountryTimeZoneEntity",
                columns: table => new
                {
                    CountriesId = table.Column<int>(type: "int", nullable: false),
                    CountryTimeZonesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryEntityCountryTimeZoneEntity", x => new { x.CountriesId, x.CountryTimeZonesId });
                    table.ForeignKey(
                        name: "FK_CountryEntityCountryTimeZoneEntity_Countries_CountriesId",
                        column: x => x.CountriesId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountryEntityCountryTimeZoneEntity_CountryTimeZones_CountryTimeZonesId",
                        column: x => x.CountryTimeZonesId,
                        principalTable: "CountryTimeZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Longitude = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                    table.ForeignKey(
                        name: "FK_States_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentConcepts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RegularHours = table.Column<bool>(type: "bit", nullable: false),
                    ExtraHours = table.Column<bool>(type: "bit", nullable: false),
                    PayFactor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RecurrentPayment = table.Column<bool>(type: "bit", nullable: false),
                    IsPredetermined = table.Column<bool>(type: "bit", nullable: false),
                    IsConstruction = table.Column<bool>(type: "bit", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentConcepts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentConcepts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PaymentConcepts_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConceptLegalDeductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentConceptId = table.Column<int>(type: "int", nullable: false),
                    LegalDeductionEntityId = table.Column<int>(type: "int", nullable: false),
                    HasSpecialRule = table.Column<bool>(type: "bit", nullable: false),
                    SpecialRule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptLegalDeductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConceptLegalDeductions_LegalDeductions_LegalDeductionEntityId",
                        column: x => x.LegalDeductionEntityId,
                        principalTable: "LegalDeductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConceptLegalDeductions_PaymentConcepts_PaymentConceptId",
                        column: x => x.PaymentConceptId,
                        principalTable: "PaymentConcepts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CostCenterPaymentConceptAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostCenterId = table.Column<int>(type: "int", nullable: false),
                    PaymentConceptId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenterPaymentConceptAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostCenterPaymentConceptAccounts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostCenterPaymentConceptAccounts_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostCenterPaymentConceptAccounts_PaymentConcepts_PaymentConceptId",
                        column: x => x.PaymentConceptId,
                        principalTable: "PaymentConcepts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BusinessGroupId = table.Column<int>(type: "int", nullable: false),
                    FixedPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CellPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ruc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dv = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ProfessionalRisk = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PaymentBankId = table.Column<int>(type: "int", nullable: true),
                    VoucherFormatId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_Banks_PaymentBankId",
                        column: x => x.PaymentBankId,
                        principalTable: "Banks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Companies_BusinessGroups_BusinessGroupId",
                        column: x => x.BusinessGroupId,
                        principalTable: "BusinessGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Companies_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Companies_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Companies_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    CodOfClock = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    DivisionId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    PhaseId = table.Column<int>(type: "int", nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    DateOfBird = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdDocument = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Dv = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    CivilStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Genere = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SocSecNum = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsrGroup = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    EmployeeTypeId = table.Column<int>(type: "int", nullable: false),
                    OriginCountryId = table.Column<int>(type: "int", nullable: false),
                    BloodType = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CDIR1 = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CDIR2 = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CEMAIL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FixedPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CellPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SalaryType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RegularHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AgreeSalary = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    HourSalary = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    RetainISR = table.Column<bool>(type: "bit", nullable: false),
                    IsrMethod = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    IsrFixed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalIsr = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeclareIsr = table.Column<bool>(type: "bit", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unionized = table.Column<bool>(type: "bit", nullable: false),
                    TypeOfWorkerId = table.Column<int>(type: "int", nullable: false),
                    PayAccountType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PayAccount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: false),
                    IdentityDocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    HiringDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LiquidationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndOfContract = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSalaryIncrease = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeObservationId = table.Column<int>(type: "int", nullable: false),
                    PaymentGroupId = table.Column<int>(type: "int", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ActivityEntityId = table.Column<int>(type: "int", nullable: true),
                    ObservationTypeEntityId = table.Column<int>(type: "int", nullable: true),
                    PaymentGroupEntityId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Activities_ActivityEntityId",
                        column: x => x.ActivityEntityId,
                        principalTable: "Activities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Banks_BankId",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Countries_OriginCountryId",
                        column: x => x.OriginCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_EmployeeTypes_EmployeeTypeId",
                        column: x => x.EmployeeTypeId,
                        principalTable: "EmployeeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_IdentityDocumentTypes_IdentityDocumentTypeId",
                        column: x => x.IdentityDocumentTypeId,
                        principalTable: "IdentityDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_ObservationTypes_ObservationTypeEntityId",
                        column: x => x.ObservationTypeEntityId,
                        principalTable: "ObservationTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_PaymentGroups_PaymentGroupEntityId",
                        column: x => x.PaymentGroupEntityId,
                        principalTable: "PaymentGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_PaymentGroups_PaymentGroupId",
                        column: x => x.PaymentGroupId,
                        principalTable: "PaymentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Phases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "Phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_TypeOfWorkers_TypeOfWorkerId",
                        column: x => x.TypeOfWorkerId,
                        principalTable: "TypeOfWorkers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentGroupId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AbsensestDateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AbsensestDateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtraTimeDateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtraTimeDateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollHeaders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollHeaders_PaymentGroups_PaymentGroupId",
                        column: x => x.PaymentGroupId,
                        principalTable: "PaymentGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PayrollTmpHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentGroupId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AbsensestDateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AbsensestDateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtraTimeDateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtraTimeDateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PayrollTypeId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CompanyEntityId = table.Column<int>(type: "int", nullable: true),
                    PayrollTypeEntityId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollTmpHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollTmpHeaders_Companies_CompanyEntityId",
                        column: x => x.CompanyEntityId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollTmpHeaders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollTmpHeaders_PaymentGroups_PaymentGroupId",
                        column: x => x.PaymentGroupId,
                        principalTable: "PaymentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayrollTmpHeaders_PayrollTypes_PayrollTypeEntityId",
                        column: x => x.PayrollTypeEntityId,
                        principalTable: "PayrollTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollTmpHeaders_PayrollTypes_PayrollTypeId",
                        column: x => x.PayrollTypeId,
                        principalTable: "PayrollTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayrollVoucherFormats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    PayrollTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FormatTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyEntityId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollVoucherFormats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollVoucherFormats_Companies_CompanyEntityId",
                        column: x => x.CompanyEntityId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollVoucherFormats_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollVoucherFormats_PayrollTypes_PayrollTypeId",
                        column: x => x.PayrollTypeId,
                        principalTable: "PayrollTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCompanies_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCompanies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeObservacions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ObservationTypeId = table.Column<int>(type: "int", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeObservacions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeObservacions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeObservacions_ObservationTypes_ObservationTypeId",
                        column: x => x.ObservationTypeId,
                        principalTable: "ObservationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FixedPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentConceptId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PaymentFrequency = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PayAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comentary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AuthorizeBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FixedPayments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FixedPayments_PaymentConcepts_PaymentConceptId",
                        column: x => x.PaymentConceptId,
                        principalTable: "PaymentConcepts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Liabitities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    CreditorId = table.Column<int>(type: "int", nullable: false),
                    InitialAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Dicsount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaxPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Liabitities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Liabitities_Creditors_CreditorId",
                        column: x => x.CreditorId,
                        principalTable: "Creditors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Liabitities_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShiftAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftAssignments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftAssignments_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftAssignments_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayrollEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollTmpHeaderId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    TotalEarnings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayrollHeaderId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollEmployees_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollEmployees_PayrollHeaders_PayrollHeaderId",
                        column: x => x.PayrollHeaderId,
                        principalTable: "PayrollHeaders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PayrollTmpEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollTmpHeaderId = table.Column<int>(type: "int", nullable: false),
                    RegularHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HourlySalary = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    AgreeSalary = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    RegularSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OverTimeSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalEarnings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalLegalDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalOtherDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayrollHeaderEntityId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollTmpEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollTmpEmployees_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollTmpEmployees_PayrollHeaders_PayrollHeaderEntityId",
                        column: x => x.PayrollHeaderEntityId,
                        principalTable: "PayrollHeaders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollTmpEmployees_PayrollTmpHeaders_PayrollTmpHeaderId",
                        column: x => x.PayrollTmpHeaderId,
                        principalTable: "PayrollTmpHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyPayrollVouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    PayrollTypeId = table.Column<int>(type: "int", nullable: false),
                    PayrollVoucherFormatId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPayrollVouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyPayrollVouchers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPayrollVouchers_PayrollTypes_PayrollTypeId",
                        column: x => x.PayrollTypeId,
                        principalTable: "PayrollTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPayrollVouchers_PayrollVoucherFormats_PayrollVoucherFormatId",
                        column: x => x.PayrollVoucherFormatId,
                        principalTable: "PayrollVoucherFormats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoricLiabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollTmpHeaderId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LiabilityId = table.Column<int>(type: "int", nullable: false),
                    AmountToDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricLiabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricLiabilities_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoricLiabilities_Liabitities_LiabilityId",
                        column: x => x.LiabilityId,
                        principalTable: "Liabitities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoricLiabilities_PayrollTmpHeaders_PayrollTmpHeaderId",
                        column: x => x.PayrollTmpHeaderId,
                        principalTable: "PayrollTmpHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollLegalDeductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollHeaderId = table.Column<int>(type: "int", nullable: false),
                    PayrollEmployeeId = table.Column<int>(type: "int", nullable: false),
                    LegalDeductionId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollLegalDeductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollLegalDeductions_LegalDeductions_LegalDeductionId",
                        column: x => x.LegalDeductionId,
                        principalTable: "LegalDeductions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollLegalDeductions_PayrollEmployees_PayrollEmployeeId",
                        column: x => x.PayrollEmployeeId,
                        principalTable: "PayrollEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollLegalDeductions_PayrollHeaders_PayrollHeaderId",
                        column: x => x.PayrollHeaderId,
                        principalTable: "PayrollHeaders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HistoricTmpLiabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollTmpHeaderId = table.Column<int>(type: "int", nullable: false),
                    PayrollTmpEmployeeId = table.Column<int>(type: "int", nullable: false),
                    LiabilityId = table.Column<int>(type: "int", nullable: false),
                    AmountToDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayrrollTmpHeaderId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricTmpLiabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricTmpLiabilities_Liabitities_LiabilityId",
                        column: x => x.LiabilityId,
                        principalTable: "Liabitities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoricTmpLiabilities_PayrollTmpEmployees_PayrollTmpEmployeeId",
                        column: x => x.PayrollTmpEmployeeId,
                        principalTable: "PayrollTmpEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoricTmpLiabilities_PayrollTmpHeaders_PayrrollTmpHeaderId",
                        column: x => x.PayrrollTmpHeaderId,
                        principalTable: "PayrollTmpHeaders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PayrollConcepts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollTmpEmployeeId = table.Column<int>(type: "int", nullable: false),
                    PaymentConceptId = table.Column<int>(type: "int", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PayrollEmployeeEntityId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollConcepts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollConcepts_PaymentConcepts_PaymentConceptId",
                        column: x => x.PaymentConceptId,
                        principalTable: "PaymentConcepts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayrollConcepts_PayrollEmployees_PayrollEmployeeEntityId",
                        column: x => x.PayrollEmployeeEntityId,
                        principalTable: "PayrollEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollConcepts_PayrollTmpEmployees_PayrollTmpEmployeeId",
                        column: x => x.PayrollTmpEmployeeId,
                        principalTable: "PayrollTmpEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollTmpConcepts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollTmpEmployeeId = table.Column<int>(type: "int", nullable: false),
                    PaymentConceptId = table.Column<int>(type: "int", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitAmount = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    PayFactor = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayrollTmpHeaderId = table.Column<int>(type: "int", nullable: false),
                    PayrrollTmpHeaderId = table.Column<int>(type: "int", nullable: true),
                    OverTimeFactorEntityId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollTmpConcepts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollTmpConcepts_OverTimeFactors_OverTimeFactorEntityId",
                        column: x => x.OverTimeFactorEntityId,
                        principalTable: "OverTimeFactors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollTmpConcepts_PaymentConcepts_PaymentConceptId",
                        column: x => x.PaymentConceptId,
                        principalTable: "PaymentConcepts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayrollTmpConcepts_PayrollTmpEmployees_PayrollTmpEmployeeId",
                        column: x => x.PayrollTmpEmployeeId,
                        principalTable: "PayrollTmpEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayrollTmpConcepts_PayrollTmpHeaders_PayrrollTmpHeaderId",
                        column: x => x.PayrrollTmpHeaderId,
                        principalTable: "PayrollTmpHeaders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PayrollTmpLegalDeductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollTmpHeaderId = table.Column<int>(type: "int", nullable: false),
                    PayrollTmpEmployeeId = table.Column<int>(type: "int", nullable: false),
                    LegalDeductionId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayrollTmpEmployeeEntityId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollTmpLegalDeductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollTmpLegalDeductions_LegalDeductions_LegalDeductionId",
                        column: x => x.LegalDeductionId,
                        principalTable: "LegalDeductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayrollTmpLegalDeductions_PayrollTmpEmployees_PayrollTmpEmployeeEntityId",
                        column: x => x.PayrollTmpEmployeeEntityId,
                        principalTable: "PayrollTmpEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollTmpLegalDeductions_PayrollTmpEmployees_PayrollTmpEmployeeId",
                        column: x => x.PayrollTmpEmployeeId,
                        principalTable: "PayrollTmpEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollTmpLegalDeductions_PayrollTmpHeaders_PayrollTmpHeaderId",
                        column: x => x.PayrollTmpHeaderId,
                        principalTable: "PayrollTmpHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollTmpOvertime",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollTmpEmployeeId = table.Column<int>(type: "int", nullable: false),
                    PayrollTmpHeaderId = table.Column<int>(type: "int", nullable: false),
                    OvertimeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntryTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ExitTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CalculatedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FactorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AppliedFactor = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsSunday = table.Column<bool>(type: "bit", nullable: false),
                    IsHoliday = table.Column<bool>(type: "bit", nullable: false),
                    WeeklyAccumulated = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TypeOfDayId = table.Column<int>(type: "int", nullable: true),
                    TypeOfWorkScheduleId = table.Column<int>(type: "int", nullable: true),
                    OverTimeFactorId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollTmpOvertime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollTmpOvertime_OverTimeFactors_OverTimeFactorId",
                        column: x => x.OverTimeFactorId,
                        principalTable: "OverTimeFactors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayrollTmpOvertime_PayrollTmpEmployees_PayrollTmpEmployeeId",
                        column: x => x.PayrollTmpEmployeeId,
                        principalTable: "PayrollTmpEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayrollTmpOvertime_PayrollTmpHeaders_PayrollTmpHeaderId",
                        column: x => x.PayrollTmpHeaderId,
                        principalTable: "PayrollTmpHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollTmpOvertime_TypeOfDays_TypeOfDayId",
                        column: x => x.TypeOfDayId,
                        principalTable: "TypeOfDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollTmpOvertime_TypeOfWorkSchedules_TypeOfWorkScheduleId",
                        column: x => x.TypeOfWorkScheduleId,
                        principalTable: "TypeOfWorkSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CompanyId",
                table: "Accounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BusinessGroupId",
                table: "AspNetUsers",
                column: "BusinessGroupId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_TransitBankId",
                table: "Banks",
                column: "TransitBankId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_StateId",
                table: "Cities",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_BusinessGroupId",
                table: "Companies",
                column: "BusinessGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CityId",
                table: "Companies",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CountryId",
                table: "Companies",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_PaymentBankId",
                table: "Companies",
                column: "PaymentBankId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_StateId",
                table: "Companies",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_VoucherFormatId",
                table: "Companies",
                column: "VoucherFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPayrollVouchers_CompanyId",
                table: "CompanyPayrollVouchers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPayrollVouchers_PayrollTypeId",
                table: "CompanyPayrollVouchers",
                column: "PayrollTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPayrollVouchers_PayrollVoucherFormatId",
                table: "CompanyPayrollVouchers",
                column: "PayrollVoucherFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_ConceptLegalDeductions_LegalDeductionEntityId",
                table: "ConceptLegalDeductions",
                column: "LegalDeductionEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ConceptLegalDeductions_PaymentConceptId",
                table: "ConceptLegalDeductions",
                column: "PaymentConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenterPaymentConceptAccounts_AccountId",
                table: "CostCenterPaymentConceptAccounts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenterPaymentConceptAccounts_CostCenterId",
                table: "CostCenterPaymentConceptAccounts",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenterPaymentConceptAccounts_PaymentConceptId",
                table: "CostCenterPaymentConceptAccounts",
                column: "PaymentConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_CountryEntityId",
                table: "Countries",
                column: "CountryEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_TimeZoneId",
                table: "Countries",
                column: "TimeZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryEntityCountryTimeZoneEntity_CountryTimeZonesId",
                table: "CountryEntityCountryTimeZoneEntity",
                column: "CountryTimeZonesId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeObservacions_EmployeeId",
                table: "EmployeeObservacions",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeObservacions_ObservationTypeId",
                table: "EmployeeObservacions",
                column: "ObservationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ActivityEntityId",
                table: "Employees",
                column: "ActivityEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ActivityId",
                table: "Employees",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BankId",
                table: "Employees",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BranchId",
                table: "Employees",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                table: "Employees",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CostCenterId",
                table: "Employees",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DivisionId",
                table: "Employees",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeTypeId",
                table: "Employees",
                column: "EmployeeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IdentityDocumentTypeId",
                table: "Employees",
                column: "IdentityDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ObservationTypeEntityId",
                table: "Employees",
                column: "ObservationTypeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_OriginCountryId",
                table: "Employees",
                column: "OriginCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PaymentGroupEntityId",
                table: "Employees",
                column: "PaymentGroupEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PaymentGroupId",
                table: "Employees",
                column: "PaymentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PhaseId",
                table: "Employees",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PositionId",
                table: "Employees",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ProjectId",
                table: "Employees",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ScheduleId",
                table: "Employees",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SectionId",
                table: "Employees",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TypeOfWorkerId",
                table: "Employees",
                column: "TypeOfWorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedPayments_EmployeeId",
                table: "FixedPayments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedPayments_PaymentConceptId",
                table: "FixedPayments",
                column: "PaymentConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricLiabilities_EmployeeId",
                table: "HistoricLiabilities",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricLiabilities_LiabilityId",
                table: "HistoricLiabilities",
                column: "LiabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricLiabilities_PayrollTmpHeaderId",
                table: "HistoricLiabilities",
                column: "PayrollTmpHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricTmpLiabilities_LiabilityId",
                table: "HistoricTmpLiabilities",
                column: "LiabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricTmpLiabilities_PayrollTmpEmployeeId",
                table: "HistoricTmpLiabilities",
                column: "PayrollTmpEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricTmpLiabilities_PayrrollTmpHeaderId",
                table: "HistoricTmpLiabilities",
                column: "PayrrollTmpHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalDeductions_PayrollTypeEntityId",
                table: "LegalDeductions",
                column: "PayrollTypeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalDeductions_PayrollTypeId",
                table: "LegalDeductions",
                column: "PayrollTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Liabitities_CreditorId",
                table: "Liabitities",
                column: "CreditorId");

            migrationBuilder.CreateIndex(
                name: "IX_Liabitities_EmployeeId",
                table: "Liabitities",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentConcepts_AccountId",
                table: "PaymentConcepts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentConcepts_CountryId",
                table: "PaymentConcepts",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGroups_PaymentFrequencyId",
                table: "PaymentGroups",
                column: "PaymentFrequencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollConcepts_PaymentConceptId",
                table: "PayrollConcepts",
                column: "PaymentConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollConcepts_PayrollEmployeeEntityId",
                table: "PayrollConcepts",
                column: "PayrollEmployeeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollConcepts_PayrollTmpEmployeeId",
                table: "PayrollConcepts",
                column: "PayrollTmpEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollEmployees_EmployeeId",
                table: "PayrollEmployees",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollEmployees_PayrollHeaderId",
                table: "PayrollEmployees",
                column: "PayrollHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollHeaders_CompanyId",
                table: "PayrollHeaders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollHeaders_PaymentGroupId",
                table: "PayrollHeaders",
                column: "PaymentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollLegalDeductions_LegalDeductionId",
                table: "PayrollLegalDeductions",
                column: "LegalDeductionId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollLegalDeductions_PayrollEmployeeId",
                table: "PayrollLegalDeductions",
                column: "PayrollEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollLegalDeductions_PayrollHeaderId",
                table: "PayrollLegalDeductions",
                column: "PayrollHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpConcepts_OverTimeFactorEntityId",
                table: "PayrollTmpConcepts",
                column: "OverTimeFactorEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpConcepts_PaymentConceptId",
                table: "PayrollTmpConcepts",
                column: "PaymentConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpConcepts_PayrollTmpEmployeeId",
                table: "PayrollTmpConcepts",
                column: "PayrollTmpEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpConcepts_PayrrollTmpHeaderId",
                table: "PayrollTmpConcepts",
                column: "PayrrollTmpHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpEmployees_EmployeeId",
                table: "PayrollTmpEmployees",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpEmployees_PayrollHeaderEntityId",
                table: "PayrollTmpEmployees",
                column: "PayrollHeaderEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpEmployees_PayrollTmpHeaderId",
                table: "PayrollTmpEmployees",
                column: "PayrollTmpHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpHeaders_CompanyEntityId",
                table: "PayrollTmpHeaders",
                column: "CompanyEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpHeaders_CompanyId",
                table: "PayrollTmpHeaders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpHeaders_PaymentGroupId",
                table: "PayrollTmpHeaders",
                column: "PaymentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpHeaders_PayrollTypeEntityId",
                table: "PayrollTmpHeaders",
                column: "PayrollTypeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpHeaders_PayrollTypeId",
                table: "PayrollTmpHeaders",
                column: "PayrollTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpLegalDeductions_LegalDeductionId",
                table: "PayrollTmpLegalDeductions",
                column: "LegalDeductionId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpLegalDeductions_PayrollTmpEmployeeEntityId",
                table: "PayrollTmpLegalDeductions",
                column: "PayrollTmpEmployeeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpLegalDeductions_PayrollTmpEmployeeId",
                table: "PayrollTmpLegalDeductions",
                column: "PayrollTmpEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpLegalDeductions_PayrollTmpHeaderId",
                table: "PayrollTmpLegalDeductions",
                column: "PayrollTmpHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpOvertime_OverTimeFactorId",
                table: "PayrollTmpOvertime",
                column: "OverTimeFactorId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpOvertime_PayrollTmpEmployeeId",
                table: "PayrollTmpOvertime",
                column: "PayrollTmpEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpOvertime_PayrollTmpHeaderId",
                table: "PayrollTmpOvertime",
                column: "PayrollTmpHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpOvertime_TypeOfDayId",
                table: "PayrollTmpOvertime",
                column: "TypeOfDayId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollTmpOvertime_TypeOfWorkScheduleId",
                table: "PayrollTmpOvertime",
                column: "TypeOfWorkScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollVoucherFormats_CompanyEntityId",
                table: "PayrollVoucherFormats",
                column: "CompanyEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollVoucherFormats_CompanyId",
                table: "PayrollVoucherFormats",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollVoucherFormats_PayrollTypeId",
                table: "PayrollVoucherFormats",
                column: "PayrollTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_EmployeeId",
                table: "ShiftAssignments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_ScheduleId",
                table: "ShiftAssignments",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftAssignments_ShiftId",
                table: "ShiftAssignments",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_ScheduleId",
                table: "Shifts",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSegments_ShiftEntityId",
                table: "ShiftSegments",
                column: "ShiftEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSegments_ShiftId",
                table: "ShiftSegments",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_States_CountryId",
                table: "States",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_CompanyId",
                table: "UserCompanies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_UserId",
                table: "UserCompanies",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Companies_CompanyId",
                table: "Accounts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_PayrollVoucherFormats_VoucherFormatId",
                table: "Companies",
                column: "VoucherFormatId",
                principalTable: "PayrollVoucherFormats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayrollVoucherFormats_Companies_CompanyEntityId",
                table: "PayrollVoucherFormats");

            migrationBuilder.DropForeignKey(
                name: "FK_PayrollVoucherFormats_Companies_CompanyId",
                table: "PayrollVoucherFormats");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CompanyPayrollVouchers");

            migrationBuilder.DropTable(
                name: "ConceptLegalDeductions");

            migrationBuilder.DropTable(
                name: "CostCenterPaymentConceptAccounts");

            migrationBuilder.DropTable(
                name: "CountryEntityCountryTimeZoneEntity");

            migrationBuilder.DropTable(
                name: "EmployeeObservacions");

            migrationBuilder.DropTable(
                name: "FixedPayments");

            migrationBuilder.DropTable(
                name: "HistoricLiabilities");

            migrationBuilder.DropTable(
                name: "HistoricTmpLiabilities");

            migrationBuilder.DropTable(
                name: "HoliDays");

            migrationBuilder.DropTable(
                name: "PayrollConcepts");

            migrationBuilder.DropTable(
                name: "PayrollLegalDeductions");

            migrationBuilder.DropTable(
                name: "PayrollTmpConcepts");

            migrationBuilder.DropTable(
                name: "PayrollTmpLegalDeductions");

            migrationBuilder.DropTable(
                name: "PayrollTmpOvertime");

            migrationBuilder.DropTable(
                name: "ShiftAssignments");

            migrationBuilder.DropTable(
                name: "ShiftSegments");

            migrationBuilder.DropTable(
                name: "UserCompanies");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CountryTimeZones");

            migrationBuilder.DropTable(
                name: "Liabitities");

            migrationBuilder.DropTable(
                name: "PayrollEmployees");

            migrationBuilder.DropTable(
                name: "PaymentConcepts");

            migrationBuilder.DropTable(
                name: "LegalDeductions");

            migrationBuilder.DropTable(
                name: "OverTimeFactors");

            migrationBuilder.DropTable(
                name: "PayrollTmpEmployees");

            migrationBuilder.DropTable(
                name: "TypeOfDays");

            migrationBuilder.DropTable(
                name: "TypeOfWorkSchedules");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Creditors");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "PayrollHeaders");

            migrationBuilder.DropTable(
                name: "PayrollTmpHeaders");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "CostCenters");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropTable(
                name: "EmployeeTypes");

            migrationBuilder.DropTable(
                name: "IdentityDocumentTypes");

            migrationBuilder.DropTable(
                name: "ObservationTypes");

            migrationBuilder.DropTable(
                name: "Phases");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "TypeOfWorkers");

            migrationBuilder.DropTable(
                name: "PaymentGroups");

            migrationBuilder.DropTable(
                name: "PaymentFrequencies");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "BusinessGroups");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "PayrollVoucherFormats");

            migrationBuilder.DropTable(
                name: "TransitBanks");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "PayrollTypes");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "TimeZones");
        }
    }
}
