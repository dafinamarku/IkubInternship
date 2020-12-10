namespace IkubInternship.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PermissionChange1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Permissions", "PermissionDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Permissions", "PermissionStartDate");
            DropColumn("dbo.Permissions", "PermissionEndDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Permissions", "PermissionEndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Permissions", "PermissionStartDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Permissions", "PermissionDate");
        }
    }
}
