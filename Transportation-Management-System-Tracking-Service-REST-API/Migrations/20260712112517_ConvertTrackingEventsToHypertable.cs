using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transportation_Management_System_Tracking_Service_REST_API.Migrations
{
    /// <inheritdoc />
    public partial class ConvertTrackingEventsToHypertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE EXTENSION IF NOT EXISTS timescaledb;");

            migrationBuilder.Sql(@"
                ALTER TABLE ""TrackingEvents"" DROP CONSTRAINT ""PK_TrackingEvents"";
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""TrackingEvents"" ADD CONSTRAINT ""PK_TrackingEvents"" PRIMARY KEY (""Id"", ""Timestamp"");
            ");

            migrationBuilder.Sql(@"
                SELECT create_hypertable(
                    '""TrackingEvents""',
                    'Timestamp',
                    if_not_exists => TRUE,
                    migrate_data => TRUE
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""TrackingEvents""");
        }
    }
}
