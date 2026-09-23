using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPagamentoReservaEntrada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LocacaoId",
                table: "Pagamentos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ReservaId",
                table: "Pagamentos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_ReservaId",
                table: "Pagamentos",
                column: "ReservaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagamentos_Reservas_ReservaId",
                table: "Pagamentos",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pagamentos_Reservas_ReservaId",
                table: "Pagamentos");

            migrationBuilder.DropIndex(
                name: "IX_Pagamentos_ReservaId",
                table: "Pagamentos");

            migrationBuilder.DropColumn(
                name: "ReservaId",
                table: "Pagamentos");

            migrationBuilder.AlterColumn<int>(
                name: "LocacaoId",
                table: "Pagamentos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
