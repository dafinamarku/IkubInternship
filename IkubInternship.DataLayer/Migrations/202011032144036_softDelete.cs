namespace IkubInternship.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class softDelete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "DeleteStatus", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DeleteStatus");
        }
    }
}
