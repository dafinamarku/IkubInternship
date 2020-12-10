namespace IkubInternship.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbContextchangeErr : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Departaments", "Name", c => c.String(nullable: false, maxLength: 30));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Departaments", "Name", c => c.String());
        }
    }
}
