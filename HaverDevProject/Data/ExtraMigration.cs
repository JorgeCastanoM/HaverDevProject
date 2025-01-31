using Microsoft.EntityFrameworkCore.Migrations;

namespace HaverDevProject.Data
{
    public static class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetNcrQaTimestampOnUpdate
                    AFTER UPDATE ON NcrQas
                    BEGIN
                        UPDATE NcrQas
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetNcrQaTimestampOnInsert
                    AFTER INSERT ON NcrQas
                    BEGIN
                        UPDATE NcrQas
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetNcrEngTimestampOnUpdate
                    AFTER UPDATE ON NcrEngs
                    BEGIN
                        UPDATE NcrEngs
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetNcrEngTimestampOnInsert
                    AFTER INSERT ON NcrEngs
                    BEGIN
                        UPDATE NcrEngs
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetNcrOperationTimestampOnInsert
                    AFTER INSERT ON NcrOperations
                    BEGIN
                        UPDATE NcrOperations
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetNcrReInspectTimestampOnUpdate
                    AFTER UPDATE ON NcrReInspects
                    BEGIN
                        UPDATE NcrReInspects
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetNcrReInspectTimestampOnInsert
                    AFTER INSERT ON NcrReInspects
                    BEGIN
                        UPDATE NcrReInspects
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetNcrProcurementTimestampOnUpdate
                    AFTER UPDATE ON NcrProcurements
                    BEGIN
                        UPDATE NcrProcurements
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetNcrProcurementTimestampOnInsert
                    AFTER INSERT ON NcrProcurements
                    BEGIN
                        UPDATE NcrProcurements
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        }
    }
}
