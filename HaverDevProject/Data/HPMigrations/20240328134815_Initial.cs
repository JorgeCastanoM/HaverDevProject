using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HaverDevProject.Data.HPMigrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "defect",
                columns: table => new
                {
                    defectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    defectName = table.Column<string>(type: "TEXT", unicode: false, maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_defect_defectId", x => x.defectId);
                });

            migrationBuilder.CreateTable(
                name: "engDispositionType",
                columns: table => new
                {
                    engDispositionTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    engDispositionTypeName = table.Column<string>(type: "TEXT", unicode: false, maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_engDispoistionType_engDispositionTypeId", x => x.engDispositionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "followUpType",
                columns: table => new
                {
                    FollowUpTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FollowUpTypeName = table.Column<string>(type: "TEXT", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_followUpType_followUpTypeId", x => x.FollowUpTypeId);
                });

            migrationBuilder.CreateTable(
                name: "item",
                columns: table => new
                {
                    itemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    itemNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    itemName = table.Column<string>(type: "TEXT", unicode: false, maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_itemId", x => x.itemId);
                });

            migrationBuilder.CreateTable(
                name: "ncr",
                columns: table => new
                {
                    NcrId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NcrNumber = table.Column<string>(type: "TEXT", nullable: true),
                    NcrLastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NcrVoidReason = table.Column<string>(type: "TEXT", nullable: true),
                    NcrStatus = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrPhase = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ncr_ncrId", x => x.NcrId);
                    table.ForeignKey(
                        name: "fk_ncr_parentId",
                        column: x => x.ParentId,
                        principalTable: "ncr",
                        principalColumn: "NcrId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "opDispositionType",
                columns: table => new
                {
                    OpDispositionTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OpDispositionTypeName = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_opDispositionType_opDispositionTypeId", x => x.OpDispositionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "supplier",
                columns: table => new
                {
                    supplierId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    supplierCode = table.Column<string>(type: "TEXT", unicode: false, maxLength: 45, nullable: false),
                    supplierName = table.Column<string>(type: "TEXT", unicode: false, maxLength: 45, nullable: false),
                    supplierContactName = table.Column<string>(type: "TEXT", unicode: false, maxLength: 90, nullable: true),
                    supplierEmail = table.Column<string>(type: "TEXT", unicode: false, maxLength: 45, nullable: true),
                    supplierStatus = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_supplier_supplierId", x => x.supplierId);
                });

            migrationBuilder.CreateTable(
                name: "ncrEng",
                columns: table => new
                {
                    NcrEngId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NcrEngCustomerNotification = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrEngDispositionDescription = table.Column<string>(type: "TEXT", nullable: true),
                    NcrEngStatusFlag = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrEngCompleteDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NcrEngCreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NcrPhase = table.Column<int>(type: "INTEGER", nullable: false),
                    NcrEngUserId = table.Column<string>(type: "TEXT", nullable: true),
                    EngDispositionTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    NcrId = table.Column<int>(type: "INTEGER", nullable: false),
                    DrawingId = table.Column<int>(type: "INTEGER", nullable: false),
                    DrawingRequireUpdating = table.Column<bool>(type: "INTEGER", nullable: false),
                    DrawingOriginalRevNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    DrawingUpdatedRevNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    DrawingRevDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DrawingUserId = table.Column<string>(type: "TEXT", nullable: true),
                    NcrEngDefectVideo = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ncrEng_ncrEngId", x => x.NcrEngId);
                    table.ForeignKey(
                        name: "FK_ncrEng_ncr_NcrId",
                        column: x => x.NcrId,
                        principalTable: "ncr",
                        principalColumn: "NcrId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ncrEng_engDispositionType",
                        column: x => x.EngDispositionTypeId,
                        principalTable: "engDispositionType",
                        principalColumn: "engDispositionTypeId");
                });

            migrationBuilder.CreateTable(
                name: "NcrProcurements",
                columns: table => new
                {
                    NcrProcurementId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NcrProcSupplierReturnReq = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrProcExpectedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NcrProcDisposedAllowed = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrProcSAPReturnCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrProcCreditExpected = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrProcSupplierBilled = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrProcRejectedValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    NcrProcFlagStatus = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrProcUserId = table.Column<string>(type: "TEXT", nullable: true),
                    NcrProcCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NcrProcCompleteDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SupplierReturnMANum = table.Column<string>(type: "TEXT", nullable: true),
                    SupplierReturnName = table.Column<string>(type: "TEXT", nullable: true),
                    SupplierReturnAccount = table.Column<string>(type: "TEXT", nullable: true),
                    NcrId = table.Column<int>(type: "INTEGER", nullable: false),
                    NcrProcDefectVideo = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ncrProcurement_ncrProcurementId", x => x.NcrProcurementId);
                    table.ForeignKey(
                        name: "FK_NcrProcurements_ncr_NcrId",
                        column: x => x.NcrId,
                        principalTable: "ncr",
                        principalColumn: "NcrId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NcrReInspects",
                columns: table => new
                {
                    NcrReInspectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NcrReInspectAcceptable = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrNumber = table.Column<string>(type: "TEXT", nullable: true),
                    NcrReInspectCreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NcrReInspectCompleteDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NcrReInspectNewNcrNumber = table.Column<string>(type: "TEXT", nullable: true),
                    NcrReInspectNotes = table.Column<string>(type: "TEXT", nullable: true),
                    NcrReInspectUserId = table.Column<string>(type: "TEXT", nullable: true),
                    NcrQaStatusFlag = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrReInspectDefectVideo = table.Column<string>(type: "TEXT", nullable: true),
                    NcrId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ncrReInspect_ncrReInspectId", x => x.NcrReInspectId);
                    table.ForeignKey(
                        name: "FK_NcrReInspects_ncr_NcrId",
                        column: x => x.NcrId,
                        principalTable: "ncr",
                        principalColumn: "NcrId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ncrQA",
                columns: table => new
                {
                    NcrQaId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NcrQaStatusFlag = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrQaItemMarNonConforming = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrQaProcessApplicable = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrQacreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NcrQaOrderNumber = table.Column<string>(type: "TEXT", nullable: true),
                    NcrQaSalesOrder = table.Column<string>(type: "TEXT", nullable: true),
                    NcrQaQuanReceived = table.Column<int>(type: "INTEGER", nullable: false),
                    NcrQaQuanDefective = table.Column<int>(type: "INTEGER", nullable: false),
                    NcrQaDescriptionOfDefect = table.Column<string>(type: "TEXT", nullable: true),
                    NcrQaUserId = table.Column<string>(type: "TEXT", nullable: true),
                    NcrQaEngDispositionRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    NcrId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    DefectId = table.Column<int>(type: "INTEGER", nullable: false),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: false),
                    NcrQaDefectVideo = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ncrQA_ncrQAId", x => x.NcrQaId);
                    table.ForeignKey(
                        name: "FK_ncrQA_ncr_NcrId",
                        column: x => x.NcrId,
                        principalTable: "ncr",
                        principalColumn: "NcrId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ncrQa_defect",
                        column: x => x.DefectId,
                        principalTable: "defect",
                        principalColumn: "defectId");
                    table.ForeignKey(
                        name: "fk_ncrQa_item",
                        column: x => x.ItemId,
                        principalTable: "item",
                        principalColumn: "itemId");
                    table.ForeignKey(
                        name: "fk_ncrQa_supplier",
                        column: x => x.SupplierId,
                        principalTable: "supplier",
                        principalColumn: "supplierId");
                });

            migrationBuilder.CreateTable(
                name: "drawing",
                columns: table => new
                {
                    drawingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DrawingRequireUpdating = table.Column<bool>(type: "INTEGER", nullable: false),
                    drawingOriginalRevNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    drawingUpdatedRevNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    drawingRevDate = table.Column<DateTime>(type: "date", nullable: false),
                    drawingUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ncrEngId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drawing_drawingId", x => x.drawingId);
                    table.ForeignKey(
                        name: "FK_drawing_ncrEng_ncrEngId",
                        column: x => x.ncrEngId,
                        principalTable: "ncrEng",
                        principalColumn: "NcrEngId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "engDefectPhoto",
                columns: table => new
                {
                    engDefectPhotoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    engDefectPhotoContent = table.Column<byte[]>(type: "BLOB", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    engDefectPhotoMimeType = table.Column<string>(type: "TEXT", unicode: false, maxLength: 45, nullable: false),
                    engDefectPhotoDescription = table.Column<string>(type: "TEXT", unicode: false, maxLength: 300, nullable: true),
                    ncrEngId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_engDefectPhoto_engDefectPhotoId", x => x.engDefectPhotoId);
                    table.ForeignKey(
                        name: "fk_engDefectPhoto_itemDefect",
                        column: x => x.ncrEngId,
                        principalTable: "ncrEng",
                        principalColumn: "NcrEngId");
                });

            migrationBuilder.CreateTable(
                name: "NcrOperations",
                columns: table => new
                {
                    NcrOpId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NcrId = table.Column<int>(type: "INTEGER", nullable: false),
                    NcrOpCompleteDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NcrOpCreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OpDispositionTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    NcrPurchasingDescription = table.Column<string>(type: "TEXT", nullable: true),
                    Car = table.Column<bool>(type: "INTEGER", nullable: false),
                    CarNumber = table.Column<string>(type: "TEXT", nullable: true),
                    FollowUp = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExpectedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FollowUpTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdateOp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NcrOperationUserId = table.Column<string>(type: "TEXT", nullable: true),
                    NcrEngId = table.Column<int>(type: "INTEGER", nullable: true),
                    NcrOperationVideo = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ncrOperation_ncrOpId", x => x.NcrOpId);
                    table.ForeignKey(
                        name: "FK_NcrOperations_followUpType_FollowUpTypeId",
                        column: x => x.FollowUpTypeId,
                        principalTable: "followUpType",
                        principalColumn: "FollowUpTypeId");
                    table.ForeignKey(
                        name: "FK_NcrOperations_ncrEng_NcrEngId",
                        column: x => x.NcrEngId,
                        principalTable: "ncrEng",
                        principalColumn: "NcrEngId");
                    table.ForeignKey(
                        name: "FK_NcrOperations_ncr_NcrId",
                        column: x => x.NcrId,
                        principalTable: "ncr",
                        principalColumn: "NcrId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ncrOperation_opDispositionType",
                        column: x => x.OpDispositionTypeId,
                        principalTable: "opDispositionType",
                        principalColumn: "OpDispositionTypeId");
                });

            migrationBuilder.CreateTable(
                name: "procDefectPhoto",
                columns: table => new
                {
                    procDefectPhotoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    procDefectPhotoContent = table.Column<byte[]>(type: "BLOB", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    procDefectPhotoType = table.Column<string>(type: "TEXT", unicode: false, maxLength: 45, nullable: false),
                    procDefectPhotoDescription = table.Column<string>(type: "TEXT", unicode: false, maxLength: 300, nullable: true),
                    ncrProcurementId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_procDefectPhoto_procDefectPhotoId", x => x.procDefectPhotoId);
                    table.ForeignKey(
                        name: "fk_procDefectPhoto_itemDefect",
                        column: x => x.ncrProcurementId,
                        principalTable: "NcrProcurements",
                        principalColumn: "NcrProcurementId");
                });

            migrationBuilder.CreateTable(
                name: "ncrReInspectPhoto",
                columns: table => new
                {
                    ncrReInspectPhotoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ncrReInspectPhotoContent = table.Column<byte[]>(type: "BLOB", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ncrReInspectPhotoMimeType = table.Column<string>(type: "TEXT", unicode: false, maxLength: 45, nullable: false),
                    ncrReInspectPhotoDescription = table.Column<string>(type: "TEXT", unicode: false, maxLength: 300, nullable: true),
                    ncrReInspectId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ncrReInspectPhoto_ncrReInspectPhotoId", x => x.ncrReInspectPhotoId);
                    table.ForeignKey(
                        name: "fk_ncrReInspectPhoto_itemDefect",
                        column: x => x.ncrReInspectId,
                        principalTable: "NcrReInspects",
                        principalColumn: "NcrReInspectId");
                });

            migrationBuilder.CreateTable(
                name: "itemDefectPhoto",
                columns: table => new
                {
                    itemDefectPhotoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    itemDefectPhotoContent = table.Column<byte[]>(type: "BLOB", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    itemDefectPhotoMimeType = table.Column<string>(type: "TEXT", unicode: false, maxLength: 45, nullable: false),
                    itemDefectPhotoDescription = table.Column<string>(type: "TEXT", unicode: false, maxLength: 300, nullable: true),
                    ncrQaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_itemDefectPhoto_itemDefectPhotoId", x => x.itemDefectPhotoId);
                    table.ForeignKey(
                        name: "fk_itemDefectPhoto_itemDefect",
                        column: x => x.ncrQaId,
                        principalTable: "ncrQA",
                        principalColumn: "NcrQaId");
                });

            migrationBuilder.CreateTable(
                name: "EngFileContent",
                columns: table => new
                {
                    EngFileContentID = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngFileContent", x => x.EngFileContentID);
                    table.ForeignKey(
                        name: "FK_EngFileContent_engDefectPhoto_EngFileContentID",
                        column: x => x.EngFileContentID,
                        principalTable: "engDefectPhoto",
                        principalColumn: "engDefectPhotoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "opDefectPhoto",
                columns: table => new
                {
                    opDefectPhotoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    opDefectPhotoContent = table.Column<byte[]>(type: "BLOB", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    opDefectPhotoMimeType = table.Column<string>(type: "TEXT", unicode: false, maxLength: 45, nullable: false),
                    opDefectPhotoDescription = table.Column<string>(type: "TEXT", unicode: false, maxLength: 300, nullable: true),
                    ncrOpId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opDefectPhoto", x => x.opDefectPhotoId);
                    table.ForeignKey(
                        name: "FK_opDefectPhoto_NcrOperations_ncrOpId",
                        column: x => x.ncrOpId,
                        principalTable: "NcrOperations",
                        principalColumn: "NcrOpId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcFileContent",
                columns: table => new
                {
                    ProcFileContentID = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcFileContent", x => x.ProcFileContentID);
                    table.ForeignKey(
                        name: "FK_ProcFileContent_procDefectPhoto_ProcFileContentID",
                        column: x => x.ProcFileContentID,
                        principalTable: "procDefectPhoto",
                        principalColumn: "procDefectPhotoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReInspectFileContent",
                columns: table => new
                {
                    ReInspectFileContentID = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReInspectFileContent", x => x.ReInspectFileContentID);
                    table.ForeignKey(
                        name: "FK_ReInspectFileContent_ncrReInspectPhoto_ReInspectFileContentID",
                        column: x => x.ReInspectFileContentID,
                        principalTable: "ncrReInspectPhoto",
                        principalColumn: "ncrReInspectPhotoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileContent",
                columns: table => new
                {
                    FileContentID = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileContent", x => x.FileContentID);
                    table.ForeignKey(
                        name: "FK_FileContent_itemDefectPhoto_FileContentID",
                        column: x => x.FileContentID,
                        principalTable: "itemDefectPhoto",
                        principalColumn: "itemDefectPhotoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpFileContent",
                columns: table => new
                {
                    FileContentID = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpFileContent", x => x.FileContentID);
                    table.ForeignKey(
                        name: "FK_OpFileContent_opDefectPhoto_FileContentID",
                        column: x => x.FileContentID,
                        principalTable: "opDefectPhoto",
                        principalColumn: "opDefectPhotoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_drawing_ncrEngId",
                table: "drawing",
                column: "ncrEngId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_engDefectPhoto_ncrEngId",
                table: "engDefectPhoto",
                column: "ncrEngId");

            migrationBuilder.CreateIndex(
                name: "IX_item_itemNumber",
                table: "item",
                column: "itemNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_itemDefectPhoto_ncrQaId",
                table: "itemDefectPhoto",
                column: "ncrQaId");

            migrationBuilder.CreateIndex(
                name: "IX_ncr_ParentId",
                table: "ncr",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ncrEng_EngDispositionTypeId",
                table: "ncrEng",
                column: "EngDispositionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ncrEng_NcrId",
                table: "ncrEng",
                column: "NcrId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NcrOperations_FollowUpTypeId",
                table: "NcrOperations",
                column: "FollowUpTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NcrOperations_NcrEngId",
                table: "NcrOperations",
                column: "NcrEngId");

            migrationBuilder.CreateIndex(
                name: "IX_NcrOperations_NcrId",
                table: "NcrOperations",
                column: "NcrId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NcrOperations_OpDispositionTypeId",
                table: "NcrOperations",
                column: "OpDispositionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NcrProcurements_NcrId",
                table: "NcrProcurements",
                column: "NcrId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ncrQA_DefectId",
                table: "ncrQA",
                column: "DefectId");

            migrationBuilder.CreateIndex(
                name: "IX_ncrQA_ItemId",
                table: "ncrQA",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ncrQA_NcrId",
                table: "ncrQA",
                column: "NcrId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ncrQA_SupplierId",
                table: "ncrQA",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ncrReInspectPhoto_ncrReInspectId",
                table: "ncrReInspectPhoto",
                column: "ncrReInspectId");

            migrationBuilder.CreateIndex(
                name: "IX_NcrReInspects_NcrId",
                table: "NcrReInspects",
                column: "NcrId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_opDefectPhoto_ncrOpId",
                table: "opDefectPhoto",
                column: "ncrOpId");

            migrationBuilder.CreateIndex(
                name: "IX_procDefectPhoto_ncrProcurementId",
                table: "procDefectPhoto",
                column: "ncrProcurementId");

            migrationBuilder.CreateIndex(
                name: "IX_supplier_supplierCode",
                table: "supplier",
                column: "supplierCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "drawing");

            migrationBuilder.DropTable(
                name: "EngFileContent");

            migrationBuilder.DropTable(
                name: "FileContent");

            migrationBuilder.DropTable(
                name: "OpFileContent");

            migrationBuilder.DropTable(
                name: "ProcFileContent");

            migrationBuilder.DropTable(
                name: "ReInspectFileContent");

            migrationBuilder.DropTable(
                name: "engDefectPhoto");

            migrationBuilder.DropTable(
                name: "itemDefectPhoto");

            migrationBuilder.DropTable(
                name: "opDefectPhoto");

            migrationBuilder.DropTable(
                name: "procDefectPhoto");

            migrationBuilder.DropTable(
                name: "ncrReInspectPhoto");

            migrationBuilder.DropTable(
                name: "ncrQA");

            migrationBuilder.DropTable(
                name: "NcrOperations");

            migrationBuilder.DropTable(
                name: "NcrProcurements");

            migrationBuilder.DropTable(
                name: "NcrReInspects");

            migrationBuilder.DropTable(
                name: "defect");

            migrationBuilder.DropTable(
                name: "item");

            migrationBuilder.DropTable(
                name: "supplier");

            migrationBuilder.DropTable(
                name: "followUpType");

            migrationBuilder.DropTable(
                name: "ncrEng");

            migrationBuilder.DropTable(
                name: "opDispositionType");

            migrationBuilder.DropTable(
                name: "ncr");

            migrationBuilder.DropTable(
                name: "engDispositionType");
        }
    }
}
