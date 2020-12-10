namespace IkubInternship.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PermissionChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Permissions", "PermissionStartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Permissions", "PermissionEndDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Permissions", "PermissionDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Permissions", "PermissionDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Permissions", "PermissionEndDate");
            DropColumn("dbo.Permissions", "PermissionStartDate");
        }
    }
}
