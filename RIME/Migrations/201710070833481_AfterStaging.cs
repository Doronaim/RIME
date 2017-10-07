namespace RIME.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AfterStaging : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EvidenceComments",
                c => new
                    {
                        EvidenceCommentId = c.Int(nullable: false, identity: true),
                        EvidenceId = c.Int(nullable: false),
                        Name = c.String(),
                        Email = c.String(),
                        Date = c.DateTime(nullable: false),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.EvidenceCommentId)
                .ForeignKey("dbo.Evidences", t => t.EvidenceId, cascadeDelete: true)
                .Index(t => t.EvidenceId);
            
            CreateTable(
                "dbo.SubComments",
                c => new
                    {
                        SubCommentId = c.Int(nullable: false, identity: true),
                        EvidenceCommentId = c.Int(nullable: false),
                        Name = c.String(),
                        Email = c.String(),
                        Date = c.DateTime(nullable: false),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.SubCommentId)
                .ForeignKey("dbo.EvidenceComments", t => t.EvidenceCommentId, cascadeDelete: true)
                .Index(t => t.EvidenceCommentId);
            
            CreateTable(
                "dbo.Evidences",
                c => new
                    {
                        EvidenceId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        EvidencePic = c.String(),
                        EvidencePath = c.String(),
                        EvidenceLocation = c.String(),
                        Title = c.String(),
                        Prolog = c.String(),
                        Content = c.String(),
                        Date = c.DateTime(nullable: false),
                        Likes = c.Int(nullable: false),
                        Quote = c.String(),
                    })
                .PrimaryKey(t => t.EvidenceId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        EvidenceId = c.Int(nullable: false),
                        Categoty = c.String(),
                        TagName = c.String(),
                    })
                .PrimaryKey(t => t.TagId)
                .ForeignKey("dbo.Evidences", t => t.EvidenceId, cascadeDelete: true)
                .Index(t => t.EvidenceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tags", "EvidenceId", "dbo.Evidences");
            DropForeignKey("dbo.EvidenceComments", "EvidenceId", "dbo.Evidences");
            DropForeignKey("dbo.SubComments", "EvidenceCommentId", "dbo.EvidenceComments");
            DropIndex("dbo.Tags", new[] { "EvidenceId" });
            DropIndex("dbo.SubComments", new[] { "EvidenceCommentId" });
            DropIndex("dbo.EvidenceComments", new[] { "EvidenceId" });
            DropTable("dbo.Tags");
            DropTable("dbo.Evidences");
            DropTable("dbo.SubComments");
            DropTable("dbo.EvidenceComments");
        }
    }
}
