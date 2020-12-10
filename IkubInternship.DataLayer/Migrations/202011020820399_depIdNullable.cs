namespace IkubInternship.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class depIdNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "DepId", "dbo.Departaments");
            DropIndex("dbo.AspNetUsers", new[] { "DepId" });
            AlterColumn("dbo.AspNetUsers", "DepId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "DepId");
            AddForeignKey("dbo.AspNetUsers", "DepId", "dbo.Departaments", "DepartamentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "DepId", "dbo.Departaments");
            DropIndex("dbo.AspNetUsers", new[] { "DepId" });
            AlterColumn("dbo.AspNetUsers", "DepId", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "DepId");
            AddForeignKey("dbo.AspNetUsers", "DepId", "dbo.Departaments", "DepartamentId", cascadeDelete: true);
        }
    }
}
