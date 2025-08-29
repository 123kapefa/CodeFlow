using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReputationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_parent_id_colum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Total",
                table: "reputation_summaries",
                newName: "total");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "reputation_summaries",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_reputation_summaries_Total",
                table: "reputation_summaries",
                newName: "IX_reputation_summaries_total");

            migrationBuilder.RenameColumn(
                name: "Delta",
                table: "reputation_entries",
                newName: "delta");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "reputation_entries",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "SourceType",
                table: "reputation_entries",
                newName: "source_type");

            migrationBuilder.RenameColumn(
                name: "SourceService",
                table: "reputation_entries",
                newName: "source_service");

            migrationBuilder.RenameColumn(
                name: "SourceId",
                table: "reputation_entries",
                newName: "source_id");

            migrationBuilder.RenameColumn(
                name: "SourceEventId",
                table: "reputation_entries",
                newName: "source_event_id");

            migrationBuilder.RenameColumn(
                name: "ReasonDetails",
                table: "reputation_entries",
                newName: "reason_details");

            migrationBuilder.RenameColumn(
                name: "ReasonCode",
                table: "reputation_entries",
                newName: "reason_code");

            migrationBuilder.RenameColumn(
                name: "OccurredAt",
                table: "reputation_entries",
                newName: "occurred_at");

            migrationBuilder.RenameIndex(
                name: "IX_reputation_entries_UserId_OccurredAt",
                table: "reputation_entries",
                newName: "IX_reputation_entries_user_id_occurred_at");

            migrationBuilder.RenameIndex(
                name: "IX_reputation_entries_UserId",
                table: "reputation_entries",
                newName: "IX_reputation_entries_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_reputation_entries_SourceType_SourceId",
                table: "reputation_entries",
                newName: "IX_reputation_entries_source_type_source_id");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "reputation_effects",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "reputation_effects",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "reputation_effects",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SourceType",
                table: "reputation_effects",
                newName: "source_type");

            migrationBuilder.RenameColumn(
                name: "SourceService",
                table: "reputation_effects",
                newName: "source_service");

            migrationBuilder.RenameColumn(
                name: "SourceId",
                table: "reputation_effects",
                newName: "source_id");

            migrationBuilder.RenameColumn(
                name: "LastVersion",
                table: "reputation_effects",
                newName: "last_version");

            migrationBuilder.RenameColumn(
                name: "LastEventId",
                table: "reputation_effects",
                newName: "last_event_id");

            migrationBuilder.RenameColumn(
                name: "EffectKind",
                table: "reputation_effects",
                newName: "effect_kind");

            migrationBuilder.RenameIndex(
                name: "IX_reputation_effects_UserId_SourceId_SourceType_EffectKind",
                table: "reputation_effects",
                newName: "IX_reputation_effects_user_id_source_id_source_type_effect_kind");

            migrationBuilder.AddColumn<Guid>(
                name: "parent_id",
                table: "reputation_entries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "parent_id",
                table: "reputation_effects",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "parent_id",
                table: "reputation_entries");

            migrationBuilder.DropColumn(
                name: "parent_id",
                table: "reputation_effects");

            migrationBuilder.RenameColumn(
                name: "total",
                table: "reputation_summaries",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "reputation_summaries",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_reputation_summaries_total",
                table: "reputation_summaries",
                newName: "IX_reputation_summaries_Total");

            migrationBuilder.RenameColumn(
                name: "delta",
                table: "reputation_entries",
                newName: "Delta");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "reputation_entries",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "source_type",
                table: "reputation_entries",
                newName: "SourceType");

            migrationBuilder.RenameColumn(
                name: "source_service",
                table: "reputation_entries",
                newName: "SourceService");

            migrationBuilder.RenameColumn(
                name: "source_id",
                table: "reputation_entries",
                newName: "SourceId");

            migrationBuilder.RenameColumn(
                name: "source_event_id",
                table: "reputation_entries",
                newName: "SourceEventId");

            migrationBuilder.RenameColumn(
                name: "reason_details",
                table: "reputation_entries",
                newName: "ReasonDetails");

            migrationBuilder.RenameColumn(
                name: "reason_code",
                table: "reputation_entries",
                newName: "ReasonCode");

            migrationBuilder.RenameColumn(
                name: "occurred_at",
                table: "reputation_entries",
                newName: "OccurredAt");

            migrationBuilder.RenameIndex(
                name: "IX_reputation_entries_user_id_occurred_at",
                table: "reputation_entries",
                newName: "IX_reputation_entries_UserId_OccurredAt");

            migrationBuilder.RenameIndex(
                name: "IX_reputation_entries_user_id",
                table: "reputation_entries",
                newName: "IX_reputation_entries_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_reputation_entries_source_type_source_id",
                table: "reputation_entries",
                newName: "IX_reputation_entries_SourceType_SourceId");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "reputation_effects",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "reputation_effects",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "reputation_effects",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "source_type",
                table: "reputation_effects",
                newName: "SourceType");

            migrationBuilder.RenameColumn(
                name: "source_service",
                table: "reputation_effects",
                newName: "SourceService");

            migrationBuilder.RenameColumn(
                name: "source_id",
                table: "reputation_effects",
                newName: "SourceId");

            migrationBuilder.RenameColumn(
                name: "last_version",
                table: "reputation_effects",
                newName: "LastVersion");

            migrationBuilder.RenameColumn(
                name: "last_event_id",
                table: "reputation_effects",
                newName: "LastEventId");

            migrationBuilder.RenameColumn(
                name: "effect_kind",
                table: "reputation_effects",
                newName: "EffectKind");

            migrationBuilder.RenameIndex(
                name: "IX_reputation_effects_user_id_source_id_source_type_effect_kind",
                table: "reputation_effects",
                newName: "IX_reputation_effects_UserId_SourceId_SourceType_EffectKind");
        }
    }
}
