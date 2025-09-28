using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VlcTracker.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddScrobbleDurationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Duration",
                schema: "tracker",
                table: "Scrobbles",
                newName: "VideoDuration"
            );

            migrationBuilder.AddColumn<double>(
                name: "ScrobbleDuration",
                schema: "tracker",
                table: "Scrobbles",
                type: "REAL",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScrobbleDuration",
                schema: "tracker",
                table: "Scrobbles"
            );

            migrationBuilder.RenameColumn(
                name: "VideoDuration",
                schema: "tracker",
                table: "Scrobbles",
                newName: "Duration"
            );
        }
    }
}
