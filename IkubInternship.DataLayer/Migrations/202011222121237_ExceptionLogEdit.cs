namespace IkubInternship.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExceptionLogEdit : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ExceptionLogs", "ControllerName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExceptionLogs", "ControllerName", c => c.String());
        }
    }
}
