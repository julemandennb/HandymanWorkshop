using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandymanWorkshop.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraDepartmentId1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Departments_DepartmentId1",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_DepartmentId1",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "DepartmentId1",
                table: "WorkOrders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId1",
                table: "WorkOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_DepartmentId1",
                table: "WorkOrders",
                column: "DepartmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Departments_DepartmentId1",
                table: "WorkOrders",
                column: "DepartmentId1",
                principalTable: "Departments",
                principalColumn: "Id");
        }
    }
}
