using Microsoft.EntityFrameworkCore.Migrations;

namespace CommonData.Migrations
{
    public partial class event_simplified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomProperty1",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CustomProperty2",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CustomProperty3",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CustomProperty4",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CustomProperty5",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "IsPropertyScalingRequired",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NameOfProperty1",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NameOfProperty2",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NameOfProperty3",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NameOfProperty4",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NameOfProperty5",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Operation1",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Operation2",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Operation3",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Operation4",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "WeightOfProperty1",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "WeightOfProperty2",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "WeightOfProperty3",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "WeightOfProperty4",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "WeightOfProperty5",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "JobWeight",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfNeighboursWeight",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfProjectsWeight",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfSubordinatesWeight",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectImportanceWeight",
                table: "Events",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobWeight",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NumberOfNeighboursWeight",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NumberOfProjectsWeight",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NumberOfSubordinatesWeight",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ProjectImportanceWeight",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "CustomProperty1",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomProperty2",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomProperty3",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomProperty4",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomProperty5",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPropertyScalingRequired",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NameOfProperty1",
                table: "Events",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameOfProperty2",
                table: "Events",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameOfProperty3",
                table: "Events",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameOfProperty4",
                table: "Events",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameOfProperty5",
                table: "Events",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Operation1",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Operation2",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Operation3",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Operation4",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeightOfProperty1",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeightOfProperty2",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeightOfProperty3",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeightOfProperty4",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeightOfProperty5",
                table: "Events",
                type: "int",
                nullable: true);
        }
    }
}
