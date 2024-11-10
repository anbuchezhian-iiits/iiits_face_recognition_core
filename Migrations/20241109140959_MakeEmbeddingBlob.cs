using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iiits_face_recognition_core.Migrations
{
    /// <inheritdoc />
    public partial class MakeEmbeddingBlob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Embedding",
                table: "Persons",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(double[]),
                oldType: "double precision[]",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double[]>(
                name: "Embedding",
                table: "Persons",
                type: "double precision[]",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "bytea");
        }
    }
}
