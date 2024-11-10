using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iiits_face_recognition_core.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddingColumnToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double[]>(
                name: "Embedding",
                table: "Persons",
                type: "double precision[]",
                nullable: false,
                defaultValue: new double[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "Persons");
        }
    }
}
