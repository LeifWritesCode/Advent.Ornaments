using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ornaments.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInputTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Identities_IdentityId",
                table: "Submissions");

            migrationBuilder.RenameColumn(
                name: "IdentityId",
                table: "Submissions",
                newName: "InputId");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_IdentityId",
                table: "Submissions",
                newName: "IX_Submissions_InputId");

            migrationBuilder.CreateTable(
                name: "Inputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    IdentityId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChallengeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inputs_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inputs_Identities_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inputs_ChallengeId",
                table: "Inputs",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_Inputs_IdentityId",
                table: "Inputs",
                column: "IdentityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Inputs_InputId",
                table: "Submissions",
                column: "InputId",
                principalTable: "Inputs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Inputs_InputId",
                table: "Submissions");

            migrationBuilder.DropTable(
                name: "Inputs");

            migrationBuilder.RenameColumn(
                name: "InputId",
                table: "Submissions",
                newName: "IdentityId");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_InputId",
                table: "Submissions",
                newName: "IX_Submissions_IdentityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Identities_IdentityId",
                table: "Submissions",
                column: "IdentityId",
                principalTable: "Identities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
