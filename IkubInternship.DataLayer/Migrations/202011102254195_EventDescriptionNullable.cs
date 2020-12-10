namespace IkubInternship.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventDescriptionNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Events", "Description", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Events", "Description", c => c.String(nullable: false));
        }
    }
}
