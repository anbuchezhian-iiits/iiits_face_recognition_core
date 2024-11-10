using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iiits_face_recognition_core.Migrations
{
    /// <inheritdoc />
    public partial class AddRecognizedPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecognizedPersons",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecognizedPersonName = table.Column<string>(type: "text", nullable: false),
                    CapturedPhotoPath = table.Column<string>(type: "text", nullable: false),
                    UnknownEmbedding = table.Column<byte[]>(type: "bytea", nullable: false),
                    SimilarityScore = table.Column<double>(type: "double precision", nullable: false),
                    EntryDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecognizedPersons", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecognizedPersons");
        }
    }
}
