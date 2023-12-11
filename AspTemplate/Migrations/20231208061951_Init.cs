using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspTemplate.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bus",
                columns: table => new
                {
                    BusId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LicensePlate = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bus__6A0F60B57855F72B", x => x.BusId);
                });

            migrationBuilder.CreateTable(
                name: "Route",
                columns: table => new
                {
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    From = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, defaultValue: ""),
                    To = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, defaultValue: ""),
                    BasePrice = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Route__80979B4D4283D226", x => x.RouteId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, defaultValue: ""),
                    Phone = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, defaultValue: ""),
                    Address = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, defaultValue: ""),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__1788CC4C9F281741", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Seat",
                columns: table => new
                {
                    SeatId = table.Column<int>(type: "int", nullable: false),
                    BusId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Seat__311713F3B50CBDEE", x => x.SeatId);
                    table.ForeignKey(
                        name: "FK__Seat__BusId__4AB81AF0",
                        column: x => x.BusId,
                        principalTable: "Bus",
                        principalColumn: "BusId");
                });

            migrationBuilder.CreateTable(
                name: "Trip",
                columns: table => new
                {
                    TripId = table.Column<int>(type: "int", nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    BusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Trip__51DC713EBA3E0D21", x => x.TripId);
                    table.ForeignKey(
                        name: "FK__Trip__RouteId__45F365D3",
                        column: x => x.RouteId,
                        principalTable: "Route",
                        principalColumn: "RouteId");
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    SeatId = table.Column<int>(type: "int", nullable: false),
                    BookedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    From = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    To = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ticket__712CC60766A37313", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK__Ticket__SeatId__5070F446",
                        column: x => x.SeatId,
                        principalTable: "Seat",
                        principalColumn: "SeatId");
                    table.ForeignKey(
                        name: "FK__Ticket__TripId__4F7CD00D",
                        column: x => x.TripId,
                        principalTable: "Trip",
                        principalColumn: "TripId");
                    table.ForeignKey(
                        name: "FK__Ticket__UserId__5165187F",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seat_BusId",
                table: "Seat",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_SeatId",
                table: "Ticket",
                column: "SeatId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_TripId",
                table: "Ticket",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_UserId",
                table: "Ticket",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_RouteId",
                table: "Trip",
                column: "RouteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Seat");

            migrationBuilder.DropTable(
                name: "Trip");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Bus");

            migrationBuilder.DropTable(
                name: "Route");
        }
    }
}
