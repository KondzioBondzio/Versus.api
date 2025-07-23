using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Versus.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityAuditLog2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ChangedBy",
                table: "AuditLogs",
                type: "uuid",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldMaxLength: 255);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ChangedBy",
                table: "AuditLogs",
                type: "uuid",
                maxLength: 255,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
