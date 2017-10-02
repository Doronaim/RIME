namespace RIME.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Eran2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Evidences", "Quote", c => c.String(nullable: true));
        }
        
        public override void Down()
        {
        }
    }
}
