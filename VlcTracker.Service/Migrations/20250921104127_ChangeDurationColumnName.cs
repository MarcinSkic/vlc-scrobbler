using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VlcTracker.Service.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDurationColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DurationMs",
                schema: "tracker",
                table: "Scrobbles",
                newName: "Duration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Duration",
                schema: "tracker",
                table: "Scrobbles",
                newName: "DurationMs");
        }
    }
}
