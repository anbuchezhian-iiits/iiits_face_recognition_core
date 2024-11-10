using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iiits_face_recognition_core.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEmbeddingToBytea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add your column type change here
            migrationBuilder.AlterColumn<byte[]>(
                name: "Embedding",
                table: "Persons",
                type: "bytea", // Set the column type to bytea
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
