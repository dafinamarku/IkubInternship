namespace IkubInternship.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsSupervisor : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Departaments", "Supervisor_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Departaments", new[] { "Supervisor_Id" });
            AddColumn("dbo.AspNetUsers", "DepId", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "isSupervisor", c => c.Boolean(nullable: false));
            CreateIndex("dbo.AspNetUsers", "DepId");
            AddForeignKey("dbo.AspNetUsers", "DepId", "dbo.Departaments", "DepartamentId", cascadeDelete: true);
            DropColumn("dbo.Departaments", "Supervisor_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Departaments", "Supervisor_Id", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.AspNetUsers", "DepId", "dbo.Departaments");
            DropIndex("dbo.AspNetUsers", new[] { "DepId" });
            DropColumn("dbo.AspNetUsers", "isSupervisor");
            DropColumn("dbo.AspNetUsers", "DepId");
            CreateIndex("dbo.Departaments", "Supervisor_Id");
            AddForeignKey("dbo.Departaments", "Supervisor_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
