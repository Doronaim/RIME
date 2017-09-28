namespace RIME.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Second : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EvidenceComments", "Email", c => c.String());
            AddColumn("dbo.SubComments", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubComments", "Email");
            DropColumn("dbo.EvidenceComments", "Email");
        }
    }
}
