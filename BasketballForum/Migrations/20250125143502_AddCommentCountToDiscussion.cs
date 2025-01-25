﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasketballForum.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentCountToDiscussion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentsCount",
                table: "Discussion",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentsCount",
                table: "Discussion");
        }
    }
}
