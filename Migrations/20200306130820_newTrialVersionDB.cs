using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AutomateBussiness.Migrations
{
    public partial class newTrialVersionDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    factoryID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatGroupsTable",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    groupID = table.Column<string>(nullable: false),
                    groupName = table.Column<string>(nullable: false),
                    groupImage = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatGroupsTable", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ChatHistorysTable",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    messageDate = table.Column<DateTime>(nullable: false),
                    senderId = table.Column<string>(nullable: false),
                    receiverId = table.Column<string>(nullable: false),
                    message = table.Column<string>(nullable: false),
                    messageType = table.Column<int>(nullable: false),
                    messageStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistorysTable", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "FactoryTable",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    factoryName = table.Column<string>(nullable: false),
                    founder = table.Column<string>(nullable: true),
                    foundDate = table.Column<DateTime>(nullable: false),
                    capitalRegister = table.Column<string>(nullable: true),
                    factoryDescription = table.Column<string>(nullable: true),
                    businessArea = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    phone = table.Column<string>(nullable: true),
                    mobile = table.Column<string>(nullable: true),
                    fax = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    website = table.Column<string>(nullable: true),
                    taxId = table.Column<string>(nullable: true),
                    imgLogo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactoryTable", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MachineDailyTable",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    record_date = table.Column<DateTime>(nullable: false),
                    machine_id = table.Column<int>(nullable: false),
                    job_number = table.Column<string>(nullable: false),
                    total_input = table.Column<int>(nullable: false),
                    total_reject = table.Column<int>(nullable: false),
                    runningtime = table.Column<int>(nullable: false),
                    downtime = table.Column<int>(nullable: false),
                    idletime = table.Column<int>(nullable: false),
                    breaktime = table.Column<int>(nullable: false),
                    factoryID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineDailyTable", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MachineErrorTable",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    record_date = table.Column<DateTime>(nullable: false),
                    machine_id = table.Column<int>(nullable: false),
                    job_number = table.Column<string>(nullable: false),
                    errorType = table.Column<int>(nullable: false),
                    description = table.Column<string>(nullable: false),
                    solve = table.Column<int>(nullable: false),
                    informby = table.Column<string>(nullable: false),
                    solvedby = table.Column<string>(nullable: false),
                    factoryID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineErrorTable", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MachineTable",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(nullable: false),
                    plant = table.Column<string>(nullable: false),
                    process = table.Column<string>(nullable: false),
                    line = table.Column<string>(nullable: false),
                    description = table.Column<string>(nullable: false),
                    vendor = table.Column<string>(nullable: false),
                    supervisor = table.Column<string>(nullable: false),
                    installed_date = table.Column<DateTime>(nullable: false),
                    factoryID = table.Column<string>(nullable: false),
                    machineImage = table.Column<string>(nullable: false),
                    machineHashID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineTable", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Movie",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 60, nullable: false),
                    ReleaseDate = table.Column<DateTime>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Genre = table.Column<string>(maxLength: 30, nullable: false),
                    Rating = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTable",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(nullable: false),
                    position = table.Column<string>(nullable: false),
                    photo = table.Column<string>(nullable: false),
                    phone = table.Column<string>(nullable: false),
                    address = table.Column<string>(nullable: false),
                    email = table.Column<string>(nullable: false),
                    parent = table.Column<string>(nullable: true),
                    work_quality = table.Column<int>(nullable: true),
                    initiative = table.Column<int>(nullable: true),
                    cooperative = table.Column<int>(nullable: true),
                    factoryID = table.Column<string>(nullable: false),
                    defaultPassword = table.Column<string>(nullable: false),
                    nickName = table.Column<string>(nullable: true),
                    accessType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTable", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "RelationshipsTable",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    relationDate = table.Column<DateTime>(nullable: false),
                    requestId = table.Column<string>(nullable: false),
                    responedId = table.Column<string>(nullable: false),
                    requestById = table.Column<string>(nullable: true),
                    relationType = table.Column<int>(nullable: false),
                    relationStatus = table.Column<int>(nullable: false),
                    isFavorites = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationshipsTable", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
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
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
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
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
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
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
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
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
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
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "ChatGroupsTable");

            migrationBuilder.DropTable(
                name: "ChatHistorysTable");

            migrationBuilder.DropTable(
                name: "FactoryTable");

            migrationBuilder.DropTable(
                name: "MachineDailyTable");

            migrationBuilder.DropTable(
                name: "MachineErrorTable");

            migrationBuilder.DropTable(
                name: "MachineTable");

            migrationBuilder.DropTable(
                name: "Movie");

            migrationBuilder.DropTable(
                name: "OrganizationTable");

            migrationBuilder.DropTable(
                name: "RelationshipsTable");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
