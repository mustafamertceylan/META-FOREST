using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace META_FOREST.Migrations
{
    /// <inheritdoc />
    public partial class TarihGuncellemesi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "FocusSessions",
                newName: "SessionDate");

            migrationBuilder.AlterColumn<string>(
                name: "SelectedRealm",
                table: "FocusSessions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SessionDate",
                table: "FocusSessions",
                newName: "CompletedAt");

            migrationBuilder.AlterColumn<string>(
                name: "SelectedRealm",
                table: "FocusSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
