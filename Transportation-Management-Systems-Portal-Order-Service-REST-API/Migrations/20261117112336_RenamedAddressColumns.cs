using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transportation_Management_Systems_Portal_Order_Service_REST_API.Migrations
{
    /// <inheritdoc />
    public partial class RenamedAddressColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_DeliveryAddressId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_ShipmentAddressId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ShipmentAddressId",
                table: "Orders",
                newName: "OriginAddressId");

            migrationBuilder.RenameColumn(
                name: "DeliveryAddressId",
                table: "Orders",
                newName: "DestinationAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_ShipmentAddressId",
                table: "Orders",
                newName: "IX_Orders_OriginAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_DeliveryAddressId",
                table: "Orders",
                newName: "IX_Orders_DestinationAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_DestinationAddressId",
                table: "Orders",
                column: "DestinationAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_OriginAddressId",
                table: "Orders",
                column: "OriginAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_DestinationAddressId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_OriginAddressId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "OriginAddressId",
                table: "Orders",
                newName: "ShipmentAddressId");

            migrationBuilder.RenameColumn(
                name: "DestinationAddressId",
                table: "Orders",
                newName: "DeliveryAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_OriginAddressId",
                table: "Orders",
                newName: "IX_Orders_ShipmentAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_DestinationAddressId",
                table: "Orders",
                newName: "IX_Orders_DeliveryAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_DeliveryAddressId",
                table: "Orders",
                column: "DeliveryAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_ShipmentAddressId",
                table: "Orders",
                column: "ShipmentAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
