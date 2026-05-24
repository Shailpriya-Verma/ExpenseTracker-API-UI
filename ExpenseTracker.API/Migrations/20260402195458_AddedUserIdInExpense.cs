using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserIdInExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "tbl_Expense",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Expense_UserId",
                table: "tbl_Expense",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_Expense_tbl_User_UserId",
                table: "tbl_Expense",
                column: "UserId",
                principalTable: "tbl_User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_Expense_tbl_User_UserId",
                table: "tbl_Expense");

            migrationBuilder.DropIndex(
                name: "IX_tbl_Expense_UserId",
                table: "tbl_Expense");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tbl_Expense");
        }
    }
}
