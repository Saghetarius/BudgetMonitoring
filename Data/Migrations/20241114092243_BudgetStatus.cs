using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetMonitoring.Data.Migrations
{
    /// <inheritdoc />
    public partial class BudgetStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_BudgetStatusModels_BudgetStatusModelId",
                table: "Goals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetStatusModels",
                table: "BudgetStatusModels");

            migrationBuilder.RenameTable(
                name: "BudgetStatusModels",
                newName: "BudgetStatusModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetStatusModel",
                table: "BudgetStatusModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_BudgetStatusModel_BudgetStatusModelId",
                table: "Goals",
                column: "BudgetStatusModelId",
                principalTable: "BudgetStatusModel",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_BudgetStatusModel_BudgetStatusModelId",
                table: "Goals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetStatusModel",
                table: "BudgetStatusModel");

            migrationBuilder.RenameTable(
                name: "BudgetStatusModel",
                newName: "BudgetStatusModels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetStatusModels",
                table: "BudgetStatusModels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_BudgetStatusModels_BudgetStatusModelId",
                table: "Goals",
                column: "BudgetStatusModelId",
                principalTable: "BudgetStatusModels",
                principalColumn: "Id");
        }
    }
}
