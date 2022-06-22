using Microsoft.EntityFrameworkCore.Migrations;

namespace core_graph_v2.Migrations
{
    public partial class AddUserToDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Action",
                columns: table => new
                {
                    ActionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionGroupId = table.Column<int>(type: "int", nullable: false),
                    ActionNo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Action", x => x.ActionId);
                });

            migrationBuilder.CreateTable(
                name: "ClashCheck",
                columns: table => new
                {
                    CheckItem = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClashesCnt = table.Column<int>(type: "int", nullable: false),
                    UndefinedCnt = table.Column<int>(type: "int", nullable: false),
                    EditCnt = table.Column<int>(type: "int", nullable: false),
                    NoneCnt = table.Column<int>(type: "int", nullable: false),
                    SevereCnt = table.Column<int>(type: "int", nullable: false),
                    OptionalCnt = table.Column<int>(type: "int", nullable: false),
                    BadPartCnt = table.Column<int>(type: "int", nullable: false),
                    DelayedPartCnt = table.Column<int>(type: "int", nullable: false),
                    LastUpdateDt = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClashCheck", x => x.CheckItem);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Idx = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Idx);
                });

            migrationBuilder.CreateTable(
                name: "ActionCmt",
                columns: table => new
                {
                    ActionCmtId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CView = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionCmt", x => x.ActionCmtId);
                    table.ForeignKey(
                        name: "FK_ActionCmt_Action_ActionId",
                        column: x => x.ActionId,
                        principalTable: "Action",
                        principalColumn: "ActionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionCmt_ActionId",
                table: "ActionCmt",
                column: "ActionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionCmt");

            migrationBuilder.DropTable(
                name: "ClashCheck");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Action");
        }
    }
}
