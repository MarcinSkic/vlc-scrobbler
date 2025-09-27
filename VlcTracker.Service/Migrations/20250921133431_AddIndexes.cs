using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VlcTracker.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Scrobbles_FileName",
                schema: "tracker",
                table: "Scrobbles",
                column: "FileName"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Scrobbles_Title",
                schema: "tracker",
                table: "Scrobbles",
                column: "Title"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Scrobbles_FileName",
                schema: "tracker",
                table: "Scrobbles"
            );

            migrationBuilder.DropIndex(
                name: "IX_Scrobbles_Title",
                schema: "tracker",
                table: "Scrobbles"
            );
        }
    }
}
