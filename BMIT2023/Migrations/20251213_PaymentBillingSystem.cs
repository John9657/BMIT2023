using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BMIT2023.Migrations
{
    /// <inheritdoc />
    public partial class PaymentBillingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create StudentBillingProfile table
            migrationBuilder.CreateTable(
                name: "StudentBillingProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    BillingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalOutstanding = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AutoPaymentEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrimaryContactId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentBillingProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentBillingProfiles_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create FeeStructure table
            migrationBuilder.CreateTable(
                name: "FeeStructures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApplicableTo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApplicableValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FrequencyInMonths = table.Column<int>(type: "int", nullable: true),
                    IsRefundable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeStructures", x => x.Id);
                });

            // Create PaymentGateway table
            migrationBuilder.CreateTable(
                name: "PaymentGateways",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GatewayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SecretKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApiEndpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransactionFeePercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TransactionFeeFixed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SupportedCurrencies = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinimumAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGateways", x => x.Id);
                });

            // Create Invoice table
            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentBillingProfileId = table.Column<int>(type: "int", nullable: false),
                    FeeStructureId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    RecurrenceEndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_FeeStructures_FeeStructureId",
                        column: x => x.FeeStructureId,
                        principalTable: "FeeStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_StudentBillingProfiles_StudentBillingProfileId",
                        column: x => x.StudentBillingProfileId,
                        principalTable: "StudentBillingProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create Payment table
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentBillingProfileId = table.Column<int>(type: "int", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsReconciled = table.Column<bool>(type: "bit", nullable: false),
                    ReconciledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentGatewayId = table.Column<int>(type: "int", nullable: true),
                    GatewayResponse = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GatewayFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentGateways_PaymentGatewayId",
                        column: x => x.PaymentGatewayId,
                        principalTable: "PaymentGateways",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payments_StudentBillingProfiles_StudentBillingProfileId",
                        column: x => x.StudentBillingProfileId,
                        principalTable: "StudentBillingProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create Refund table
            migrationBuilder.CreateTable(
                name: "Refunds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefundReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    StudentBillingProfileId = table.Column<int>(type: "int", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RefundReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RefundMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedByAdminId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Refunds_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_StudentBillingProfiles_StudentBillingProfileId",
                        column: x => x.StudentBillingProfileId,
                        principalTable: "StudentBillingProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create PaymentTracking table
            migrationBuilder.CreateTable(
                name: "PaymentTrackings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StatusChangedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AmountTracked = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BankReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankClearingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsMatched = table.Column<bool>(type: "bit", nullable: false),
                    MatchedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MatchingNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTrackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTrackings_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create InvoiceLineItem table
            migrationBuilder.CreateTable(
                name: "InvoiceLineItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceLineItems_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create FinancialReport table
            migrationBuilder.CreateTable(
                name: "FinancialReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalInvoiced = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCollected = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalOutstanding = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRefunded = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCredits = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CollectionRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeneratedByAdminId = table.Column<int>(type: "int", nullable: true),
                    AverageInvoiceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalInvoiceCount = table.Column<int>(type: "int", nullable: false),
                    TotalPaidInvoiceCount = table.Column<int>(type: "int", nullable: false),
                    TotalOverdueInvoiceCount = table.Column<int>(type: "int", nullable: false),
                    JsonData = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialReports", x => x.Id);
                });

            // Create DunningNotice table
            migrationBuilder.CreateTable(
                name: "DunningNotices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoticeNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentBillingProfileId = table.Column<int>(type: "int", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    NoticeLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OutstandingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDateOriginal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaysOverdue = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoticeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoticeTargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NoticeMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NoticeTemplate = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsAutomatic = table.Column<bool>(type: "bit", nullable: false),
                    ResolutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NotificationMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WasAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    AcknowledgedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LateFeeCharged = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateFeeAppliedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EscalationLevel = table.Column<int>(type: "int", nullable: true),
                    EscalatedToAdminDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DunningNotices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DunningNotices_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DunningNotices_StudentBillingProfiles_StudentBillingProfileId",
                        column: x => x.StudentBillingProfileId,
                        principalTable: "StudentBillingProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Invoices_FeeStructureId",
                table: "Invoices",
                column: "FeeStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_StudentBillingProfileId",
                table: "Invoices",
                column: "StudentBillingProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItems_InvoiceId",
                table: "InvoiceLineItems",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvoiceId",
                table: "Payments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentGatewayId",
                table: "Payments",
                column: "PaymentGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StudentBillingProfileId",
                table: "Payments",
                column: "StudentBillingProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTrackings_PaymentId",
                table: "PaymentTrackings",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_PaymentId",
                table: "Refunds",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_StudentBillingProfileId",
                table: "Refunds",
                column: "StudentBillingProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentBillingProfiles_StudentId",
                table: "StudentBillingProfiles",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_DunningNotices_InvoiceId",
                table: "DunningNotices",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DunningNotices_StudentBillingProfileId",
                table: "DunningNotices",
                column: "StudentBillingProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DunningNotices");

            migrationBuilder.DropTable(
                name: "FinancialReports");

            migrationBuilder.DropTable(
                name: "InvoiceLineItems");

            migrationBuilder.DropTable(
                name: "PaymentTrackings");

            migrationBuilder.DropTable(
                name: "Refunds");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "PaymentGateways");

            migrationBuilder.DropTable(
                name: "FeeStructures");

            migrationBuilder.DropTable(
                name: "StudentBillingProfiles");
        }
    }
}
