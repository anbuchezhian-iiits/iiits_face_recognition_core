using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iiits_face_recognition_core.Migrations
{
    /// <inheritdoc />
    public partial class MakeEmbeddingNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double[]>(
                name: "Embedding",
                table: "Persons",
                type: "double precision[]",
                nullable: true,
                oldClrType: typeof(double[]),
                oldType: "double precision[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double[]>(
                name: "Embedding",
                table: "Persons",
                type: "double precision[]",
                nullable: false,
                defaultValue: new double[0],
                oldClrType: typeof(double[]),
                oldType: "double precision[]",
                oldNullable: true);
        }
    }
}
