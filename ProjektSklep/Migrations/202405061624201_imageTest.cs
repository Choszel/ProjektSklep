namespace ProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class imageTest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        imageId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.imageId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Images");
        }
    }
}
