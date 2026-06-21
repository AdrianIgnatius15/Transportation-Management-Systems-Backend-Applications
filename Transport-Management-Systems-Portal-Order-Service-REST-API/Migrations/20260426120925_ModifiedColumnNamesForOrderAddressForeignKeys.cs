using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedColumnNamesForOrderAddressForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_PickupAddressId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "PickupAddressId",
                table: "Orders",
                newName: "ShipmentAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_PickupAddressId",
                table: "Orders",
                newName: "IX_Orders_ShipmentAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_ShipmentAddressId",
                table: "Orders",
                column: "ShipmentAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_ShipmentAddressId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ShipmentAddressId",
                table: "Orders",
                newName: "PickupAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_ShipmentAddressId",
                table: "Orders",
                newName: "IX_Orders_PickupAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_PickupAddressId",
                table: "Orders",
                column: "PickupAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
