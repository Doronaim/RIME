namespace RIME.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserModel : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Users");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 128),
                        UserHash = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Country = c.String(),
                        DOB = c.DateTime(nullable: false),
                        Summary = c.String(),
                        Photo = c.String(),
                    })
                .PrimaryKey(t => t.UserName);
            
        }
    }
}
