using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VlcTracker.Service.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Duration",
                schema: "tracker",
                table: "Scrobbles",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                schema: "tracker",
                table: "Scrobbles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true
            );
        }
    }
}
