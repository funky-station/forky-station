using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AdminPlayerMonitoringLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admin_player_monitoring_log",
                columns: table => new
                {
                    admin_player_monitoring_log_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    round_id = table.Column<int>(type: "INTEGER", nullable: false),
                    player_user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    player_last_seen_user_name = table.Column<string>(type: "TEXT", nullable: false),
                    event_type = table.Column<int>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    details = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_player_monitoring_log", x => x.admin_player_monitoring_log_id);
                    table.ForeignKey(
                        name: "FK_admin_player_monitoring_log_round_round_id",
                        column: x => x.round_id,
                        principalTable: "round",
                        principalColumn: "round_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admin_player_monitoring_log_player_user_id_date",
                table: "admin_player_monitoring_log",
                columns: new[] { "player_user_id", "date" });

            migrationBuilder.CreateIndex(
                name: "IX_admin_player_monitoring_log_player_user_id_event_type_round_id",
                table: "admin_player_monitoring_log",
                columns: new[] { "player_user_id", "event_type", "round_id" });

            migrationBuilder.CreateIndex(
                name: "IX_admin_player_monitoring_log_round_id",
                table: "admin_player_monitoring_log",
                column: "round_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_player_monitoring_log");
        }
    }
}
